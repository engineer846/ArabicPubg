using EMI.Player;
using Mirror;
using UnityEngine;

namespace Invector
{
    public class MP_vTutorialTextTrigger : vTutorialTextTrigger
    {
        protected override void OnTriggerEnter(Collider other)
        {
            if ((other.GetComponent<BasicNetworkCalls>() && other.GetComponent<BasicNetworkCalls>().hasAuthority && NetworkClient.active) || 
                !NetworkClient.active && !NetworkServer.active)
            {
                base.OnTriggerEnter(other);
            }
        }
        protected override void OnTriggerExit(Collider other)
        {
            if ((other.GetComponent<BasicNetworkCalls>() && other.GetComponent<BasicNetworkCalls>().hasAuthority && NetworkClient.active) ||
                !NetworkClient.active && !NetworkServer.active)
            {
                base.OnTriggerExit(other);
            }
        }
    }
}
