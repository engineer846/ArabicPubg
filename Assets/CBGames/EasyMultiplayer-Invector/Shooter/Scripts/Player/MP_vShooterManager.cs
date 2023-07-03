using EMI.Player;
using EMI.Utils;
using Invector.vItemManager;
using Invector.vMelee;
using Mirror;
using System.Collections;
using UnityEngine;

namespace Invector.vShooter
{
    public class MP_vShooterManager : vShooterManager
    {
        #region Properties
        protected ShooterNetworkCalls nc = null;
        protected MP_vItemManager im = null;
        protected bool performCheck = false;
        #endregion

        #region Initialization
        protected virtual void Awake()
        {
            nc = GetComponent<ShooterNetworkCalls>();
            im = GetComponent<MP_vItemManager>();
        }
        public override void Start()
        {
            if (!nc.hasAuthority || (NetworkServer.active && !NetworkClient.active))
            {
                useAmmoDisplay = false; // make only none networked versions of the player have an ammo display.
            }
            if (!ignoreTags.Contains(gameObject.tag))
            {
                performCheck = true;
            }
            base.Start();
            if (performCheck && ignoreTags.Contains(gameObject.tag))
            {
                ignoreTags.Remove(gameObject.tag);
            }
        }
        #endregion

        #region Setting Weapons
        /// <summary>
        /// Having the server send this across the network guarantees that a players weapon stance will 
        /// always match the servers. So if a player desyncs with the server they will re-sync on the 
        /// next update that they receive from the server (roughly every 10ms)
        /// </summary>

        public override void SetLeftWeapon(GameObject weaponObject)
        {
            if (NetworkServer.active && GetComponent<vCollectShooterMeleeControl>())
            {
                if (weaponObject != null)
                {
                    NetworkIdentity ni = (NetworkIdentity)weaponObject.FindComponent(typeof(NetworkIdentity));
                    if (!ni)
                    {
                        Debug.LogError(weaponObject + ": All equippable objects MUST have a NetworkIdentity on them. This object does not.");
                    }
                    else
                    {
                        nc.setCMCShooterLeftWeapon = ni.netId;
                    }
                }
                else
                {
                    nc.setCMCShooterLeftWeapon = 0;
                }
            }
            base.SetLeftWeapon(weaponObject);
            MP_vItemManager im = GetComponent<MP_vItemManager>();
            if (im && nc)
            {
                if (NetworkServer.active && weaponObject)
                {
                    vShooterWeapon weapon = weaponObject.GetComponent<vShooterWeapon>();
                    vShooterEquipment equipment = weaponObject.GetComponent<vShooterEquipment>();
                    if (weapon && equipment)
                    {
                        var ammoAttribute = equipment.referenceItem.GetItemAttribute(vItemManager.vItemAttributes.AmmoCount);
                        if (ammoAttribute != null)
                        {
                            if (weapon.isLeftWeapon)
                                nc.leftWeaponAmmo = ammoAttribute.value;
                            else
                                nc.rightWeaponAmmo = ammoAttribute.value;
                        }
                    }
                }
            }
        }
        public override void SetRightWeapon(GameObject weaponObject)
        {
            if (NetworkServer.active && GetComponent<vCollectShooterMeleeControl>())
            {
                if (weaponObject != null)
                {
                    NetworkIdentity ni = (NetworkIdentity)weaponObject.FindComponent(typeof(NetworkIdentity));
                    if (!ni)
                    {
                        Debug.LogError(weaponObject + ": All equippable objects MUST have a NetworkIdentity on them. This object does not.");
                    }
                    else
                    {
                        nc.setCMCShooterRightWeapon = ni.netId;
                    }
                }
                else
                {
                    nc.setCMCShooterRightWeapon = 0;
                }
            }
            base.SetRightWeapon(weaponObject);
            MP_vItemManager im = GetComponent<MP_vItemManager>();
            if (im && nc)
            {
                if (NetworkServer.active && nc && weaponObject)
                {
                    vShooterWeapon weapon = weaponObject.GetComponent<vShooterWeapon>();
                    vShooterEquipment equipment = weaponObject.GetComponent<vShooterEquipment>();
                    if (weapon && equipment)
                    {
                        var ammoAttribute = equipment.referenceItem.GetItemAttribute(vItemManager.vItemAttributes.AmmoCount);
                        if (ammoAttribute != null)
                        {
                            if (weapon.isLeftWeapon)
                                nc.leftWeaponAmmo = ammoAttribute.value;
                            else
                                nc.rightWeaponAmmo = ammoAttribute.value;
                        }
                    }
                }
            }
        }
        protected override void SetLeftWeapon(vShooterWeapon weapon)
        {
            if (NetworkServer.active && GetComponent<vCollectShooterMeleeControl>())
            {
                if (weapon != null && GetComponent<vCollectShooterMeleeControl>())
                {
                    NetworkIdentity ni = (NetworkIdentity)weapon.gameObject.FindComponent(typeof(NetworkIdentity));
                    if (!ni)
                    {
                        Debug.LogError(weapon + ": All equippable objects MUST have a NetworkIdentity on them. This object does not.");
                    }
                    else
                    {
                        nc.setCMCShooterLeftWeapon = ni.netId;
                    }
                }
                else
                {
                    nc.setCMCShooterLeftWeapon = 0;
                }
            }
            base.SetLeftWeapon(weapon);
            MP_vItemManager im = GetComponent<MP_vItemManager>();
            if (im && nc)
            {
                if (NetworkServer.active && weapon)
                {
                    vShooterEquipment equipment = weapon.gameObject.GetComponent<vShooterEquipment>();
                    if (equipment)
                    {
                        var ammoAttribute = equipment.referenceItem.GetItemAttribute(vItemManager.vItemAttributes.AmmoCount);
                        if (ammoAttribute != null)
                        {
                            if (weapon.isLeftWeapon)
                                nc.leftWeaponAmmo = ammoAttribute.value;
                            else
                                nc.rightWeaponAmmo = ammoAttribute.value;
                        }
                    }
                }
            }
        }
        protected override void SetRightWeapon(vShooterWeapon weapon)
        {
            if (NetworkServer.active && GetComponent<vCollectShooterMeleeControl>())
            {
                if (weapon != null)
                {
                    NetworkIdentity ni = (NetworkIdentity)weapon.gameObject.FindComponent(typeof(NetworkIdentity));
                    if (!ni)
                    {
                        Debug.LogError(weapon + ": All equippable objects MUST have a NetworkIdentity on them. This object does not.");
                    }
                    else
                    {
                        nc.setCMCShooterRightWeapon = ((NetworkIdentity)weapon.gameObject.FindComponent(typeof(NetworkIdentity))).netId;
                    }
                }
                else
                {
                    nc.setCMCShooterRightWeapon = 0;
                }
            }
            base.SetRightWeapon(weapon);
            MP_vItemManager im = GetComponent<MP_vItemManager>();
            if (im && nc)
            {
                if (NetworkServer.active && nc && weapon)
                {
                    vShooterEquipment equipment = weapon.gameObject.GetComponent<vShooterEquipment>();
                    if (equipment)
                    {
                        var ammoAttribute = equipment.referenceItem.GetItemAttribute(vItemManager.vItemAttributes.AmmoCount);
                        if (ammoAttribute != null)
                        {
                            if (weapon.isLeftWeapon)
                                nc.leftWeaponAmmo = ammoAttribute.value;
                            else
                                nc.rightWeaponAmmo = ammoAttribute.value;
                        }
                    }
                }
            }
        }
        #endregion

