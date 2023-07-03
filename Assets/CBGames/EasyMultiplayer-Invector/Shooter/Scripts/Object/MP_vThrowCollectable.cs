using System.Collections.Generic;
using UnityEngine;

namespace Invector.vCharacterController
{
    public class MP_vThrowCollectable : vThrowCollectable
    {
        #region Properties
        [SerializeField, Tooltip("List of ALL player tags you're using in this project (if something other than OtherPlayer and Player). \n\nDon't edit this unless you know what you're doing!")]
        protected vTagMask playerTags = new List<string> { "Player" };
        #endregion

        #region Overrides
        protected override void OnTriggerEnter(Collider other)
        {
            if (playerTags.Contains(other.gameObject.tag) && other.GetComponentInChildren<vThrowManager>() != null)
                throwManager = other.GetComponentInChildren<vThrowManager>();
        }
        #endregion
    }
}
