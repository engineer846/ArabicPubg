using Consolation;
using EMI.Managers;
using EMI.Object;
using EMI.Player;
using Invector;
using Invector.vCamera;
using Invector.vCharacterController;
using Invector.vCharacterController.vActions;
using Mirror;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EMI.Editors
{
    public class BasicPerformTests
    {
        #region Root Function
        public virtual List<TestResult> PerformTests(List<string> scenes, List<string> projectPaths)
        {
            int objectsTested = 0;
            BasicTests bt = new BasicTests();
            List<TestResult> results = new List<TestResult>();

            // Test Project Prefabs
            foreach (string path in projectPaths)
            {
                if (path.Contains("Assets/Invector-3rdPersonController")) continue;
                objectsTested++;
                UnityEngine.Object o = AssetDatabase.LoadMainAssetAtPath(path);
                GameObject go;
                go = (GameObject)o;
                results.AddRange(TestObjectComponents(go));
            }

            // Test Scenes
            foreach(string scene in scenes)
            {
                if (EditorSceneManager.GetSceneByName(scene).isLoaded)
                {
                    // scene is already loaded test the objects
                    foreach(GameObject obj in EditorSceneManager.GetSceneByName(scene).GetRootGameObjects())
                    {
                        results.AddRange(TestObjectComponents(obj, scene));
                    }
                }
                else
                {
                    // Load the scene and test it since it isn't loaded
                    Scene sceneObj = EditorSceneManager.OpenScene(scene, OpenSceneMode.Additive);
                    foreach (GameObject obj in sceneObj.GetRootGameObjects())
                    {
                        results.AddRange(TestObjectComponents(obj, scene));
                    }
                }
            }

            return results;
        }
        #endregion

        #region Helper Loop Function
        public virtual List<TestResult> TestObjectComponents(GameObject obj, string sceneName = "")
        {
            BasicTests bt = new BasicTests();
            List<TestResult> results = new List<TestResult>();
            Component[] components = obj.GetComponentsInChildren<Component>(true);
            foreach (Component c in components)
            {
                switch (c)
                {
                    case vThirdPersonController controller:
                        results.AddRange(bt.TEST_vThirdPersonController(c.gameObject, sceneName));
                        break;
                    case vTutorialTextTrigger tt:
                        results.AddRange(bt.TEST_vTutorialTextTrigger(c.gameObject, sceneName));
                        break;
                    case EMI_NetworkManager nm:
                        results.AddRange(bt.TEST_EMI_NetworkManager(c.gameObject, sceneName));
                        break;
                    case vThirdPersonCamera cam:
                        results.AddRange(bt.TEST_vThirdPersonCamera(c.gameObject, sceneName));
                        break;
                    case TriggerChangeTeam ct:
                        results.AddRange(bt.TEST_TriggerChangeTeam(c.gameObject, sceneName));
                        break;
                    case ClientConnection cc:
                        results.AddRange(bt.TEST_ClientConnection(c.gameObject, sceneName));
                        break;
                    case DebugConsole dc:
                        results.AddRange(bt.TEST_DebugConsole(c.gameObject, sceneName));
                        break;
                    case vHitDamageParticle hp:
                        results.AddRange(bt.TEST_vHitDamageParticle(c.gameObject, sceneName));
                        break;
                    case BasicNetworkCalls nc:
                        results.AddRange(bt.TEST_BasicNetworkCalls(c.gameObject, sceneName));
                        break;
                    case vThirdPersonInput tpi:
                        results.AddRange(bt.TEST_vThirdPersonInput(c.gameObject, sceneName));
                        break;
                    case vLadderAction la:
                        results.AddRange(bt.TEST_vLadderAction(c.gameObject, sceneName));
                        break;
                    case vHeadTrack ht:
                        results.AddRange(bt.TEST_vHeadTrack(c.gameObject, sceneName));
                        break;
                    case vGenericAction ga:
                        results.AddRange(bt.TEST_vGenericAction(c.gameObject, sceneName));
                        break;
                    case Revive rv:
                        results.AddRange(bt.TEST_Revive(c.gameObject, sceneName));
                        break;
                    case Team team:
                        results.AddRange(bt.TEST_Team(c.gameObject, sceneName));
                        break;
                    case NetworkIdentity ni:
                        results.AddRange(bt.TEST_NetworkIdentity(c.gameObject, sceneName));
                        break;
                }
            }
            return results;
        }
        #endregion
    }

    [System.Serializable]
    public class BasicTests
    {
        #region Properties
        protected BindingFlags bindings = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;
        #endregion

        #region Individual Component Tests
        public virtual List<TestResult> TEST_vThirdPersonController(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "MP_vThirdPersonController";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            if (!target.GetComponent<MP_vThirdPersonController>())
            {
                result.result = TestResultType.Critical;
                result.details = "This is using the vThirdPersonController component instead of the MP_vThirdPersonController component. This will not work in a multiplayer scenario.";
                results.Add(result);
            }
            else
            {
                MP_vThirdPersonController controller = target.GetComponent<MP_vThirdPersonController>();
                if ((bool)controller.GetType().GetField("preventSelfDamage", bindings).GetValue(controller) == false)
                {
                    result.result = TestResultType.Warning;
                    result.details = "Prevent Self Damage is false. This will result in being able to shoot or slash yourself. Dont do this unless you know what you're doing.";
                    results.Add(result);
                }
                if (controller.removeComponentsAfterDie == true)
                {
                    result.result = TestResultType.Critical;
                    result.details = "Remove Components After Die should always be false. You have it marked true.";
                    results.Add(result);
                }
                if (controller.debugActionListener == true)
                {
                    result.result = TestResultType.Warning;
                    result.details = "Debug Action Listener is true. This is fine for debugging, just make sure this is disabled for a production build.";
                    results.Add(result);
                }
                if (controller.debugWindow == true)
                {
                    result.result = TestResultType.Warning;
                    result.details = "Debug Window is true. This is fine for debugging, just make sure this is disabled for a production build.";
                    results.Add(result);
                }
            }

            if (results.Count < 1)
                results.Add(result);
            return results;
        }
        public virtual List<TestResult> TEST_vTutorialTextTrigger(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "MP_vTutorialTextTrigger";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            if (target.GetComponentInChildren<MP_vTutorialTextTrigger>(true))
            {
                MP_vTutorialTextTrigger tt = target.GetComponentInChildren<MP_vTutorialTextTrigger>(true);
                if (tt._textUI == null)
                {
                    if (!string.IsNullOrEmpty(sceneName))
                    {
                        result.result = TestResultType.Critical;
                        result.details = "This text trigger has not assigned Text UI. This WILL throw an error at runtime.";
                        results.Add(result);
                    }
                    else
                    {
                        result.result = TestResultType.Warning;
                        result.details = "This text trigger has not assigned Text UI. This WILL throw an error at runtime if this doesn't end up getting assigned in the scene it's used in.";
                        results.Add(result);
                    }
                }
                if (tt.painel == null)
                {
                    if (!string.IsNullOrEmpty(sceneName))
                    {
                        result.result = TestResultType.Critical;
                        result.details = "This text trigger has not assigned Painel. This WILL throw an error at runtime.";
                        results.Add(result);
                    }
                    else
                    {
                        result.result = TestResultType.Warning;
                        result.details = "This text trigger has not assigned Painel. This WILL throw an error at runtime if this doesn't end up getting assigned in the scene it's used in.";
                        results.Add(result);
                    }
                }
                if (string.IsNullOrEmpty(tt.text))
                {
                    result.result = TestResultType.Warning;
                    result.details = "This text trigger has not assigned Text. There is literally no point to this trigger, it should be removed.";
                    results.Add(result);
                }
            }
            else
            {
                result.result = TestResultType.Critical;
                result.details = "This is using the vTutorialTextTrigger instead of the MP_vTutorialTextTrigger. This will not work in a multiplayer scenario.";
                results.Add(result);
            }

            if (results.Count < 1)
                results.Add(result);

            return results;
        }
        public virtual List<TestResult> TEST_EMI_NetworkManager(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "EMI_NetworkManager";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            if (target.GetComponent<EMI_NetworkManager>())
            {
                EMI_NetworkManager nm = target.GetComponent<EMI_NetworkManager>();
                if (nm.dontDestroyOnLoad == true)
                {
                    result.result = TestResultType.Warning;
                    result.details = "Dont Destroy On Load is meant as a feature for advanced users only. This should not be used if you don't fully understand the ramifications of using this.";
                    results.Add(result);
                }
                //if (nm.PersistNetworkManagerToOfflineScene == true)
                //{
                //    result.result = TestResultType.Warning;
                //    result.details = "Dont Destroy On Load is meant as a feature for advanced users only. This should not be used if you don't fully understand the ramifications of using this.";
                //    results.Add(result);
                //}
                if (nm.runInBackground == false)
                {
                    result.result = TestResultType.Critical;
                    result.details = "Run In Background is false. This needs to be true to have this work in the editor.";
                    results.Add(result);
                }
                if (nm.autoStartServerBuild == false)
                {
                    result.result = TestResultType.Critical;
                    result.details = "Auto Start Server Build is false. This needs to be true to automatically start your server when you run your built server executable.";
                    results.Add(result);
                }
                if (nm.serverTickRate < 60)
                {
                    result.result = TestResultType.Warning;
                    result.details = "The Server Tick Rate is less than 60. The way invector works this should probably remain at 60. Don't adjust this unless you are an advanced user and know exactly the ramifications of modifying this might be.";
                    results.Add(result);
                }
                if ((Transport)nm.GetType().GetField("transport", bindings).GetValue(nm) == null)
                {
                    result.result = TestResultType.Critical;
                    result.details = "You haven't specified a Transport. Nothing will work on the network until you specify this.";
                    results.Add(result);
                }
                if (string.IsNullOrEmpty(nm.networkAddress))
                {
                    result.result = TestResultType.Warning;
                    result.details = "IPv4, IPv6, or FQDN must exist in the Network address BEFORE starting your client. This is currently empty.";
                    results.Add(result);
                }
                if (nm.maxConnections < 2)
                {
                    result.result = TestResultType.Critical;
                    result.details = "The max connections is less than 2. That means only one client will ever be able to connect to the server, whats the point?";
                    results.Add(result);
                }
                if (nm.playerPrefab == null)
                {
                    result.result = TestResultType.Critical;
                    result.details = "The Player Prefab is null. Add the \"ClientConnection\" in here.";
                    results.Add(result);
                }
                if (nm.characterToSpawn == null)
                {
                    result.result = TestResultType.Warning;
                    result.details = "The Character To Spawn is null. This needs to be populated BEFORE attempting to spawn a clients character.";
                    results.Add(result);
                }
                if ((bool)nm.GetType().GetField("debugWindow", bindings).GetValue(nm) == true)
                {
                    result.result = TestResultType.Warning;
                    result.details = "The Debug Window is true. This is fine for testing but should be disable for production builds.";
                    results.Add(result);
                }
                if ((bool)nm.GetType().GetField("verboseLogging", bindings).GetValue(nm) == true)
                {
                    result.result = TestResultType.Warning;
                    result.details = "The Verbose Logging is true. This is fine for testing but should be disable for production builds.";
                    results.Add(result);
                }
                if (((List<GameObject>)nm.GetType().GetField("spawnPrefabs", bindings).GetValue(nm)).Count < 1)
                {
                    result.result = TestResultType.Critical;
                    result.details = "You don't have any spawnable prefabs, this includes player characters. You need to populate your Registered Spawn Prefabs list.";
                    results.Add(result);
                }
            }


            if (results.Count < 1)
                results.Add(result);

            return results;
        }
        public virtual List<TestResult> TEST_vThirdPersonCamera(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "MP_vThirdPersonCamera";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            if (target.GetComponent<MP_vThirdPersonCamera>())
            {
                MP_vThirdPersonCamera cam = target.GetComponent<MP_vThirdPersonCamera>();
                if (cam.CameraStateList == null)
                {
                    result.result = TestResultType.Critical;
                    result.details = "The CameraState List is empty. This will result in errors with the camera states at runtime.";
                    results.Add(result);
                }
                if (!target.GetComponentInChildren<Camera>(true))
                {
                    result.result = TestResultType.Critical;
                    result.details = "There is no camera as a child of this object. This component only works with a camera as it's child.";
                    results.Add(result);
                }
                if (target.transform.parent != null && !target.GetComponent<vRemoveParent>())
                {
                    result.result = TestResultType.Critical;
                    result.details = "The camera must be a free standing object. It cannot have any parent. To guarantee this add the \"vRemoveParent\" component.";
                    results.Add(result);
                }
            }
            else
            {
                result.result = TestResultType.Critical;
                result.details = "This is using the vThirdPersonCamera component not the MP_vThirdPersonCamera component. This will not work in a multiplayer scenario.";
                results.Add(result);
            }

            if (results.Count < 1)
                results.Add(result);

            return results;
        }
        public virtual List<TestResult> TEST_TriggerChangeTeam(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "TriggerChangeTeam";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            if (target.GetComponent<TriggerChangeTeam>())
            {
                TriggerChangeTeam ct = target.GetComponent<TriggerChangeTeam>();
                if (string.IsNullOrEmpty((string)ct.GetType().GetField("newTeamName", bindings).GetValue(ct)))
                {
                    result.result = TestResultType.Critical;
                    result.details = "The New Team Name field is empty.";
                    results.Add(result);
                }
                if ((bool)ct.GetType().GetField("onEnter", bindings).GetValue(ct) == false && (bool)ct.GetType().GetField("onExit", bindings).GetValue(ct) == false)
                {
                    result.result = TestResultType.Critical;
                    result.details = "The On Enter AND On Exit are false. This will never fire.";
                    results.Add(result);
                }
            }

            if (results.Count < 1)
                results.Add(result);

            return results;
        }
        public virtual List<TestResult> TEST_ClientConnection(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "ClientConnection";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            if (target.GetComponent<ClientConnection>())
            {
                ClientConnection cc = target.GetComponent<ClientConnection>();
                if (cc.syncMode == SyncMode.Owner)
                {
                    result.result = TestResultType.Critical;
                    result.details = "The Sync Mode is set to owner. This will not work properly. This must be set to Observers.";
                    results.Add(result);
                }
                if (cc.syncInterval > 0.1f)
                {
                    result.result = TestResultType.Warning;
                    result.details = "The Sync Interval is greater than 0.1. This is fine but if you looking for immediate changes over the network that's not going to happen with a sync value greater than 0.1;";
                    results.Add(result);
                }
            }

            if (results.Count < 1)
                results.Add(result);

            return results;
        }
        public virtual List<TestResult> TEST_DebugConsole(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "DebugConsole";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            if (target.GetComponent<DebugConsole>())
            {
                DebugConsole dc = target.GetComponent<DebugConsole>();
                if (!target.GetComponent<EMI_NetworkManager>())
                {
                    result.result = TestResultType.Warning;
                    result.details = "This has a Debug Console but is not part of the EMI_NetworkManager. This is operating outside of it's intended parameters. It will be up to you to debug any other potential errors you have.";
                    results.Add(result);
                }
            }

            if (results.Count < 1)
                results.Add(result);

            return results;
        }
        public virtual List<TestResult> TEST_vHitDamageParticle(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "MP_vHitDamageParticle";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            if (target.GetComponent<vThirdPersonController>())
            {
                if (!target.GetComponent<MP_vHitDamageParticle>())
                {
                    result.result = TestResultType.Critical;
                    result.details = "This is using the vHitDamageParticle component instead of the MP_vHitDamageParticle component. This will not work properly in a multiplayer scenario.";
                    results.Add(result);
                }
                else
                {
                    MP_vHitDamageParticle dp = target.GetComponent<MP_vHitDamageParticle>();
                    if ((bool)dp.GetType().GetField("displaySelfDamage", bindings).GetValue(dp) == true)
                    {
                        result.result = TestResultType.Warning;
                        result.details = "This Display Self Damage is marked as true. This should only be used by advanced users who know exactly how this work.";
                        results.Add(result);
                    }
                }
            }
            else
            {
                vHitDamageParticle dp = target.GetComponent<vHitDamageParticle>();
                if (dp.defaultDamageEffects.Count < 1)
                {
                    result.result = TestResultType.Warning;
                    result.details = "This doesn't have the Default Damage Effect particle set.";
                    results.Add(result);
                }
            }

            if (results.Count < 1)
                results.Add(result);

            return results;
        }
        public virtual List<TestResult> TEST_BasicNetworkCalls(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "BasicNetworkCalls";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            if (target.GetComponent("MeleeAINetworkCalls") || 
                target.GetComponent("MeleeNetworkCalls") || 
                target.GetComponent("ShooterNetworkCalls"))
            {
                results.Add(result);
                return results;
            }
            BasicNetworkCalls nc = target.GetComponent<BasicNetworkCalls>();
            if (nc.syncMode != SyncMode.Observers)
            {
                result.result = TestResultType.Critical;
                result.details = "The Sync Mode is set to Owner. This should be set to Observers.";
                results.Add(result);
            }
            if (nc.syncInterval != 0)
            {
                result.result = TestResultType.Warning;
                result.details = "The Sync Interval is not set to 0. While this will save on network traffic it could produce laggy movement for clients.";
                results.Add(result);
            }

            if (results.Count < 1)
                results.Add(result);

            return results;
        }
        public virtual List<TestResult> TEST_ServerSync(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "BasicNetworkCalls";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            ServerSync ss = target.GetComponent<ServerSync>();
            if (target.GetComponent<vThirdPersonController>())
            {
                if ((float)ss.GetType().GetField("interpolation", bindings).GetValue(ss) < 0.5f)
                {
                    result.result = TestResultType.Warning;
                    result.details = "The Interpolation value is less than 0.5f. While this will snap to positions faster it might also generate jumpy movement for people if set too low.";
                    results.Add(result);
                }
                if ((float)ss.GetType().GetField("forceSnapDistance", bindings).GetValue(ss) > 2)
                {
                    result.result = TestResultType.Warning;
                    result.details = "The Force Snap Distance is greater than 2. Having this too high might result in clients not correctly syncing with the server enough to create meaningful movement.";
                    results.Add(result);
                }
                if ((float)ss.GetType().GetField("forceSnapDistance", bindings).GetValue(ss) < 1)
                {
                    result.result = TestResultType.Warning;
                    result.details = "The Force Snap Distance is less than 1. Having this too low will making client movement very jumpy.";
                    results.Add(result);
                }
                if ((bool)ss.GetType().GetField("position", bindings).GetValue(ss) == false)
                {
                    result.result = TestResultType.Critical;
                    result.details = "This isn't syncing the players position.";
                    results.Add(result);
                }
                if ((bool)ss.GetType().GetField("rotation", bindings).GetValue(ss) == false)
                {
                    result.result = TestResultType.Critical;
                    result.details = "This isn't syncing the players rotation.";
                    results.Add(result);
                }
                if ((bool)ss.GetType().GetField("scale", bindings).GetValue(ss) == false)
                {
                    result.result = TestResultType.Warning;
                    result.details = "This isn't syncing the players scale.";
                    results.Add(result);
                }
                if (ss.syncInterval != 0)
                {
                    result.result = TestResultType.Warning;
                    result.details = "The Sync Interval is not 0. While this will lower the network traffic having a too high value with cause other clients to get desynced and snapped to positions.";
                    results.Add(result);
                }
            }
            if ((bool)ss.GetType().GetField("position", bindings).GetValue(ss) == false &&
                    (bool)ss.GetType().GetField("rotation", bindings).GetValue(ss) == false &&
                    (bool)ss.GetType().GetField("scale", bindings).GetValue(ss) == false)
            {
                result.result = TestResultType.Critical;
                result.details = "This isn't syncing anything. There is not point in having this component at all if you're not syncing anything.";
                results.Add(result);
            }
            if (ss.syncMode != SyncMode.Observers)
            {
                result.result = TestResultType.Critical;
                result.details = "The Sync Mode is set to Owner. This must be set to Observers.";
                results.Add(result);
            }

            if (results.Count < 1)
                results.Add(result);

            return results;
        }
        public virtual List<TestResult> TEST_vThirdPersonInput(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "MP_vThirdPersonInput";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            if (!target.GetComponent<MP_vThirdPersonInput>() && !target.GetComponent("MP_vShooterMeleeInput") && !target.GetComponent("MP_v_AIController") && !target.GetComponent("MP_vMeleeCombatInput"))
            {
                result.result = TestResultType.Critical;
                result.details = "This using the vThirdPersonInput component and not the MP_vThirdPersonInput component. This will not work in multiplayer games.";
                results.Add(result);
            }

            if (results.Count < 1)
                results.Add(result);

            return results;
        }
        public virtual List<TestResult> TEST_vLadderAction(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "MP_vLadderAction";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            if (!target.GetComponent<MP_vLadderAction>())
            {
                result.result = TestResultType.Critical;
                result.details = "This using the vLadderAction component and not the MP_vLadderAction component. This will not work in multiplayer games.";
                results.Add(result);
            }

            if (results.Count < 1)
                results.Add(result);

            return results;
        }
        public virtual List<TestResult> TEST_vHeadTrack(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "MP_vHeadTrack";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            if (!target.GetComponent<MP_vHeadTrack>())
            {
                result.result = TestResultType.Critical;
                result.details = "This using the vHeadTrack component and not the MP_vHeadTrack component. This will not work in multiplayer games.";
                results.Add(result);
            }

            if (results.Count < 1)
                results.Add(result);

            return results;
        }
        public virtual List<TestResult> TEST_vGenericAction(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "MP_vGenericAction";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            if (!target.GetComponent<MP_vGenericAction>())
            {
                result.result = TestResultType.Critical;
                result.details = "This using the vGenericAction component and not the MP_vGenericAction component. This will not work in multiplayer games.";
                results.Add(result);
            }

            if (results.Count < 1)
                results.Add(result);

            return results;
        }
        public virtual List<TestResult> TEST_Revive(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "Revive";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            Revive rv = target.GetComponent<Revive>();
            if (rv.healthPercent == 0)
            {
                result.result = TestResultType.Critical;
                result.details = "The health percent is 0. That means when the revive is trigger will is not going to restore any health.";
                results.Add(result);
            }

            if (results.Count < 1)
                results.Add(result);

            return results;
        }
        public virtual List<TestResult> TEST_Team(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "Team";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            Team team = target.GetComponent<Team>();
            if (target.GetComponent<vThirdPersonController>())
            {
                if ((bool)team.GetType().GetField("autoAssignCharacterTeam", bindings).GetValue(team) == false)
                {
                    result.result = TestResultType.Critical;
                    result.details = "Auto Assign Character Team is false. This needs to be true for players to dynamically pickup their team settings properly.";
                    results.Add(result);
                }
                if ((TeamData)team.GetType().GetField("teamData", bindings).GetValue(team) != null)
                {
                    result.result = TestResultType.Critical;
                    result.details = "The Team Data is assigned on a player. This should never be assigned to allow players to correctly get assigned to teams via code.";
                    results.Add(result);
                }
            }
            else
            {
                if ((bool)team.GetType().GetField("autoAssignCharacterTeam", bindings).GetValue(team) == true)
                {
                    result.result = TestResultType.Warning;
                    result.details = "Auto Assign Character Team is true. This is going to assign this object to the same team settings as the player. Make sure this is really what you want.";
                    results.Add(result);
                }
            }

            if (team.syncMode != SyncMode.Observers)
            {
                result.result = TestResultType.Critical;
                result.details = "The Sync Mode is set to Owner. This must be set to Observers.";
                results.Add(result);
            }
            if (team.syncInterval != 0)
            {
                result.result = TestResultType.Critical;
                result.details = "The Sync Interval is not 0. While this saves on network traffic its not good for high intensity games (that are designed with the Invector Controller).";
                results.Add(result);
            }

            if (results.Count < 1)
                results.Add(result);

            return results;
        }
        public virtual List<TestResult> TEST_NetworkIdentity(GameObject target, string sceneName = "")
        {
            TestResult result = new TestResult();
            result.compName = "NetworkIdentity";
            result.details = "";
            result.result = TestResultType.Pass;
            result.target = target;
            result.sceneName = sceneName;
            List<TestResult> results = new List<TestResult>();

            if (target.GetComponent<NetworkIdentity>())
            {
                if (target.transform.parent != null)
                {
                    result.result = TestResultType.Critical;
                    result.details = "This object has a NetworkIdentity on it, but also has a parent. Objects with NetworkIdentities cannot have any parent object. This is a Mirror requirement.";
                    results.Add(result);
                }
            }

            if (results.Count < 1)
                results.Add(result);

            return results;
        }
        #endregion
    }
}