        #region Setting Ammo
        protected override void CollectExtraAmmo(vShooterWeapon weapon)
        {
            #if UNITY_SERVER || UNITY_EDITOR
            if (weapon.gameObject.GetComponent<MP_vShooterWeapon>())
            {
                // make sure all values have initiliazed on the server.
                if (((MP_vShooterWeapon)weapon).i_ammo == false)
                    _ = weapon.ammo;
                if (((MP_vShooterWeapon)weapon).i_clipSize == false)
                    _ = weapon.clipSize;
                if (((MP_vShooterWeapon)weapon).i_shootFrequency == false)
                    _ = weapon.shootFrequency;
                if (((MP_vShooterWeapon)weapon).i_minDmgDist == false)
                    _ = weapon.minDamageDistance;
                if (((MP_vShooterWeapon)weapon).i_maxDmgDist == false)
                    _ = weapon.maxDamageDistance;
                if (((MP_vShooterWeapon)weapon).i_minDmg == false)
                    _ = weapon.minDamage;
                if (((MP_vShooterWeapon)weapon).i_maxDmg == false)
                    _ = weapon.maxDamage;
            }
            #endif
            
            base.CollectExtraAmmo(weapon);
        }
        public virtual void ForceDisplayUpdate(int side)
        {
            UpdateAmmoDisplay(side);
        }
        #endregion

        #region Camera
        /// <summary>
        /// Makes it so the owner camera doesn't have recoil when another player shoots.
        /// </summary>
        /// <param name="horizontal"></param>
        /// <param name="up"></param>
        /// <returns></returns>
        protected override IEnumerator Recoil(float horizontal, float up)
        {
            if ((!NetworkClient.active && !NetworkServer.active) || nc == null || (NetworkClient.active && nc.hasAuthority))
            {
                yield return base.Recoil(horizontal, up);
            }
            else
            {
                yield return null;
            }
        }
        #endregion
    }
}
