using Unity.EditorCoroutines.Editor;
using Common.Utils;
using EMI.Editors.Windows;
using EMI.Utils;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using EMI.Object;
using static EMI.Object.TeamData;
using System.Linq;
using Invector.vCharacterController;
using System;

namespace EMI.Menus
{
    #region Partial classes
    public partial class ObjectConverter
    {
        public void ConvertObjectInPlace(GameObject target, bool unpack)
        {
            ConvertObject(target, unpack);
        }
        public void ExecuteConvertSO(ScriptableObject target, string savePath)
        {
            StandaloneConvertSO(target, savePath);
        }
        public void ExecuteConvertObject(GameObject target, string savePath, bool unpack)
        {
            StandaloneConvertObject(target, savePath, unpack);
        }
        public void ExecuteConvertPrefabs(List<string> prefabPaths, string savePath, bool unpack)
        {
            ConvertPrefabs(prefabPaths, savePath, unpack);
        }
        partial void ConvertObject(GameObject target, bool unpack);
        partial void ConvertPrefabs(List<string> prefabPaths, string savePath, bool unpack);
        partial void ConvertCharacter(GameObject target, bool unpack);
        partial void ConvertSingleObj(GameObject target, bool unpack);
        partial void StandaloneConvertObject(GameObject inputTarget, string savePath, bool unpack);
        partial void StandaloneConvertSO(ScriptableObject inputTarget, string savePath);
        partial void ConvertAI(GameObject target, bool unpack);
    }
    
    public partial class BasicTestActions : EditorWindow
    {
        public void PerformTests(Dictionary<string, bool> whatScenesToScan, Dictionary<string, bool> whatPrefabsToScan, Action callback)
        {
            BasicTests(whatScenesToScan, whatPrefabsToScan, callback);
        }
        partial void BasicTests(Dictionary<string, bool> whatScenesToScan, Dictionary<string, bool> whatPrefabsToScan, Action callback);
    }
    #endregion

    public class BasicNavigator : MainNavigator
    {
        #region Properties
        protected bool hasCorrectVersion = false;
        protected bool modTeams = false;
        protected bool underContruction = true;
        protected bool hasMirror = false;
        protected bool convertedScriptsCompiled, isCompiling = false;
        protected bool scriptsConverted = false;
        protected bool packagesImported = false;
        protected bool welcome, scenes, objects, tests = true;
        protected bool converting = false;
        protected bool s_scene = true;
        protected bool s_prefabs = true; 
        protected bool s_characters = true;
        protected bool includeInvDir = false;
        protected bool unpack = false;

        protected Dictionary<string, bool> whatScenesToScan = new Dictionary<string, bool>();
        protected Dictionary<string, bool> whatPrefabsToScan = new Dictionary<string, bool>();

        protected List<string> teamDataPaths = new List<string>();
        protected List<string> objectPaths = new List<string>();
        protected List<string> buildScenes = new List<string>();
        protected List<string> convertScenes = new List<string>();
        protected List<string> test_prefabPaths = new List<string>();
        protected List<float> sceneScanPrecent = new List<float>();
        protected List<string> scanPaths = new List<string>();
        protected List<UnityEngine.Object> prefabPaths = new List<UnityEngine.Object>();
        protected List<float> prefabPathsPercent = new List<float>();
        protected bool c_scene = false;
        protected bool c_prefabs = false;
        protected bool c_characters = false;

        protected bool done_scene = false;
        protected bool done_prefabs = false;
        protected bool done_characters = false;

        protected bool test_prefabsRefreshing = true;
        protected bool test_prev_scenes = false;
        protected bool test_prev_prefabs = false;
        protected bool test_scenes = false;
        protected bool test_all_mp_prefabs = false;
        protected bool test_running = false;
        protected bool test_showDisabledScenes = false;
        protected bool test_prev_showDisabledScenes = false;
        protected bool scanning = false;

        protected bool includeDisabledScenes = false;
        protected bool includedNotIncludedScenes = false;

        protected Vector2 teamData_scroll = Vector2.zero;
        protected Vector2 teamData_GridScroll = Vector2.zero;
        protected Vector2 scene_scroll = Vector2.zero;
        protected Vector2 convert_scene_scroll = Vector2.zero;
        protected Vector2 prefabs_scroll = Vector2.zero;
        protected Vector2 test_PrefabScroll = Vector2.zero;
        protected Vector2 characters_scroll = Vector2.zero;

        protected string progress_title = "";
        protected string progress_message = "";
        protected string status_message = "";
        protected string prefab_path = "";
        protected string newTeamName = "";
        protected string emi_supported_version = "";

        protected string output_path;
        protected string test_project_path;

        protected int teamData_selectedIndex = -1;
        protected int progress_id = 0;

        protected float progress_percent = 0;

        GUIStyle teamDataSelectionBackground = new GUIStyle();
        GUIStyle teamDataLabel = new GUIStyle();
        public GameObject character;
        protected ScriptableObject itemListData;

        #endregion

