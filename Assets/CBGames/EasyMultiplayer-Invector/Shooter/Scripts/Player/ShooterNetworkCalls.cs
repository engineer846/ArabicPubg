using EMI.Utils;
using Invector.vCharacterController;
using Invector.vCharacterController.vActions;
using Invector.vItemManager;
using Invector.vMelee;
using Invector.vShooter;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EMI.Player
{
    public struct SyncvAmmo
    {
        public string ammoName;
        public int ammoID;
        public int count;
        public List<int> ammoItemIds;
    }

    public class ShooterNetworkCalls : MeleeNetworkCalls
    {
        #region Properties
        #region vCollectShooterMeleeControl
        protected MP_vCollectShooterMeleeControl csmc = null;
        #endregion

        #region vShooterManager
        protected MP_vShooterManager sm = null;
        [HideInInspector, SyncVar(hook = nameof(Hook_SetCMCShooterLeftWeapon))] public uint setCMCShooterLeftWeapon = 0;
        [HideInInspector, SyncVar(hook = nameof(Hook_SetCMCShooterRightWeapon))] public uint setCMCShooterRightWeapon = 0;
        [HideInInspector, SyncVar(hook = nameof(Hook_OnRightWeaponAmmoChanged))] public int rightWeaponAmmo = 0;
        [HideInInspector, SyncVar(hook = nameof(Hook_OnLeftWeaponAmmoChanged))] public int leftWeaponAmmo = 0;
        protected List<int> outOfSyncIDs = new List<int>();
        #endregion

        #region vShooterMeleeInput
        protected MP_vShooterMeleeInput smi = null;
        [HideInInspector] public GameObject clientScopeCamera = null;
        [HideInInspector] public GameObject clientZoomScopeCamera = null;
        [HideInInspector, SyncVar] public bool isUsingScopeView = false;
        [HideInInspector, SyncVar(hook = nameof(Hook_SetScopeCameraPosition))] public Vector3 scopeCameraPosition = Vector3.zero;
        [HideInInspector, SyncVar(hook = nameof(Hook_SetScopeCameraRotation))] public Quaternion scopeCameraRotation = Quaternion.identity;
        [HideInInspector, SyncVar(hook = nameof(Hook_SetZoomCameraPosition))] public Vector3 zoomCameraPosition = Vector3.zero;
        [HideInInspector, SyncVar(hook = nameof(Hook_SetZoomCameraRotation))] public Quaternion zoomCameraRotation = Quaternion.identity;
        [HideInInspector, SyncVar] public bool isScopeCameraActive = false;

        [HideInInspector, SyncVar] public float clientCamFarClipPlane = 180f;
        #endregion

        #region vThrowManager
        protected MP_vThrowManager tm = null;
        [HideInInspector, SyncVar(hook = nameof(Hook_UpdateCurrentThrow))] public int currentThrowObject = 0;
        [HideInInspector, SyncVar(hook = nameof(Hook_UpdateThrowUI))] public int maxThrowObjects = 0;
        #endregion
        #endregion

        #region Initilization
        protected override void Awake()
        {
            csmc = GetComponent<MP_vCollectShooterMeleeControl>();
            sm = GetComponent<MP_vShooterManager>();
            tm = (MP_vThrowManager)gameObject.FindComponent(typeof(MP_vThrowManager));
            base.Awake();
        }
        protected override void Start()
        {
            base.Start();
            if (!NetworkClient.active)
            if (NetworkClient.active && !hasAuthority || (NetworkServer.active && !NetworkClient.active)) // not owning client or the server without a client
            {
                StartCoroutine(InitClientCameras());
            }
            else
            {
                #region vShooterMeleeInput
                smi = GetComponent<MP_vShooterMeleeInput>();
                Camera owningCamera = (Camera)ownerCam.gameObject.FindComponent(typeof(Camera));
                Cmd_SetFarClipPlane(owningCamera.farClipPlane);
                smi.controlAimCanvas.onDisableScopeCamera.AddListener(DisableScopeCamera);
                smi.controlAimCanvas.onEnableScopeCamera.AddListener(EnableScopeCamera);
                #endregion
            }
            if (sm)
            {
                Hook_OnRightWeaponAmmoChanged(0, rightWeaponAmmo);
                Hook_OnLeftWeaponAmmoChanged(0, leftWeaponAmmo);
            }

            if (csmc != null)
            {
                StartCoroutine(InitEquippedWeapons());
            }

            StartCoroutine(InitChildColliders());
        }
        protected override IEnumerator InitEquippedWeapons()
        {
            yield return new WaitForSeconds(0.1f);
            // Check for already equipped melee weapons
            if (setleftWeapon != 0)
            {
                GameObject weapon = FindObjectsOfType<NetworkIdentity>().ToList().Find(x => x.netId == setleftWeapon).gameObject;
                vCollectableStandalone collectable = (vCollectableStandalone)weapon.FindComponent(typeof(vCollectableStandalone));
                if (collectable != null)
                {
                    if (csmc != null)
                    {
                        csmc.ClientEquipMeleeWeapon(collectable);
                    }
                    else if (cmc != null)
                    {
                        cmc.ClientEquipMeleeWeapon(collectable);
                    }
                }
            }
            if (setrightWeapon != 0)
            {
                GameObject weapon = FindObjectsOfType<NetworkIdentity>().ToList().Find(x => x.netId == setrightWeapon).gameObject;
                vCollectableStandalone collectable = (vCollectableStandalone)weapon.FindComponent(typeof(vCollectableStandalone));
                if (collectable != null)
                {
                    if (csmc != null)
                    {
                        csmc.ClientEquipMeleeWeapon(collectable);
                    }
                    else if (cmc != null)
                    {
                        cmc.ClientEquipMeleeWeapon(collectable);
                    }
                }
            }

            // Check for already equipped shooter weapons
            if (setCMCShooterLeftWeapon != 0)
            {
                GameObject weapon = FindObjectsOfType<NetworkIdentity>().ToList().Find(x => x.netId == setCMCShooterLeftWeapon).gameObject;
                if (weapon.GetComponent<MP_vShooterWeapon>())
                {
                    vCollectableStandalone collectable = (vCollectableStandalone)weapon.FindComponent(typeof(vCollectableStandalone));
                    if (collectable != null)
                        csmc.ClientEquipShooterWeapon(collectable);
                }
            }
            if (setCMCShooterRightWeapon != 0)
            {
                GameObject weapon = FindObjectsOfType<NetworkIdentity>().ToList().Find(x => x.netId == setCMCShooterRightWeapon).gameObject;
                if (weapon.GetComponent<MP_vShooterWeapon>())
                {
                    vCollectableStandalone collectable = (vCollectableStandalone)weapon.FindComponent(typeof(vCollectableStandalone));
                    if (collectable != null)
                        csmc.ClientEquipShooterWeapon(collectable);
                }
            }
        }
        protected virtual IEnumerator InitClientCameras()
        {
            yield return new WaitUntil(() => client != null);
            clientScopeCamera = new GameObject($"Client Scope Camera [connectionId={client.connId}]");
            clientZoomScopeCamera = new GameObject($"Client Zoom Camera [connectionId={client.connId}]");
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        /// <summary>
        ///  Need this for shooter weapons to be able to have their raycasts register on other players.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator InitChildColliders()
        {
            yield return new WaitForSeconds(0.01f);
            foreach(Collider col in GetComponentsInChildren<Collider>(true))
            {
                col.enabled = true;
            }
        }
        protected override void OnDestroy()
        {
            if (clientScopeCamera != null) Destroy(clientScopeCamera);
            if (clientZoomScopeCamera != null) Destroy(clientZoomScopeCamera);
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
            base.OnDestroy();
        }
        #endregion

        #region SceneManagement
        protected virtual void OnActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            if (clientScopeCamera) SceneManager.MoveGameObjectToScene(clientScopeCamera, newScene);
            if (clientZoomScopeCamera) SceneManager.MoveGameObjectToScene(clientZoomScopeCamera, newScene);
        }
        #endregion

        #region Callable Methods
        [Client]
        public virtual void UpdateScopeCamera(Transform scopeCamera)
        {
            if (NetworkClient.active && hasAuthority)
            {
                Cmd_SetScopeCameraValues(scopeCamera.position, scopeCamera.rotation);
            }
        }
        [Client]
        public virtual void UpdateZoomCamera(Transform zoomCamera)
        {
            if (NetworkClient.active && hasAuthority)
            {
                Cmd_SetScopeCameraValues(zoomCamera.position, zoomCamera.rotation);
            }
        }
        #endregion

        #region Callbacks
        #region vShooterMeleeInput
        public virtual void DisableScopeCamera()
        {
            if (NetworkServer.active) return;
            if (NetworkClient.active && hasAuthority)
                Cmd_SetInScopeView(1);
        }
        public virtual void EnableScopeCamera()
        {
            if (NetworkServer.active) return;
            if (NetworkClient.active && hasAuthority)
                Cmd_SetInScopeView(0);
        }
        #endregion

        #region vItemManager
        /// <summary>
        /// This will sync the slot, thanks to the vMeleeNetworkCalls logic, but will also sync the equipped weapon in the MP_vShooterManager.
        /// </summary>
        /// <param name="op"></param>
        /// <param name="itemIndex"></param>
        /// <param name="oldItem"></param>
        /// <param name="newItem"></param>
        //protected override void InventoryEquippedItemsChanged(SyncList<InventoryEquippedItem>.Operation op, int itemIndex, InventoryEquippedItem oldItem, InventoryEquippedItem newItem)
        //{
        //    // Verify slots & Equipped Melee Weapons
        //    base.InventoryEquippedItemsChanged(op, itemIndex, oldItem, newItem);

        //    // Verify Equipped Shooter Weapons
        //    StartCoroutine(VerifyShooterEquippedWeapon(op, itemIndex, oldItem, newItem));
        //}
        //protected virtual IEnumerator VerifyShooterEquippedWeapon(SyncList<InventoryEquippedItem>.Operation op, int itemIndex, InventoryEquippedItem oldItem, InventoryEquippedItem newItem)
        //{
        //    // Verify correct shooter weapon equipped
        //    if (op == SyncList<InventoryEquippedItem>.Operation.OP_ADD ||
        //        op == SyncList<InventoryEquippedItem>.Operation.OP_INSERT ||
        //        op == SyncList<InventoryEquippedItem>.Operation.OP_SET) // equipping item
        //    {
        //        yield return new WaitForSeconds(0.001f);
        //        Transform target = transform.GetChildFromTree(newItem.area);
        //        vEquipArea equipArea = target.GetComponent<vEquipArea>();
        //        int indexOfArea = inv.changeEquipmentControllers.FindIndex(x => x.equipArea == target.GetComponent<vEquipArea>());
        //        vItem item = im.items.Find(x => x.id == newItem.itemId);
        //        if (item == null) // add item that is missing
        //        {
        //            ItemReference ir = new ItemReference(newItem.itemId);
        //            ir.indexArea = indexOfArea;
        //            im.AddItem(ir);
        //        }
        //        if (item.originalObject)
        //        {
        //            vShooterWeapon sw = item.originalObject.GetComponent<vShooterWeapon>();
        //            if (sw)
        //            {
        //                // Checking equipped shooter weapons
        //                if (sw.isLeftWeapon)
        //                {
        //                    if (sm.lWeapon == null || sm.lWeapon.gameObject.GetComponent<vMeleeEquipment>().referenceItem == null || sm.lWeapon.gameObject.GetComponent<vMeleeEquipment>().referenceItem.id != newItem.itemId)
        //                    {
        //                        if (!outOfSyncIDs.Contains(newItem.itemId)) //if for some reason a ton of the same requests come in it prevents the client from equipping the same weapon over and over.
        //                        {
        //                            outOfSyncIDs.Add(newItem.itemId);
        //                            // Out of sync, resync
        //                            im.EquipItemToEquipSlot(indexOfArea, newItem.slotIndex, im.items.Find(x => x.id == newItem.itemId), true, true);
        //                            StartCoroutine(FinishShooterSync(newItem.itemId));
        //                        }
        //                    }
        //                }
        //                else if (sm.rWeapon == null || sm.rWeapon.gameObject.GetComponent<vMeleeEquipment>().referenceItem == null || sm.rWeapon.gameObject.GetComponent<vMeleeEquipment>().referenceItem.id != newItem.itemId)
        //                {
        //                    if (!outOfSyncIDs.Contains(newItem.itemId)) //if for some reason a ton of the same requests come in it prevents the client from equipping the same weapon over and over.
        //                    {
        //                        outOfSyncIDs.Add(newItem.itemId);
        //                        // Out of sync, resync
        //                        im.EquipItemToEquipSlot(indexOfArea, newItem.slotIndex, im.items.Find(x => x.id == newItem.itemId), true, true);
        //                        StartCoroutine(FinishShooterSync(newItem.itemId));
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
        //protected virtual IEnumerator FinishShooterSync(int id)
        //{
        //    yield return new WaitForSeconds(0.2f); // rough guess on how long the animation is to equip the weapon
        //    outOfSyncIDs.Remove(id);
        //}
        #endregion
        #endregion

        #region Hooks
        #region vShooterManager
        protected virtual void Hook_OnRightWeaponAmmoChanged(int oldValue, int newValue)
        {
            if (NetworkServer.active) return;
            if (!sm) return;
            if (sm.rWeapon)
            {
                vShooterEquipment equipment = sm.rWeapon.gameObject.GetComponent<vShooterEquipment>();
                if (equipment)
                {
                    var ammoAttribute = equipment.referenceItem.GetItemAttribute(vItemAttributes.AmmoCount);
                    if (ammoAttribute != null && ammoAttribute.value != newValue)
                        ammoAttribute.value = (hasAuthority) ? newValue -1 : newValue; // The owning client calculates the shot AFTER, the non owner calculates the shot BEFORE
                }
                if (sm.rWeapon.ammo != newValue)
                {
                    sm.rWeapon.ammo = newValue;
                    sm.ForceDisplayUpdate(1);
                }
            }
        }
        protected virtual void Hook_OnLeftWeaponAmmoChanged(int oldValue, int newValue)
        {
            if (NetworkServer.active) return;
            if (!sm) return;
            if (sm.lWeapon)
            {
                vShooterEquipment equipment = sm.lWeapon.gameObject.GetComponent<vShooterEquipment>();
                if (equipment)
                {
                    vItemAttribute ammoAttribute = equipment.referenceItem.GetItemAttribute(vItemAttributes.AmmoCount);
                    if (ammoAttribute != null && ammoAttribute.value != newValue)
                        ammoAttribute.value = (hasAuthority) ? newValue - 1 : newValue; // The owning client calculates the shot AFTER, the non owner calculates the shot BEFORE
                }
                if (sm.lWeapon.ammo != newValue)
                {
                    sm.lWeapon.ammo = newValue;
                    sm.ForceDisplayUpdate(-1);
                }
            }
        }
        #endregion

        #region vShooterMeleeInput
        protected virtual void Hook_SetScopeCameraPosition(Vector3 oldPos, Vector3 newPos)
        {
            if (NetworkServer.active) return;
            if (NetworkClient.active && hasAuthority) return;
            if (clientScopeCamera) clientScopeCamera.transform.position = newPos;
        }
        protected virtual void Hook_SetScopeCameraRotation(Quaternion oldRot, Quaternion newRot)
        {
            if (NetworkServer.active) return;
            if (NetworkClient.active && hasAuthority) return;
            if (clientScopeCamera) clientScopeCamera.transform.rotation = newRot;
        }
        protected virtual void Hook_SetZoomCameraPosition(Vector3 oldPos, Vector3 newPos)
        {
            if (NetworkServer.active) return;
            if (NetworkClient.active && hasAuthority) return;
            if (clientZoomScopeCamera) clientZoomScopeCamera.transform.position = newPos;
        }
        protected virtual void Hook_SetZoomCameraRotation(Quaternion oldRot, Quaternion newRot)
        {
            if (NetworkServer.active) return;
            if (NetworkClient.active && hasAuthority) return;
            if (clientZoomScopeCamera) clientZoomScopeCamera.transform.rotation = newRot;
        }
        #endregion

        #region vShooterManager
        protected virtual void Hook_SetCMCShooterLeftWeapon(uint oldWeapon, uint newWeapon)
        {
            if (NetworkServer.active || !NetworkClient.active) return;
            try
            {
                if (newWeapon == 0)
                {
                    //sm.SetLeftWeapon((GameObject)null);
                    StartCoroutine(VerifyShooterWeapon((GameObject)null, false));
                }
                else
                {
                    GameObject weapon = FindObjectsOfType<NetworkIdentity>().ToList().Find(x => x.netId == newWeapon).gameObject;
                    StartCoroutine(VerifyShooterWeapon(weapon, false));
                    //if (sm.lWeapon != weapon)
                    //    sm.SetLeftWeapon(weapon);
                }
            }
            catch { }
        }
        protected virtual void Hook_SetCMCShooterRightWeapon(uint oldWeapon, uint newWeapon)
        {
            if (NetworkServer.active) return;
            if (!NetworkClient.active) return;
            try
            {
                if (newWeapon == 0)
                {
                    //sm.SetRightWeapon((GameObject)null);
                    StartCoroutine(VerifyShooterWeapon((GameObject)null, true));
                }
                else
                {
                    GameObject weapon = FindObjectsOfType<NetworkIdentity>().ToList().Find(x => x.netId == newWeapon).gameObject;
                    StartCoroutine(VerifyShooterWeapon(weapon, true));
                }
            }
            catch { }
        }
        protected virtual IEnumerator VerifyShooterWeapon(GameObject weapon, bool isRight)
        {
            yield return new WaitForSeconds(0.01f);
            if (isRight && sm.rWeapon != weapon)
            {
                sm.SetRightWeapon(weapon);
            }
            else if (!isRight && sm.lWeapon != weapon)
            {
                sm.SetLeftWeapon(weapon);
            }
        }
        #endregion

        #region vThrowManager
        protected virtual void Hook_UpdateCurrentThrow(int oldValue, int newValue)
        {
            if (tm && oldValue < newValue) // threw an object
            {
                if (tm.WaitingToThrow()) // still in aim
                {
                    tm.StartThrow(); // force a throw to get back in sync with the server
                }
            }
            Hook_UpdateThrowUI(oldValue, newValue);
        }
        protected virtual void Hook_UpdateThrowUI(int oldValue, int newValue)
        {
            if (tm) tm.ui.UpdateCount(tm); // update the ui of the owning player when the server values change.
        }
        #endregion

        #region vLockOnShooter
        public override void SetLockOnStatus(bool oldValue, bool newValue)
        {
            base.SetLockOnStatus(oldValue, newValue);
            if (GetComponent<MP_vLockOnShooter>())
            {
                if (GetComponent<MP_vLockOnShooter>().IsLockedOn() != newValue)
                {
                    GetComponent<MP_vLockOnShooter>().SetLockOn(newValue);
                }
            }
        }
        #endregion
        #endregion

        #region RPCs
        #region Command
        #region vShooterMeleeInput
        [Command]
        public override void Cmd_LockAllInput(bool value)
        {
            #if UNITY_SERVER || UNITY_EDITOR
            base.Cmd_LockAllInput(value);
            if (GetComponent<MP_vShooterMeleeInput>())
            {
                GetComponent<MP_vShooterMeleeInput>().SetLockAllInput(value);
            }
            #endif
        }
        [Command]
        public virtual void Cmd_SetIsUsingScopeView(byte value)
        {
            #if UNITY_SERVER || UNITY_EDITOR
            isUsingScopeView = Convert.ToBoolean(value);
            #endif
        }
        [Command]
        protected virtual void Cmd_SetScopeCameraValues(Vector3 pos, Quaternion rot)
        {
            #if UNITY_SERVER || UNITY_EDITOR
            scopeCameraPosition = pos;
            scopeCameraRotation = rot;
            #endif
        }
        [Command]
        protected virtual void Cmd_SetZoomCameraValues(Vector3 pos, Quaternion rot)
        {
            #if UNITY_SERVER || UNITY_EDITOR
            zoomCameraPosition = pos;
            zoomCameraRotation = rot;
            #endif
        }
        [Command]
        protected virtual void Cmd_SetFarClipPlane(float value)
        {
            #if UNITY_SERVER || UNITY_EDITOR
            clientCamFarClipPlane = value;
            #endif
        }
        [Command]
        protected virtual void Cmd_SetInScopeView(byte value)
        {
            #if UNITY_SERVER || UNITY_EDITOR
            isScopeCameraActive = Convert.ToBoolean(value);
            #endif
        }
        #endregion
        #endregion

        #region ClientRpc
        #region vShooterMeleeInput
        [ClientRpc(includeOwner = false)]
        public override void Rpc_LockAllInput(bool value)
        {
            #if !UNITY_SERVER || UNITY_EDITOR
            base.Rpc_LockAllInput(value);
            if (GetComponent<MP_vShooterMeleeInput>())
            {
                GetComponent<MP_vShooterMeleeInput>().SetLockAllInput(value);
            }
            #endif
        }
        #endregion

        #region vCollectShooterMeleeControl
        [ClientRpc]
        public override void Rpc_UnequipLeftWeapon(uint netId)
        {
            #if !UNITY_SERVER || UNITY_EDITOR
            if (((NetworkIdentity)csmc.leftWeapon.gameObject.FindComponent(typeof(NetworkIdentity))).netId == netId)
                csmc.ClientRemoveLeftWeapon();
            #endif
        }
        [ClientRpc]
        public override void Rpc_UnequipRightWeapon(uint netId)
        {
            #if !UNITY_SERVER || UNITY_EDITOR
            if (((NetworkIdentity)csmc.rightWeapon.gameObject.FindComponent(typeof(NetworkIdentity))).netId == netId)
                csmc.ClientRemoveRightWeapon();
            #endif
        }
        [ClientRpc]
        public virtual void Rpc_EquipShooterWeapon(uint netId)
        {
            #if !UNITY_SERVER || UNITY_EDITOR
            NetworkIdentity ni = FindObjectsOfType<NetworkIdentity>().ToList().Find(x => x.netId == netId);
            if (ni)
            {
                csmc.ClientEquipShooterWeapon((vCollectableStandalone)ni.gameObject.FindComponent(typeof(vCollectableStandalone)));
            }
            #endif
        }
        [ClientRpc]
        public override void Rpc_EquipMeleeWeapon(uint netId)
        {
            #if !UNITY_SERVER || UNITY_EDITOR
            NetworkIdentity ni = FindObjectsOfType<NetworkIdentity>().ToList().Find(x => x.netId == netId);
            if (ni)
            {
                csmc.ClientEquipMeleeWeapon((vCollectableStandalone)ni.gameObject.FindComponent(typeof(vCollectableStandalone)));
            }
            #endif
        }
        #endregion
        #endregion
        #endregion
    }
}
