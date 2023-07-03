using EMI.Object;
using EMI.Player;
using Invector;
using Invector.vCharacterController;
using Invector.vItemManager;
using Invector.vMelee;
using Invector.vShooter;
using System.Collections.Generic;
using UnityEngine;

namespace EMI.Editors
{
    public class ShooterPerformTests : MeleePerformTests
    {
        public override List<TestResult> TestObjectComponents(GameObject obj, string sceneName = "")
        {
            // Run through Basic & Melee tests
            List<TestResult> results = new List<TestResult>();
            results.AddRange(base.TestObjectComponents(obj, sceneName));

            // Run through Shooter Tests
            ShooterTests st = new ShooterTests();
            Component[] components = obj.GetComponentsInChildren<Component>(true);
            foreach (Component c in components)
            {
                switch (c)
                {
                    case vCollectShooterMeleeControl csc:
                        results.AddRange(st.TEST_vCollectShooterMeleeControl(c.gameObject, sceneName));
                        break;
                    case vDrawHideShooterWeapons dsw:
                        results.AddRange(st.TEST_vDrawHideShooterWeapons(c.gameObject, sceneName));
                        break;
                    case vLockOnShooter lo:
                        results.AddRange(st.TEST_vLockOnShooter(c.gameObject, sceneName));
                        break;
                    case vShooterManager sm:
                        results.AddRange(st.TEST_vShooterManager(c.gameObject, sceneName));
                        break;
                    case vShooterMeleeInput smi:
                        results.AddRange(st.TEST_vShooterMeleeInput(c.gameObject, sceneName));
                        break;
                    case vThrowManager tm:
                        results.AddRange(st.TEST_vThrowManager(c.gameObject, sceneName));
                        break;
                    case PerformActions pa:
                        results.AddRange(st.TEST_PerformActions(c.gameObject, sceneName));
                        break;
                    case ShooterNetworkCalls nc:
                        results.AddRange(st.TEST_ShooterNetworkCalls(c.gameObject, sceneName));
                        break;
                    case DoorNetworkCalls dnc:
                        results.AddRange(st.TEST_DoorNetworkCalls(c.gameObject, sceneName));
                        break;
                    case UseItemEventTrigger uit:
                        results.AddRange(st.TEST_UseItemEventTrigger(c.gameObject, sceneName));
                        break;
                    case vShooterEquipment se:
                        results.AddRange(st.TEST_vShooterEquipment(c.gameObject, sceneName));
                        break;
                    case vShooterWeapon sw:
                        results.AddRange(st.TEST_vShooterWeapon(c.gameObject, sceneName));
                        break;
                    case vThrowCollectable tc:
                        results.AddRange(st.TEST_vThrowCollectable(c.gameObject, sceneName));
                        break;
                    case vShooterWeaponNetworkCalls swnc:
                        results.AddRange(st.TEST_vShooterWeaponNetworkCalls(c.gameObject, sceneName));
                        break;

                }
            }
            return results;
        }
    }

