using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace EMI.Object
{
    public class ActivatedMemory : NetworkBehaviour
    {
        #region Properties
        [SerializeField] protected UnityEvent LateJoinAlreadyActived;
        [HideInInspector, SyncVar] public bool hasBeenActivated = false;
        #endregion

        #region Initilization
        protected virtual void Start()
        {
            if (hasBeenActivated)
            {
                LateJoinAlreadyActived?.Invoke();
            }
        }
        #endregion

        #region Event Callbacks
        public virtual void SaveActivatedStateToMemory(bool activated)
        {
            if (NetworkServer.active)
            {
                hasBeenActivated = true;
            }
        }
        #endregion
    }
}
