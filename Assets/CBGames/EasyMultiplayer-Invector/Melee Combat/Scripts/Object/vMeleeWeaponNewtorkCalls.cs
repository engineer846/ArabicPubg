using EMI.Utils;
using Invector.vCharacterController.vActions;
using Mirror;
using UnityEngine;

namespace EMI.Object
{
    public class vMeleeWeaponNewtorkCalls : NetworkBehaviour
    {
        #region Properties
        protected NetworkIdentity ni = null;
        protected vWeaponConstrain wc = null;
        protected Rigidbody rb = null;
        [HideInInspector, SyncVar(hook = nameof(OnInv_Weapon_FreezeAll))] public bool frozen = false;
        [HideInInspector, SyncVar(hook = nameof(RBKinematicSetting))] public bool rb_iskinematic = false;
        [HideInInspector, SyncVar] public bool hasParent = false;
        #endregion

        #region Initilization
        protected virtual void Awake()
        {
            ni = GetComponent<NetworkIdentity>();
            wc = GetComponent<vWeaponConstrain>();
            rb = GetComponent<Rigidbody>();
            vCollectableStandalone collectable = (vCollectableStandalone)gameObject.FindComponent(typeof(vCollectableStandalone));
            if (NetworkServer.active)
            {
                collectable.OnEquip.AddListener(SetKinematicToTrue);
                collectable.OnDrop.AddListener(SetKinematicToFalse);
            }
        }
        #endregion

        #region Event Actions
        public virtual void SetKinematicToTrue()
        {
            rb_iskinematic = true;
        }
        public virtual void SetKinematicToFalse()
        {
            rb_iskinematic = false;
        }
        #endregion

        #region Hooks
        protected virtual void RBKinematicSetting(bool oldValue, bool newValue)
        {
            if (NetworkServer.active) return;
            if (rb == null) return;
            rb.isKinematic = newValue;
        }
        protected virtual void OnInv_Weapon_FreezeAll(bool oldValue, bool newValue)
        {
            if (NetworkServer.active) return;
            if (wc != null)
            {
                wc.Inv_Weapon_FreezeAll(newValue);
            }
        }
        #endregion

    }
}
