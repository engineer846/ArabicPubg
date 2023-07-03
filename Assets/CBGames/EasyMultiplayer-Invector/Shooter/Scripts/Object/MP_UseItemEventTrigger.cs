using System.Collections.Generic;
using UnityEngine;

namespace Invector.vItemManager
{
    public class MP_UseItemEventTrigger : UseItemEventTrigger
    {
        #region Properties
        [SerializeField, Tooltip("The tags you want to trigger these")]
        protected vTagMask tagsCanInteract = new List<string> { "Player" };
        protected GameObject lastEntered;
        #endregion

        #region Overrides
        public override void OnTriggerEnter(Collider other)
        {
            if (tagsCanInteract.Contains(other.gameObject.tag))
            {
                itemManager = other.GetComponent<vItemManager>();
                if (itemManager)
                {
                    lastEntered = other.gameObject;
                    itemEvent.targetItem = itemManager.GetItem(itemEvent.id);
                    if (itemEvent.targetItem)
                    {
                        itemEvent.ChangeItemUsageDelay();
                        itemManager.onUseItem.AddListener(OnUseItem);
                        itemManager.onOpenCloseInventory.AddListener(itemEvent.OnOpenInventory);
                        itemEvent.targetItem.canBeUsed = true;
                    }
                }
            }
        }
        public override void OnTriggerExit(Collider other)
        {
            if (tagsCanInteract.Contains(other.gameObject.tag))
            {
                if (itemManager)
                {
                    lastEntered = null;
                    itemManager.onUseItem.RemoveListener(OnUseItem);
                    itemManager.onOpenCloseInventory.RemoveListener(itemEvent.OnOpenInventory);
                    if (itemEvent.targetItem)
                    {
                        itemEvent.targetItem.canBeUsed = false;
                        itemEvent.ResetItemUsageDelay();
                        itemEvent.targetItem = null;
                    }
                }
            }
        }
        protected override void OnUseItem(vItem item)
        {
            if (itemManager == null && lastEntered != null)
                itemManager = lastEntered.GetComponent<vItemManager>();
            base.OnUseItem(item);
        }
        #endregion
    }
}
