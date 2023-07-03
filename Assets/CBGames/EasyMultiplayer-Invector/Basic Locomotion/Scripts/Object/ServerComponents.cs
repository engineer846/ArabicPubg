using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EMI.Object
{
    public class ServerComponents : MonoBehaviour
    {
        #region Properties
        [SerializeField, Tooltip("The list of components to enable if this is the server, otherwise disable them.")]
        protected List<Behaviour> components = new List<Behaviour>();
        #endregion

        protected virtual void Start()
        {
            RefreshList();
        }

        public virtual void RefreshList()
        {
            bool enableComp = NetworkServer.active;
            components.ForEach(c => c.enabled = enableComp);
        }
    }
}
