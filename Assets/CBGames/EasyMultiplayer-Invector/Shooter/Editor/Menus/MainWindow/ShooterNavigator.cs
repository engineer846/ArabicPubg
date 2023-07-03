using Common.Utils;
using Unity.EditorCoroutines.Editor;
using EMI.Editors;
using EMI.Editors.Windows;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace EMI.Menus
{
    public partial class ShooterTestActions : EditorWindow
    {
        public void PerformTests(Dictionary<string, bool> whatScenesToScan, Dictionary<string, bool> whatPrefabsToScan, Action callback)
        {
            ShooterTests(whatScenesToScan, whatPrefabsToScan, callback);
        }
        partial void ShooterTests(Dictionary<string, bool> whatScenesToScan, Dictionary<string, bool> whatPrefabsToScan, Action callback);
    }

    public class ShooterNavigator : MeleeNavigator
    {
        #region Initilization
        [MenuItem("Easy Multiplayer - Invector/Open Navigator With Shooter Tier Support")]
        public static void OpenShooterNavigator()
        {
            EditorWindow.GetWindow<ShooterNavigator>("Easy Multiplayer - Invector", focus: true);
        }
        #endregion

        #region Actions
        #region Checks
        protected override void AfterScriptsCompiled()
        {
            base.AfterScriptsCompiled();
            string scriptConvertedFile = FilesUtil.FindFolderPath("EasyMultiplayer-Invector");
            scriptConvertedFile += "/Shooter/Editor/ScriptsConverted.txt";
            if (File.Exists(scriptConvertedFile))
            {
                StreamWriter writer = new StreamWriter(scriptConvertedFile, false);
                writer.WriteLine("true");
                writer.Close();
            }

            // Fix the components that were broken from the script conversion based on the saved data
            ShooterSaver ss = new ShooterSaver();
            ss.FixAllBrokenComponents();
        }
        protected override void CheckScriptsConverted()
        {
            string scriptConvertedFile = FilesUtil.FindFolderPath("EasyMultiplayer-Invector");
            scriptConvertedFile += "/Shooter/Editor/ScriptsConverted.txt";
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
            importedMemoryFile += "/Shooter/Editor/Imported.txt";
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
            progress_percent = 0;
            progress_title = "Initilizating Converters";
            progress_message = "Please Wait...";
            showProgressBar = true;
            Repaint();
            yield return new EditorWaitForSeconds(0.1f);

            // Since this script modification breaks components we need to save ALL prefabs that use this component
            // prior to conversion
            ShooterSaver ss = new ShooterSaver();
            ss.SaveAllBreakingComponents();

            yield return new EditorWaitForSeconds(0.1f);

            // Convert all the scripts
            ShooterScriptConvert converter = new ShooterScriptConvert();
            yield return this.StartCoroutine(converter.Run(FinishedScriptConvert, true, ProgressUpdate));
            yield return null;
        }

        protected override void FinishedScriptConvert()
        {
            string scriptConvertedFile = FilesUtil.FindFolderPath("EasyMultiplayer-Invector");
            string basicPath = scriptConvertedFile + "/Basic Locomotion/Editor/ScriptsConverted.txt";
            string meleePath = scriptConvertedFile + "/Melee Combat/Editor/ScriptsConverted.txt";
            string shooterPath = scriptConvertedFile + "/Shooter/Editor/ScriptsConverted.txt";
            File.WriteAllText(basicPath, "false");
            File.WriteAllText(meleePath, "false");
            File.WriteAllText(shooterPath, "false");
            scriptsConverted = true;
        }
        #endregion

        #region Import Packages
        protected override void ImportPackages()
        {
            string packagePath = FilesUtil.FindFolderPath("EasyMultiplayer-Invector");
            string basicMemoryFilePath = $"{packagePath}/Basic Locomotion/Editor/Imported.txt";
            string meleeMemoryFilePath = $"{packagePath}/Melee Combat/Editor/Imported.txt";
            string shooterMemoryFilePath = $"{packagePath}/Shooter/Editor/Imported.txt";
            packagePath += "/Shooter/Editor/Contents.unitypackage"; //This package combines both basic & melee & shooter contents
            if (File.Exists(packagePath))
            {
                string vInputPath = FilesUtil.FindFilePath("vInput.cs");
                FilesUtil.CommentOutFile(vInputPath);
                AssetDatabase.ImportPackage(packagePath, false);
                File.WriteAllText(basicMemoryFilePath, "true");
                File.WriteAllText(meleeMemoryFilePath, "true");
                File.WriteAllText(shooterMemoryFilePath, "true");
                packagesImported = true;
            }
            else
            {
                Debug.LogError($"Looks like the package is missing from your project. Expected to find it at the following location: {packagePath}");
            }
        }
        #endregion
        #endregion

        #region Testing
        protected override List<string> ComponentsList()
        {
            List<string> components = base.ComponentsList();
            components.AddRange(new List<string>()
            {
                "vCollectShooterMeleeControl",
                "vDrawHideShooterWeapons",
                "vLockOnShooter",
                "vShooterManager",
                "vShooterMeleeInput",
                "vThrowManager",
                "PerformActions",
                "ShooterNetworkCalls",
                "DoorNetworkCalls",
                "UseItemEventTrigger",
                "vShooterEquipment",
                "vShooterWeapon",
                "vThrowCollectable",
                "vShooterWeaponNetworkCalls"
            });
            return components;
        }
        protected override void ExecuteTests()
        {
            #pragma warning disable 0649
            ShooterTestActions st = (ShooterTestActions)ScriptableObject.CreateInstance("ShooterTestActions");
            st.PerformTests(whatScenesToScan, whatPrefabsToScan, TestCompleted);
            #pragma warning restore 0649
        }
        #endregion

        #region Converting
        protected override List<string> ConvertableComponents()
        {
            List<string> comps = new List<string>();
            comps.AddRange(base.ConvertableComponents());
            comps.Remove("PerformActions");
            comps.Remove("ShooterNetworkCalls");
            comps.Remove("DoorNetworkCalls");
            comps.Remove("vShooterWeaponNetworkCalls");
            return comps;
        }
        #endregion
    }
}
