using Invector;
using Invector.vCharacterController;
using Mirror;
using System.Collections;
using UnityEngine;

namespace EMI.Player
{
    /// <summary>
    /// This component will work with anything that has a vHealthController component
    /// </summary>
    public class Revive : MonoBehaviour
    {
        [Tooltip("How long to wait before setting the healt to the desired value.")]
        public float waitTime = 5.0f;
        [Tooltip("How much health (percentage) to give back on revive."), Range(0,100)]
        public float healthPercent = 100;
        [Tooltip("Don't revive the character until they have no parent. This helps with the bug of when you revive your character on a spike it will make them disappear.")]
        public bool dontReviveWithParent = true;

        protected bool reviving = false;
        /// <summary>
        /// Only the server can call this function. It will set the player health to the desired percentage
        /// after waiting for a certain amount of time.
        /// </summary>
        [ServerCallback]
        public virtual void TriggerRevive(GameObject blank)
        {
            TriggerRevive();
        }
        [ServerCallback]
        public virtual void TriggerRevive()
        {
            if (reviving) return;
            reviving = true;
            StartCoroutine(E_Revive());
        }
        protected virtual IEnumerator E_Revive()
        {
            vHealthController cont = GetComponent<vHealthController>();
            if (!cont)
            {
                reviving = false;
            }
            else
            {
                yield return new WaitForSeconds(waitTime);
                if (dontReviveWithParent && transform.parent != null)
                    yield return new WaitUntil(() => transform.parent == null);
                cont.ChangeHealth((int)(cont.maxHealth * healthPercent / 100)); // set health to desired percentage
                reviving = false;
            }
        }
    }
}
