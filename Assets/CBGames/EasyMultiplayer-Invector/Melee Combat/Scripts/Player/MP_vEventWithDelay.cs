using EMI.Player;
using Mirror;
using UnityEngine;

namespace Invector.Utils
{
    public class MP_vEventWithDelay : vEventWithDelay
    {
        #region Properties
        protected MeleeNetworkCalls nc = null;
        [SerializeField, Tooltip("If you want these events to ONLY trigger if they get called by the owning player")]
        protected bool ownerOnlyEvents = true;
        #endregion

        #region Initilization
        protected override void Start()
        {
            nc = GetComponentInParent<MeleeNetworkCalls>();
        }
        #endregion

        #region Overrides
        public override void DoEvents()
        {
            if ((!NetworkClient.active && !NetworkServer.active) || (NetworkClient.active && nc && nc.hasAuthority && ownerOnlyEvents) || !ownerOnlyEvents)
            {
                base.DoEvents();
            }
        }
        public override void DoEvent(int index)
        {
            if ((!NetworkClient.active && !NetworkServer.active) || (NetworkClient.active && nc && nc.hasAuthority && ownerOnlyEvents) || !ownerOnlyEvents)
                base.DoEvent(index);
        }
        public override void DoEvent(string name)
        {
            if ((!NetworkClient.active && !NetworkServer.active) || (NetworkClient.active && nc && nc.hasAuthority && ownerOnlyEvents) || !ownerOnlyEvents)
                base.DoEvent(name);
        }
        #endregion
    }
}
