using Mirror;
using UnityEngine;

namespace EMI.Object
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class DoorNetworkCalls : NetworkBehaviour
    {
        #region Properties
        [HideInInspector, SyncVar] public bool autoOpen = false;
        [HideInInspector, SyncVar] public bool autoClose = true;
        [HideInInspector, SyncVar] public bool startOpened = false;
        #endregion
    }
}
