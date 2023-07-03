using Common.Utils;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace EMI.Menus
{
    public class MenuSettingsBasic : EditorWindow
    {
        #region Properties
        protected bool hostModeEnabled = false;
        #endregion

        #region Initilization
        [MenuItem("Easy Multiplayer - Invector/Open Settings Window", priority = 101)]
        public static void OpenSettingsBasicWindow()
        {
            EditorWindow.GetWindow<MenuSettingsBasic>("EMI Settings", focus: true);
        }
        protected virtual void OnEnable()
        {
            CheckIfHostModeEnabled();
        }
        #endregion

        #region Host Mode Setting
        protected virtual void CheckIfHostModeEnabled()
        {
            string hostModeFile = FilesUtil.FindFolderPath("EasyMultiplayer-Invector");
            hostModeFile += "/Basic Locomotion/Editor/HostMode.txt";
            if (File.Exists(hostModeFile))
            {
                StreamReader reader = new StreamReader(hostModeFile);
                string contents = reader.ReadToEnd();
                contents = (string.IsNullOrEmpty(contents)) ? "false" : contents;
                hostModeEnabled = bool.Parse(contents.Trim());
                reader.Close();
            }
            else
            {
                hostModeEnabled = false;
            }
        }
        protected virtual void EnableHostMode(bool enabled)
        {
            if (enabled)
                ModifyToEnable();
            else
                ModifyToDisable();

            string hostModeFile = FilesUtil.FindFolderPath("EasyMultiplayer-Invector");
            hostModeFile += "/Basic Locomotion/Editor/HostMode.txt";
            File.WriteAllText(hostModeFile, enabled.ToString());
            CheckIfHostModeEnabled();

        }
        protected virtual void ModifyToEnable()
        {
            string dataPath = Application.dataPath.Replace('/', Path.DirectorySeparatorChar);
            string emi_folder = FilesUtil.FindFolderPath("EasyMultiplayer-Invector");
            string relative_folder = emi_folder.Replace(dataPath, "Assets").Replace(Path.DirectorySeparatorChar,'/');
            List<string> scriptPaths = new List<string>();

            string[] assetPaths = AssetDatabase.GetAllAssetPaths();
            foreach (string assetPath in assetPaths)
            {
                if (assetPath.EndsWith(".cs") && assetPath.Contains(relative_folder))
                {
                    scriptPaths.Add(assetPath);
                }
            }

            foreach (string filePath in scriptPaths)
            {
                //string filePath = FilesUtil.FindFilePath("BasicNetworkCalls.cs", emi_folder);
                FilesUtil.ModifyFileAtPath(filePath, new List<FilesUtil.Modification>()
                {
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "#if UNITY_SERVER || UNITY_EDITOR" },
                        modifiedLines: new List<string>() { "//#if UNITY_SERVER || UNITY_EDITOR" },
                        modificationType: FilesUtil.ModiftyType.Replace,
                        matchAll: true
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "#if !UNITY_SERVER || UNITY_EDITOR" },
                        modifiedLines: new List<string>() { "//#if !UNITY_SERVER || UNITY_EDITOR" },
                        modificationType: FilesUtil.ModiftyType.Replace,
                        matchAll: true
                    ),
                    new FilesUtil.Modification(
                        previousLines: new List<string>() { "//#if !UNITY_SERVER || UNITY_EDITOR" },
                        targetLines: new List<string>() { "#endif" },
                        modifiedLines: new List<string>() { "//#endif" },
                        modificationType: FilesUtil.ModiftyType.Replace,
                        matchAll: true
                    ),
                    new FilesUtil.Modification(
                        previousLines: new List<string>() { "//#if UNITY_SERVER || UNITY_EDITOR" },
                        targetLines: new List<string>() { "#endif" },
                        modifiedLines: new List<string>() { "//#endif" },
                        modificationType: FilesUtil.ModiftyType.Replace,
                        matchAll: true
                    )
                });
            }
        }
        protected virtual void ModifyToDisable()
        {
            string dataPath = Application.dataPath.Replace('/', Path.DirectorySeparatorChar);
            string emi_folder = FilesUtil.FindFolderPath("EasyMultiplayer-Invector");
            string relative_folder = emi_folder.Replace(dataPath, "Assets").Replace(Path.DirectorySeparatorChar, '/');
            List<string> scriptPaths = new List<string>();

            string[] assetPaths = AssetDatabase.GetAllAssetPaths();
            foreach (string assetPath in assetPaths)
            {
                if (assetPath.EndsWith(".cs") && assetPath.Contains(relative_folder))
                {
                    scriptPaths.Add(assetPath);
                }
            }

            foreach (string filePath in scriptPaths)
            {
                FilesUtil.ModifyFileAtPath(filePath, new List<FilesUtil.Modification>()
                {
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "//#if UNITY_SERVER || UNITY_EDITOR" },
                        modifiedLines: new List<string>() { "#if UNITY_SERVER || UNITY_EDITOR" },
                        modificationType: FilesUtil.ModiftyType.Replace,
                        matchAll: true
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "//#if !UNITY_SERVER || UNITY_EDITOR" },
                        modifiedLines: new List<string>() { "#if !UNITY_SERVER || UNITY_EDITOR" },
                        modificationType: FilesUtil.ModiftyType.Replace,
                        matchAll: true
                    ),
                    new FilesUtil.Modification(
                        previousLines: new List<string>() { "#if !UNITY_SERVER || UNITY_EDITOR" },
                        targetLines: new List<string>() { "//#endif" },
                        modifiedLines: new List<string>() { "#endif" },
                        modificationType: FilesUtil.ModiftyType.Replace,
                        matchAll: true
                    ),
                    new FilesUtil.Modification(
                        previousLines: new List<string>() { "#if UNITY_SERVER || UNITY_EDITOR" },
                        targetLines: new List<string>() { "//#endif" },
                        modifiedLines: new List<string>() { "#endif" },
                        modificationType: FilesUtil.ModiftyType.Replace,
                        matchAll: true
                    )
                });
            }
        }
        #endregion

        #region GUI
        protected virtual void OnGUI()
        {
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.HelpBox($"Host Mode Enabled: {hostModeEnabled}", MessageType.None);
            if (!hostModeEnabled)
            {
                EditorGUILayout.HelpBox("If you enable this, it means you will be able to run Server+Client on the same machine. It also means you will NOT be protected by decompilation protection anymore. That means clients will be able to decompile the game down to the source code and see the EMI server code to potentially generate exploits for your game.", MessageType.Warning);
                if (GUILayout.Button("Enable Host Mode"))
                {
                    EnableHostMode(true);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Host mode is currently enabled. That means clients can potentially decompile this to see the server code to write potential exploits for you game. It also means you can run a Server+Client on the same machine. If you disable this it means you will NOT be able to run a Server+Client on the same machine, just a Server or a Client (depending on what it's build for). It also means decompilation protection will be in place preventing people from decompiling EMI server source code.", MessageType.None);
                if (GUILayout.Button("Disable Host Mode"))
                {
                    EnableHostMode(false);
                }
            }
            EditorGUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
        }
        #endregion
    }
}
