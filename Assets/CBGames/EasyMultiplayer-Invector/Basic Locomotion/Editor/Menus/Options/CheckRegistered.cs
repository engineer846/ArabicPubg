using EMI.Managers;
using Mirror;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EMI.Menus
{
    public class CheckRegistered : EditorWindow
    {
        #region Check Registered Prefabs
        [MenuItem("Easy Multiplayer - Invector/Options/Check If SceneObjects Registered", priority = 102)]
        static void CheckSceneObjectsRegistered()
        {

            EMI_NetworkManager networkManager = FindObjectOfType<EMI_NetworkManager>();

            if (networkManager == null)
            {
                EditorUtility.DisplayDialog("Check If SceneObjects Registered", "No EMI_NetworkManager found", "OK", "");
                return;
            }


            List<string> registeredPrefabNames = new List<string>(networkManager.spawnPrefabs.Count);
            foreach (GameObject g in networkManager.spawnPrefabs)
                registeredPrefabNames.Add(g.name);

            int i = 0;
            foreach (NetworkIdentity uv in FindObjectsOfType<NetworkIdentity>())
            {
                GameObject found = PrefabUtility.GetCorrespondingObjectFromSource(uv.gameObject);
                if (found == null)
                {
                    Debug.LogError(uv.gameObject + " is in scene, has no prefab, and isn't registered", uv.gameObject);
                    i++;
                }
                else if (!registeredPrefabNames.Contains(found.name))
                {
                    Debug.LogError("Found " + uv.name + " in the scene, but its prefab " + found.name + " was not registered", uv.gameObject);
                    i++;
                }
            }

            EditorUtility.DisplayDialog("Check If SceneObjects Registered", (i > 0 ? "Found " + i + " unregistered Scene Objects.  See Logs." : "All OK"), "OK", "");
        }
        #endregion
    }
}
