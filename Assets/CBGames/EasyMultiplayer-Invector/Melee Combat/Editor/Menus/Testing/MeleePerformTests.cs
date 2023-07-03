using EMI.AI;
using EMI.Object;
using EMI.Player;
using Invector;
using Invector.Utils;
using Invector.vCharacterController;
using Invector.vCharacterController.AI;
using Invector.vItemManager;
using Invector.vMelee;
using System.Collections.Generic;
using UnityEngine;

namespace EMI.Editors
{
    public class MeleePerformTests : BasicPerformTests
    {
        public override List<TestResult> TestObjectComponents(GameObject obj, string sceneName = "")
        {
            // Run through Basic tests
            List<TestResult> results = new List<TestResult>();
            results.AddRange(base.TestObjectComponents(obj, sceneName));

            // Run through Melee Tests
            MeleeTests mt = new MeleeTests();
            Component[] components = obj.GetComponentsInChildren<Component>(true);
            foreach (Component c in components)
            {
                switch (c)
                {
                    case MeleeNetworkCalls nc:
                        results.AddRange(mt.TEST_MeleeNetworkCalls(c.gameObject, sceneName));
                        break;
                    case vCollectMeleeControl cmc:
                        results.AddRange(mt.TEST_vCollectMeleeControl(c.gameObject, sceneName));
                        break;
                    case vDrawHideMeleeWeapons dhw:
                        results.AddRange(mt.TEST_vDrawHideMeleeWeapons(c.gameObject, sceneName));
                        break;
                    case vEquipmentDisplay ed:
                        results.AddRange(mt.TEST_vEquipmentDisplay(c.gameObject, sceneName));
                        break;
                    case vEquipSlot es:
                        results.AddRange(mt.TEST_vEquipSlot(c.gameObject, sceneName));
                        break;
                    case vEventWithDelay ewd:
                        results.AddRange(mt.TEST_vEventWithDelay(c.gameObject, sceneName));
                        break;
                    case vFadeCanvas fc:
                        results.AddRange(mt.TEST_vFadeCanvas(c.gameObject, sceneName));
                        break;
                    case vHUDController hc:
                        results.AddRange(mt.TEST_vHUDController(c.gameObject, sceneName));
                        break;
                    case vInventory inv:
                        results.AddRange(mt.TEST_vInventory(c.gameObject, sceneName));
                        break;
                    case vItemCollectionDisplay icd:
                        results.AddRange(mt.TEST_vItemCollectionDisplay(c.gameObject, sceneName));
                        break;
                    case vItemManager im:
                        results.AddRange(mt.TEST_vItemManager(c.gameObject, sceneName));
                        break;
                    case vLockOn lo:
                        results.AddRange(mt.TEST_vLockOn(c.gameObject, sceneName));
                        break;
                    case vMeleeCombatInput mci:
                        results.AddRange(mt.TEST_vMeleeCombatInput(c.gameObject, sceneName));
                        break;
                    case vMeleeManager mm:
                        results.AddRange(mt.TEST_vMeleeManager(c.gameObject, sceneName));
                        break;
                    case vOpenCloseInventoryTrigger oci:
                        results.AddRange(mt.TEST_vOpenCloseInventoryTrigger(c.gameObject, sceneName));
                        break;
                    case vSimpleInput si:
                        results.AddRange(mt.TEST_vSimpleInput(c.gameObject, sceneName));
                        break;
                    case ActivatedMemory am:
                        results.AddRange(mt.TEST_ActivatedMemory(c.gameObject, sceneName));
                        break;
                    case MeleeObjectNetworkCalls monc:
                        results.AddRange(mt.TEST_MeleeObjectNetworkCalls(c.gameObject, sceneName));
                        break;
                    case vBreakableObject bo:
                        results.AddRange(mt.TEST_vBreakableObject(c.gameObject, sceneName));
                        break;
                    case vSimpleMeleeAI_Controller aic:
                        results.AddRange(mt.TEST_v_AIController(c.gameObject, sceneName));
                        break;
                    case vEquipArea ea:
                        results.AddRange(mt.TEST_vEquipArea(c.gameObject, sceneName));
                        break;
                    case vSimpleDoor sd:
                        results.AddRange(mt.TEST_vSimpleDoor(c.gameObject, sceneName));
                        break;
                    case vBodySnappingControl bsc:
                        results.AddRange(mt.TEST_vBodySnappingControl(c.gameObject, sceneName));
                        break;
                }
            }
            return results;
        }
    }

