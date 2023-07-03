using EMI.Managers;
using EMI.Object;
using EMI.Utils;
using Invector;
using Invector.vCamera;
using Invector.vCharacterController;
using Invector.vCharacterController.vActions;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EMI.Player
{
    public class BasicNetworkCalls : BaseNetworkCalls
    {
        #region Properties
        #region Inputs
        protected SyncDictionary<string, float> float_inputs = new SyncDictionary<string, float>();
        protected SyncDictionary<string, int> int_inputs = new SyncDictionary<string, int>();
        protected SyncDictionary<string, bool> bool_inputs = new SyncDictionary<string, bool>();
        protected List<string> waiting = new List<string>(); // only usable on the server
        #endregion

        #region Referenced
        protected NetworkIdentity ni = null;
        [HideInInspector] public ClientConnection client = null;
        [SerializeField, Tooltip("The tag that will be applied if not the owner.")]
        #endregion

        #region vHealthController
        protected vHealthController hc = null;
        [HideInInspector, SyncVar(hook = nameof(IsDeadChanged))] public bool isDead = false;
        [HideInInspector, SyncVar(hook = nameof(HealthRecoveryChanged))] public float healthRecovery = 0;
        [HideInInspector, SyncVar(hook = nameof(CurrentHealthChanged))] public float currentHealth = 100;
        [HideInInspector, SyncVar(hook = nameof(MaxHealthChanged))] public int maxHealth = 100;
        [HideInInspector, SyncVar(hook = nameof(CurrentHealthRecoveryChanged))] public float currentHealthRecoveryDelay = 0;
        #endregion

        #region vThirdPersonController
        protected MP_vThirdPersonController tpController = null;
        [HideInInspector, SyncVar(hook = nameof(Hook_SetStrafing))] public bool isStrafing = false;
        [HideInInspector, SyncVar(hook = nameof(Hook_SetSprinting))] public bool isSprinting = false;
        [HideInInspector, SyncVar(hook = nameof(Hook_SetWalking))] public bool isWalking = false;
        [HideInInspector, SyncVar(hook = nameof(Hook_SetCrouching))] public bool isCrouching = false;
        [HideInInspector, SyncVar(hook = nameof(Hook_Ragdolled))] public bool ragdolled = false;
        #endregion

        #region vThirdPersonCamera
        public vThirdPersonCamera ownerCam = null;
        [HideInInspector] public GameObject clientCam = null;
        protected Quaternion prevRot = Quaternion.identity;
        protected Vector3 prevPos = Vector3.zero;
        [HideInInspector, SyncVar(hook = nameof(Hook_SetCamPos))] public Vector3 camPos = Vector3.zero;
        [HideInInspector, SyncVar(hook = nameof(Hook_SetCamRot))] public Quaternion camRot = Quaternion.identity;
        [HideInInspector, SyncVar(hook = nameof(Hook_SetCamLockTarget))] public uint camLockTarget = 999999999;
        [HideInInspector, SyncVar] public float switchRight = 0;
        #endregion

        #region vThirdPersonInput
        protected vThirdPersonInput tpi = null;
        [HideInInspector, SyncVar(hook = nameof(LockInput))] public bool lockInput = false;
        #endregion

        #region vLadderAction
        [HideInInspector, SyncVar(hook = nameof(UsingLadder))] public bool onLadder = false;
        #endregion

        #region vGenericAction
        protected MP_vGenericAction ga = null;
        //[SyncVar(hook = nameof(UpdateButtonTimer)), HideInInspector] public float buttonTimer = 0;
        #endregion

        #region SceneManagement
        protected Scene prevScene;
        #endregion

        #endregion

        #region Initialization
        protected virtual void Awake()
        {
            hc = GetComponent<vHealthController>();
            tpi = GetComponent<vThirdPersonInput>();
            ga = GetComponent<MP_vGenericAction>();
            tpController = GetComponent<MP_vThirdPersonController>();
            if (tpController && NetworkServer.active)
                currentHealth = tpController.currentHealth;
        }
        protected virtual void Start()
        {
            ni = GetComponent<NetworkIdentity>();
            ownerCam = FindObjectOfType<MP_vThirdPersonCamera>();
            
            if (ni.hasAuthority && NetworkClient.active)
            {
                FindObjectOfType<EMI_NetworkManager>().MarkInitialSpawnCompleted();
                client = ClientUtil.GetMyClientConnectionObject();
                Cmd_SetClientConnectionValues(client.connId, gameObject, gameObject.scene.name);
            }
            else if (tpController != null)
            {
                clientCam = new GameObject($"Client Camera"); // if Camera isn't initialized by a network player the player will NOT animate while moving.
                clientCam.AddComponent<ClientCamera>();
                clientCam.transform.position = transform.position;
                tpController.rotateTarget = clientCam.transform;
                SceneManager.MoveGameObjectToScene(clientCam, SceneManager.GetSceneAt(0));
                StartCoroutine(InitializeNetworkedCharacter()); // make sure character is in the correct scene
                tpController.isStrafing = isStrafing;
                tpController.Sprint(isSprinting);
                tpController.freeSpeed.walkByDefault = isWalking;
                tpController.strafeSpeed.walkByDefault = isWalking;
                tpController.isCrouching = isCrouching;
                clientCam.transform.position = camPos;
                clientCam.transform.rotation = camRot;
                if (camLockTarget != 999999999) Hook_SetCamLockTarget(0, camLockTarget);
                UsingLadder(false, onLadder);

                //prevent generic action animation locks for late joiners
                GetComponent<Animator>().SetInteger("ActionState", 0);
            }
        }

        /// <summary>
        /// This waits for the "client" to exist then sets the initial values on this 
        /// character according to the settings stored in the client connection component.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator InitializeNetworkedCharacter()
        {
            yield return new WaitUntil(() => client != null);
            if (clientCam) clientCam.name = $"Client Camera[connectionId={client.connId}]"; // assign a unique findable name to the client camera.
            tpController.ChangeHealth(client.playerHealth);
            tpController.isDead = client.isDead;
            if (gameObject.scene.name != client.inScene)
            {
                SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByName(client.inScene));
            }
            yield return new WaitForSeconds(0.001f); // wait 100ms for scene to stabalize.
        }

        protected virtual void OnDestroy()
        {
            // All of this stuff should only execute for players not AI.
            if (tpController == null) return;
            if (clientCam) Destroy(clientCam);
            if (NetworkServer.active)
            {
                ClientConnection conn = ClientUtil.GetClientConnection(gameObject);
                if (conn)
                    NetworkServer.ReplacePlayerForConnection(GetComponent<NetworkIdentity>().connectionToClient, conn.gameObject, true);
            }
            if (client)
                client.playerCharacter = null;
        }
        #endregion

        #region Inputs Listener
        /// <summary>
        /// Will return the variable name from one of the dictionaries where it's stored.
        /// </summary>
        /// <param name="varName"></param>
        /// <returns></returns>
        public virtual object GetValue(string varName, Type ofType)
        {
            switch(Type.GetTypeCode(ofType))
            {
                case TypeCode.Boolean:
                    return (bool_inputs.ContainsKey(varName)) ? bool_inputs[varName] : false;
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return (int_inputs.ContainsKey(varName)) ? int_inputs[varName] : 0;
                case TypeCode.Single:
                    return (float_inputs.ContainsKey(varName)) ? float_inputs[varName] : 0;
            }
            return null;
        }
        
        /// <summary>
        /// Will update the appropriate dictionary with the desired value
        /// </summary>
        /// <param name="varName"></param>
        /// <param name="value"></param>
        [ClientCallback]
        public virtual void UpdateInput(string varName, object value)
        {
            if (!hasAuthority) return;
            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Boolean:
                    Cmd_UpdateBoolInput(varName, (bool)value);
                    break;
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    Cmd_UpdateIntInput(varName, (int)value);
                    break;
                case TypeCode.Single:
                    Cmd_UpdateFloatInput(varName, (float)value);
                    break;
            }
        }
        
        #if UNITY_SERVER || UNITY_EDITOR
        /// <summary>
        /// Note: We cannot sync an object because even though we pass in an int (for example)
        /// across the network it will only be recognized as a "object". So we must explicitly
        /// have the other functions passing data into this one be syncing explicit values
        /// such as float, int, or bool. 
        /// 
        /// This function has the server update the SyncDictionaries with updated input values
        /// that are received from the owning clients.
        /// </summary>
        /// <param name="varName">The button name</param>
        /// <param name="value">The current value for this button.</param>
        [Server]
        protected virtual void SetInputValue(string varName, object value)
        {
            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Boolean:
                    if (bool_inputs.ContainsKey(varName))
                    {
                        if (bool_inputs[varName] != (bool)value)
                        {
                            if (bool_inputs[varName] == true && (bool)value == false && !waiting.Contains(varName))
                            {
                                StartCoroutine(WaitForKeyPress(varName));
                            }
                            else
                            {
                                if ((bool)value == true && waiting.Contains(varName)) waiting.Remove(varName);
                                bool_inputs[varName] = (bool)value;
                            }
                        }
                    }
                    else
                    {
                        bool_inputs.Add(varName, (bool)value);
                    }
                    break;
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    if (int_inputs.ContainsKey(varName))
                    {
                        int_inputs[varName] = (int)value;
                    }
                    else
                    {
                        int_inputs.Add(varName, (int)value);
                    }
                    break;
                case TypeCode.Single:
                    if (float_inputs.ContainsKey(varName))
                    {
                        float_inputs[varName] = (float)value;
                    }
                    else
                    {
                        float_inputs.Add(varName, (float)value);
                    }
                    break;
            }
        }
        
        [Server]
        protected IEnumerator WaitForKeyPress(string varName)
        {
            waiting.Add(varName);
            yield return new WaitForSeconds(0.0005f); //50ms
            if (waiting.Contains(varName))
            {
                bool_inputs[varName] = false;
                waiting.Remove(varName);
            }
        }
        #endif
        #endregion

        #region Authority
        /// <summary>
        /// Will return true or false if you are currently the authority of this object
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool HasAuthority()
        {
            if (ni == null || (!NetworkClient.active && !NetworkServer.active))
            {
                return true;
            }
            else
            {
                return ni.hasAuthority || ni.isLocalPlayer;
            }
        }
        #endregion

        #region Heartbeat
        protected virtual void Update()
        {
            if (client && NetworkServer.active && gameObject.scene != prevScene)
            {
                prevScene = gameObject.scene;
                client.inScene = gameObject.scene.name.GetCleanSceneName();
            }

            if (ownerCam == null || !HasAuthority()) return;
            // Sync Camera position
            if (prevPos != ownerCam.transform.position)
            {
                prevPos = ownerCam.transform.position;
                Cmd_SetClientCamPos(ownerCam.transform.position);
            }
            // Sync camera rotation
            if (prevRot != ownerCam.transform.rotation)
            {
                prevRot = ownerCam.transform.rotation;
                Cmd_SetClientCamRot(ownerCam.transform.rotation);
            }
            // Sync camera lock target
            if (ownerCam.lockTarget && ownerCam.lockTarget.GetComponent<NetworkIdentity>() && 
                ownerCam.lockTarget.GetComponent<NetworkIdentity>().netId != camLockTarget)
            {
                camLockTarget = ownerCam.lockTarget.GetComponent<NetworkIdentity>().netId;
                Cmd_SetClientCamLockTarget(camLockTarget);
            }
        }
        #endregion

        #region RPCs
        #region Commands

        #region ClientConnection
        [Command]
        protected virtual void Cmd_SetClientConnectionValues(int connId, GameObject player, string sceneName)
        {
            ClientConnection conn = ClientUtil.GetClientConnection(connId);
            //conn.playerCharacter = player; // NOTE: This is now set in the EMI_NetworkManager
            conn.isDead = player.GetComponent<vThirdPersonController>().isDead;
            conn.playerHealth = (int)player.GetComponent<vThirdPersonController>().currentHealth;
            client = conn;
            GetComponent<vHealthController>().onChangeHealth.AddListener(OnHealthChanged);
            GetComponent<vHealthController>().onDead.AddListener(OnDead);
            GetComponent<MP_vThirdPersonController>().onMaxHealthChanged += OnMaxHealthChanged;
            conn.inScene = sceneName.GetCleanSceneName();
        }
        #endregion

        #region Input Listener
        [Command]
        protected virtual void Cmd_UpdateIntInput(string varName, int value)
        {
            #if UNITY_SERVER || UNITY_EDITOR
            SetInputValue(varName, value);
            #endif
        }
        [Command]
        protected virtual void Cmd_UpdateBoolInput(string varName, bool value)
        {
            #if UNITY_SERVER || UNITY_EDITOR
            SetInputValue(varName, value);
            #endif
        }
        [Command]
        protected virtual void Cmd_UpdateFloatInput(string varName, float value)
        {
            #if UNITY_SERVER || UNITY_EDITOR
            SetInputValue(varName, value);
            #endif
        }
        #endregion

        #region vThirdPersonCamera
        [Command]
        public virtual void Cmd_SetSwitchRight(float value)
        {
            #if UNITY_SERVER || UNITY_EDITOR
            switchRight = value;
            #endif
        }
        [Command]
        public virtual void Cmd_SetClientCamLockTarget(uint id)
        {
            #if UNITY_SERVER || UNITY_EDITOR
            camLockTarget = id;
            if (!NetworkClient.active)
            {
                Hook_SetCamLockTarget(0, id);
            }
            #endif
        }
        [Command]
        protected virtual void Cmd_SetClientCamPos(Vector3 pos)
        {
            #if UNITY_SERVER || UNITY_EDITOR
            camPos = pos;
            if (!NetworkClient.active)
            {
                Hook_SetCamPos(Vector3.zero, pos);
            }
            #endif
        }
        [Command]
        protected virtual void Cmd_SetClientCamRot(Quaternion rot)
        {
            #if UNITY_SERVER || UNITY_EDITOR
            camRot = rot;
            if (!NetworkClient.active)
            {
                Hook_SetCamRot(Quaternion.identity, rot);
            }
            #endif
        }
        #endregion

        #region vThirdPersonInput
        [Command]
        public virtual void Cmd_LockInput(bool value)
        {
            #if UNITY_SERVER || UNITY_EDITOR
            lockInput = value;
            if (tpi.lockInput != value)
                tpi.SetLockAllInput(value);
            #endif
        }
        #endregion

        #endregion

        #region ClientRPCs
        [ClientRpc]
        public virtual void Rpc_LadderResetPlayerSettings()
        {
            #if !UNITY_SERVER || UNITY_EDITOR
            GetComponent<MP_vLadderAction>().Server_ResetPlayerSettings();
            #endif
        }
        #endregion
        #endregion

        #region Hooks
        #region vHealthController
        protected virtual void IsDeadChanged(bool oldValue, bool newValue)
        {
            if (NetworkServer.active) return;
            hc.isDead = newValue;
        }
        public virtual void HealthRecoveryChanged(float oldValue, float newValue)
        {
            if (NetworkServer.active) return;
            hc.healthRecovery = newValue;
        }
        public virtual void CurrentHealthRecoveryChanged(float oldValue, float newValue)
        {
            if (NetworkServer.active) return;
            hc.currentHealthRecoveryDelay = newValue;
        }
        protected virtual void CurrentHealthChanged(float oldValue, float newValue)
        {
            if (NetworkServer.active) return;
            hc.ChangeHealth((int)newValue);
        }
        protected virtual void MaxHealthChanged(int oldValue, int newValue)
        {
            if (NetworkServer.active) return;
            hc.ChangeMaxHealth(newValue);
        }
        #endregion

        #region vThirdPersonController
        protected virtual void Hook_SetStrafing(bool oldValue, bool newValue)
        {
            if (tpController && tpController.isStrafing != newValue)
            {
                tpController.isStrafing = newValue;
            }
        }
        protected virtual void Hook_SetSprinting(bool oldValue, bool newValue)
        {
            if (tpController && tpController.isSprinting != newValue)
                tpController.Sprint(newValue);
        }
        protected virtual void Hook_SetWalking(bool oldValue, bool newValue)
        {
            if (tpController && (tpController.freeSpeed.walkByDefault != newValue ||
                    tpController.strafeSpeed.walkByDefault != newValue))
            {
                tpController.freeSpeed.walkByDefault = newValue;
                tpController.strafeSpeed.walkByDefault = newValue;
            }
        }
        protected virtual void Hook_SetCrouching(bool oldValue, bool newValue)
        {
            if (tpController && tpController.isCrouching != newValue)
            {
                tpController.isCrouching = newValue;
            }
        }
        protected virtual void Hook_Ragdolled(bool oldValue, bool newValue)
        {
            if (NetworkServer.active) return;
            tpController._ragdolled = newValue;
            if (newValue == true)
            {
                transform.position = GetComponent<ServerSync>().GetServerPosition();
                transform.rotation = GetComponent<ServerSync>().GetServerRotation();
            }
        }
        #endregion

        #region vThirdPersonCamera
        protected virtual void Hook_SetCamLockTarget(uint oldValue, uint newValue)
        {
            if (newValue == 999999999)
            {
                if (clientCam)
                {
                    clientCam.GetComponent<ClientCamera>().lockTarget = null;
                }
                return;
            }
            NetworkIdentity target = FindObjectsOfType<NetworkIdentity>().ToList().Find(x => x.netId == newValue);
            if (target && clientCam)
            {
                clientCam.GetComponent<ClientCamera>().lockTarget = target.transform;
            }
            else if (clientCam)
            {
                clientCam.GetComponent<ClientCamera>().lockTarget = null;
            }
        }
        protected virtual void Hook_SetCamRot(Quaternion oldValue, Quaternion newValue)
        {
            if (clientCam != null) clientCam.transform.rotation = newValue;
        }
        protected virtual void Hook_SetCamPos(Vector3 oldValue, Vector3 newValue)
        {
            if (clientCam != null) clientCam.transform.position = newValue;
        }
        #endregion

        #region vLadderAction
        public virtual void UsingLadder(bool oldValue, bool newValue)
        {
            if (!NetworkServer.active && !NetworkClient.active || NetworkServer.active || !GetComponent<MP_vLadderAction>()) 
                return;
            if (newValue == true)
                GetComponent<MP_vLadderAction>().EnterLadder();
            else
                GetComponent<MP_vLadderAction>().ExitLadder();
        }
        #endregion

        #region vThirdPersonInput
        protected virtual void LockInput(bool oldValue, bool newValue)
        {
            if (NetworkServer.active) return; //taken care of in Command, no need to double dip
            if (tpi.lockInput != newValue)
                tpi.SetLockAllInput(newValue);
        }
        #endregion

        #endregion

        #region Events/Delegates
        public virtual void OnHealthChanged(float health)
        {
            client.playerHealth = (int)health;
            if (health > 0 && client.isDead)
            {
                client.isDead = false;
            }
        }
        public virtual void OnMaxHealthChanged(float value)
        {
            client.playerMaxHealth = (int)value;
        }
        public virtual void OnDead(GameObject target)
        {
            client.isDead = true;
        }
        #endregion
    }
}
