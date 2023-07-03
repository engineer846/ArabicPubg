using Common.Utils;
using Unity.EditorCoroutines.Editor;
using EMI.Editors.Windows;
using EMI.Utils;
using Invector.vItemManager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace EMI.Menus
{
    public partial class MeleeTestActions : EditorWindow
    {
        public void PerformTests(Dictionary<string, bool> whatScenesToScan, Dictionary<string, bool> whatPrefabsToScan, Action callback)
        {
            MeleeTests(whatScenesToScan, whatPrefabsToScan, callback);
        }
        partial void MeleeTests(Dictionary<string, bool> whatScenesToScan, Dictionary<string, bool> whatPrefabsToScan, Action callback);
    }

    public class MeleeNavigator : BasicNavigator
    {
        #region Initilization
        [MenuItem("Easy Multiplayer - Invector/Open Navigator With Melee Tier Support")]
        public static void OpenMeleeNavigator()
        {
            EditorWindow.GetWindow<MeleeNavigator>("Easy Multiplayer - Invector", focus: true);
        }
        [MenuItem("Easy Multiplayer - Invector/Open Navigator With Melee Tier Support", true)]
        static bool ValidateOpenMeleeNavigator()
        {
            string shooter = "";
            if (Directory.Exists($"Assets{Path.DirectorySeparatorChar}Invector-3rdPersonController{Path.DirectorySeparatorChar}Shooter"))
                shooter = FilesUtil.FindFolderPath("Shooter", "Assets/Invector-3rdPersonController/Shooter");

            return string.IsNullOrEmpty(shooter);
        }
        #endregion

        #region Actions
        #region Checks
        protected override void AfterScriptsCompiled()
        {
            base.AfterScriptsCompiled();
            string scriptConvertedFile = FilesUtil.FindFolderPath("EasyMultiplayer-Invector");
            scriptConvertedFile += "/Melee Combat/Editor/ScriptsConverted.txt";
            if (File.Exists(scriptConvertedFile))
            {
                StreamWriter writer = new StreamWriter(scriptConvertedFile, false);
                writer.WriteLine("true");
                writer.Close();
            }
        }
        protected override void CheckScriptsConverted()
        {
            string scriptConvertedFile = FilesUtil.FindFolderPath("EasyMultiplayer-Invector");
            scriptConvertedFile += "/Melee Combat/Editor/ScriptsConverted.txt";
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
        protected override void CheckPackagesImported()
        {
            string importedMemoryFile = FilesUtil.FindFolderPath("EasyMultiplayer-Invector");
            importedMemoryFile += "/Melee Combat/Editor/Imported.txt";
            packagesImported = File.Exists(importedMemoryFile);
        }
        #endregion

        #region Converting Invector Scripts
        protected override void ConvertInvectorScripts()
        {
            this.StartCoroutine(Convert());
        }
        IEnumerator Convert()
        {
            showProgressBar = true;
            Repaint();
            yield return new WaitForSeconds(0.1f);
            MeleeScriptConvert converter = new MeleeScriptConvert();
            yield return this.StartCoroutine(converter.Run(FinishedScriptConvert, true, ProgressUpdate));
            yield return null;
        }
        protected override void FinishedScriptConvert()
        {
            string scriptConvertedFile = FilesUtil.FindFolderPath("EasyMultiplayer-Invector");
            string basicPath = scriptConvertedFile + "/Basic Locomotion/Editor/ScriptsConverted.txt";
            string meleePath = scriptConvertedFile + "/Melee Combat/Editor/ScriptsConverted.txt";
            File.WriteAllText(basicPath, "false");
            File.WriteAllText(meleePath, "false");
            scriptsConverted = true;
        }
        #endregion

        #region Import Packages
        protected override void ImportPackages()
        {
            string packagePath = FilesUtil.FindFolderPath("EasyMultiplayer-Invector");
            string basicMemoryFilePath = $"{packagePath}/Basic Locomotion/Editor/Imported.txt";
            string meleeMemoryFilePath = $"{packagePath}/Melee Combat/Editor/Imported.txt";
            packagePath += "/Melee Combat/Editor/Contents.unitypackage"; //This package combines both basic & melee contents
            if (File.Exists(packagePath))
            {
                string vInputPath = FilesUtil.FindFilePath("vInput.cs");
                FilesUtil.CommentOutFile(vInputPath);
                AssetDatabase.ImportPackage(packagePath, false);
                File.WriteAllText(basicMemoryFilePath, "true");
                File.WriteAllText(meleeMemoryFilePath, "true");
                packagesImported = true;
            }
            else
            {
                Debug.LogError($"Looks like the package is missing from your project. Expected to find it at the following location: {packagePath}");
            }
        }
        #endregion

        #region Testing
        protected override List<string> ComponentsList()
        {
            List<string> components = base.ComponentsList();
            components.AddRange(new List<string>()
            {
                "MeleeNetworkCalls",
                "vCollectMeleeControl",
                "vDrawHideMeleeWeapons",
                "vEquipmentDisplay",
                "vEquipSlot",
                "vEventWithDelay",
                "vFadeCanvas",
                "vInventory",
                "vItemCollectionDisplay",
                "vItemManager",
                "vLockOn",
                "vMeleeCombatInput",
                "vMeleeManager",
                "vOpenCloseInventoryTrigger",
                "vSimpleInput",
                "vSimpleDoor",
                "ActivatedMemory",
                "MeleeObjectNetworkCalls",
                "vBreakableObject",
                "MeleeAINetworkCalls",
                "v_AIController",
                "vEquipArea",
                "vBodySnappingControl"
            });
            return components;
        }
        protected override void ExecuteTests()
        {
            MeleeTestActions mt = (MeleeTestActions)ScriptableObject.CreateInstance("MeleeTestActions");
            mt.PerformTests(whatScenesToScan, whatPrefabsToScan, TestCompleted);
        }
        #endregion

        #region Converting
        protected override List<string> ConvertableComponents()
        {
            List<string> comps = new List<string>();
            comps.AddRange(base.ConvertableComponents());
            comps.Remove("MeleeNetworkCalls");
            comps.Remove("ActivatedMemory");
            comps.Remove("MeleeObjectNetworkCalls");
            comps.Remove("MeleeAINetworkCalls");
            comps.Remove("vSimpleInput");
            comps.Remove("MP_vSimpleInput");
            comps.Add("vItemCollection");
            comps.Add("vAmmoStandalone");
            comps.Add("vMeleeWeapon");
            return comps;
        }
        protected override IEnumerator GetObjectPrefabs(string startingPath)
        {
            objectPaths.Clear();
            Repaint();
            yield return new WaitForSeconds(0.1f);
            objectPaths.AddRange(EditorUtils.GetPrefabPathsWithComponents(ConvertableComponents(), startingPath, ignorePath: (includeInvDir) ? "" : "Assets/Invector-3rdPersonController", ignoreMPComps: true));
            objectPaths.AddRange(EditorUtils.GetAllInstances<vItemListData>(startingPath, ignorePath: (includeInvDir) ? "" : "Assets/Invector-3rdPersonController"));
            scanning = false;
        }
        #endregion
        #endregion
    }
}