        #region Initilization
        [MenuItem("Easy Multiplayer - Invector/Open Navigator With Basic Tier Support")]
        public static void OpenBasicNavigator()
        {
            EditorWindow.GetWindow<BasicNavigator>("Easy Multiplayer - Invector", focus: true);
        }
        [MenuItem("Easy Multiplayer - Invector/Open Navigator With Basic Tier Support", true)]
        static bool ValidateOpenBasicNavigator()
        {
            string meleeCombat = FilesUtil.FindFolderPath("Melee Combat", "Assets/Invector-3rdPersonController");
            string shooter = "";
            if (Directory.Exists($"Assets{Path.DirectorySeparatorChar}Invector-3rdPersonController{Path.DirectorySeparatorChar}Shooter"))
                shooter = FilesUtil.FindFolderPath("Shooter", "Assets/Invector-3rdPersonController/Shooter");

            return string.IsNullOrEmpty(meleeCombat) && string.IsNullOrEmpty(shooter);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            DisableAll();
            converting = false;
            welcome = true;
            rightViewTitle = "Welcome To Easy Invector Multiplayer Basic";
            //teamDataSelectionBackground.normal.background = EditorUtils.MakeTex(Color.gray);
            teamDataLabel.alignment = TextAnchor.MiddleRight;
            teamDataLabel.normal.textColor = Color.grey;
            teamDataLabel.hover.textColor = Color.grey;
            teamDataLabel.active.textColor = Color.grey;

            test_project_path = "Assets";
            CheckInvectorVersion();
            CheckHasMirror();
            CheckScriptsConverted();
            CheckPackagesImported();
        }
        #endregion

        #region Right View Content Drawer
        protected override void RightViewContents()
        {
            if (showProgressBar)
            {
                EditorUtility.DisplayProgressBar(progress_title, progress_message, progress_percent);
            }
            else
            {
                if (welcome) WelcomeWindow();
                if (objects) ConvertObjectsWindow();
                if (scenes) ConvertScenesWindow();
                if (tests) PerformProjectTestsWindow();
                if (modTeams) ModifyTeamsWindow();
            }
        }
        #endregion

        #region Left Panel Drawer
        protected override void LeftPanelContents()
        {
            if (!scriptsConverted)
            {
                EditorGUILayout.HelpBox("You must first convert the invector scripts before you can do anything else.", MessageType.Error);
                return;
            }
            if (!convertedScriptsCompiled)
            {
                EditorGUILayout.HelpBox("You need to compile the scripts before continuing.", MessageType.Warning);
                return;
            }
            if (!packagesImported)
            {
                EditorGUILayout.HelpBox("You must import the multiplayer packages into your project before you can continue.", MessageType.Warning);
                GUILayout.Space(15);
                if (GUILayout.Button("Import Packages", leftButton))
                {
                    ImportPackages();
                }
                return;
            }
            if (converting)
            {
                EditorGUILayout.HelpBox("A process is running, please wait...", MessageType.None);
                return;
            }
            GUILayout.Space(10);
            if (GUILayout.Button("Convert Objects", leftButton))
            {
                DisableAll();
                rightViewTitle = "Convert Target Objects";
                objects = true;
            }
            if (GUILayout.Button("Convert Scenes", leftButton))
            {
                DisableAll();
                rightViewTitle = "Convert Selected Unity Scenes";
                scenes = true;
            }

            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("----------");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            if (GUILayout.Button("Perform Project Tests", leftButton))
            {
                DisableAll();
                rightViewTitle = "Perform Project Tests";
                tests = true;
            }

            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("----------");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            if (GUILayout.Button("Modify Team Data", leftButton))
            {
                DisableAll();
                rightViewTitle = "Modify Team Data";
                modTeams = true;
            }

            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("----------");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            if (GUILayout.Button("Available Transports", leftButton))
            {
                Application.OpenURL("https://mirror-networking.gitbook.io/docs/transports");
            }
        }
        #endregion

