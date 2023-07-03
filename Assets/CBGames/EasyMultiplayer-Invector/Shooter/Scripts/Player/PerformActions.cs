using UnityEngine;
using Mirror;
using UnityEngine.Events;

public class PerformActions : MonoBehaviour
{
    #region Properties
    [SerializeField, Tooltip("Needs a NetworkBehavior to determine if this is owned by you or not.")]
    protected NetworkBehaviour nb = null;
    [SerializeField, Tooltip("Only execute if you're the owning player, or only execute if your NOT the owning player")]
    protected bool ifOwningPlayer = false;
    [SerializeField]
    protected UnityEvent OnStart;
    #endregion

    #region Initilization
    protected virtual void Start()
    {
        if (!nb)
        {
            nb = GetComponentInParent<NetworkBehaviour>();
            if (!nb)
            {
                enabled = false;
                return;
            }
        }
        if (nb.hasAuthority == ifOwningPlayer)
        {
            OnStart?.Invoke();
        }
    }
    #endregion
}
