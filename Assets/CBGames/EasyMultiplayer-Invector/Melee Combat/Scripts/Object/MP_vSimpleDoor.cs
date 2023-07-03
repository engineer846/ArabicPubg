using EMI.Object;
using Mirror;
using UnityEngine;

namespace Invector
{
    [RequireComponent(typeof(DoorNetworkCalls))]
    public class MP_vSimpleDoor : vSimpleDoor
    {
        #region Properties
        protected DoorNetworkCalls nc = null;
        public override bool startOpened 
        { 
            get 
            { 
                return (nc) ? nc.startOpened : _startOpened; 
            } 
            set 
            { 
                _startOpened = value;
                #if UNITY_SERVER || UNITY_EDITOR
                if (NetworkServer.active)
                {
                    nc.startOpened = value;
                }
                #endif
            } 
        }
        public override bool autoOpen 
        { 
            get 
            { 
                return (nc) ? nc.autoOpen : _autoOpen; 
            } 
            set 
            { 
                _autoOpen = value;
                #if UNITY_SERVER || UNITY_EDITOR
                if (NetworkServer.active)
                {
                    nc.autoOpen = value;
                }
                #endif
            }
        }
        public override bool autoClose 
        { 
            get 
            { 
                return (nc) ? nc.autoClose : _autoClose; 
            } 
            set 
            { 
                _autoClose = value;
                #if UNITY_SERVER || UNITY_EDITOR
                if (NetworkServer.active)
                {
                    nc.autoClose = value;
                }
                #endif
            }
        }
        #endregion

        #region Initilization
        protected override void Start()
        {
            nc = GetComponent<DoorNetworkCalls>();
            #if UNITY_SERVER || UNITY_EDITOR
            if (NetworkServer.active)
            {
                nc.startOpened = _startOpened;
                nc.autoOpen = _autoOpen;
                nc.autoClose = _autoClose;
            }
            #endif
            base.Start();
        }
        #endregion
    }
}
