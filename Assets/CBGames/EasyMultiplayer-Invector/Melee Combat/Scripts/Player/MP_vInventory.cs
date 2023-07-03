using EMI.Player;
using EMI.Utils;
using Mirror;

namespace Invector.vItemManager
{
    public class MP_vInventory : vInventory
    {
        #region Properties
        protected MeleeNetworkCalls nc = null;
        protected MP_vItemManager im = null;
        #endregion

        #region Initialization
        protected virtual void Awake()
        {
            nc = (MeleeNetworkCalls)gameObject.FindComponent(typeof(MeleeNetworkCalls));
            im = GetComponentInParent<MP_vItemManager>();
            horizontal.SetNetworkCalls(nc, "Inventory_HorizontalInput");
            vertical.SetNetworkCalls(nc, "Inventory_VerticalInput");
            submit.SetNetworkCalls(nc, "Inventory_SubmitInput");
            cancel.SetNetworkCalls(nc, "Inventory_CancelInput");
            openInventory.SetNetworkCalls(nc, "Inventory_OpenInventory");
            removeEquipment.SetNetworkCalls(nc, "Inventory_RemoveEquipment");

            for (int i = 0; i < changeEquipmentControllers.Count; i++)
            {
                changeEquipmentControllers[i].useItemInput.SetNetworkCalls(nc, "Inventory_CEC_UseItemInput" + i);
                changeEquipmentControllers[i].previousItemInput.SetNetworkCalls(nc, "Inventory_CEC_PreviousItemInput" + i);
                changeEquipmentControllers[i].nextItemInput.SetNetworkCalls(nc, "Inventory_CEC_NextItemInput" + i);
            }
        }
        #endregion

        #region Inputs
        public override void OpenCloseInventoryInput()
        {
            if ((nc.hasAuthority && NetworkClient.active) || (!NetworkClient.active && !NetworkServer.active))
            {
                base.OpenCloseInventoryInput();
            }
        }
        #endregion
    }
}
