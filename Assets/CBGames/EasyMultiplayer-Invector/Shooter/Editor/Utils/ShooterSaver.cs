using Common.Utils;
using Invector;
using Invector.vShooter;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace EMI.Editors
{
    public class ShooterSaver : MonoBehaviour
    {
        struct ShooterWeaponType
        {
            public string path;
            public int ammo;
            public float shootFrequency;
            public int clipSize;
            public float reloadTime;
            public float minDamageDistance;
            public float maxDamageDistance;
            public int minDamage;
            public int maxDamage;
        }
        struct SimpleDoorType
        {
            public string path;
            public bool startOpened;
            public bool autoOpen;
            public bool autoClose;
        }
        public virtual void SaveComponentToFile(string saveFolder, string componentPath, Component comp)
        {
            string fileName = comp.name;
            int i = 0;
            
            if (!Directory.Exists(saveFolder))
                Directory.CreateDirectory(saveFolder);

            while (File.Exists($"{saveFolder}/{comp.GetType().Name}_{fileName}"))
            {
                i++;
                fileName += i.ToString();
            }
            string fileContents = null;
            switch(comp.GetType())
            {
                case Type item when item == typeof(vShooterWeapon):
                    vShooterWeapon weapon = ((vShooterWeapon)comp);
                    ShooterWeaponType contents = new ShooterWeaponType
                    {
                        path              = componentPath,
                        ammo              = weapon.ammo,
                        shootFrequency    = weapon.shootFrequency,
                        clipSize          = weapon.clipSize,
                        reloadTime        = weapon.reloadTime,
                        minDamageDistance = weapon.minDamageDistance,
                        maxDamageDistance = weapon.maxDamageDistance,
                        minDamage         = weapon.minDamage,
                        maxDamage         = weapon.maxDamage
                    };
                    fileContents = JsonUtility.ToJson(contents);
                    break;
                case Type item when item == typeof(vSimpleDoor):
                    vSimpleDoor door = ((vSimpleDoor)comp);
                    SimpleDoorType d_cont = new SimpleDoorType
                    {
                        path        = componentPath,
                        startOpened = door.startOpened,
                        autoOpen    = door.autoOpen,
                        autoClose   = door.autoClose
                    };
                    fileContents = JsonUtility.ToJson(d_cont);
                    break;
            }
            if (!string.IsNullOrEmpty(fileContents))
            {
                StreamWriter writer = new StreamWriter($"{saveFolder}/{comp.GetType().Name}_{fileName}.json");
                try
                {
                    writer.Write(fileContents);
                }
                finally
                {
                    writer.Close();
                }
            }
        }

        public virtual void ApplyAllSaveFilesBackToComponents(string saveFolder)
        {
            if (!Directory.Exists(saveFolder))
                return;

            DirectoryInfo di = new DirectoryInfo(saveFolder);
            FileInfo[] fileInfos = di.GetFiles();
            ShooterWeaponType weapon;
            SimpleDoorType door;
            GameObject target;
            foreach (FileInfo file in fileInfos)
            {
                if (file.Name.Contains(".meta")) continue;
                using (StreamReader reader = new StreamReader(file.FullName))
                {
                    switch (file.Name.Split('_')[0])
                    {
                        case "vShooterWeapon":
                            weapon = JsonUtility.FromJson<ShooterWeaponType>(reader.ReadToEnd());
                            target = PrefabUtility.LoadPrefabContents(weapon.path);
                            vShooterWeapon weapComp = target.GetComponent<vShooterWeapon>();
                            if (!weapComp) weapComp = target.GetComponentInChildren<vShooterWeapon>(true);
                            weapComp.ammo = weapon.ammo;
                            weapComp.shootFrequency = weapon.shootFrequency;
                            weapComp.clipSize = weapon.clipSize;
                            weapComp.reloadTime = weapon.reloadTime;
                            weapComp.minDamageDistance = weapon.minDamageDistance;
                            weapComp.maxDamageDistance = weapon.maxDamageDistance;
                            weapComp.minDamage = weapon.minDamage;
                            weapComp.maxDamage = weapon.maxDamage;
                            PrefabUtility.SaveAsPrefabAsset(target, weapon.path);
                            PrefabUtility.UnloadPrefabContents(target);
                            break;
                        case "vSimpleDoor":
                            door = JsonUtility.FromJson<SimpleDoorType>(reader.ReadToEnd());
                            target = PrefabUtility.LoadPrefabContents(door.path);
                            vSimpleDoor doorComp = target.GetComponent<vSimpleDoor>();
                            if (!doorComp) doorComp = target.GetComponentInChildren<vSimpleDoor>(true);
                            doorComp.startOpened = door.startOpened;
                            doorComp.autoOpen = door.autoOpen;
                            doorComp.autoClose = door.autoClose;
                            PrefabUtility.SaveAsPrefabAsset(target, door.path);
                            PrefabUtility.UnloadPrefabContents(target);
                            break;
                    }
                }
            }
        }

        public virtual void SaveAllBreakingComponents()
        {
            string emi = FilesUtil.FindFolderPath("EasyMultiplayer-Invector");
            string[] guids = AssetDatabase.FindAssets("t:Object");
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (string.IsNullOrEmpty(assetPath)) continue;
                object[] myObjs = LoadAllAssetsAtPath(assetPath);
                foreach (object thisObject in myObjs)
                {
                    if (thisObject == null) continue;
                    string myType = thisObject.GetType().Name;
                    if (myType == "vShooterWeapon")
                    {
                        SaveComponentToFile($"{emi}/Shooter/Editor/SavedContent/", assetPath, (vShooterWeapon)thisObject);
                    }
                    else if ( myType == "vSimpleDoor")
                    {
                        SaveComponentToFile($"{emi}/Shooter/Editor/SavedContent/", assetPath, (vSimpleDoor)thisObject);
                    }
                }
            }
        }

        public virtual void FixAllBrokenComponents()
        {
            string emi = FilesUtil.FindFolderPath("EasyMultiplayer-Invector");
            ApplyAllSaveFilesBackToComponents($"{emi}/Shooter/Editor/SavedContent/");
        }
        public virtual object[] LoadAllAssetsAtPath(string assetPath)
        {
            return typeof(SceneAsset).Equals(AssetDatabase.GetMainAssetTypeAtPath(assetPath)) ?
                // prevent error "Do not use readobjectthreaded on scene objects!"
                new[] { AssetDatabase.LoadMainAssetAtPath(assetPath) } :
                AssetDatabase.LoadAllAssetsAtPath(assetPath);
        }
    }
}
