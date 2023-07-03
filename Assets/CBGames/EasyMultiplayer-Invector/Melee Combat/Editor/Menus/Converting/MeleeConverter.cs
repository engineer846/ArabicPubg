using EMI.AI;
using EMI.Object;
using EMI.Player;
using EMI.Utils;
using Invector;
using Invector.vCharacterController;
using Invector.vCharacterController.AI;
using Invector.vItemManager;
using Invector.vMelee;
using Mirror;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EMI.Menus
{
    public partial class ObjectConverter
    {
        partial void ExecuteStandaloneConvertSO(string itemListPath, string savePath)
        {
            vItemListData data = (vItemListData)AssetDatabase.LoadAssetAtPath(itemListPath, typeof(vItemListData));
            StandaloneConvertSO(data, savePath);
        }
        partial void StandaloneConvertSO(ScriptableObject inputTarget, string savePath)
        {
            string directory = "";
            directory = savePath;
            if (!directory.Contains(Application.dataPath))
            {
                directory = $"{Application.dataPath}/{directory}";
                directory = savePath.Replace("/Assets/Assets/", "/Assets/");
            }
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            vItemListData target;
            string prefabPath = "";
            string path = AssetDatabase.GetAssetPath(inputTarget);
            prefabPath = $"{savePath}/{Path.GetFileName(path)}";
            AssetDatabase.CopyAsset(path, prefabPath);
            AssetDatabase.Refresh();
            target = (vItemListData)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(vItemListData));

            ConvertScriptableObject(target, savePath);

            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        void ConvertScriptableObject(vItemListData target, string savePath)
        {
            foreach(vItem item in target.items)
            {
                if (item.originalObject != null)
                {
                    GameObject found = (GameObject)AssetDatabase.LoadAssetAtPath($"{savePath}/{item.originalObject.name}.prefab", typeof(GameObject));
                    if (found != null)
                    {
                        item.originalObject = found;
                    }
                    else
                    {
                        found = (GameObject)AssetDatabase.LoadAssetAtPath($"{savePath}/MP_{item.originalObject.name}.prefab", typeof(GameObject));
                        if (found)
                        {
                            item.originalObject = found;
                        }
                    }
                }
                if (item.dropObject != null)
                {
                    GameObject found = (GameObject)AssetDatabase.LoadAssetAtPath($"{savePath}/{item.dropObject.name}.prefab", typeof(GameObject));
                    if (found)
                    {
                        item.dropObject = found;
                    }
                    else
                    {
                        found = (GameObject)AssetDatabase.LoadAssetAtPath($"{savePath}/MP_{item.dropObject.name}.prefab", typeof(GameObject));
                        if (found != null)
                        {
                            item.dropObject = found;
                        }
                    }
                }
            }
        }
        partial void ConvertMeleeAI(GameObject target)
        {
            ReplaceComponent(target, typeof(vHeadTrack), typeof(MP_vHeadTrack));
            ReplaceComponent(target, typeof(vMeleeManager), typeof(MP_vMeleeManager));
            ReplaceComponent(target, typeof(vSimpleMeleeAI_Controller), typeof(MP_v_AIController));
            ReplaceComponent(target, typeof(vHitDamageParticle), typeof(MP_vHitDamageParticle));
            ReplaceComponent(target, typeof(vWeaponConstrain), typeof(MP_vWeaponConstrain));
            if (!target.GetComponent<MeleeAINetworkCalls>())
            {
                target.AddComponent<MeleeAINetworkCalls>();
                target.GetComponent<MeleeAINetworkCalls>().syncInterval = 0;
            }
            if (!target.GetComponent<ServerSync>())
            {
                target.AddComponent<ServerSync>();
                ServerSync ss = target.GetComponent<ServerSync>();
                ss.GetType().GetField("interpolation", EditorUtils.allBindingValues).SetValue(ss, 0.1f);
                ss.GetType().GetField("forceSnapDistance", EditorUtils.allBindingValues).SetValue(ss, 1f);
                ss.syncInterval = 0;
            }
        }
        partial void ConvertMeleeObject(GameObject target)
        {
            // Convert Inventory Items
            if (target.GetComponentInChildren<vOpenCloseInventoryTrigger>())
            {
                GameObject childTarget = target.GetComponentInChildren<vOpenCloseInventoryTrigger>(true).gameObject;
                ReplaceComponent(childTarget, typeof(vOpenCloseInventoryTrigger), typeof(MP_vOpenCloseInventoryTrigger));
            }
            if (target.GetComponentInChildren<vInventory>(true))
            {
                GameObject childTarget = target.GetComponentInChildren<vInventory>(true).gameObject;
                if (PrefabUtility.GetPrefabInstanceStatus(childTarget) != PrefabInstanceStatus.NotAPrefab)
                {
                    GameObject instanceRoot = PrefabUtility.GetNearestPrefabInstanceRoot(childTarget);
                    PrefabUtility.UnpackPrefabInstance(instanceRoot, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                }
                ReplaceComponent(childTarget, typeof(vInventory), typeof(MP_vInventory));
                childTarget.GetComponent<MP_vInventory>().timeScaleWhileIsOpen = 1;
                childTarget.GetComponent<MP_vInventory>().dontDestroyOnLoad = false;
            }
            if (target.GetComponentInChildren<vItemCollectionDisplay>(true))
            {
                GameObject childTarget = target.GetComponentInChildren<vItemCollectionDisplay>(true).gameObject;
                ReplaceComponent(childTarget, typeof(vItemCollectionDisplay), typeof(MP_vItemCollectionDisplay));
            }

            foreach (vEquipSlot slot in target.GetComponentsInChildren<vEquipSlot>(true))
            {
                // convert the component
                GameObject childTarget = slot.gameObject;
                ReplaceComponent(childTarget, typeof(vEquipSlot), typeof(MP_vEquipSlot));

                // update the area if it exists
                vEquipArea[] area = childTarget.GetComponentsInParent<vEquipArea>(true);
                if (area.Length < 1) continue;
                int index = area[0].equipSlots.FindIndex(x => x == slot);
                area[0].equipSlots[index] = childTarget.GetComponent<MP_vEquipSlot>();
            }

            MP_vInventory inv = target.GetComponentInChildren<MP_vInventory>(true);
            foreach (vEquipmentDisplay display in target.GetComponentsInChildren<vEquipmentDisplay>(true))
            {
                int index = inv.changeEquipmentControllers.FindIndex(x => x.display == display);
                GameObject childTarget = display.gameObject;
                ReplaceComponent(childTarget, typeof(vEquipmentDisplay), typeof(MP_vEquipmentDisplay));

                if (index > -1)
                    inv.changeEquipmentControllers[index].display = childTarget.GetComponent<MP_vEquipmentDisplay>();
            }

            if (target.GetComponentInChildren<vBreakableObject>(true))
            {
                GameObject child = target.GetComponentInChildren<vBreakableObject>(true).gameObject;
                ReplaceComponent(child, typeof(vBreakableObject), typeof(MP_vBreakableObject));
                if (target.GetComponent<MP_vBreakableObject>())
                {
                    if (!target.GetComponent<NetworkIdentity>())
                    {
                        target.AddComponent<NetworkIdentity>();
                    }
                    if (!target.GetComponent<MeleeObjectNetworkCalls>())
                    {
                        target.AddComponent<MeleeObjectNetworkCalls>();
                    }
                    target.GetComponent<MeleeObjectNetworkCalls>().syncInterval = 0;
                }
            }
            if (target.GetComponent<vItemCollection>() && !target.transform.root.GetComponent<NetworkIdentity>())
            {
                target.transform.root.gameObject.AddComponent<NetworkIdentity>();
            }
            if (!target.GetComponent<vMeleeEquipment>())
            {
                if (target.GetComponent<vMeleeWeapon>() && !target.GetComponent<NetworkIdentity>() && !target.GetComponentInParent<vThirdPersonController>() && !target.GetComponentInParent<vSimpleMeleeAI_Controller>())
                {
                    target.AddComponent<NetworkIdentity>();
                }
                if (target.GetComponent<vMeleeWeapon>() && !target.GetComponent<ServerSync>())
                {
                    target.AddComponent<ServerSync>();
                    ServerSync ss = target.GetComponent<ServerSync>();
                    ss.GetType().GetField("interpolation", EditorUtils.allBindingValues).SetValue(ss, 0.2f);
                    ss.GetType().GetField("forceSnapDistance", EditorUtils.allBindingValues).SetValue(ss, 0.5f);
                    ss.syncInterval = 0;
                }
            }
        }
        partial void ConvertMeleeCharacter(GameObject target)
        {
            ReplaceComponent(target, typeof(vWeaponConstrain), typeof(MP_vWeaponConstrain));

            if (!target.GetComponent("vShooterMeleeInput") && !target.GetComponent("MP_vShooterMeleeInput"))
                ReplaceComponent(target, typeof(vMeleeCombatInput), typeof(MP_vMeleeCombatInput));
            ReplaceComponent(target, typeof(vMeleeManager), typeof(MP_vMeleeManager));
            if (!target.GetComponent("vCollectShooterMeleeControl") && !target.GetComponent("MP_vCollectShooterMeleeControl"))
                ReplaceComponent(target, typeof(vCollectMeleeControl), typeof(MP_vCollectMeleeControl));
            if (!target.GetComponent("vLockOnShooter") && !target.GetComponent("MP_vLockOnShooter"))
                ReplaceComponent(target, typeof(vLockOn), typeof(MP_vLockOn));
            ReplaceComponent(target, typeof(vItemManager), typeof(MP_vItemManager));
            if (!target.GetComponent("vDrawHideShooterWeapons") && !target.GetComponent("MP_vDrawHideShooterWeapons")) 
                ReplaceComponent(target, typeof(vDrawHideMeleeWeapons), typeof(MP_vDrawHideMeleeWeapons));

            // Move Bone Positions
            Animator anim = target.GetComponent<Animator>();
            foreach(vSnapToBody body in target.GetComponentsInChildren<vSnapToBody>(true))
            {
                body.bodySnap.SnapAll();
                foreach(Transform child in body.transform)
                {
                    child.parent = body.boneToSnap;
                }
            }

            // Remove root snapping, too inacurate
            if (target.GetComponentInChildren<vBodySnappingControl>(true))
            {
                MonoBehaviour.DestroyImmediate(target.GetComponentInChildren<vBodySnappingControl>(true).gameObject);
            }

            // Convert Inventory Items
            if (target.GetComponentInChildren<vOpenCloseInventoryTrigger>(true))
            {
                GameObject childTarget = target.GetComponentInChildren<vOpenCloseInventoryTrigger>(true).gameObject;
                ReplaceComponent(childTarget, typeof(vOpenCloseInventoryTrigger), typeof(MP_vOpenCloseInventoryTrigger));
            }
            if (target.GetComponentInChildren<vInventory>(true))
            {
                GameObject childTarget = target.GetComponentInChildren<vInventory>(true).gameObject;
                if (PrefabUtility.GetPrefabInstanceStatus(childTarget) != PrefabInstanceStatus.NotAPrefab)
                {
                    GameObject instanceRoot = PrefabUtility.GetNearestPrefabInstanceRoot(childTarget);
                    PrefabUtility.UnpackPrefabInstance(instanceRoot, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                }
                ReplaceComponent(childTarget, typeof(vInventory), typeof(MP_vInventory));
                childTarget.GetComponent<MP_vInventory>().timeScaleWhileIsOpen = 1;
                childTarget.GetComponent<MP_vInventory>().dontDestroyOnLoad = false;
            }
            if (target.GetComponentInChildren<vItemCollectionDisplay>(true))
            {
                GameObject childTarget = target.GetComponentInChildren<vItemCollectionDisplay>(true).gameObject;
                ReplaceComponent(childTarget, typeof(vItemCollectionDisplay), typeof(MP_vItemCollectionDisplay));
            }

            foreach(vEquipSlot slot in target.GetComponentsInChildren<vEquipSlot>(true))
            {
                GameObject childTarget = slot.gameObject;
                ReplaceComponent(childTarget, typeof(vEquipSlot), typeof(MP_vEquipSlot));

                vEquipArea[] area = childTarget.GetComponentsInParent<vEquipArea>(true);
                if (area.Length < 1) continue;
                int index = area[0].equipSlots.FindIndex(x => x == slot);
                if (index > -1)
                    area[0].equipSlots[index] = childTarget.GetComponent<MP_vEquipSlot>();
            }

            MP_vInventory inv = target.GetComponentInChildren<MP_vInventory>(true);
            foreach(vEquipmentDisplay display in target.GetComponentsInChildren<vEquipmentDisplay>(true))
            {
                int index = inv.changeEquipmentControllers.FindIndex(x => x.display.Equals(display));
                GameObject childTarget = display.gameObject;
                ReplaceComponent(childTarget, typeof(vEquipmentDisplay), typeof(MP_vEquipmentDisplay));
                if (index > -1)
                    inv.changeEquipmentControllers[index].display = childTarget.GetComponent<MP_vEquipmentDisplay>();
            }

            List<GameObject> equipAreaMemory = new List<GameObject>();
            foreach(ChangeEquipmentControl control in inv.changeEquipmentControllers)
            {
                equipAreaMemory.Add(control.equipArea.gameObject);
            }
            foreach (vEquipArea area in inv.gameObject.GetComponentsInChildren<vEquipArea>(true))
            {
                GameObject childTarget = area.gameObject;
                ReplaceComponent(childTarget, typeof(vEquipArea), typeof(MP_vEquipArea));
                int foundIndex = equipAreaMemory.FindIndex(x => x.Equals(childTarget));
                if (foundIndex > -1)
                    inv.changeEquipmentControllers[foundIndex].equipArea = childTarget.GetComponent<MP_vEquipArea>();
            }

            // update vItemWindowDisplay to MP
            foreach(vItemWindowDisplay display in inv.gameObject.GetComponentsInChildren<vItemWindowDisplay>(true))
            {
                GameObject targetDisplay = display.gameObject;
                vItemOptionWindow options = display.gameObject.GetComponentInChildren<vItemOptionWindow>(true);

                List<GameObject> useButtons = new List<GameObject>();
                int useButtonListenerIndex = -1;
                for(int i = 0; i < options.useItemButton.onClick.GetPersistentEventCount(); i++)
                {
                    System.Object indexTarget = options.useItemButton.onClick.GetPersistentTarget(i);
                    if (indexTarget is vItemWindowDisplay)
                    {
                        useButtonListenerIndex = i;
                        useButtons.Add(options.useItemButton.gameObject);
                        break;
                    }
                }
                if (useButtonListenerIndex > -1)
                    UnityEventTools.RemovePersistentListener(options.useItemButton.onClick, useButtonListenerIndex);

                List<GameObject> leaveButtons = new List<GameObject>();
                List<GameObject> dropButtons = new List<GameObject>();

                foreach (vItemAmountWindow window in options.gameObject.GetComponentsInChildren<vItemAmountWindow>(true))
                {
                    // singles
                    foreach (Transform child in window.singleAmountControl.transform)
                    {
                        if (!child.GetComponent<EventTrigger>())
                            continue;

                        foreach(EventTrigger.Entry trigger in child.GetComponent<EventTrigger>().triggers)
                        {
                            switch(trigger.eventID)
                            {
                                case EventTriggerType.Submit:
                                case EventTriggerType.PointerClick:
                                    for (int ci = 0; ci < trigger.callback.GetPersistentEventCount(); ci++)
                                    {
                                        System.Object comp = trigger.callback.GetPersistentTarget(ci);
                                        string functionName = trigger.callback.GetPersistentMethodName(ci);

                                        if (comp is vItemWindowDisplay)
                                        {
                                            GameObject targetWindowDisplay = ((vItemWindowDisplay)comp).gameObject;
                                            if (functionName == "LeaveItem" && !leaveButtons.Contains(child.gameObject)) 
                                               leaveButtons.Add(child.gameObject);
                                            if (functionName == "DropItem" && !dropButtons.Contains(child.gameObject))
                                               dropButtons.Add(child.gameObject);
                                            break;
                                        }
                                    }
                                    break;
                            }
                        }
                    }

                    // multis
                    foreach (Transform child in window.multAmountControl.transform)
                    {
                        if (!child.GetComponent<EventTrigger>())
                            continue;

                        foreach (EventTrigger.Entry trigger in child.GetComponent<EventTrigger>().triggers)
                        {
                            switch (trigger.eventID)
                            {
                                case EventTriggerType.Submit:
                                case EventTriggerType.PointerClick:
                                    for (int ci = 0; ci < trigger.callback.GetPersistentEventCount(); ci++)
                                    {
                                        System.Object comp = trigger.callback.GetPersistentTarget(ci);
                                        string functionName = trigger.callback.GetPersistentMethodName(ci);

                                        if (comp is vItemWindowDisplay)
                                        {
                                            GameObject targetWindowDisplay = ((vItemWindowDisplay)comp).gameObject;
                                            if (functionName == "LeaveItem" && !leaveButtons.Contains(child.gameObject))
                                                leaveButtons.Add(child.gameObject);
                                            if (functionName == "DropItem" && !dropButtons.Contains(child.gameObject))
                                                dropButtons.Add(child.gameObject);
                                            break;
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }

                // replace with MP version
                ReplaceComponent(targetDisplay, typeof(vItemWindowDisplay), typeof(MP_vItemWindowDisplay));
                MP_vItemWindowDisplay mpVersion = targetDisplay.GetComponent<MP_vItemWindowDisplay>();

                // update vItemAmountWindow refs
                foreach (vItemAmountWindow window in targetDisplay.GetComponentsInChildren<vItemAmountWindow>(true))
                {
                    window.itemWindowDisplay = targetDisplay.GetComponent<MP_vItemWindowDisplay>();
                }
                
                // use trigger
                foreach (GameObject useButton in useButtons)
                {
                    UnityEventTools.AddPersistentListener(useButton.GetComponent<Button>().onClick, mpVersion.UseItem);
                }

                //leave trigger
                foreach (GameObject leaveButton in leaveButtons)
                {
                    foreach (EventTrigger.Entry entry in leaveButton.GetComponent<EventTrigger>().triggers)
                    {
                        switch (entry.eventID)
                        {
                            case EventTriggerType.Submit:
                            case EventTriggerType.PointerClick:
                                for (int i = 0; i < entry.callback.GetPersistentEventCount(); i++)
                                {
                                    if (entry.callback.GetPersistentTarget(i) == null)
                                    {
                                        UnityEventTools.RemovePersistentListener(entry.callback, i);
                                        break;
                                    }
                                }
                                UnityEventTools.AddVoidPersistentListener(entry.callback, mpVersion.LeaveItem);
                                break;
                        }
                    }
                }

                //drop trigger
                foreach (GameObject dropButton in dropButtons)
                {
                    foreach(EventTrigger.Entry entry in dropButton.GetComponent<EventTrigger>().triggers)
                    {
                        switch(entry.eventID)
                        {
                            case EventTriggerType.Submit:
                            case EventTriggerType.PointerClick:
                                for(int i = 0; i < entry.callback.GetPersistentEventCount(); i++)
                                {
                                    if (entry.callback.GetPersistentTarget(i) == null)
                                    {
                                        UnityEventTools.RemovePersistentListener(entry.callback, i);
                                        break;
                                    }
                                }
                                UnityEventTools.AddVoidPersistentListener(entry.callback, mpVersion.DropItem);
                                break;
                        }
                    }
                }
            }

            if (!target.GetComponent("ShooterNetworkCalls"))
            {
                if (target.GetComponent<MP_vDrawHideMeleeWeapons>() ||
                    target.GetComponent<MP_vLockOn>() ||
                    target.GetComponent<MP_vCollectMeleeControl>() ||
                    target.GetComponent<MP_vMeleeManager>() ||
                    target.GetComponent<MP_vMeleeCombatInput>() ||
                    target.GetComponentInChildren<MP_vInventory>(true) ||
                    target.GetComponentInChildren<MP_vEquipSlot>(true) ||
                    target.GetComponentInChildren<MP_vItemCollectionDisplay>(true) ||
                    target.GetComponentInChildren<MP_vOpenCloseInventoryTrigger>(true) ||
                    target.GetComponentInChildren<MP_vEquipmentDisplay>(true)
                    )
                {
                    if (!target.GetComponent<MeleeNetworkCalls>())
                    {
                        target.AddComponent<MeleeNetworkCalls>();
                    }
                    target.GetComponent<MeleeNetworkCalls>().syncInterval = 0;
                }
            }
        }
    }
}
