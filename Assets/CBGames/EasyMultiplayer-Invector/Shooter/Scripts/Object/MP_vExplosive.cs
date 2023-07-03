using EMI.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Invector
{
    public class MP_vExplosive : vExplosive
    {
        #region Properties
        [SerializeField, Tooltip("The layer to start on that won't collide with players.")]
        protected string startingLayer = "Pushable";
        [SerializeField, Tooltip("The layer to move to that will collider with players.")]
        protected string endingLayer = "Default";
        [SerializeField, Tooltip("If you want to ignore team settings or not")]
        protected bool ignoreTeamSettings = false;
        #endregion

        #region Additions
        protected virtual void Awake()
        {
            gameObject.layer = LayerMask.NameToLayer(startingLayer);
            StartCoroutine(MoveLayers());
        }
        protected virtual IEnumerator MoveLayers()
        {
            yield return new WaitForSeconds(0.1f);
            gameObject.layer = LayerMask.NameToLayer(endingLayer);
        }
        #endregion

        #region Overrides
        protected override void Explode()
        {
            onExplode.Invoke();
            var colliders = Physics.OverlapSphere(transform.position, maxExplosionRadius); // not sure why but the layers don't seem to work in mp (have to check in another way)
            
            if (collidersReached == null)
            {
                collidersReached = new List<GameObject>();
            }

            for (int i = 0; i < colliders.Length; ++i)
            {
                if (colliders[i] != null && colliders[i].gameObject != null && applyDamageLayer.ContainsLayer(colliders[i].gameObject.layer) && !collidersReached.Contains(colliders[i].gameObject))
                {
                    if (!ignoreTeamSettings && damage.sender != null && damage.sender.transform.root.GetComponent<Team>() && colliders[i].transform.root.GetComponent<Team>())
                    {
                        if (!colliders[i].transform.root.GetComponent<Team>().IsEnemy(damage.sender.transform.root.GetComponent<Team>().teamName))
                            continue;
                    }
                    collidersReached.Add(colliders[i].gameObject);
                    var _damage = new vDamage(damage);
                    if (!_damage.sender) _damage.sender = transform;

                    _damage.hitPosition = colliders[i].ClosestPointOnBounds(transform.position);
                    _damage.receiver = colliders[i].transform;
                    var distance = Vector3.Distance(transform.position, _damage.hitPosition);
                    var damageValue = distance <= minExplosionRadius ? damage.damageValue : GetPercentageForce(distance, damage.damageValue);
                    _damage.activeRagdoll = distance > maxExplosionRadius * 0.5f ? false : _damage.activeRagdoll;

                    _damage.damageValue = (int)damageValue;
                    colliders[i].gameObject.ApplyDamage(_damage);
                }
            }
            StartCoroutine(ApplyExplosionForce());
            StartCoroutine(DestroyBomb());
        }
        #endregion
    }
}
