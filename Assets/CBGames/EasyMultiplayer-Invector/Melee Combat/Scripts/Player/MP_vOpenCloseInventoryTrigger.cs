using EMI.Utils;
using Mirror;
using System.Collections;

namespace Invector.vItemManager
{
    public class MP_vOpenCloseInventoryTrigger : vOpenCloseInventoryTrigger
    {
        protected override IEnumerator Start()
        {
            yield return base.Start();
            NetworkIdentity ni = (NetworkIdentity)gameObject.FindComponent(typeof(NetworkIdentity));
            if ((ni != null && !ni.hasAuthority && NetworkClient.active) || (NetworkServer.active && !NetworkClient.active))
            {
                Destroy(gameObject);
            }
        }
    }
}
