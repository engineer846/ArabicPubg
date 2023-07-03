using EMI.Object;
using System.Collections;
using UnityEngine;

namespace Invector
{
    [RequireComponent(typeof(MeleeObjectNetworkCalls))]
    public class MP_vBreakableObject : vBreakableObject
    {
        public virtual void BreakObject()
        {
            if (!isBroken)
                StartCoroutine(LateJoinersBreakObjet());
        }

        protected override IEnumerator BreakObjet()
        {
            if (_rigidBody) Destroy(_rigidBody);
            if (_collider) Destroy(_collider);
            if (GetComponent<MeshCollider>()) Destroy(GetComponent<MeshCollider>());
            yield return new WaitForSeconds(0.0001f);
            brokenObject.transform.parent = null;
            brokenObject.gameObject.SetActive(true);
            OnBroken.Invoke();
            gameObject.GetComponent<MeshRenderer>().enabled = false; // make object appear invisible but keep components active.
        }
        protected virtual IEnumerator LateJoinersBreakObjet()
        {
            if (_rigidBody) Destroy(_rigidBody);
            if (_collider) Destroy(_collider);
            if (GetComponent<MeshCollider>()) Destroy(GetComponent<MeshCollider>());
            yield return new WaitForSeconds(0.0001f);
            brokenObject.transform.parent = null;
            brokenObject.gameObject.SetActive(true);
            //OnBroken.Invoke();
            gameObject.GetComponent<MeshRenderer>().enabled = false; // make object appear invisible but keep components active.
        }
    }
}