        #region Windows
        protected void WelcomeWindow()
        {
            ResetStyles();
            titleBarSize = 150f;
            rightViewTitle = "";
            titleBar = Resources.Load<Texture2D>("EasyMultiplayerBanner");
            EditorGUILayout.HelpBox("Don't know where to start? Look at the available YouTube playlist or the documentation to help you get started! This navigator will direct you through the first time import and setup process.\n\nAfter that you will have a series of options available to you to help you convert your invector project to multiplayer.", MessageType.Info);
            GUILayout.Space(15);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Go To Documentation Website", GUILayout.Width(300), GUILayout.Height(50)))
            {
                Application.OpenURL(DocumentationWebsite());
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Go To YouTube Playlists", GUILayout.Width(300), GUILayout.Height(50)))
            {
                Application.OpenURL(YouTubePlayList());
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (!hasCorrectVersion)
            {
                EditorGUILayout.HelpBox("You don't have a matching version of Invector in your project. After converting the Invector scripts this will most likely lead to errors that you will have to be manually resolved. If you're code savy and are willing to fix these errors yourself you can do this. Otherwise use this with the supported version of Invector.\n\n" +
                    $"Current Supported Invector Version: {emi_supported_version}\n" +
                    $"Current Invector Version In Project: {vInvectorWelcomeWindow._thirdPersonVersion}", MessageType.Error);
            }
            if (!hasMirror)
            {
                // help box
                GUILayout.Space(15);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                EditorGUILayout.HelpBox("You don't have Mirror in your project. Mirror is a free open-source library that this package is based on and requires. Visit their github page and download the package, import it into your project, and re-open this window to continue", MessageType.Error);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                // download link
                GUILayout.Space(15);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Download Mirror UnityPackage", GUILayout.Width(300), GUILayout.Height(50)))
                {
                    string startingFolder = FilesUtil.FindFolderPath("EasyMultiplayer-Invector");
                    string mirror_version_file_path = FilesUtil.FindFilePath("MIRROR_VERSION.txt");
                    StreamReader reader = new StreamReader(mirror_version_file_path);
                    string mirror_version = reader.ReadToEnd();
                    reader.Close();
                    Application.OpenURL("https://github.com/vis2k/Mirror/releases/tag/"+mirror_version);
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                return;
            }
            if (!scriptsConverted)
            {
                GUILayout.Space(15);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Convert Invector Scripts", GUILayout.Width(300), GUILayout.Height(50)))
                {
                    ConvertInvectorScripts();
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            else if (!convertedScriptsCompiled)
            {
                if (EditorApplication.isUpdating || EditorApplication.isCompiling)
                {
                    GUILayout.Space(15);
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.HelpBox("Editor is currently compiling, please wait...", MessageType.Info);
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    if (!isCompiling) isCompiling = true;
                }
                else
                {
                    GUILayout.Space(15);
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.HelpBox("You need to force Unity to recompile the newly converted scripts. Click out of the Unity editor and back into it in order to force the editor to recompile. Then you can continue.", MessageType.Warning);
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
            }
        }
        protected virtual void ConvertObjectsWindow()
        {
            ResetStyles();
            titleBar = Resources.Load<Texture2D>("ConvertObjects");
            titleBarSize = 120f;
            rightViewTitle = "";
            
            if (converting)
            {
                GUILayout.FlexibleSpace();
                EditorGUILayout.HelpBox("Currently converting selected objects, please wait...", MessageType.Info);
                GUILayout.FlexibleSpace();
                return;
            }
            EditorGUILayout.HelpBox("This feature is used to convert objects to be multiplayer. If you search for prefabs automatically in your project you can change the path below to only scan target directories. By default it will scan your entire project (Except the Invector directories). \n\n It's important to know that this can find already converted prefabs when it scans. If you don't want to reconvert a prefab just remove it from the list prior to clicking the convert button.", MessageType.Info);
                
            includeInvDir = EditorGUILayout.Toggle("Include Invector Directory", includeInvDir);
            unpack = EditorGUILayout.Toggle("Unpack Prefabs Objects", unpack);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Recursively Scan Directory", GUILayout.Width(200)))
            {
                test_project_path = EditorUtility.OpenFolderPanel("Select A Directory To Scan", "", "");
            }
            test_project_path = EditorGUILayout.TextField("", test_project_path.Replace(Application.dataPath, "Assets"));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (string.IsNullOrEmpty(output_path))
            {
                output_path = "Assets/Converted";
            }
            if (GUILayout.Button("Saved To Directory", GUILayout.Width(200)))
            {
                output_path = EditorUtility.OpenFolderPanel("Select A Directory To Save To", "", "");
            }
            output_path = EditorGUILayout.TextField("", output_path.Replace(Application.dataPath, "Assets"));
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            // Character Input
            character = (GameObject)EditorGUILayout.ObjectField("Object", character, typeof(GameObject), true);
            itemListData = (ScriptableObject)EditorGUILayout.ObjectField("ItemListData", itemListData, typeof(ScriptableObject), true);
            GUILayout.Space(10);

            // Scan Prefabs Button
            if (!scanning)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Scan Project For Convertable Prefabs", GUILayout.Height(50)))
                {
                    scanning = true;
                    this.StartCoroutine(GetObjectPrefabs(test_project_path));
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.HelpBox($"Currently scanning your project for character prefabs at path:\n{test_project_path}\nPlease wait...", MessageType.Info);
            }

            // Display Results
            GUILayout.Space(10);
            if (objectPaths.Count > 0)
            {
                EditorGUILayout.BeginHorizontal("box");
                EditorGUILayout.BeginVertical();

                characters_scroll = EditorGUILayout.BeginScrollView(characters_scroll);
                List<string> tmpCharPaths = new List<string>();
                tmpCharPaths.AddRange(objectPaths);
                tmpCharPaths.Sort();
                for (int i = 0; i < tmpCharPaths.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("X", GUILayout.Width(20)))
                    {
                        objectPaths.Remove(tmpCharPaths[i]);
                    }
                    if (GUILayout.Button(tmpCharPaths[i].Replace(Application.dataPath, "Assets").FlattenDirectories()))
                    {
                        UnityEngine.Object target = AssetDatabase.LoadAssetAtPath(tmpCharPaths[i], typeof(UnityEngine.Object));
                        Selection.activeObject = target;
                        EditorGUIUtility.PingObject(target);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                tmpCharPaths.Clear();
                EditorGUILayout.EndScrollView();

                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }
            else if (character != null || itemListData != null)
            {
                EditorGUILayout.HelpBox("Currently have a targeted object(s). You can click the convert button to convert this single object and save it as a prefab in the output path.", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("Missing Any Object - Either drag a scene object or prefab into the object slot or scan your project for convertable prefabs to get started", MessageType.Warning);
            }
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Purge Object Selections", GUILayout.Height(30), GUILayout.Width(200)))
            {
                objectPaths.Clear();
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Convert Object Selections", GUILayout.Height(30), GUILayout.Width(200)))
            {
                this.StartCoroutine(ConvertObjects());
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
        }
        protected virtual void ConvertScenesWindow()
        {
            ResetStyles();
            titleBar = Resources.Load<Texture2D>("ConvertScenes");
            titleBarSize = 120f;
            rightViewTitle = "";
            if (converting)
            {
                GUILayout.FlexibleSpace();
                EditorGUILayout.HelpBox("Converting scenes, please wait...", MessageType.Info);
                GUILayout.FlexibleSpace();
                return;
            }
            EditorGUILayout.HelpBox("This feature will allow you to automatically convert selected scene objects to support multiplayer. This might take some manual tweaking after the fact to make sure it performs exactly the way you want it to.\n\nNOTE: This will not scan the Mirror directory.", MessageType.Info);
            EditorGUILayout.HelpBox("Be warned, this will convert scenes in place. Meaning it does not copy the scene prior to conversion. If you're worried about this breaking your scene you need to make a backup first.", MessageType.Warning);
            GUILayout.Space(10);
            includeDisabledScenes = EditorGUILayout.Toggle("Include Disabled Scenes", includeDisabledScenes);
            includedNotIncludedScenes = EditorGUILayout.Toggle("Include None Build Scenes", includedNotIncludedScenes);
            unpack = EditorGUILayout.Toggle("Unpack Prefabs Objects", unpack);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Search For Scenes", GUILayout.Width(200), GUILayout.Height(50)))
            {
                convertScenes.Clear();
                convertScenes.AddRange(SceneConvert.FindBuildScenes(includeDisabledScenes));
                if (includedNotIncludedScenes)
                {
                    convertScenes.AddRange(SceneConvert.FindAllScenesInProject());
                    convertScenes = convertScenes.Distinct().ToList();
                }
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            if (convertScenes.Count > 0)
            {
                convert_scene_scroll = EditorGUILayout.BeginScrollView(convert_scene_scroll);
                List<string> tmp = new List<string>();
                tmp.AddRange(convertScenes);
                for(int i = 0; i < tmp.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("X", GUILayout.Width(20)))
                    {
                        convertScenes.RemoveAt(i);
                    }
                    if (GUILayout.Button(Path.GetFileNameWithoutExtension(tmp[i])))
                    {
                        UnityEngine.Object scene = AssetDatabase.LoadAssetAtPath(tmp[i].Replace(Application.dataPath, "Assets"), typeof(UnityEngine.Object));
                        Selection.activeObject = scene;
                        EditorGUIUtility.PingObject(scene);
                    }
                    GUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
            }
            else
            {
                GUILayout.FlexibleSpace();
                EditorGUILayout.HelpBox("No scenes currently selected. Click Search For Scenes to populate the list of selectable scenes.", MessageType.Info);
                GUILayout.FlexibleSpace();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Purge Scene Selections", GUILayout.Height(30)))
            {
                convertScenes.Clear();
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Load And Convert Selected Scenes", GUILayout.Height(30)))
            {
                this.StartCoroutine(ConvertScenes());
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5);
        }
        protected virtual void PerformProjectTestsWindow()
        {
            ResetStyles();
            titleBar = Resources.Load<Texture2D>("ProjectTests");
            titleBarSize = 180f;
            rightViewTitle = "";
            
            // Test Prefabs
            if (!test_running)
            {
                EditorGUILayout.HelpBox("This will test your desired selections for things that could potentially cause issues in your final build of your project.\n\n" +
                    "Important Note: Tests are not run on the \"Assets\\Invector-3rdPersonController\" directory.", MessageType.Info);

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Test In Directory"))
                {
                    test_project_path = EditorUtility.OpenFolderPanel("Select A Directory To Run Tests In", "","");
                }
                test_project_path = EditorGUILayout.TextField("", test_project_path.Replace(Application.dataPath, "Assets"));
                if (GUILayout.Button("Open Previous Test Results"))
                {
                    TestResults.OpenTestResultsWindow("Previous Test Results");
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
                test_all_mp_prefabs = EditorGUILayout.Toggle("Test Prefabs", test_all_mp_prefabs, GUILayout.Width(170));
                if (test_prev_prefabs != test_all_mp_prefabs)
                {
                    test_prev_prefabs = test_all_mp_prefabs;
                    if (test_all_mp_prefabs)
                    {
                        // search asset database
                        test_prefabsRefreshing = true;
                        PopulatePrefabsList();
                    }
                }
                if (test_all_mp_prefabs)
                {
                    if (test_prefabsRefreshing)
                    {
                        EditorGUILayout.HelpBox("Searching for prefabs in your project, please wait...", MessageType.Info);
                    }
                    else if (test_prefabPaths.Count < 1)
                    {
                        EditorGUILayout.HelpBox("No multiplayer prefabs found in your project.", MessageType.None);
                    }
                    else
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Test Prefab", GUILayout.Width(80));
                        EditorGUILayout.LabelField("Prefab Path");
                        EditorGUILayout.EndHorizontal();

                        test_PrefabScroll = EditorGUILayout.BeginScrollView(test_PrefabScroll);
                        var list = whatPrefabsToScan.Keys.ToList();
                        list.Sort();
                        foreach (string keyName in list)
                        {
                            string displayPath = "";
                            if (new DirectoryInfo(keyName).Parent != null)
                            {
                                if (new DirectoryInfo(keyName).Parent.Parent != null)
                                {
                                    if (new DirectoryInfo(keyName).Parent.Parent.Parent != null)
                                    {
                                       displayPath = ".../"+new DirectoryInfo(keyName).Parent.Parent.Name + "/" + new DirectoryInfo(keyName).Parent.Name + "/" + Path.GetFileNameWithoutExtension(keyName);
                                    }
                                    else
                                    {
                                        displayPath = new DirectoryInfo(keyName).Parent.Parent.Name + "/" + new DirectoryInfo(keyName).Parent.Name + "/" + Path.GetFileNameWithoutExtension(keyName);
                                    }
                                }
                                else
                                {
                                    displayPath = new DirectoryInfo(keyName).Parent.Name + "/" + new DirectoryInfo(keyName).Parent.Name + "/" + Path.GetFileNameWithoutExtension(keyName);
                                }
                            }
                            else
                            {
                                displayPath = Path.GetFileNameWithoutExtension(keyName);
                            }
                            EditorGUILayout.BeginHorizontal("box");
                            GUILayout.Space(25);
                            whatPrefabsToScan[keyName] = EditorGUILayout.Toggle(whatPrefabsToScan[keyName], GUILayout.Width(20));
                            GUILayout.Space(30);
                            float originalValue = EditorGUIUtility.labelWidth;
                            EditorGUIUtility.labelWidth = position.x - 55;
                            EditorGUILayout.LabelField(displayPath, GUILayout.ExpandWidth(true));
                            EditorGUIUtility.labelWidth = originalValue;
                            GUILayout.FlexibleSpace();
                            EditorGUILayout.EndHorizontal();
                            GUILayout.Space(1);
                        }
                        list.Clear();
                        EditorGUILayout.EndScrollView();
                    }
                }

                // Select Scenes
                test_scenes = EditorGUILayout.Toggle("Test Scenes", test_scenes);
                if (test_scenes)
                {
                    EditorGUILayout.HelpBox("This will addively load all selected scenes into your hierarchy in order to scan its objects. Be sure to not load too many scenes at once so as to not overload your computer if it can't handle all of them at once.", MessageType.Warning);
                    if (test_prev_scenes != test_scenes || test_prev_showDisabledScenes != test_showDisabledScenes)
                    {
                        test_prev_scenes = test_scenes;
                        test_prev_showDisabledScenes = test_showDisabledScenes;
                        whatScenesToScan.Clear();
                        if (test_scenes)
                        {
                            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
                            {
                                if (!test_showDisabledScenes && !scene.enabled)
                                    continue;
                                whatScenesToScan.Add(scene.path, true);
                            }
                        }
                    }
                    test_showDisabledScenes = EditorGUILayout.Toggle("Show Disabled Scenes", test_showDisabledScenes);
                    GUILayout.Space(10);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Test Scene", GUILayout.Width(80));
                    EditorGUILayout.LabelField("Scene Name");
                    EditorGUILayout.EndHorizontal();

                    scene_scroll = EditorGUILayout.BeginScrollView(scene_scroll);
                    Dictionary<string, bool> tmp = new Dictionary<string, bool>();
                    foreach (KeyValuePair<string, bool> item in whatScenesToScan)
                    {
                        tmp.Add(item.Key, item.Value);
                    }
                    foreach (string sceneName in tmp.Keys)
                    {
                        EditorGUILayout.BeginHorizontal("box");
                        GUILayout.Space(25);
                        whatScenesToScan[sceneName] = EditorGUILayout.Toggle(whatScenesToScan[sceneName], GUILayout.Width(20));
                        GUILayout.Space(30);
                        float originalValue = EditorGUIUtility.labelWidth;
                        EditorGUIUtility.labelWidth = position.x - 55;
                        EditorGUILayout.LabelField(Path.GetFileNameWithoutExtension(sceneName), GUILayout.ExpandWidth(true));
                        EditorGUIUtility.labelWidth = originalValue;
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.EndHorizontal();
                        GUILayout.Space(1);
                    }
                    tmp.Clear();
                    EditorGUILayout.EndScrollView();
                }

                // Show Actions Box
                GUILayout.Space(10);
                EditorGUILayout.HelpBox($"Testing Below Project Settings In The Above \"Project Path\"\n\n" +
                    $"Testing Selected Prefab In Your Project: {test_all_mp_prefabs}\n" +
                    $"Testing Selected Scenes: {test_scenes}", MessageType.None);
                GUILayout.Space(10);

                // Start Test Button
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Perform Tests", GUILayout.Width(300), GUILayout.Height(50)))
                {
                    PerformProjectTests();
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(5);
            }
            else
            {
                GUILayout.FlexibleSpace();
                EditorGUILayout.HelpBox("Tests are currently running...\nPlease wait for these tests to complete. \n\nDo not close this window otherwise you will lose your test results.", MessageType.Warning);
                GUILayout.FlexibleSpace();
            }
        }
        protected virtual void ModifyTeamsWindow()
        {
            ResetStyles();

            // Title bar
            titleBar = Resources.Load<Texture2D>("ModTeamData");
            rightViewTitle = "";
            titleBarSize = 150f;

            // Help Box
            EditorGUILayout.HelpBox("Use this to modify existing TeamData files. TeamData files are what are used to determine teams and their corresponding enemy teams. This data will be used with the NetworkManager or directly with Team components.\n\n" +
                "To create a new TeamData file, right click in the project explorer and select Create > EMI > Generate New Team Data", MessageType.Info);
            
            // Find Data Files Button
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Find Team Data Files", GUILayout.Width(200), GUILayout.Height(50)))
            {
                teamData_selectedIndex = -1;
                ModifyTeam.teamData = null;
                teamDataPaths.Clear();
                teamDataPaths.AddRange(ModifyTeam.FindTeamDataPaths());
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            // Scroll List Team Data Selection
            if (teamDataPaths.Count > 0)
            {
                if (teamData_selectedIndex != -1)
                {
                    ModifyTeam.teamData = AssetDatabase.LoadAssetAtPath<TeamData>(teamDataPaths[teamData_selectedIndex]);
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.BeginVertical();
                    teamData_scroll = EditorGUILayout.BeginScrollView(teamData_scroll);
                    // team data selection list
                    for (int i = 0; i < teamDataPaths.Count; i++)
                    {
                        string buttonName = Path.GetFileNameWithoutExtension(teamDataPaths[i]);
                        if (teamData_selectedIndex == i)
                        {
                            GUILayout.BeginHorizontal(teamDataSelectionBackground);
                            GUILayout.Button(buttonName);
                            GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal();
                        }
                        else
                        {
                            GUILayout.BeginHorizontal();
                            if (GUILayout.Button(buttonName))
                            {
                                teamData_selectedIndex = i;
                            }
                            GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal();
                        }
                    }
                    EditorGUILayout.EndScrollView();
                    GUILayout.EndVertical();
                    GUILayout.FlexibleSpace();
                    GUILayout.EndScrollView();
                }
            }

            // Team Data
            if (ModifyTeam.teamData)
            {
                // draw team name title
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(ModifyTeam.teamData.name, EditorStyles.largeLabel);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                // draw team checkbox grid
                teamData_GridScroll = GUILayout.BeginScrollView(teamData_GridScroll);

                // Rotated labels
                Rect rotatePos = new Rect(94, 34, 100, 50);
                GUILayout.Space(20);
                foreach (TeamInfo teamInfo in ModifyTeam.teamData.teams)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUIUtility.RotateAroundPivot(90f, rotatePos.center);
                    EditorGUI.LabelField(rotatePos, teamInfo.name.Truncate(15), teamDataLabel);
                    EditorGUIUtility.RotateAroundPivot(-90f, rotatePos.center);
                    rotatePos.x += 19.7f;
                    EditorGUILayout.EndHorizontal();
                }

                // Normal labels
                GUILayout.Space(95);
                EditorStyles.label.alignment = TextAnchor.MiddleRight;
                foreach (TeamInfo teamInfo in ModifyTeam.teamData.teams)
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(" - ", GUILayout.Width(20)))
                    {
                        ModifyTeam.RemoveTeam(teamInfo.name);
                        break;
                    }
                    GUILayout.Label(teamInfo.name, GUILayout.Width(100));
                    GUILayout.Space(3);
                    for (int i = 0; i < ModifyTeam.teamData.teams.Count; i++)
                    {
                        bool value = teamInfo.enemyTeams.Contains(ModifyTeam.teamData.teams[i].name);
                        bool newValue = EditorGUILayout.Toggle(value, GUILayout.Width(17));
                        if (value != newValue)
                        {
                            if (newValue == true)
                                teamInfo.enemyTeams.Add(ModifyTeam.teamData.teams[i].name);
                            else if (value == true)
                                teamInfo.enemyTeams.Remove(ModifyTeam.teamData.teams[i].name);
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("+ Add Team", GUILayout.Width(100)))
                {
                    if (!string.IsNullOrEmpty(newTeamName))
                    {
                        if (newTeamName.ToCharArray().ToList().Contains(' '))
                        {
                            EditorUtility.DisplayDialog("Team Name Error!", "The team name has spaces, it is not allow to have spaces", "Okay");
                        }
                        else
                        {
                            ModifyTeam.AddTeam(newTeamName);
                            newTeamName = "";
                        }
                    }
                }
                newTeamName = EditorGUILayout.TextField(newTeamName);
                GUILayout.EndHorizontal();

                GUILayout.EndScrollView();

                // Delete Team Data Button
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.BeginVertical();
                if (GUILayout.Button("Delete This Team Data", GUILayout.Width(200)))
                {
                    if (EditorUtility.DisplayDialog("Are You Sure?", "Are you sure you want to completely delete this TeamData set from your project? There is no coming back from this.", "Yes", "No"))
                    {
                        File.Delete(teamDataPaths[teamData_selectedIndex]);
                        string fullFileName = Path.GetFileName(teamDataPaths[teamData_selectedIndex]);
                        string metaFileName = Path.GetFileNameWithoutExtension(teamDataPaths[teamData_selectedIndex]) + ".meta";
                        File.Delete(teamDataPaths[teamData_selectedIndex].Replace(fullFileName, metaFileName));
                        AssetDatabase.Refresh();
                    }
                }
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
            }
        }
        #endregion

        #region Helpers
        protected virtual void DisableAll()
        {
            rightViewTitle = "Welcome To Easy Invector Multiplayer\nBasic";
            welcome = false;
            scenes = false;
            objects = false;
            tests = false;
            modTeams = false;
        }
        #endregion

        #region Actions
        #region Checks
        protected virtual void AfterScriptsCompiled()
        {
            AssemblyReloadEvents.afterAssemblyReload -= AfterScriptsCompiled;
            isCompiling = false;
            string scriptConvertedFile = FilesUtil.FindFolderPath("EasyMultiplayer-Invector");
            scriptConvertedFile += "/Basic Locomotion/Editor/ScriptsConverted.txt";
            if (File.Exists(scriptConvertedFile))
            {
                scriptsConverted = true;
                StreamWriter writer = new StreamWriter(scriptConvertedFile, false);
                writer.WriteLine("true");
                writer.Close();
                convertedScriptsCompiled = true;
            }
        }
        protected virtual void CheckHasMirror()
        {
            string mirrorPath = FilesUtil.FindFolderPath("Mirror");
            if (!string.IsNullOrEmpty(mirrorPath))
            {
                hasMirror = true;
            }
        }
        protected virtual void CheckScriptsConverted()
        {
            string scriptConvertedFile = FilesUtil.FindFolderPath("EasyMultiplayer-Invector");
            scriptConvertedFile += "/Basic Locomotion/Editor/ScriptsConverted.txt";
            if (File.Exists(scriptConvertedFile))
            {
                scriptsConverted = true;
                StreamReader reader = new StreamReader(scriptConvertedFile);
                string contents = reader.ReadToEnd();
                contents = (string.IsNullOrEmpty(contents)) ? "false" : contents;
                convertedScriptsCompiled = bool.Parse(contents.Trim());
                if (!convertedScriptsCompiled)
                {
                    AssemblyReloadEvents.afterAssemblyReload += AfterScriptsCompiled;
                }
                reader.Close();
            }
        }
        protected virtual void CheckPackagesImported()
        {
            string importedMemoryFile = FilesUtil.FindFolderPath("EasyMultiplayer-Invector");
            importedMemoryFile += "/Basic Locomotion/Editor/Imported.txt";
            packagesImported = File.Exists(importedMemoryFile);
        }
        protected virtual void CheckInvectorVersion()
        {
            string versionPath = FilesUtil.FindFilePath("INVECTOR_VERSION.txt");
            StreamReader reader = null;
            try
            {
                reader = new StreamReader(versionPath);
                emi_supported_version = reader.ReadToEnd();
            }
            finally
            {
                reader.Close();
            }
            if (emi_supported_version.Trim() == vInvectorWelcomeWindow._thirdPersonVersion.Trim())
                hasCorrectVersion = true;
            else
                hasCorrectVersion = false;
        }
        #endregion

        #region Converting Invector Scripts
        protected virtual void ConvertInvectorScripts()
        {
            this.StartCoroutine(Convert());
        }
        IEnumerator Convert()
        {
            showProgressBar = true;
            Repaint();
            yield return new WaitForSeconds(0.1f);
            BasicScriptConvert converter = new BasicScriptConvert();
            yield return this.StartCoroutine(converter.Run(FinishedScriptConvert, true, ProgressUpdate));
            yield return null;
        }
        protected virtual void FinishedScriptConvert()
        {
            string scriptConvertedFile = FilesUtil.FindFolderPath("EasyMultiplayer-Invector");
            scriptConvertedFile += "/Basic Locomotion/Editor/ScriptsConverted.txt";
            File.WriteAllText(scriptConvertedFile, "false");
            scriptsConverted = true;
        }
        protected virtual void ProgressUpdate(string title, string action, float percent)
        {
            showProgressBar = (percent < 1) ? true : false;
            progress_title = title;
            progress_message = action;
            progress_percent = percent;
            if (!showProgressBar) EditorUtility.ClearProgressBar();
            Repaint();
        }
        #endregion

        #region Import Packages
        protected virtual void ImportPackages()
        {
            string packagePath = FilesUtil.FindFolderPath("EasyMultiplayer-Invector");
            string memoryFilePath = $"{packagePath}/Basic Locomotion/Editor/Imported.txt";
            packagePath += "/Basic Locomotion/Editor/Contents.unitypackage";
            if (File.Exists(packagePath))
            {
                string vInputPath = FilesUtil.FindFilePath("vInput.cs");
                FilesUtil.CommentOutFile(vInputPath);
                AssetDatabase.ImportPackage(packagePath, false);
                File.WriteAllText(memoryFilePath, "true");
                packagesImported = true;
            }
            else
            {
                Debug.LogError($"Looks like the package is missing from your project. Expected to find it at the following location: {packagePath}");
            }
        }
        #endregion

        #region Converting
        protected virtual IEnumerator ConvertScenes()
        {
            converting = true;
            Repaint();
            yield return new WaitForSeconds(0.1f);
            SceneConvert.ConvertScenes(convertScenes, unpack);
            converting = false;
            Repaint();
        }
        protected virtual IEnumerator ConvertObjects()
        {
            converting = true;
            Repaint();
            yield return new WaitForSeconds(0.1f);
            ObjectConverter converter = new ObjectConverter();
            if (character != null)
            {
                converter.ExecuteConvertObject(character, output_path, unpack);
            }
            if (objectPaths.Count > 0)
            {
                converter.ExecuteConvertPrefabs(objectPaths, output_path, unpack);
            }
            if (itemListData != null)
            {
                converter.ExecuteConvertSO(itemListData, output_path);
            }
            Repaint();
            if (objectPaths.Count < 1 && itemListData == null && character != null && character.scene != null)
            {
                EditorUtility.DisplayDialog("Finished Scene Object Conversion", "The scene object's components have been successfully replaced with the multiplayer versions.", "Ok");
            }
            else if (objectPaths.Count < 1 && itemListData != null && character != null && character.scene != null)
            {
                EditorUtility.DisplayDialog("Finished Conversions", "The selected scene object has been converted in your scene.Also the item list data has been copied, converted, and saved to your selected output path.", "Ok");
            }
            else if (objectPaths.Count > 0 && itemListData != null && character != null && character.scene != null)
            {
                EditorUtility.DisplayDialog("Finished Conversions", "The selected scene object has been converted in your scene. Also the item list data and the selected prefabs has been copied, converted, and saved to your selected output path.", "Ok");
            }
            else
            {
                EditorUtility.DisplayDialog("Finished Conversions", "The selected prefabs have been copied, converted, and saved to your selected output path.", "Ok");
            }
            converting = false;
        }
        protected virtual IEnumerator GetObjectPrefabs(string startingPath)
        {
            objectPaths.Clear();
            Repaint();
            yield return new WaitForSeconds(0.1f);
            objectPaths.AddRange(EditorUtils.GetPrefabPathsWithComponents(ConvertableComponents(), startingPath, ignorePath: (includeInvDir) ? "" : "Assets/Invector-3rdPersonController", ignoreMPComps: true));
            scanning = false;
        }
        protected virtual List<string> ConvertableComponents()
        {
            List<string> comps = new List<string>();
            comps.AddRange(ComponentsList());
            comps.Remove("Revive");
            comps.Remove("Team");
            comps.Remove("NetworkIdentity");
            comps.Remove("ServerSync");
            comps.Remove("ClientConnection");
            comps.Remove("DebugConsole");
            comps.Remove("TriggerChangeTeam");
            comps.Remove("BasicNetworkCalls");
            comps.Remove("EMI_NetworkManager");
            comps.Add("vPickupItem");
            comps.Add("vObjectDamage");
            return comps;
        }
        #endregion

        #region Testing
        protected virtual void PopulatePrefabsList()
        {
            whatPrefabsToScan.Clear();
            test_prefabPaths.Clear();
            this.StartCoroutine(PopulateBasicList());
        }
        protected virtual List<string> ComponentsList()
        {
            return new List<string> {
                "vThirdPersonController",
                "vTutorialTextTrigger",
                "EMI_NetworkManager",
                "vThirdPersonCamera",
                "TriggerChangeTeam",
                "ClientConnection",
                "DebugConsole",
                "vHitDamageParticle",
                "BasicNetworkCalls",
                "ServerSync",
                "vThirdPersonInput",
                "vLadderAction",
                "vHeadTrack",
                "vGenericAction",
                "Revive",
                "Team",
                "NetworkIdentity",
                "vHUDController"
            };
        }
        protected IEnumerator PopulateBasicList()
        {
            Repaint();
            yield return new WaitForSeconds(0.1f);
            try
            {
                test_prefabPaths.AddRange(EditorUtils.GetPrefabPathsWithComponents(ComponentsList(), test_project_path));
                test_prefabPaths.ForEach(x => whatPrefabsToScan.Add(x, true));
            }
            finally
            {
                test_prefabsRefreshing = false;
                Repaint();
            }
            yield return null;
        }
        protected void PerformProjectTests()
        {
            test_running = true;
            Repaint();
            ExecuteTests();
        }
        protected virtual void ExecuteTests()
        {
            BasicTestActions bt = (BasicTestActions)ScriptableObject.CreateInstance("BasicTestActions");
            bt.PerformTests(whatScenesToScan, whatPrefabsToScan, TestCompleted);
        }
        protected void TestCompleted()
        {
            test_running = false;
            Repaint();
        }
        #endregion
        #endregion
    }
}
