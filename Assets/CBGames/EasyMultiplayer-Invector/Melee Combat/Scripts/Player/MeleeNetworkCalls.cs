using EMI.Utils;
using Invector.vCharacterController;
using Invector.vCharacterController.vActions;
using Invector.vItemManager;
using Invector.vMelee;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EMI.Player
{
    public class MeleeNetworkCalls : BasicNetworkCalls
    {
        #region Properties

        #region vMeleeManager
        protected MP_vMeleeManager mm = null;
        [HideInInspector, SyncVar(hook = nameof(Hook_SetCMCLeftWeapon))] public uint setleftWeapon = 0;
        [HideInInspector, SyncVar(hook = nameof(Hook_SetCMCRightWeapon))] public uint setrightWeapon = 0;
        #endregion

        #region vCollectMeleeControl
        protected MP_vCollectMeleeControl cmc = null;
        #endregion

        #region Inventory/ItemManager
        public struct EquipSlotItem
        {
            public int slotIndex;
            public int itemId;
        }
        public struct EquipAreaItem
        {
            public int equipAreaIndex;
            public int slotIndex;
            public int itemId;
            public int amount;
        }
        
        protected MP_vItemManager im = null;            // Caching reference so I don't have to find it every time
        protected bool initilalizedInv = false;         // Prevents errors occuring at start with initilizating the inventory
        protected SyncList<EquipAreaItem> equipSlotItems = new SyncList<EquipAreaItem>();
        protected SyncDictionary<int, EquipSlotItem> activeSlots = new SyncDictionary<int, EquipSlotItem>();
        protected MP_vInventory inv = null;
        protected int sync_wait = 0;
        #endregion

        #region vLockOn
        [HideInInspector, SyncVar(hook = nameof(SetLockOnStatus))] public bool lockedOn = false;
        #endregion

        #endregion

        #region Initialization
        protected override void Awake()
        {
            cmc = GetComponent<MP_vCollectMeleeControl>();
            mm = GetComponent<MP_vMeleeManager>();
            im = GetComponent<MP_vItemManager>();
            inv = (MP_vInventory)gameObject.FindComponent(typeof(MP_vInventory));

            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
            
            if (im)
            {
                equipSlotItems.Callback += EquipSlotsUpdated;
                activeSlots.Callback += ActiveSlotUpdated;
                StartCoroutine(InitInventory());
            }
        }

        protected virtual IEnumerator InitInventory()
        {
            yield return new WaitForSeconds(0.1f);
            if (NetworkServer.active)
            {
                UpdateActiveSlot();
                foreach (vEquipArea area in inv.equipAreas)
                {
                    UpdateEquipSlotItems(area);
                }
            }
            
            RegisterInventoryEvents();

            //NOTE: Ordering is important. Active slot selecting first, followed by slot filling
            // Set active slots for this character based on what the server
            // is currently set to
            foreach (KeyValuePair<int, EquipSlotItem> slot in activeSlots)
            {
                ActiveSlotUpdated(SyncIDictionary<int, EquipSlotItem>.Operation.OP_SET, slot.Key, slot.Value);
            }

            // Set items in equip slots according to what is currently on the 
            // server based character
            for (int i = 0; i < equipSlotItems.Count; i++)
            {
                EquipSlotsUpdated(SyncList<EquipAreaItem>.Operation.OP_SET, i, equipSlotItems[i], equipSlotItems[i]);
            }
        }
        #endregion

        #region No Inventory
        /// <summary>
        /// This will automatically syncronize your state with the server regarding equipped weapons.
        /// </summary>
        protected virtual IEnumerator InitEquippedWeapons()
        {
            yield return new WaitForSeconds(0.1f); // allow time for handlers to initalize

            // Check for already equipped melee weapons
            if (setleftWeapon != 0)
            {
                GameObject weapon = FindObjectsOfType<NetworkIdentity>().ToList().Find(x => x.netId == setleftWeapon).gameObject;
                vCollectableStandalone collectable = (vCollectableStandalone)weapon.FindComponent(typeof(vCollectableStandalone));
                if (collectable != null)
                    cmc.ClientEquipMeleeWeapon(collectable);
            }
            if (setrightWeapon != 0)
            {
                GameObject weapon = FindObjectsOfType<NetworkIdentity>().ToList().Find(x => x.netId == setrightWeapon).gameObject;
                vCollectableStandalone collectable = (vCollectableStandalone)weapon.FindComponent(typeof(vCollectableStandalone));
                if (collectable != null)
                    cmc.ClientEquipMeleeWeapon(collectable);
            }
        }
        #endregion

        #region Inventory
        /// <summary>
        /// This is used by the vEquipSlot to make sure it doesn't make a call for something that
        /// has already happened on the server. (Prevents cyclical calls)
        /// </summary>
        public int GetServerItem(int equipArea, int slotIndex)
        {
            int foundIndex = equipSlotItems.FindIndex(x => x.equipAreaIndex == equipArea && x.slotIndex == slotIndex);
            return (foundIndex < 0) ? -1 : equipSlotItems[foundIndex].itemId;
        }
        [Server]
        protected virtual void UpdateActiveSlot()
        {
            foreach(vEquipArea area in inv.equipAreas)
            {
                int equipAreaIndex = inv.equipAreas.ToList().FindIndex(x => x.Equals(area));
                int activeSlotIndex = area.equipSlots.FindIndex(x => x.Equals(area.currentEquippedSlot));
                if (activeSlotIndex == -1)
                    activeSlotIndex = area.equipSlots.FindIndex(x => x.Equals(area.currentSelectedSlot));
                int itemId = (area.equipSlots[activeSlotIndex].item == null) ? -1 : area.equipSlots[activeSlotIndex].item.id;
                
                if (!activeSlots.ContainsKey(equipAreaIndex))
                {
                    activeSlots.Add(equipAreaIndex, new EquipSlotItem()
                    {
                        slotIndex = activeSlotIndex,
                        itemId = itemId
                    });
                }
                else if (activeSlots[equipAreaIndex].itemId != itemId || activeSlots[equipAreaIndex].slotIndex != activeSlotIndex)
                {
                    activeSlots[equipAreaIndex] = new EquipSlotItem()
                    {
                        slotIndex = activeSlotIndex,
                        itemId = itemId
                    };
                }
            }
        }
        [Server]
        protected virtual void UpdateEquipSlotItems(vEquipArea area)
        {
            int areaIndex = inv.equipAreas.ToList().FindIndex(x => x.Equals(area));
            for (int i = 0; i < area.equipSlots.Count; i++)
            {
                int foundIndex = equipSlotItems.FindIndex(x => x.equipAreaIndex == areaIndex && x.slotIndex == i);
                vItem equipSlotItem = area.equipSlots[i].item;
                int slotItemId = (equipSlotItem == null) ? -1 : equipSlotItem.id;
                int slotItemAmount = (equipSlotItem == null) ? -1 : equipSlotItem.amount;

                if (foundIndex < 0)
                {
                    equipSlotItems.Add(new EquipAreaItem()
                    {
                        equipAreaIndex = areaIndex,
                        slotIndex = i,
                        itemId = slotItemId,
                        amount = slotItemAmount
                    });
                }
                else if (equipSlotItems[foundIndex].itemId != slotItemId || equipSlotItems[foundIndex].amount != slotItemAmount) 
                {
                    equipSlotItems[foundIndex] = new EquipAreaItem()
                    {
                        equipAreaIndex = equipSlotItems[foundIndex].equipAreaIndex,
                        slotIndex = equipSlotItems[foundIndex].slotIndex,
                        itemId = slotItemId,
                        amount = slotItemAmount
                    };
                }
            }
        }
        protected virtual void RegisterInventoryEvents()
        {
            if (inv.equipAreas.Length < 1) // Solves the invector bug with trying to equip items before this has been initiliazed
                inv.equipAreas = GetComponentsInChildren<MP_vEquipArea>(true);

            #if UNITY_SERVER || UNITY_EDITOR
            if (NetworkServer.active)
            {
                // Adding items to inventory list
                im.onAddItemID.AddListener((id) =>
                {
                    Rpc_AddToInventory(id, im.items.Find(x => x.id == id).amount);
                });

                // Removing items from inventory list
                im.onRemoveItemID.AddListener((id) =>
                {
                    vItem target = im.items.Find(x => x.id == id);
                    int total = (target == null) ? 0 : target.amount;
                    Rpc_RemoveFromInventory(id, total);
                });
                im.onDestroyItem.AddListener((item, amount) =>
                {
                    Rpc_RemoveFromInventory(item.id, item.amount);
                });
                im.onChangeItemAmount.AddListener((item) =>
                {
                    Rpc_ChangeAmount(item.id, item.amount);
                });

                // Equipping Items
                im.onEquipItem.AddListener((equipArea, item) =>
                {
                    UpdateActiveSlot();
                    UpdateEquipSlotItems(equipArea);
                });

                // Unequipping Items
                im.onUnequipItem.AddListener((equipArea, item) =>
                {
                    UpdateEquipSlotItems(equipArea);
                    UpdateActiveSlot();
                });
            }
            #endif

            if (!NetworkServer.active)
            {
                im.onFinishEquipItem.AddListener((equipArea, item) =>
                {
                    StartCoroutine(CheckServerAfterSync(equipArea));
                });
            }

            // Loop through the vEquipArea's of the inventory
            for (int i = 0; i < inv.equipAreas.Length; i++)
            {
                int equipAreaIndex = i;

                // Loop through each vEquipArea's available slots
                for (int j = 0; j < inv.equipAreas[equipAreaIndex].equipSlots.Count; j++)
                {
                    int slotIndex = j;

                    if (NetworkServer.active || hasAuthority)
                    {
                        // Item Slot Clicked
                        inv.equipAreas[equipAreaIndex].equipSlots[slotIndex].onClick.AddListener(() =>
                        {
                            if (NetworkServer.active)
                                UpdateActiveSlot();
                            else if (hasAuthority)
                                Cmd_OnSlotClicked(equipAreaIndex, slotIndex);
                        });
                    }

                    if (NetworkServer.active)
                    {
                        // Item Removed From Slot
                        inv.equipAreas[equipAreaIndex].equipSlots[slotIndex].onRemoveItem.AddListener((item) =>
                        {
                            UpdateActiveSlot();
                            UpdateEquipSlotItems(inv.equipAreas[equipAreaIndex]);
                        });

                        // Item Added To Slot
                        inv.equipAreas[equipAreaIndex].equipSlots[slotIndex].onAddItem.AddListener((item) =>
                        {
                            UpdateActiveSlot();
                            UpdateEquipSlotItems(inv.equipAreas[equipAreaIndex]);
                        });
                    }
                }
            }
        }
        protected virtual IEnumerator CheckServerAfterSync(vEquipArea equipArea)
        {
            sync_wait++;
            int fired_sync = 0;
            fired_sync = sync_wait;
            yield return new WaitUntil(() => fired_sync > sync_wait); // Have to wait for the next sync because technically we're ahead of the server
            int areaIndex = inv.equipAreas.ToList().FindIndex(x => x.Equals(equipArea));
            int slotIndex = inv.equipAreas[areaIndex].equipSlots.FindIndex(x => x.Equals(equipArea.currentEquippedSlot));
            if (slotIndex != activeSlots[areaIndex].slotIndex)
            {
                inv.equipAreas[areaIndex].SetEquipSlot(activeSlots[areaIndex].slotIndex);
            }
        }
        #endregion

        #region Hooks

        #region vMeleeManager
        #region CMC
        // No Inventory based weapons
        protected virtual void Hook_SetCMCLeftWeapon(uint oldWeapon, uint newWeapon)
        {
            if (NetworkServer.active || !NetworkClient.active) return;
            try
            {
                if (newWeapon == 0)
                {
                    StartCoroutine(VerifyMeleeWeapon((GameObject)null, false));
                }
                else
                {
                    GameObject weapon = FindObjectsOfType<NetworkIdentity>().ToList().Find(x => x.netId == newWeapon).gameObject;
                    StartCoroutine(VerifyMeleeWeapon(weapon, false));
                }
            }
            catch { }
        }
        protected virtual void Hook_SetCMCRightWeapon(uint oldWeapon, uint newWeapon)
        {
            if (NetworkServer.active || !NetworkClient.active) return;
            try
            {
                if (newWeapon == 0)
                {
                    StartCoroutine(VerifyMeleeWeapon((GameObject)null, true));
                }
                else
                {
                    GameObject weapon = FindObjectsOfType<NetworkIdentity>().ToList().Find(x => x.netId == newWeapon).gameObject;
                    StartCoroutine(VerifyMeleeWeapon(weapon, true));
                }
            }
            catch { }
        }
        #endregion

        protected virtual IEnumerator VerifyMeleeWeapon(GameObject weapon, bool isRight)
        {
            yield return new WaitForSeconds(0.01f);
            if (isRight && !GameObject.ReferenceEquals((mm.rightWeapon == null) ? null : mm.rightWeapon.gameObject, weapon))
            {
                mm.SetRightWeapon(weapon);
            }
            else if (!isRight && !GameObject.ReferenceEquals((mm.leftWeapon == null) ? null : mm.leftWeapon.gameObject, weapon))
            {
                mm.SetLeftWeapon(weapon);
            }
        }
        #endregion

        #region vLockOn
        public virtual void SetLockOnStatus(bool oldValue, bool newValue)
        {
            if (NetworkServer.active) return;
            if (GetComponent<MP_vLockOn>())
            {
                if (GetComponent<MP_vLockOn>().IsLockedOn() != newValue)
                {
                    GetComponent<MP_vLockOn>().SetLockOn(newValue);
                }
            }
        }
        #endregion

        #endregion

        #region Callbacks

        #region vItemManager
        protected virtual void OnItemSlotSubmitted(int equipAreaIndex, int slotIndex)
        {
            if (hasAuthority) return;
            inv.equipAreas[equipAreaIndex].OnSubmitSlot(inv.equipAreas[equipAreaIndex].equipSlots[slotIndex]);
        }
        protected virtual void OnRemoveFromSlot(int itemId, int equipAreaIndex, int slotIndex)
        {
            if (inv.equipAreas[equipAreaIndex].equipSlots[slotIndex].item != null &&
                inv.equipAreas[equipAreaIndex].equipSlots[slotIndex].item.id == itemId)
            {
                inv.equipAreas[equipAreaIndex].equipSlots[slotIndex].RemoveItem();
            }
        }
        
        protected virtual void ActiveSlotUpdated(SyncIDictionary<int, EquipSlotItem>.Operation op, int equipAreaIndex, EquipSlotItem item)
        {
            if (NetworkServer.active) return;

            switch(op)
            {
                case SyncIDictionary<int, EquipSlotItem>.Operation.OP_ADD:
                case SyncIDictionary<int, EquipSlotItem>.Operation.OP_SET:
                    sync_wait = (sync_wait - 1 > -1) ? sync_wait - 1 : 0;
                    int activeSlotIndex = -1;
                    if (inv.equipAreas[equipAreaIndex].currentEquippedSlot != null)
                        activeSlotIndex = inv.equipAreas[equipAreaIndex].equipSlots.FindIndex(x => x.Equals(inv.equipAreas[equipAreaIndex].currentEquippedSlot));
                    if (activeSlotIndex != item.slotIndex)
                    {
                        inv.equipAreas[equipAreaIndex].SetEquipSlot(item.slotIndex);
                    }
                    break;
            }
        }
        protected virtual void EquipSlotsUpdated(SyncList<EquipAreaItem>.Operation op, int itemIndex, EquipAreaItem oldItem, EquipAreaItem newItem)
        {
            if (NetworkServer.active) return;
            switch (op)
            {
                case SyncList<EquipAreaItem>.Operation.OP_ADD:
                case SyncList<EquipAreaItem>.Operation.OP_INSERT:
                case SyncList<EquipAreaItem>.Operation.OP_SET:
                    vItem slotItem = inv.equipAreas[newItem.equipAreaIndex].equipSlots[newItem.slotIndex].item;
                    int slotItemId = (slotItem == null) ? -1 : slotItem.id;
                    if (slotItemId != newItem.itemId)
                    {
                        if (newItem.itemId == -1)
                        {
                            if (inv.equipAreas[newItem.equipAreaIndex].currentEquippedSlot.Equals(inv.equipAreas[newItem.equipAreaIndex].equipSlots[newItem.slotIndex]))
                            {
                                im.UnequipItemOfEquipSlot(newItem.equipAreaIndex, newItem.slotIndex, false);
                            }
                            else
                            {
                                inv.equipAreas[newItem.equipAreaIndex].equipSlots[newItem.slotIndex].RemoveItem();
                            }
                        }
                        else
                        {
                            vItem foundItem = im.items.Find(x => x.id == newItem.itemId);
                            if (foundItem == null)
                            {
                                ItemReference addItem = new ItemReference(newItem.itemId);
                                addItem.autoEquip = false;
                                addItem.addToEquipArea = false;
                                addItem.amount = newItem.amount;
                                im.AddItem(addItem);
                                foundItem = im.items.Find(x => x.id == newItem.itemId);
                            }
                            if (inv.equipAreas[newItem.equipAreaIndex].currentEquippedSlot.Equals(inv.equipAreas[newItem.equipAreaIndex].equipSlots[newItem.slotIndex]))
                            {
                                im.EquipItemToEquipSlot(newItem.equipAreaIndex, newItem.slotIndex, foundItem, true, false);
                            }
                            else
                            {
                                inv.equipAreas[newItem.equipAreaIndex].equipSlots[newItem.slotIndex].AddItem(foundItem);
                            }
                        }
                        //if (activeSlots.Count > newItem.equipAreaIndex)
                        //    ActiveSlotUpdated(SyncIDictionary<int, EquipSlotItem>.Operation.OP_SET, newItem.equipAreaIndex, activeSlots[newItem.equipAreaIndex]);
                    }
                    break;
            }
        }
        #endregion
        
        #endregion

        #region RPC
        #region Commands
        #region vItemManager
        [Command]
        public virtual void Cmd_InventoryLock(bool value)
        {
            #if UNITY_SERVER || UNITY_EDITOR
            if (value == true)
            {
                tpi.SetLockCameraInput(true);
                tpi.SetLockAllInput(true);
                tpController.input = Vector3.zero;
                tpController.Sprint(false);
                tpController.animator.SetFloat("InputHorizontal", 0.0f, 0.25f, Time.deltaTime);
                tpController.animator.SetFloat("InputVertical", 0.0f, 0.25f, Time.deltaTime);
                tpController.animator.SetFloat("InputMagnitude", 0.0f, 0.25f, Time.deltaTime);
            }
            else
            {
                tpi.SetLockCameraInput(false);
                tpi.SetLockAllInput(false);
            }
            Rpc_InventoryLock(value);
            #endif
        }
        [Command]
        public virtual void Cmd_RequestUseItem(int itemId)
        {
            #if UNITY_SERVER || UNITY_EDITOR
            vItem item = im.items.Find(x => x.id == itemId);
            if (item != null)
                im.Server_UseItem(item);
            #endif
        }
        [Command]
        public virtual void Cmd_DestroyItem(int itemId, int amount)
        {
            #if UNITY_SERVER || UNITY_EDITOR
            vItem item = im.items.Find(x => x.id == itemId);
            if (item != null)
                im.DestroyItem(item, amount);
            #endif
        }
        [Command]
        public virtual void Cmd_DropItem(int itemId, int amount)
        {
            #if UNITY_SERVER || UNITY_EDITOR
            vItem item = im.items.Find(x => x.id == itemId);
            if (item != null)
                im.DropItem(item, amount);
            #endif
        }
        [Command]
        public virtual void Cmd_OnRemovedFromSlot(int equipAreaIndex, int slotIndex)
        {
            #if UNITY_SERVER || UNITY_EDITOR
            if (inv.equipAreas[equipAreaIndex].equipSlots[slotIndex].item != null)
            {
                if (inv.equipAreas[equipAreaIndex].currentEquippedSlot.Equals(inv.equipAreas[equipAreaIndex].equipSlots[slotIndex]))
                    im.UnequipItemOfEquipSlot(equipAreaIndex, slotIndex, true);
                else
                    inv.equipAreas[equipAreaIndex].equipSlots[slotIndex].RemoveItem();
            }
            #endif
        }
        [Command]
        public virtual void Cmd_OnAddedToSlot(int itemId, int equipAreaIndex, int slotIndex)
        {
            #if UNITY_SERVER || UNITY_EDITOR
            vItem item = im.items.Find(x => x.id == itemId);
            if (item == null && inv.equipAreas[equipAreaIndex].equipSlots[slotIndex].item != null)
            {
                inv.equipAreas[equipAreaIndex].equipSlots[slotIndex].RemoveItem();
            }
            else if (inv.equipAreas[equipAreaIndex].equipSlots[slotIndex].item == null || 
                inv.equipAreas[equipAreaIndex].equipSlots[slotIndex].item.id != itemId)
            {
                if (inv.equipAreas[equipAreaIndex].currentEquippedSlot.Equals(inv.equipAreas[equipAreaIndex].equipSlots[slotIndex]))
                    im.EquipItemToEquipSlot(equipAreaIndex, slotIndex, item, true, false);
                else
                    inv.equipAreas[equipAreaIndex].equipSlots[slotIndex].AddItem(item);
            }
            #endif
        }
        /// <summary>
        /// Called on the server when the owning client clicks an equip slot, make the server fake click that slot as well.
        /// </summary>
        [Command]
        protected virtual void Cmd_OnSlotClicked(int equipAreaIndex, int slotIndex)
        {
            #if UNITY_SERVER || UNITY_EDITOR
            OnItemSlotSubmitted(equipAreaIndex, slotIndex);
            #endif
        }
        #endregion

        #region MeleeCombatInput
        [Command]
        public virtual void Cmd_LockAllInput(bool value)
        {
            #if UNITY_SERVER || UNITY_EDITOR
            if (GetComponent<MP_vMeleeCombatInput>())
            {
                GetComponent<MP_vMeleeCombatInput>().SetLockAllInput(value);
            }
            Rpc_LockAllInput(value);
            #endif
        }
        #endregion

        #endregion

        #region ClientRpc

        #region vShooterMeleeInput
        [ClientRpc(includeOwner = false)]
        public virtual void Rpc_LockAllInput(bool value)
        {
            #if !UNITY_SERVER || UNITY_EDITOR
            if (GetComponent<MP_vMeleeCombatInput>())
            {
                GetComponent<MP_vMeleeCombatInput>().SetLockAllInput(value);
            }
            #endif
        }
        #endregion

        #region vCollectMeleeControl
        [ClientRpc]
        public virtual void Rpc_UnequipLeftWeapon(uint netId)
        {
            #if !UNITY_SERVER || UNITY_EDITOR
            if (((NetworkIdentity)cmc.leftWeapon.gameObject.FindComponent(typeof(NetworkIdentity))).netId == netId)
                cmc.ClientRemoveLeftWeapon();
            #endif
        }
        [ClientRpc]
        public virtual void Rpc_UnequipRightWeapon(uint netId)
        {
            #if !UNITY_SERVER || UNITY_EDITOR
            if (((NetworkIdentity)cmc.rightWeapon.gameObject.FindComponent(typeof(NetworkIdentity))).netId == netId)
                cmc.ClientRemoveRightWeapon();
            #endif
        }
        [ClientRpc]
        public virtual void Rpc_EquipMeleeWeapon(uint netId)
        {
            #if !UNITY_SERVER || UNITY_EDITOR
            NetworkIdentity ni = FindObjectsOfType<NetworkIdentity>().ToList().Find(x => x.netId == netId);
            if (ni)
            {
                cmc.ClientEquipMeleeWeapon((vCollectableStandalone)ni.gameObject.FindComponent(typeof(vCollectableStandalone)));
            }
            #endif
        }
        #endregion

        #region vItemManager
        [ClientRpc(includeOwner = false)]
        protected virtual void Rpc_InventoryLock(bool value)
        {
            #if !UNITY_SERVER || UNITY_EDITOR
            if (value == true)
            {
                tpi.SetLockCameraInput(true);
                tpi.SetLockAllInput(true);
                tpController.input = Vector3.zero;
                tpController.Sprint(false);
                tpController.animator.SetFloat("InputHorizontal", 0.0f, 0.25f, Time.deltaTime);
                tpController.animator.SetFloat("InputVertical", 0.0f, 0.25f, Time.deltaTime);
                tpController.animator.SetFloat("InputMagnitude", 0.0f, 0.25f, Time.deltaTime);
            }
            else
            {
                tpi.SetLockCameraInput(false);
                tpi.SetLockAllInput(false);
            }
            #endif
        }
        [ClientRpc]
        protected virtual void Rpc_ChangeAmount(int itemId, int amount)
        {
            #if !UNITY_SERVER || UNITY_EDITOR
            if (NetworkServer.active) return;
            vItem item = im.items.Find(x => x.id == itemId);
            if (item)
            {
                item.amount = amount;
            }
            #endif
        }
        [ClientRpc]
        public virtual void Rpc_UseItem(int itemId, int amount)
        {
            #if !UNITY_SERVER || UNITY_EDITOR
            if (NetworkServer.active) return;

            vItem item = im.items.Find(x => x.id == itemId);
            if (item == null)
            {
                ItemReference newItem = new ItemReference(itemId);
                newItem.amount = amount;
                newItem.addToEquipArea = false;
                im.AddItem(newItem);
            }
            im.Client_UseItem(item);
            #endif
        }
        
        /// <summary>
        /// Called on owning client only from server when the server adds an item to its inventory.
        /// </summary>
        [TargetRpc]
        protected virtual void Rpc_AddToInventory(int id, int total)
        {
            #if !UNITY_SERVER || UNITY_EDITOR
            if (NetworkServer.active) return;

            ItemReference newItem = new ItemReference(id);
            newItem.addToEquipArea = false;
            vItem target = im.items.Find(x => x.id == id);
            if (target == null)
                newItem.amount = total;
            else
                newItem.amount = Mathf.Abs(target.amount - total);
            if (newItem.amount > 0)
                im.AddItem(newItem, true);
            #endif
        }
        
        /// <summary>
        /// Called on the owning client only from server when the server removes an item from its inventory.
        /// </summary>
        [TargetRpc]
        protected virtual void Rpc_RemoveFromInventory(int id, int total)
        {
            #if !UNITY_SERVER || UNITY_EDITOR
            if (NetworkServer.active) return;

            vItem target = im.items.Find(x => x.id == id);
            if (total == 0 && target != null)
                im.Client_DestoryItem(target, target.amount);
            else if (total > 0)
                if (target != null)
                    im.Client_DestoryItem(target, Mathf.Abs(target.amount - total));
                else
                {
                    ItemReference newItem = new ItemReference(id);
                    newItem.amount = total;
                    newItem.addToEquipArea = false;
                    im.AddItem(newItem, true);
                }

            #endif
        }
        #endregion

        #endregion
        #endregion
    }
}
