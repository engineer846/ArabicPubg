using EMI.Editors;
using Unity.EditorCoroutines.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EMI.Menus
{
    public partial class ShooterTestActions : EditorWindow
    {
        partial void ShooterTests(Dictionary<string, bool> whatScenesToScan, Dictionary<string, bool> whatPrefabsToScan, Action callback)
        {
            this.StartCoroutine(RunTests(whatScenesToScan, whatPrefabsToScan, callback));
        }
        protected virtual IEnumerator RunTests(Dictionary<string, bool> whatScenesToScan, Dictionary<string, bool> whatPrefabsToScan, Action callback)
        {
            yield return new WaitForSeconds(0.1f);
            List<string> scenes = new List<string>();
            List<string> prefabs = new List<string>();
            foreach (KeyValuePair<string, bool> item in whatScenesToScan)
            {
                if (item.Value == true)
                {
                    scenes.Add(item.Key);
                }
            }
            foreach (KeyValuePair<string, bool> item in whatPrefabsToScan)
            {
                if (item.Value == true)
                {
                    prefabs.Add(item.Key);
                }
            }

            ShooterPerformTests st = new ShooterPerformTests();
            TestResults.SetNewTestResults(st.PerformTests(scenes, prefabs)); // Apply the results to the test results window
            TestResults.OpenTestResultsWindow("Shooter Test Results");         // Open the test results window on focus on it

            callback?.Invoke();
        }
    }
}
