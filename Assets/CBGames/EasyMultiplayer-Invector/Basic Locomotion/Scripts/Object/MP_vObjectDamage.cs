using System.Collections;
using UnityEngine;

namespace Invector
{
    public class MP_vObjectDamage : vObjectDamage
    {
        protected override void OnTriggerEnter(Collider hit)
        {
            if (targets == null)
            {
                StartCoroutine(WaitForTargets(hit));
                return;
            }
            else if (hit == null || hit.gameObject == null) return;
            base.OnTriggerEnter(hit);
        }
        protected virtual IEnumerator WaitForTargets(Collider hit)
        {
            yield return new WaitUntil(() => targets != null);
            OnTriggerEnter(hit);
        }
    }
}
