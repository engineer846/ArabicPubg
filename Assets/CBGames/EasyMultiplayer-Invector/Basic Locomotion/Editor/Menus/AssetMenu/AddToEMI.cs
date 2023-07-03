using EMI.Managers;
using System;
using UnityEditor;
using UnityEngine;

namespace EMI.Editors
{
    public class AddToEMI
    { 
        [MenuItem("Assets/EMI/Add To EMI_NetworkManager")]
        public static void AddToEMI_NetworkManager()
        {
            EMI_NetworkManager[] managers = GameObject.FindObjectsOfType<EMI_NetworkManager>();
            EMI_NetworkManager manager = null;
            if (managers.Length < 1)
            {
                EditorUtility.DisplayDialog("Missing Network Manager", "Unable to find a network manager. Load a scene with a network manager in it to use this menu item", "Ok");
                return;
            }
            else if (managers.Length > 1)
            {
                EditorUtility.DisplayDialog("Multiple Network Managers", "Multiple network managers were found. Be sure to only ever have one Network Manager in your project at the index 0 scene. Unload the scenes that contain the duplicate Network Managers to continue.", "Ok");
                return;
            }
            else
            {
                manager = managers[0];
            }
            UnityEngine.Object[] objects = Selection.objects;
            foreach(UnityEngine.Object obj in objects)
            {
                if (!manager.spawnPrefabs.Contains((GameObject)obj))
                {
                    manager.spawnPrefabs.Add((GameObject)obj);
                }
            }
        }

        [MenuItem("Assets/EMI/Add To EMI_NetworkManager", true)]
        private static bool NewMenuOptionValidation()
        {
            return Selection.activeObject.GetType() == typeof(GameObject);
        }
    }
}
