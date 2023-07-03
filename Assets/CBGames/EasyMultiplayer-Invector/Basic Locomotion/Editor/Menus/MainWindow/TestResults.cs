using Common.Utils;
using EMI.Editors;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EMI.Menus
{
    [System.Serializable]
    public struct TestResultsWrapper
    {
        public List<TestResult> results;
    }
    public class TestResults : EditorWindow
    {
        protected static List<TestResult> results;
        protected static List<TestResult> prefabResults = new List<TestResult>();
        protected static Dictionary<string, List<TestResult>> sceneResults = new Dictionary<string, List<TestResult>>();
        protected static Dictionary<string, bool> showSelectedScenes = new Dictionary<string, bool>();

        protected bool showSceneSelection = false;
        protected bool showPrefabs = false;
        protected bool showPassed = true;
        protected bool showWarnings = true;
        protected bool showCritical = true;
        protected List<string> testedScenes = new List<string>();

        protected Vector2 prefabScrollView = Vector2.zero;
        protected Vector2 sceneScrollView = Vector2.zero;


        public static void OpenTestResultsWindow(string windowName = "Basic Test Results")
        {
            EditorWindow.GetWindow<TestResults>(windowName, focus: true);
        }

        public static void SetNewTestResults(List<TestResult> newResults, bool saveResults = true)
        {
            if (results != null)
            {
                results.Clear();
            }
            else
            {
                results = new List<TestResult>();
            }
            results = newResults;
            prefabResults.Clear();
            sceneResults.Clear();
            showSelectedScenes.Clear();
            foreach (TestResult result in results)
            {
                if (string.IsNullOrEmpty(result.sceneName))
                {
                    prefabResults.Add(result);
                }
                else
                {
                    if (!sceneResults.ContainsKey(result.sceneName))
                    {
                        List<TestResult> tmp = new List<TestResult>();
                        tmp.Add(result);
                        sceneResults.Add(result.sceneName, tmp);
                    }
                    else
                    {
                        sceneResults[result.sceneName].Add(result);
                    }

                    if (!showSelectedScenes.ContainsKey(result.sceneName))
                    {
                        showSelectedScenes.Add(result.sceneName, false);
                    }
                }
            }

            if (saveResults)
            {
                // delete the file if it already exists
                string filePath = FilesUtil.FindFilePath("ProjectTestResults.json");
                if (!string.IsNullOrEmpty(filePath))
                {
                    File.Delete(filePath);
                }

                // save to EasyMultiplayer-Invector/Basic Locomotion/Editor/ProjectTestResults.json
                string emi_folder = FilesUtil.FindFolderPath("EasyMultiplayer-Invector");
                string bl_folder = FilesUtil.FindFolderPath("Basic Locomotion", emi_folder);
                string editor_folder = FilesUtil.FindFolderPath("Editor", bl_folder);
                StreamWriter writer = null;
                try
                {
                    TestResultsWrapper wrapper = new TestResultsWrapper();
                    wrapper.results = results;
                    writer = new StreamWriter($"{editor_folder}{Path.DirectorySeparatorChar}ProjectTestResults.json", false);
                    writer.WriteLine(JsonUtility.ToJson(wrapper));
                }
                finally
                {
                    writer.Close();
                }
            }
        }

        protected virtual void LoadSavedResults()
        {
            string filePath = FilesUtil.FindFilePath("ProjectTestResults.json");
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                StreamReader reader = null;
                try
                {
                    reader = new StreamReader(filePath);
                    TestResultsWrapper wrapper = JsonUtility.FromJson<TestResultsWrapper>(reader.ReadToEnd());
                    SetNewTestResults(wrapper.results, false);
                }
                finally
                {
                    reader.Close();
                }
            }
        }

        protected virtual void OnEnable()
        {
            LoadSavedResults();
        }

        protected virtual void OnGUI()
        {
            // Try loading contents
            if (results == null)
            {
                LoadSavedResults();
            }

            // Symbol Descriptions
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(EditorGUIUtility.IconContent("d_greenLight"), GUILayout.Width(20));
            EditorGUILayout.HelpBox("This symbol means all is well. It passed all the tests as expected.", MessageType.None);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(EditorGUIUtility.IconContent("d_orangeLight"), GUILayout.Width(20));
            EditorGUILayout.HelpBox("This symbol means this might have unexpected consquences. It wasn't a full pass but it wasn't a full failure either. You need to review this to make sure this is doing what you want it to do.", MessageType.None);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(EditorGUIUtility.IconContent("d_redLight"), GUILayout.Width(20));
            EditorGUILayout.HelpBox("This symbol means a full failure occured. This failed the test and will most likely result in errors at runtime if not fixed by you.", MessageType.None);
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            // Prefab or Scene Selector Buttons
            GUILayout.BeginHorizontal();
            showPrefabs = GUILayout.Toggle(showPrefabs, "Show Prefabs Results", "Button");
            showSceneSelection = GUILayout.Toggle(showSceneSelection, "Show Scene Results", "Button");
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            // Passed, Warning, Critical buttons
            GUILayout.BeginHorizontal();
            showPassed = GUILayout.Toggle(showPassed, "Show Passed", "BUTTON");
            showWarnings = GUILayout.Toggle(showWarnings, "Show Warnings", "Button");
            showCritical = GUILayout.Toggle(showCritical, "Show Critical", "Button");
            GUILayout.EndHorizontal();
            
            // Make sure some results exist
            if (results.Count < 1)
            {
                EditorGUILayout.HelpBox("There are no test results to display. Run the \"Perform Project Tests\" to get results.", MessageType.Info);
            }
            else
            {
                // Display Scenes
                if (showSceneSelection)
                {
                    int rowCount = 0;
                    Dictionary<string, bool> tmp = new Dictionary<string, bool>();
                    foreach(KeyValuePair<string, bool> item in showSelectedScenes)
                    {
                        tmp.Add(item.Key, item.Value);
                    }
                    GUILayout.Space(10);

                    // Select To Display Scene
                    int i = 0;
                    foreach(KeyValuePair<string, bool> item in tmp)
                    {
                        if (rowCount == 0)
                            GUILayout.BeginHorizontal();
                        showSelectedScenes[item.Key] = GUILayout.Toggle(showSelectedScenes[item.Key], Path.GetFileNameWithoutExtension(item.Key), "button");
                        rowCount++;
                        if (rowCount > 2 || (i+1) >= tmp.Count)
                        {
                            GUILayout.EndHorizontal();
                            rowCount = 0;
                        }
                        i++;
                    }
                    tmp.Clear();

                    // Display Selected Scenes
                    EditorGUILayout.LabelField("Tests Scenes");
                    sceneScrollView = EditorGUILayout.BeginScrollView(sceneScrollView);
                    foreach (KeyValuePair<string, List<TestResult>> item in sceneResults)
                    {
                        if (showSelectedScenes[item.Key] == true)
                        {
                            EditorGUILayout.LabelField(Path.GetFileNameWithoutExtension(item.Key), EditorStyles.boldLabel);
                            foreach(TestResult result in sceneResults[item.Key].OrderByDescending(x => (int)(x.result)).ToList())
                            {
                                DrawResultEntry(result);
                            }
                            GUILayout.Space(10);
                        }
                    }
                    EditorGUILayout.EndScrollView();
                }
                if (showPrefabs)
                {
                    EditorGUILayout.LabelField("Tests Prefabs");
                    prefabScrollView = EditorGUILayout.BeginScrollView(prefabScrollView);
                    foreach (TestResult result in results.OrderByDescending(x => (int)(x.result)).ToList())
                    {
                        if (string.IsNullOrEmpty(result.sceneName))
                        {
                            DrawResultEntry(result);
                        }
                    }
                    EditorGUILayout.EndScrollView();
                }
                EditorGUILayout.HelpBox($"Total Tested Components: {results.Count}", MessageType.None);
            }
        }

        protected virtual void DrawResultEntry(TestResult result)
        {
            bool draw = false;
            if (showPassed && result.result == TestResultType.Pass)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(EditorGUIUtility.IconContent("d_greenLight"), GUILayout.Width(20));
                draw = true;
            }
            else if (showWarnings && result.result == TestResultType.Warning)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(EditorGUIUtility.IconContent("d_orangeLight"), GUILayout.Width(20));
                draw = true;
            }
            else if (showCritical && result.result == TestResultType.Critical)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(EditorGUIUtility.IconContent("d_redLight"), GUILayout.Width(20));
                draw = true;
            }
            if (draw)
            {
                if (GUILayout.Button("Highlight Object", GUILayout.Width(100)))
                {
                    if (result.target != null)
                    {
                        try
                        {
                            Selection.activeObject = result.target;
                            EditorGUIUtility.PingObject(result.target);
                        }
                        catch
                        {
                            Debug.LogError($"Failed to find the target object: {result.target}. It could be that this test result set is out of date and the scene has changed or the prefab has since been removed.");
                        }
                    }
                }
                EditorGUILayout.HelpBox($"{result.compName}({result.target.name})\n{result.details}", MessageType.None);
                GUILayout.EndHorizontal();
            }
        }
    }
}
