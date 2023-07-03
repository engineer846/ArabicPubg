using EMI.Managers;
using EMI.Player;
using EMI.Utils;
using Invector.vCharacterController;
using Mirror;
using UnityEngine;

namespace Invector.vCamera
{
    public class MP_vThirdPersonCamera : vThirdPersonCamera
    {
        #region Properties
        protected ClientConnection conn = null;
        internal override float switchRight 
        { 
            get 
            {
                if (conn != null && conn.playerCharacter != null)
                {
                    return conn.playerCharacter.GetComponent<BasicNetworkCalls>().switchRight;
                }
                else
                {
                    return _switchRight;
                }
            } 
            set 
            { 
                _switchRight = value;
                #if UNITY_SERVER || UNITY_EDITOR
                if (conn != null && conn.playerCharacter != null && NetworkServer.active)
                {
                    conn.playerCharacter.GetComponent<BasicNetworkCalls>().Cmd_SetSwitchRight(_switchRight);
                }
                #endif
            } 
        }
        #endregion

        #region Initialize
        protected override void Start()
        {
            vThirdPersonController controller = ClientUtil.GetMyCharacterControllerObject();
            if (controller)
            {
                mainTarget = controller.transform;
            }
            base.Start();
        }
        #endregion

        #region Overrides
        /// <summary>
        /// This is called via the ThirdPersonInput, need to make sure this is not 
        /// called by a controller you don't own otherwise other joining characters
        /// will steal the camera away from you.
        /// </summary>
        /// <param name="newTarget"></param>
        public override void SetMainTarget(Transform newTarget)
        {
            if (newTarget.GetComponent<NetworkIdentity>() && newTarget.GetComponent<NetworkIdentity>().hasAuthority)
            {
                base.SetMainTarget(newTarget);
            }
        }
        #endregion
    }
}