    public class ShooterTests : MeleeTests
    {
        #region Inidividual Melee Tests
        public virtual List<TestResult> TEST_vCollectShooterMeleeControl(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "MP_vCollectShooterMeleeControl";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            if (!target.GetComponent< MP_vCollectShooterMeleeControl>())
            {
                result.result = TestResultType.Critical;
                result.details = "this is using the vCollectShooterMeleeControl component instead of the MP_vCollectShooterMeleeControl component. This will not work for multiplayer. Replace the vCollectShooterMeleeControl component with the MP_vCollectShooterMeleeControl component.";
                results.Add(result);
            }
            else
            {
                MP_vCollectShooterMeleeControl csmc = target.GetComponent<MP_vCollectShooterMeleeControl>();
                if (csmc.controlDisplayPrefab == null)
                {
                    result.result = TestResultType.Critical;
                    result.details = "This doesn't have the Control Display Prefab set. This will not work properly in play mode.";
                    results.Add(result);
                }
            }

            if (results.Count < 1)
                results.Add(result);

            return results;
        }
        public virtual List<TestResult> TEST_vDrawHideShooterWeapons(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "MP_vDrawHideShooterWeapons";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            // deperecated?
            //if (!target.GetComponent<MP_vDrawHideShooterWeapons>())
            //{
            //    result.result = TestResultType.Critical;
            //    result.details = "this is using the vDrawHideShooterWeapons component instead of the MP_vDrawHideShooterWeapons component. This will not work for multiplayer. Replace the vDrawHideShooterWeapons component with the MP_vDrawHideShooterWeapons component.";
            //    results.Add(result);
            //}
            //else
            //{

            //}

            if (results.Count < 1)
                results.Add(result);

            return results;
        }
        public virtual List<TestResult> TEST_vLockOnShooter(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "MP_vLockOnShooter";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            if (!target.GetComponent<MP_vLockOnShooter>())
            {
                result.result = TestResultType.Critical;
                result.details = "this is using the vLockOnShooter component instead of the MP_vLockOnShooter component. This will not work for multiplayer. Replace the vLockOnShooter component with the MP_vLockOnShooter component.";
                results.Add(result);
            }

            if (results.Count < 1)
                results.Add(result);

            return results;
        }
        public virtual List<TestResult> TEST_vShooterManager(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "MP_vShooterManager";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            if (!target.GetComponent<MP_vShooterManager>())
            {
                result.result = TestResultType.Critical;
                result.details = "this is using the vShooterManager component instead of the MP_vShooterManager component. This will not work for multiplayer. Replace the vShooterManager component with the MP_vShooterManager component.";
                results.Add(result);
            }
            else
            {
                MP_vShooterManager sm = target.GetComponent<MP_vShooterManager>();
                if (!sm.damageLayer.ContainsLayer(target.layer))
                {
                    result.result = TestResultType.Warning;
                    result.details = "The Damage Layer doesn't include the player. That means this player wil not be able to damage other players no matter the team settings.";
                    results.Add(result);
                }
            }

            if (results.Count < 1)
                results.Add(result);

            return results;
        }
        public virtual List<TestResult> TEST_vShooterMeleeInput(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "MP_vShooterMeleeInput";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            if (!target.GetComponent<MP_vShooterMeleeInput>())
            {
                result.result = TestResultType.Critical;
                result.details = "this is using the vShooterMeleeInput component instead of the MP_vShooterMeleeInput component. This will not work for multiplayer. Replace the vShooterMeleeInput component with the MP_vShooterMeleeInput component.";
                results.Add(result);
            }
            if (results.Count < 1)
                results.Add(result);

            return results;
        }
        public virtual List<TestResult> TEST_vThrowManager(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "MP_vThrowManager";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            if (!target.GetComponent<MP_vThrowManager>())
            {
                result.result = TestResultType.Critical;
                result.details = "this is using the vThrowManager component instead of the MP_vThrowManager component. This will not work for multiplayer. Replace the vThrowManager component with the MP_vThrowManager component.";
                results.Add(result);
            }
            else
            {
                MP_vThrowManager tm = target.GetComponent<MP_vThrowManager>();
                if (tm.throwStartPoint == null)
                {
                    result.result = TestResultType.Critical;
                    result.details = "The Throw Start Point is null. This will throw errors at runtime if this isn't populated.";
                    results.Add(result);
                }
                if (tm.throwEnd == null)
                {
                    result.result = TestResultType.Critical;
                    result.details = "The Throw End is null. This will throw errors at runtime if this isn't populated.";
                    results.Add(result);
                }
                if (tm.objectToThrow == null)
                {
                    result.result = TestResultType.Critical;
                    result.details = "The Object To Throw is null. This will throw errors at runtime if this isn't populated.";
                    results.Add(result);
                }
            }

            if (results.Count < 1)
                results.Add(result);

            return results;
        }
        public virtual List<TestResult> TEST_PerformActions(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "PerformActions";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            // robust enough, it won't error out

            if (results.Count < 1)
                results.Add(result);

            return results;
        }
        public virtual List<TestResult> TEST_ShooterNetworkCalls(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "ShooterNetworkCalls";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            ShooterNetworkCalls nc = target.GetComponent<ShooterNetworkCalls>();

            if (nc.syncMode != Mirror.SyncMode.Observers)
            {
                result.result = TestResultType.Critical;
                result.details = "The Sync Mode is not set to Observers. This will not work properly. You must use Observers.";
                results.Add(result);
            }
            if (nc.syncInterval != 0)
            {
                result.result = TestResultType.Warning;
                result.details = "The Sync Interval is not set to 0. That means clients will not be exact. The higher this value gets the more desynced clients will be. It does save on network traffic though.";
                results.Add(result);
            }

            if (results.Count < 1)
                results.Add(result);

            return results;
        }
        public virtual List<TestResult> TEST_DoorNetworkCalls(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "DoorNetworkCalls";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            DoorNetworkCalls nc = target.GetComponent<DoorNetworkCalls>();

            if (nc.syncMode != Mirror.SyncMode.Observers)
            {
                result.result = TestResultType.Critical;
                result.details = "The Sync Mode is not set to Observers. This will not work properly. You must use Observers.";
                results.Add(result);
            }
            if (nc.syncInterval != 0)
            {
                result.result = TestResultType.Warning;
                result.details = "The Sync Interval is not set to 0. That means clients will not be exact. The higher this value gets the more desynced clients will be. It does save on network traffic though.";
                results.Add(result);
            }

            if (results.Count < 1)
                results.Add(result);

            return results;
        }
        public virtual List<TestResult> TEST_UseItemEventTrigger(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "MP_UseItemEventTrigger";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            if (!target.GetComponent<MP_UseItemEventTrigger>())
            {
                result.result = TestResultType.Critical;
                result.details = "this is using the UseItemEventTrigger component instead of the MP_UseItemEventTrigger component. This will not work for multiplayer. Replace the UseItemEventTrigger component with the MP_UseItemEventTrigger component.";
                results.Add(result);
            }

            if (results.Count < 1)
                results.Add(result);

            return results;
        }
        public virtual List<TestResult> TEST_vShooterEquipment(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "MP_vShooterEquipment";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            if (!target.GetComponent<MP_vShooterEquipment>())
            {
                result.result = TestResultType.Critical;
                result.details = "this is using the vShooterEquipment component instead of the MP_vShooterEquipment component. This will not work for multiplayer. Replace the vShooterEquipment component with the MP_vShooterEquipment component.";
                results.Add(result);
            }
            if (results.Count < 1)
                results.Add(result);

            return results;
        }
        public virtual List<TestResult> TEST_vShooterWeapon(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "MP_vShooterWeapon";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            if (!target.GetComponent<MP_vShooterWeapon>() && !target.GetComponent<vShooterEquipment>())
            {
                result.result = TestResultType.Critical;
                result.details = "this is using the vShooterWeapon component instead of the MP_vShooterWeapon component. This will not work for multiplayer. Replace the vShooterWeapon component with the MP_vShooterWeapon component (only if not also using the vShooterEquipment component).";
                results.Add(result);
            }

            if (results.Count < 1)
                results.Add(result);

            return results;
        }
        public virtual List<TestResult> TEST_vThrowCollectable(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "MP_vThrowCollectable";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            if (!target.GetComponent<MP_vThrowCollectable>())
            {
                result.result = TestResultType.Critical;
                result.details = "this is using the vThrowCollectable component instead of the MP_vThrowCollectable component. This will not work for multiplayer. Replace the vThrowCollectable component with the MP_vThrowCollectable component.";
                results.Add(result);
            }

            if (results.Count < 1)
                results.Add(result);

            return results;
        }
        public virtual List<TestResult> TEST_vShooterWeaponNetworkCalls(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "vShooterWeaponNetworkCalls";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            vShooterWeaponNetworkCalls nc = target.GetComponent<vShooterWeaponNetworkCalls>();
            if (nc.syncMode != Mirror.SyncMode.Observers)
            {
                result.result = TestResultType.Critical;
                result.details = "The Sync Mode is not set to Observers. This will not work properly. You must use Observers.";
                results.Add(result);
            }
            if (nc.syncInterval != 0)
            {
                result.result = TestResultType.Warning;
                result.details = "The Sync Interval is not set to 0. That means clients will not be exact. The higher this value gets the more desynced clients will be. It does save on network traffic though.";
                results.Add(result);
            }

            if (results.Count < 1)
                results.Add(result);

            return results;
        }
        #endregion
    }
}
