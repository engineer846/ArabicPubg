using EMI.Player;
using Mirror;

namespace Invector.vItemManager
{
    public class MP_vItemWindowDisplay : vItemWindowDisplay
    {
        #region Properties
        MeleeNetworkCalls nc = null;
        #endregion

        #region Initilization
        protected virtual void Start()
        {
            nc = GetComponentInParent<MeleeNetworkCalls>();
        }
        #endregion

        public override void UseItem()
        {
            if (nc == null)
                nc = GetComponentInParent<MeleeNetworkCalls>();

            if (NetworkServer.active)
            {
                base.UseItem();
            }
            else if (nc.hasAuthority)
            {
                nc.Cmd_RequestUseItem(currentSelectedSlot.item.id);
            }
        }
    }
}
