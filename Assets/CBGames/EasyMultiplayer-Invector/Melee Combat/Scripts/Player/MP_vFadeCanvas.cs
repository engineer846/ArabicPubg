using EMI.Player;
using Mirror;

namespace Invector.Utils
{
    public class MP_vFadeCanvas : vFadeCanvas
    {
        #region Properties
        protected MeleeNetworkCalls nc = null;
        #endregion

        #region Initilization
        protected override void Start()
        {
            nc = GetComponentInParent<MeleeNetworkCalls>();
            if ((!NetworkClient.active && !NetworkServer.active) || (NetworkClient.active && nc && nc.hasAuthority))
                base.Start();
            else if (group) 
                group.alpha = 0f;
        }
        #endregion

        #region Overrides
        protected override void InitilizeFadeEffect()
        {
            if ((!NetworkClient.active && !NetworkServer.active) || (NetworkClient.active && nc && nc.hasAuthority))
                base.InitilizeFadeEffect();
        }
        public override void FadeIn()
        {
            if ((!NetworkClient.active && !NetworkServer.active) || (NetworkClient.active && nc && nc.hasAuthority))
                base.FadeIn();
        }
        public override void FadeOut()
        {
            if ((!NetworkClient.active && !NetworkServer.active) || (NetworkClient.active && nc && nc.hasAuthority))
                base.FadeOut();
        }
        public override void AlphaFull()
        {
            if ((!NetworkClient.active && !NetworkServer.active) || (NetworkClient.active && nc && nc.hasAuthority))
                base.AlphaFull();
        }
        public override void AlphaZero()
        {
            if ((!NetworkClient.active && !NetworkServer.active) || (NetworkClient.active && nc && nc.hasAuthority))
                base.AlphaZero();
        }
        #endregion
    }
}
