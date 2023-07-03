using EMI.Player;
using EMI.Utils;
using UnityEngine;

public class MP_vSimpleInput : vSimpleInput
{
    #region Properties
    [SerializeField, Tooltip("The globally unique name for this key. Key names cannot match any other key names, they must be globally unique")]
    protected string keyName = "vSimpleInput";
    protected MeleeNetworkCalls nc = null;
    #endregion

    #region Initialization
    protected virtual void Awake()
    {
        nc = (MeleeNetworkCalls)gameObject.FindComponent(typeof(MeleeNetworkCalls));
        input.SetNetworkCalls(nc, keyName);
    }
    #endregion
}
