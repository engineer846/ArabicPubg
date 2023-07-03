using EMI.Player;
using Mirror;

namespace Invector.vItemManager
{
    public class MP_vItemCollectionDisplay : vItemCollectionDisplay
    {
        #region Properties
        protected MeleeNetworkCalls nc = null;
        #endregion

        #region Initilization
        protected virtual void Awake()
        {
            nc = GetComponentInParent<MeleeNetworkCalls>();
        }
        #endregion

        #region FadeText
        public override void FadeText(string message, float timeToStay, float timeToFadeOut)
        {
            if ((!NetworkServer.active && !NetworkClient.active) || nc == null || nc.hasAuthority && NetworkClient.active)
            {
                base.FadeText(message, timeToStay, timeToFadeOut);
            }
        }
        #endregion
    }
}
