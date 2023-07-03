using EMI.Player;
using Invector.vCharacterController;
using Invector.vCharacterController.vActions;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Invector.vItemManager
{
    public class MP_vItemManager : vItemManager
    {
        #region Properties
        protected class LastEquipCall
        {
            protected vEquipArea area;
            protected vItem item;
            public LastEquipCall()
            {
                area = null;
                item = null;
            }
            public LastEquipCall(vEquipArea area, vItem item)
            {
                this.area = area;
                this.item = item;
            }
            public bool IsSame(LastEquipCall call)
            {
                if (area == null)
                    return false;
                else 
                    return this.area.Equals(call.area) && this.item.Equals(call.item);
            }
            public void Set(LastEquipCall call)
            {
                this.area = call.area;
                this.item = call.item;
            }
        }
        protected MeleeNetworkCalls nc = null;
        protected LastEquipCall lastCall = null;
        protected bool canSendDestroy = true;
        protected vThirdPersonController cc = null;
        protected vThirdPersonInput tpi = null;
        #endregion

        #region Initialization
        protected virtual void Awake()
        {
            nc = GetComponent<MeleeNetworkCalls>();
            cc = GetComponent<vThirdPersonController>();
            tpi = GetComponent<vThirdPersonInput>();
        }
        protected override IEnumerator Start()
        {
            if (nc.hasAuthority)
            {
                onOpenCloseInventory.AddListener((value) =>
                {
                    if (value == true)
                    {
                        tpi.SetLockCameraInput(true);
                        tpi.SetLockAllInput(true);
                        cc.Sprint(false);
                        cc.input = Vector3.zero;
                        cc.animator.SetFloat("InputHorizontal", 0.0f, 0.25f, Time.deltaTime);
                        cc.animator.SetFloat("InputVertical", 0.0f, 0.25f, Time.deltaTime);
                        cc.animator.SetFloat("InputMagnitude", 0.0f, 0.25f, Time.deltaTime);
                    }
                    else
                    {
                        tpi.SetLockCameraInput(false);
                    }
                    nc.Cmd_InventoryLock(value);
                });
            }
            yield return base.Start();
        }
        #endregion

        #region Item Pickups
        // Only the server can pickup items and transmit its inventory status to owners and other clients
        public override void CollectItem(ItemReference itemRef, float onCollectDelay = 0, float textDelay = 0, bool ignoreItemAnimation = true)
        {
            if ((!NetworkServer.active && !NetworkClient.active) || NetworkServer.active)
            {
                base.CollectItem(itemRef, onCollectDelay, textDelay, ignoreItemAnimation);
            }
        }
        public override void CollectItems(List<ItemReference> collection, float onCollectDelay = 0, float textDelay = 0, bool ignoreItemAnimation = true)
        {
            if ((!NetworkServer.active && !NetworkClient.active) || NetworkServer.active)
            {
                base.CollectItems(collection, onCollectDelay, textDelay, ignoreItemAnimation);
            }
        }
        protected override IEnumerator CollectItemsRoutine(float onCollectDelay = 0, float textDelay = 0, bool ignoreItemAnimation = true)
        {
            if ((!NetworkServer.active && !NetworkClient.active) || NetworkServer.active)
            {
                yield return base.CollectItemsRoutine(onCollectDelay, textDelay, ignoreItemAnimation);
            }
        }
        public override void OnReceiveAction(vTriggerGenericAction action)
        {
            if ((!NetworkServer.active && !NetworkClient.active) || NetworkServer.active)
            {
                base.OnReceiveAction(action);
            }
        }
        #endregion

        #region Drop
        /// <summary>
        /// Only the server is allowed to drop items. NOTE: Drop objects MUST have a networkIdentity on them to function.
        /// </summary>
        public override void DropItem(vItem item, int amount)
        {
            if (NetworkServer.active)
            {
                item.amount -= amount;
                if (item.dropObject != null)
                {
                    var dropObject = Instantiate(item.dropObject, transform.position, transform.rotation);
                    vItemCollection collection = dropObject.GetComponent<vItemCollection>();
                    if (collection != null)
                    {
                        collection.items.Clear();
                        var itemReference = new ItemReference(item.id);
                        itemReference.amount = amount;
                        itemReference.attributes = new List<vItemAttribute>(item.attributes);
                        collection.items.Add(itemReference);
                    }
                    NetworkServer.Spawn(dropObject); // This makes it so drop objects must have a NetworkIdentity on them now.
                    SceneManager.MoveGameObjectToScene(dropObject, gameObject.scene); // will always drop at index 0, move it to the appropriate scene, otherwise gets culled
                }
                onDropItem.Invoke(item, amount);
                if (item.amount <= 0 && items.Contains(item))
                {
                    var equipArea = System.Array.Find(inventory.equipAreas, e => e.ValidSlots.Exists(s => s.item != null && s.item.id.Equals(item.id)));

                    if (equipArea != null)
                    {
                        equipArea.UnequipItem(item);
                    }
                    items.Remove(item);
                    DestroyItem(item);
                }

                inventory.UpdateInventory();
            }
            else if (nc && nc.hasAuthority)
            {
                // Only the server is allowed to drop items, ask the server to drop this item.
                nc.Cmd_DropItem(item.id, amount);
            }
        }
        #endregion

        #region Destroy
        /// <summary>
        /// Only the server is allowed to destroy items officially. The owning client 
        /// can request the server to destroy items for them.
        /// </summary>
        public override void DestroyItem(vItem item, int amount)
        {
            if (NetworkServer.active)
                base.DestroyItem(item, amount);
            else if (NetworkClient.active && nc.hasAuthority)
                nc.Cmd_DestroyItem(item.id, amount);
        }

        /// <summary>
        /// This is a callback from a RPC, still need to have clients destroy
        /// items, but only in response to the server.
        /// </summary>
        [Client]
        public virtual void Client_DestoryItem(vItem item, int amount)
        {
            base.DestroyItem(item, amount);
        }
        [Client]
        public virtual void Client_DestroyAllItems()
        {
            for (int i = items.Count - 1; i >= 0; i--)
            {
                Client_DestoryItem(items[i], items[i].amount);
            }
        }
        #endregion

        #region Use
        /// <summary>
        /// Only the server can call UseItem. Clients need to use the client
        /// method which is designed to be triggered as a callback in response
        /// to this action taking place on the server.
        /// </summary>
        protected override void UseItem(vItem item)
        {
            if (NetworkServer.active)
            {
                nc.Rpc_UseItem(item.id, item.amount);
                base.UseItem(item);
            }
        }

        /// <summary>
        /// Since the `UseItem` method isn't public we need to make it public, but only for
        /// the server to use.
        /// </summary>
        /// <param name="item"></param>
        [Server]
        public virtual void Server_UseItem(vItem item)
        {
            UseItem(item);
        }
        /// <summary>
        /// This is triggered in response to the `UseItem` being fired on the server.
        /// </summary>
        [Client]
        public virtual void Client_UseItem(vItem item)
        {
            base.UseItem(item);
        }
        #endregion
    }
}
