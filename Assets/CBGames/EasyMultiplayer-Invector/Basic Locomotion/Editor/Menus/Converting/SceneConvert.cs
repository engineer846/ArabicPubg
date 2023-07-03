using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace EMI.Menus
{
    public static class SceneConvert
    {
        public static List<string> FindBuildScenes(bool includeInactive = true)
        {
            List<string> buildScenes = new List<string>();
            foreach(EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (!includeInactive && !scene.enabled) continue;
                buildScenes.Add(scene.path);
            }
            return buildScenes;
        }

        public static List<string> FindAllScenesInProject()
        {
            List<string> buildScenes = new List<string>();
            string[] filePaths = Directory.GetFiles(Application.dataPath + "/", "*.unity", SearchOption.AllDirectories);
            foreach(string filePath in filePaths)
            {
                if (filePath.Contains("Assets/Mirror")) continue;
                buildScenes.Add(filePath);
            }
            return buildScenes;
        }

        public static void ConvertScenes(List<string> scenePaths, bool unpack)
        {
            ObjectConverter converter = new ObjectConverter();
            foreach (string scenePath in scenePaths)
            {
                EditorSceneManager.OpenScene(scenePath.Replace(Application.dataPath, "Assets"), OpenSceneMode.Single);
                UnityEngine.Object[] objs = UnityEngine.Object.FindObjectsOfType(typeof(GameObject));
                foreach(UnityEngine.Object obj in objs)
                {
                    converter.ConvertObjectInPlace((GameObject)obj, unpack);
                }
                EditorSceneManager.SaveOpenScenes();
                Debug.Log($"Converted: {Path.GetFileNameWithoutExtension(scenePath)}");
            }
        }
    }
}
