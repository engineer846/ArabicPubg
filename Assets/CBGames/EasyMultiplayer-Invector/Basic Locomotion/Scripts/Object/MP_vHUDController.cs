using EMI.Player;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Invector.vCharacterController
{
    public class MP_vHUDController : vHUDController
    {
        #region Properties
        [HideInInspector] public NetworkIdentity ownerNetId;
        #endregion

        #region Overrides
        public override void UpdateHUD(vThirdPersonController cc)
        {
            if ((!NetworkServer.active && !NetworkClient.active) || (cc.GetComponent<BasicNetworkCalls>().hasAuthority && NetworkClient.active))
            {
                base.UpdateHUD(cc);
            }
        }
        #endregion
    }
}
