using System.Collections.Generic;
using System.Linq;
using System.Reflection; //for deep copy
using UnityEditor;
using UnityEngine;

namespace EMI.Utils
{
    public static class EditorUtils
    {
        public static BindingFlags allBindingValues = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

        #region Copying
        /// <summary>
        /// Will copy the source component properties to the destination component that have matching properties.
        /// Component types don't matter, will copy any type of component to any other type of component. This 
        /// gets around the Unity limitation of only allowing to copy the same type of component to each other.
        /// </summary>
        /// <param name="sourceComp"></param>
        /// <param name="destComp"></param>
        public static void DeepCopyValues(Component sourceComp, Component destComp)
        {
            if (!destComp) return;

            // get all the properties on the source component
            FieldInfo[] properties = sourceComp.GetType().GetFields(allBindingValues);

            // one at a time copy those values to the destination component
            foreach (FieldInfo property in properties)
            {
                try
                {
                    if (destComp.GetType().GetField(property.Name, allBindingValues) != null) //if it has this property value name
                    {
                        destComp.GetType().GetField(property.Name, allBindingValues).SetValue(destComp, property.GetValue(sourceComp)); // copy the source comp property value to the dest comp property value
                    }
                }
                catch { }
            }

            // Add logic to account for UnityEvents
        }
        #endregion

        #region Textures
        /// <summary>
        /// Will generate a Texture2D from a color
        /// </summary>
        public static Texture2D MakeTex(Color col, int width = 1, int height = 1)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }

        /// <summary>
        /// Used to set the alpha on a texture
        /// </summary>
        public static void SetAlpha(Texture2D target, float value)
        {
            Color pixA;
            for (int i = 0; i < target.width; i++)
            {
                for (int j = 0; j < target.height; j++)
                {
                    // change the pixels alpha value
                    pixA = target.GetPixel(i, j);
                    pixA.a = value;

                    // Set the changed pixel to the original image
                    target.SetPixel(i, j, pixA);
                }
            }
            // Apply the results
            target.Apply();
        }
        #endregion

        #region Scenes
        public static List<string> GetAllBuildScenes()
        {
            int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
            List<string> scenes = new List<string>();
            for (int i = 0; i < sceneCount; i++)
            {
                scenes.Add(System.IO.Path.GetFileNameWithoutExtension(UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i)));
            }
            return scenes;
        }
        #endregion

        #region Prefabs
        public static string[] GetAllPathsOfType(string type=".prefab", string inPath="Assets")
        {
            string[] temp = AssetDatabase.GetAllAssetPaths();
            List<string> result = new List<string>();
            foreach (string s in temp)
            {
                if (!s.Contains(inPath)) continue;
                if (s.Contains(type)) result.Add(s);
            }
            return result.ToArray();
        }
        public static List<string> GetPrefabPathsWithComponents(List<string> comps, string inPath = "Assets", string ignorePath = "Assets/Invector-3rdPersonController", bool ignoreMPComps = false)
        {
            List<string> foundPaths = new List<string>();
            string[] prefabs = GetAllPathsOfType(".prefab", inPath);
            foreach (string prefabPath in prefabs)
            {
                if (!string.IsNullOrEmpty(ignorePath) && prefabPath.Contains(ignorePath)) continue;
                UnityEngine.Object o = AssetDatabase.LoadMainAssetAtPath(prefabPath);
                GameObject go;
                go = (GameObject)o;
                Component[] components = go.GetComponentsInChildren<Component>(true);
                foreach (Component c in components)
                {
                    if (c == null) continue;
                    if (comps.Contains(c.GetType().Name))
                    {
                        if (ignoreMPComps && c.GetType().Name.StartsWith("MP_")) 
                            continue;
                        foundPaths.Add(prefabPath);
                        break;
                    }
                }
            }
            return foundPaths;
        }
        public static List<string> GetAllInstances<T>(string inPath = "Assets", string ignorePath = "Assets/Invector-3rdPersonController") where T : ScriptableObject
        {
            List<string> foundTargets = new List<string>();
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                if (!string.IsNullOrEmpty(ignorePath) && path.Contains(ignorePath) || !path.Contains(inPath)) continue;
                foundTargets.Add(path);
            }

            return foundTargets;
        }
        public static List<UnityEngine.Object> GetAllObjectsOfTypeInProjectPaths(List<string> inPaths, List<string> onlyTypes)
        {
            List<UnityEngine.Object> foundTargets = new List<UnityEngine.Object>();
            if (onlyTypes.Count == 0) return foundTargets;
            if (inPaths.Count == 0) inPaths.Add("Assets");
            string[] guids = AssetDatabase.FindAssets($"t:Object", inPaths.ToArray());
            
            foreach (string guid in guids)
            {
                string objectPath = AssetDatabase.GUIDToAssetPath(guid);
                UnityEngine.Object[] targets = LoadAllAssetsAtPath(objectPath);
                foreach (UnityEngine.Object target in targets)
                {
                    try
                    {
                        string objType = target.GetType().Name;
                        if (onlyTypes.Contains(objType))
                        {
                            foundTargets.Add(target);
                        }
                    }
                    catch { }
                }
            }
            return foundTargets.Distinct().ToList();
        }
        public static UnityEngine.Object[] LoadAllAssetsAtPath(string assetPath)
        {
            return typeof(SceneAsset).Equals(AssetDatabase.GetMainAssetTypeAtPath(assetPath)) ?
                // prevent error "Do not use readobjectthreaded on scene objects!"
                new[] { AssetDatabase.LoadMainAssetAtPath(assetPath) } :
                AssetDatabase.LoadAllAssetsAtPath(assetPath);
        }
        #endregion
    }
}