    public class MeleeTests: BasicTests
    {
        #region Inidividual Melee Tests
        public virtual List<TestResult> TEST_MeleeAINetworkCalls(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "MP_MeleeAINetworkCalls";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            MeleeAINetworkCalls nc = target.GetComponent<MeleeAINetworkCalls>();
            if (nc.syncInterval > 0)
            {
                result.result = TestResultType.Critical;
                result.details = "The Sync Interval is greater than 0. This is needs to be exact with AI, otherwise you will be much more likely to desync client AI positions between different clients.";
                results.Add(result);
            }
            if (nc.syncMode != Mirror.SyncMode.Observers)
            {
                result.result = TestResultType.Critical;
                result.details = "Sync Mode is not set to Observers.";
                results.Add(result);
            }

            if (results.Count < 1)
                results.Add(result);

            return results;
        }
        public virtual List<TestResult> TEST_MeleeNetworkCalls(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "MeleeNetworkCalls";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            if (target.GetComponent<MeleeAINetworkCalls>())
            {
                return TEST_MeleeAINetworkCalls(target, sceneName);
            }
            else if (target.GetComponent<MeleeNetworkCalls>() && !target.GetComponent("ShooterNetworkCalls"))
            {
                MeleeNetworkCalls nc = target.GetComponent<MeleeNetworkCalls>();
                if (nc.syncInterval > 0)
                {
                    result.result = TestResultType.Warning;
                    result.details = "The Sync Interval is greater than 0. While this does save on network traffic it will result in clients not being exactly in the same position between clients.";
                    results.Add(result);
                }
                if (nc.syncMode != Mirror.SyncMode.Observers)
                {
                    result.result = TestResultType.Critical;
                    result.details = "Sync Mode is not set to Observers.";
                    results.Add(result);
                }
            }

            if (results.Count < 1)
                results.Add(result);
            return results;
        }
        public virtual List<TestResult> TEST_vCollectMeleeControl(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "MP_vCollectMeleeControl";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            if (!target.GetComponent<MP_vCollectMeleeControl>() && target.GetComponent("vCollectShooterMeleeControl") && !target.GetComponent("MP_vCollectShooterMeleeControl"))
            {
                result.result = TestResultType.Critical;
                result.details = "This has the vCollectMeleeControl component instead of the MP_vCollectMeleeControl component. This will not work with multiplayer. You need to replace vCollectMeleeControl with MP_vCollectMeleeControl.";
                results.Add(result);
            }
            else if (target.GetComponent<MP_vCollectMeleeControl>())
            {
                MP_vCollectMeleeControl cmc = target.GetComponent<MP_vCollectMeleeControl>();
                if (cmc.controlDisplayPrefab == null)
                {
                    result.result = TestResultType.Critical;
                    result.details = "The Control Display Prefab is blank. This needs to be populated.";
                    results.Add(result);
                }
                if (!target.GetComponent<MeleeNetworkCalls>())
                {
                    result.result = TestResultType.Critical;
                    result.details = "The MeleeNetworkCalls component is missing from this prefab. This is required to have the MP_vCollectMeleeControl component work properly.";
                    results.Add(result);
                }
            }
            if (results.Count < 1)
                results.Add(result);
            return results;
        }
        public virtual List<TestResult> TEST_vDrawHideMeleeWeapons(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "MP_vDrawHideMeleeWeapons";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            if (!target.GetComponent<MP_vDrawHideMeleeWeapons>() && !target.GetComponent("MP_vDrawHideShooterWeapons"))
            {
                result.result = TestResultType.Critical;
                result.details = "This has the vDrawHideMeleeWeapons component instead of the MP_vDrawHideMeleeWeapons component. This will not work with multiplayer. You need to replace vDrawHideMeleeWeapons with MP_vDrawHideMeleeWeapons.";
                results.Add(result);
            }
            else if (!target.GetComponent<MeleeNetworkCalls>())
            {
                result.result = TestResultType.Critical;
                result.details = "The MeleeNetworkCalls component is missing from this prefab. This is required to have the MP_vCollectMeleeControl component work properly.";
                results.Add(result);
            }

            if (results.Count < 1)
                results.Add(result);
            return results;
        }
        public virtual List<TestResult> TEST_vEquipmentDisplay(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "MP_vEquipmentDisplay";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            if (!target.GetComponent<MP_vEquipmentDisplay>())
            {
                result.result = TestResultType.Critical;
                result.details = "This has the vEquipmentDisplay component instead of the MP_vEquipmentDisplay component. This will not work with multiplayer. You need to replace vEquipmentDisplay with MP_vEquipmentDisplay.";
                results.Add(result);
            }

            if (results.Count < 1)
                results.Add(result);
            return results;
        }
        public virtual List<TestResult> TEST_vEquipArea(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "vEquipArea";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            vEquipArea ea = target.GetComponent<vEquipArea>();
            if (ea.equipSlots.FindIndex(x => x == null) > -1)
            {
                result.result = TestResultType.Critical;
                result.details = "This the vEquipArea component has some missing Equip Slots. These must all be populate to prevent errors at runtime.";
                results.Add(result);
            }

            if (results.Count < 1)
                results.Add(result);
            return results;
        }
        public virtual List<TestResult> TEST_vEquipSlot(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "MP_vEquipSlot";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            if (!target.GetComponent<MP_vEquipSlot>())
            {
                result.result = TestResultType.Critical;
                result.details = "This has the vEquipSlot component instead of the MP_vEquipSlot component. This will not work with multiplayer. You need to replace vEquipSlot with MP_vEquipSlot.";
                results.Add(result);
            }

            if (results.Count < 1)
                results.Add(result);
            return results;
        }
        public virtual List<TestResult> TEST_vEventWithDelay(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "MP_vEventWithDelay";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            if (results.Count < 1)
                results.Add(result);
            return results;
        }
        public virtual List<TestResult> TEST_vFadeCanvas(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "MP_vFadeCanvas";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            if (results.Count < 1)
                results.Add(result);
            return results;
        }
        public virtual List<TestResult> TEST_vHUDController(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "MP_vHUDController";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            if (!target.GetComponent<MP_vHUDController>())
            {
                result.result = TestResultType.Critical;
                result.details = "This has the vHUDController component instead of the MP_vHUDController component. This will have strange results where it sometimes tracks other clients healths/stamina. You need to replace vHUDController with MP_vHUDController.";
                results.Add(result);
            }

            if (results.Count < 1)
                results.Add(result);
            return results;
        }
        public virtual List<TestResult> TEST_vInventory(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "MP_vInventory";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            if (!target.GetComponent<MP_vInventory>())
            {
                result.result = TestResultType.Critical;
                result.details = "This has the vInventory component instead of the MP_vInventory component. This will not work properly in multiplayer. You need to replace vInventory with MP_vInventory.";
                results.Add(result);
            }
            else
            {
                MP_vInventory inv = target.GetComponent<MP_vInventory>();
                if (inv.changeEquipmentControllers.FindIndex(x => x.equipArea == null) > -1)
                {
                    result.result = TestResultType.Critical;
                    result.details = "This inventory has one of it's Equip Areas as null. All must be assigned to prevent errors at runtime.";
                    results.Add(result);
                }
                if (inv.changeEquipmentControllers.FindIndex(x => x.display == null) > -1)
                {
                    result.result = TestResultType.Critical;
                    result.details = "This inventory has one of it's Display as null. All must be assigned to prevent errors at runtime.";
                    results.Add(result);
                }
                if (inv.dontDestroyOnLoad == true)
                {
                    result.result = TestResultType.Critical;
                    result.details = "This inventory has Dont Destroy On Load as true. Fine for single player but has un-intended effects in multiplayer. This must be false.";
                    results.Add(result);
                }
                if (inv.timeScaleWhileIsOpen < 1)
                {
                    result.result = TestResultType.Critical;
                    result.details = "The Time Scale While Is Open is lower than 1. This must be one to prevent a client desyncing from the server everytime they open their inventory.";
                    results.Add(result);
                }
            }

            if (results.Count < 1)
                results.Add(result);
            return results;
        }
        public virtual List<TestResult> TEST_vItemCollectionDisplay(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "MP_vItemCollectionDisplay";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            if (!target.GetComponent<MP_vItemCollectionDisplay>())
            {
                result.result = TestResultType.Critical;
                result.details = "This has the vItemCollectionDisplay component instead of the MP_vItemCollectionDisplay component. This will not work with multiplayer. You need to replace vItemCollectionDisplay with MP_vItemCollectionDisplay.";
                results.Add(result);
            }
            else
            {
                MP_vItemCollectionDisplay icd = target.GetComponent<MP_vItemCollectionDisplay>();
                if (icd.itemCollectedDiplayPrefab == null)
                {
                    result.result = TestResultType.Critical;
                    result.details = "This Item Collected Display Prefab is null. This needs to be populated to prevent errors at runtime.";
                    results.Add(result);
                }
            }

            if (results.Count < 1)
                results.Add(result);
            return results;
        }
        public virtual List<TestResult> TEST_vItemManager(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "MP_vItemManager";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            if (!target.GetComponent<MP_vItemManager>())
            {
                result.result = TestResultType.Critical;
                result.details = "This has the vItemManager component instead of the MP_vItemManager component. This will not work with multiplayer. You need to replace vItemManager with MP_vItemManager.";
                results.Add(result);
            }
            else
            {
                MP_vItemManager im = target.GetComponent<MP_vItemManager>();
                if (im.itemListData == null)
                {
                    result.result = TestResultType.Critical;
                    result.details = "The Item List Data is null. This needs to be populated to have the inventory work at all.";
                    results.Add(result);
                }
            }
            if (results.Count < 1)
                results.Add(result);
            return results;
        }
        public virtual List<TestResult> TEST_vLockOn(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "MP_vLockOn";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            if (!target.GetComponent<MP_vLockOn>() && target.GetComponent("vLockOn") && target.GetComponent("MP_vLockOn"))
            {
                result.result = TestResultType.Critical;
                result.details = "This has the vLockOn component instead of the MP_vLockOn component. This will not work with multiplayer. You need to replace vLockOn with MP_vLockOn.";
                results.Add(result);
            }

            if (results.Count < 1)
                results.Add(result);
            return results;
        }
        public virtual List<TestResult> TEST_vMeleeCombatInput(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "MP_vMeleeCombatInput";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            if (!target.GetComponent<MP_vMeleeCombatInput>() && !target.GetComponent("vShooterMeleeInput") && !target.GetComponent("MP_vShooterMeleeInput"))
            {
                result.result = TestResultType.Critical;
                result.details = "This has the vMeleeCombatInput component instead of the MP_vMeleeCombatInput component. This will not work with multiplayer. You need to replace vMeleeCombatInput with MP_vMeleeCombatInput.";
                results.Add(result);
            }
            if (results.Count < 1)
                results.Add(result);
            return results;
        }
        public virtual List<TestResult> TEST_vMeleeManager(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "MP_vMeleeManager";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            if (!target.GetComponent<MP_vMeleeManager>() && !target.GetComponent<vSimpleMeleeAI_Controller>())
            {
                result.result = TestResultType.Critical;
                result.details = "This has the vMeleeManager component instead of the MP_vMeleeManager component. This will not work with multiplayer. You need to replace vMeleeManager with MP_vMeleeManager.";
                results.Add(result);
            }
            else if (target.GetComponent<MP_vMeleeManager>())
            {
                MP_vMeleeManager mm = target.GetComponent<MP_vMeleeManager>();
                if (!mm.hitProperties.hitDamageTags.Contains(LayerMask.LayerToName(target.layer)) && !target.GetComponent<vSimpleMeleeAI_Controller>())
                {
                    result.result = TestResultType.Warning;
                    result.details = "This is set to not damage players. No matter your team settings the melee attacks will never connect with other players.";
                    results.Add(result);
                }
            }

            if (results.Count < 1)
                results.Add(result);
            return results;
        }
        public virtual List<TestResult> TEST_vOpenCloseInventoryTrigger(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "MP_vOpenCloseInventoryTrigger";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            if (!target.GetComponent<MP_vOpenCloseInventoryTrigger>())
            {
                result.result = TestResultType.Critical;
                result.details = "This has the vOpenCloseInventoryTrigger component instead of the MP_vOpenCloseInventoryTrigger component. This will not work with multiplayer. You need to replace vOpenCloseInventoryTrigger with MP_vOpenCloseInventoryTrigger.";
                results.Add(result);
            }

            if (results.Count < 1)
                results.Add(result);
            return results;
        }
        public virtual List<TestResult> TEST_vSimpleInput(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "MP_vSimpleInput";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            if (!target.GetComponent<MP_vSimpleInput>())
            {
                result.result = TestResultType.Warning;
                result.details = "This has the vSimpleInput component instead of the MP_vSimpleInput component. This will not work with multiplayer. You need to replace vSimpleInput with MP_vSimpleInput.";
                results.Add(result);
            }

            if (results.Count < 1)
                results.Add(result);
            return results;
        }
        public virtual List<TestResult> TEST_ActivatedMemory(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "ActivatedMemory";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            ActivatedMemory am = target.GetComponent<ActivatedMemory>();

            if (am.syncMode != Mirror.SyncMode.Observers)
            {
                result.result = TestResultType.Critical;
                result.details = "The Sync Mode is not set to Observers.";
                results.Add(result);
            }
            if (am.syncInterval != 0)
            {
                result.result = TestResultType.Critical;
                result.details = "The Sync Inverval is not 0. While this will safe on network traffic it will not be exact. Some clients will get this update slower than others.";
                results.Add(result);
            }

            if (results.Count < 1)
                results.Add(result);
            return results;
        }
        public virtual List<TestResult> TEST_MeleeObjectNetworkCalls(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "MeleeObjectNetworkCalls";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            MeleeObjectNetworkCalls monc = target.GetComponent<MeleeObjectNetworkCalls>();
            if (monc.syncMode != Mirror.SyncMode.Observers)
            {
                result.result = TestResultType.Critical;
                result.details = "The Sync Mode is set to something other than Observers. This should be observers to work properly.";
                results.Add(result);
            }
            if (monc.syncInterval > 0)
            {
                result.result = TestResultType.Warning;
                result.details = "The Sync Interval is greater than 0. This this will save on network traffic it will mean this object will not be exact between clients. It will eventually, almost not realtime depending on how high this value is.";
                results.Add(result);
            }
            if (results.Count < 1)
                results.Add(result);
            return results;
        }
        public virtual List<TestResult> TEST_vBreakableObject(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "MP_vBreakableObject";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            if (!target.GetComponent<MP_vBreakableObject>())
            {
                result.result = TestResultType.Critical;
                result.details = "This has the vBreakableObject component instead of the MP_vBreakableObject component. This will not work with multiplayer. You need to replace vBreakableObject with MP_vBreakableObject.";
                results.Add(result);
            }

            if (results.Count < 1)
                results.Add(result);
            return results;
        }
        public virtual List<TestResult> TEST_v_AIController(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "MP_v_AIController";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            if (!target.GetComponent<MP_v_AIController>())
            {
                result.result = TestResultType.Critical;
                result.details = "This has the v_AIController component instead of the MP_v_AIController component. This will not work with multiplayer. You need to replace v_AIController with MP_v_AIController.";
                results.Add(result);
            }

            if (results.Count < 1)
                results.Add(result);
            return results;
        }
        public virtual List<TestResult> TEST_vSimpleDoor(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "MP_vSimpleDoor";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            if (!target.GetComponent<MP_vSimpleDoor>())
            {
                result.result = TestResultType.Critical;
                result.details = "this is using the vSimpleDoor component instead of the MP_vSimpleDoor component. This will not work for multiplayer. Replace the vSimpleDoor component with the MP_vSimpleDoor component.";
                results.Add(result);
            }
            else
            {
                MP_vSimpleDoor sd = target.GetComponent<MP_vSimpleDoor>();
                if (sd.pivot == null)
                {
                    result.result = TestResultType.Critical;
                    result.details = "The pivot is null. Without a pivot the door will not open.";
                    results.Add(result);
                }
                if (!sd.tagsToOpen.Contains("Player"))
                {
                    result.result = TestResultType.Warning;
                    result.details = "The door doesn't have the 'Player' tag. Players will not be able to open this door.";
                    results.Add(result);
                }
            }

            if (results.Count < 1)
                results.Add(result);

            return results;
        }
        public virtual List<TestResult> TEST_vBodySnappingControl(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "vBoddySnappingControl";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            if (target.GetComponent<vBodySnappingControl>())
            {
                result.result = TestResultType.Critical;
                result.details = "The vBodySnappingControl component provided by Invector is unrealiable in multiplayer mode. You need to remove this component and place the objects that they are snapping to manually. Otherwise weapon placements will be off on the characters.";
                results.Add(result);
            }

            if (results.Count < 1)
                results.Add(result);

            return results;
        }
        #endregion
    }
}
