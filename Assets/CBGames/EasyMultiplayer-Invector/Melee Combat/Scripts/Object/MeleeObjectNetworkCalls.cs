using Invector;
using Mirror;
using UnityEngine;

namespace EMI.Object
{
    public class MeleeObjectNetworkCalls : NetworkBehaviour
    {
        #region Properties
        
        #region vBreakableObject
        protected MP_vBreakableObject bo = null;
        [HideInInspector, SyncVar(hook = nameof(BrokenChanged))] public bool broken = false;
        #endregion
        
        #endregion

        #region Initilization
        protected virtual void Awake()
        {
            bo = GetComponent<MP_vBreakableObject>();
        }
        protected virtual void Start()
        {
            BrokenChanged(false, broken);
            if (bo)
            {
                if (NetworkClient.active)
                {
                    bo.OnBroken.AddListener(ClientBroke);
                }
                else if (NetworkServer.active)
                {
                    bo.OnBroken.AddListener(ServerBroke);
                }
            }
        }
        #endregion

        #region Callbacks
        public virtual void ClientBroke()
        {
            Cmd_Break();
        }
        protected virtual void ServerBroke()
        {
            broken = true;
        }
        #endregion

        #region Hooks
        protected virtual void BrokenChanged(bool oldValue, bool newValue)
        {
            if (NetworkServer.active) return;
            if (newValue == true && bo) bo.BreakObject();
        }
        #endregion

        #region RPCs
        #region Commands
        
        #region vBreakableObject
        [Command(requiresAuthority = false)]
        public virtual void Cmd_Break()
        {
            #if UNITY_SERVER || UNITY_EDITOR
            broken = true;
            BrokenChanged(false, true);
            #endif
        }
        #endregion
        
        #endregion

        #region ClientRpcs

        #endregion
        #endregion
    }
}
