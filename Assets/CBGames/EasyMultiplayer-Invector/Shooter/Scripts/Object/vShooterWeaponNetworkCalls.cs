using Invector.vShooter;
using Mirror;
using UnityEngine;

namespace EMI.Object
{ 
    public class vShooterWeaponNetworkCalls : vMeleeWeaponNewtorkCalls
    {
        #region Properties
        protected vShooterManager sm = null;
        protected MP_vShooterWeapon sw = null;
        [HideInInspector, SyncVar(hook = nameof(OnAmmoChanged))] public int ammo = 0;
        [HideInInspector, SyncVar] public int clipSize = 25;
        [HideInInspector, SyncVar] public float shootFrequency = 0.125f;
        [HideInInspector, SyncVar] public float minDamageDistance = 8f;
        [HideInInspector, SyncVar] public float maxDamageDistance = 50f;
        [HideInInspector, SyncVar] public int minDamage = 15;
        [HideInInspector, SyncVar] public int maxDamage = 25;
        #endregion

        #region Initilization
        protected override void Awake()
        {
            sw = GetComponent<MP_vShooterWeapon>();
            base.Awake();
        }
        #endregion

        #region Hooks
        protected virtual void OnAmmoChanged(int oldVlaue, int newValue)
        {
            if (NetworkServer.active) return;
            if (sw)
            {
                sm = GetComponentInParent<vShooterManager>();
                if (sm) sm.UpdateTotalAmmo();

                sw.changeAmmoHandle?.Invoke(newValue);
                sw.ammo = newValue;
            }
        }
        #endregion
    }
}
