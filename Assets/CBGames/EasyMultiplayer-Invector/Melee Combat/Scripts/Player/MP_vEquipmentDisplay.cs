using EMI.Player;
using Mirror;
using UnityEngine;

namespace Invector.vItemManager
{
    public class MP_vEquipmentDisplay : vEquipmentDisplay
    {
        #region Properties
        protected MeleeNetworkCalls nc = null;
        #endregion

        #region Initilization
        protected override void Start()
        {
            nc = GetComponentInParent<MeleeNetworkCalls>();
            base.Start();
        }
        #endregion

        #region Overrides
        protected override void UpdateDisplays(vItem item)
        {
            if (NetworkClient.active && nc && nc.hasAuthority || (!NetworkClient.active && !NetworkServer.active))
                base.UpdateDisplays(item);
        }
        #endregion
    }
}
