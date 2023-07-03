using EMI.Player;
using Invector.vShooter;
using Mirror;
using UnityEngine;

namespace Invector.vItemManager
{
    public class MP_vShooterEquipment : vShooterEquipment
    {
        #region Properties
        protected ShooterNetworkCalls nc = null;
        protected vShooterWeapon sw = null;
        #endregion

        #region Initilization
        protected virtual void Awake()
        {
            sw = GetComponent<vShooterWeapon>();
        }
        protected virtual void Start()
        {
            nc = GetComponentInParent<ShooterNetworkCalls>();
            if (!nc.hasAuthority)
            {
                foreach(Camera cam in GetComponentsInChildren<Camera>(true))
                {
                    cam.gameObject.SetActive(false);
                }
            }
        }
        #endregion

        #region overrides
        protected override void ChangeAmmo(int value)
        {
            base.ChangeAmmo(value);
            if (!referenceItem || !NetworkServer.active || sw == null) return;
            var ammoAttribute = referenceItem.GetItemAttribute(vItemAttributes.AmmoCount);
            if (ammoAttribute != null)
            {
                if (sw.isLeftWeapon && nc.leftWeaponAmmo != ammoAttribute.value)
                    nc.leftWeaponAmmo = ammoAttribute.value;
                else if (nc.rightWeaponAmmo != ammoAttribute.value)
                    nc.rightWeaponAmmo = ammoAttribute.value;
            }
        }
        protected override bool CheckAmmo(ref bool isValid, ref int totalAmmo)
        {
            if (!referenceItem) return false;
            var ammoAttribute = referenceItem.GetItemAttribute(vItemAttributes.AmmoCount);

            if (NetworkServer.active && sw && nc) // The server must calculate the shot BEFORE, otherwise none owning clients will be off by 1 bullet.
            {
                if (sw.isLeftWeapon && nc.leftWeaponAmmo != ammoAttribute.value)
                    nc.leftWeaponAmmo = ammoAttribute.value;
                else if (nc.rightWeaponAmmo != ammoAttribute.value)
                    nc.rightWeaponAmmo = ammoAttribute.value;
            }

            isValid = ammoAttribute != null && !ammoAttribute.isBool;
            if (isValid)
            {
                try
                {
                    if (sw.isLeftWeapon)
                        totalAmmo = nc.leftWeaponAmmo;
                    else
                        totalAmmo = nc.rightWeaponAmmo;
                }
                catch
                {
                    totalAmmo = ammoAttribute.value;
                }
            }
            return isValid && totalAmmo > 0;
        }
        #endregion
    }
}
