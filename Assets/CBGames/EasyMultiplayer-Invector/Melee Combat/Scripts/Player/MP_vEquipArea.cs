using EMI.Player;
using Mirror;

namespace Invector.vItemManager
{
    public class MP_vEquipArea : vEquipArea
    {
        #region Properties
        protected MeleeNetworkCalls nc = null;
        #endregion

        #region Overrides
        public override void OnSubmitSlot(vItemSlot slot)
        {
            if (nc == null) 
                nc = GetComponentInParent<MeleeNetworkCalls>();
            if (nc.hasAuthority) 
                base.OnSubmitSlot(slot);
            else
            {
                lastSelectedSlot = currentSelectedSlot;
                if (itemPicker != null)
                {
                    currentSelectedSlot = slot as vEquipSlot;
                    if (setEquipSlotWhenSubmit)
                    {
                        SetEquipSlot(equipSlots.IndexOf(currentSelectedSlot));
                    }
                    itemPicker.gameObject.SetActive(true);
                    itemPicker.onCancelSlot.RemoveAllListeners();
                    itemPicker.onCancelSlot.AddListener(CancelCurrentSlot);
                    //itemPicker.CreateEquipmentWindow(inventory.items, currentSelectedSlot.itemType, slot.item, OnPickItem);
                    onInitPickUpItem.Invoke();
                }
            }
        }
        public override void NextEquipSlot()
        {
            if (nc == null)
                nc = GetComponentInParent<MeleeNetworkCalls>();
            if (NetworkServer.active || nc.hasAuthority)
                base.NextEquipSlot();
        }
        public override void PreviousEquipSlot()
        {
            if (nc == null)
                nc = GetComponentInParent<MeleeNetworkCalls>();
            if (NetworkServer.active || nc.hasAuthority)
                base.PreviousEquipSlot();
        }
        #endregion
    }
}
