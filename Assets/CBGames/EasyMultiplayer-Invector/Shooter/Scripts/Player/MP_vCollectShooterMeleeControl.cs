using EMI.Object;
using EMI.Player;
using EMI.Utils;
using Invector.vCharacterController.vActions;
using Invector.vShooter;
using Mirror;
using System.Collections;
using UnityEngine;

namespace Invector.vMelee
{
    public class MP_vCollectShooterMeleeControl : vCollectShooterMeleeControl
    {
        #region Properties
        protected ShooterNetworkCalls nc = null;
        #endregion

        #region Initialization
        protected override void Start()
        {
            nc = GetComponent<ShooterNetworkCalls>();

            // Unequip Inputs
            unequipRightInput.SetNetworkCalls(nc, "UnequipRightInput");
            unequipLeftInput.SetNetworkCalls(nc, "UnequipLeftInput");

            // original code (modified)
            meleeManager = GetComponent<vMeleeManager>();
            if (controlDisplayPrefab && GetComponent<NetworkIdentity>().hasAuthority && NetworkClient.active)
            {
                currentDisplay = Instantiate(controlDisplayPrefab);
            }
            base.Start();
        }
        #endregion

        #region Displays
        protected override void UpdateLeftDisplay(vCollectableStandalone collectable = null)
        {
            if (nc == null || (nc.hasAuthority && NetworkClient.active))
                base.UpdateLeftDisplay(collectable);
        }
        protected override void UpdateRightDisplay(vCollectableStandalone collectable = null)
        {
            if (nc == null || (nc.hasAuthority && NetworkClient.active))
                base.UpdateRightDisplay(collectable);
        }
        #endregion

        #region Unequipping
        public override void RemoveRightWeapon()
        {
            if ((!NetworkServer.active && !NetworkClient.active) || NetworkServer.active)
            {
                // tell newly connecting clients they don't need to request the parent since this isn't equipped
                vMeleeWeaponNewtorkCalls mwnc = (vMeleeWeaponNewtorkCalls)rightWeapon.gameObject.FindComponent(typeof(vMeleeWeaponNewtorkCalls));
                if (mwnc != null)
                {
                    mwnc.hasParent = false;
                }

                // Re-enable server syncing
                ServerSync ss = (ServerSync)rightWeapon.gameObject.FindComponent(typeof(ServerSync));
                if (ss) ss.Enable(true);

                // Drop the weapon and tell all other clients to do the same
                if (nc) nc.Rpc_UnequipRightWeapon(((NetworkIdentity)rightWeapon.gameObject.FindComponent(typeof(NetworkIdentity))).netId); // tell other clients to unequip their right weapon
                nc.setrightWeapon = 0; // prevent new joining players from desyncing because the last state was equipped
                base.RemoveRightWeapon(); // unequip your right weapon yourself
            }
        }
        public override void RemoveLeftWeapon()
        {
            if ((!NetworkServer.active && !NetworkClient.active) || NetworkServer.active)
            {
                // tell newly connecting clients they don't need to request the parent since this isn't equipped
                vMeleeWeaponNewtorkCalls mwnc = (vMeleeWeaponNewtorkCalls)leftWeapon.gameObject.FindComponent(typeof(vMeleeWeaponNewtorkCalls));
                if (mwnc != null)
                {
                    mwnc.hasParent = false;
                }

                // Re-enable server syncing
                ServerSync ss = (ServerSync)leftWeapon.gameObject.FindComponent(typeof(ServerSync));
                if (ss) ss.Enable(true);

                // Drop the weapon and tell all other clients to do the same
                if (nc) nc.Rpc_UnequipLeftWeapon(((NetworkIdentity)leftWeapon.gameObject.FindComponent(typeof(NetworkIdentity))).netId); // tell other clients to unequip their left weapon
                nc.setleftWeapon = 0; // prevent new joining players from desyncing because the last state was equipped
                base.RemoveLeftWeapon(); // unequip your left weapon yourself
            }
        }
        [Client]
        public virtual void ClientRemoveRightWeapon()
        {
            // Re-enable server syncing
            ServerSync ss = (ServerSync)rightWeapon.gameObject.FindComponent(typeof(ServerSync));
            if (ss) StartCoroutine(ReEnableSync(ss));

            // Drop the weapon
            base.RemoveRightWeapon();
        }
        [Client]
        public virtual void ClientRemoveLeftWeapon()
        {
            // Re-enable server syncing
            ServerSync ss = (ServerSync)leftWeapon.gameObject.FindComponent(typeof(ServerSync));
            if (ss) StartCoroutine(ReEnableSync(ss));

            // Drop the weapon
            base.RemoveLeftWeapon();
        }
        #endregion

        #region Equipping
        #region Shooter Weapons
        protected override void EquipShooterWeapon(vCollectableStandalone collectable)
        {
            if ((!NetworkServer.active && !NetworkClient.active) || NetworkServer.active)
            {
                NetworkIdentity ni = (NetworkIdentity)collectable.gameObject.FindComponent(typeof(NetworkIdentity));
                if (ni) // prevent invalid setup objects from being picked up
                {
                    // Tell newly connecting clients this object needs to have it's parent requested by the server.
                    vMeleeWeaponNewtorkCalls mwnc = (vMeleeWeaponNewtorkCalls)collectable.gameObject.FindComponent(typeof(vMeleeWeaponNewtorkCalls));
                    if (mwnc != null)
                    {
                        mwnc.hasParent = true;
                    }

                    // Turn off server syncing to stop this object with fighting positions
                    ServerSync ss = (ServerSync)collectable.gameObject.FindComponent(typeof(ServerSync));
                    if (ss) ss.Enable(false);

                    nc.Rpc_EquipShooterWeapon(ni.netId); // tell the other clients to equip this weapon
                    base.EquipShooterWeapon(collectable); // equip this weapon yourself
                }
            }
        }
        
        [Client]
        public virtual void ClientEquipShooterWeapon(vCollectableStandalone collectable)
        {
            var weapon = collectable.weapon.GetComponent<vShooterWeapon>();
            if (!weapon)
            {
                return;
            }

            // Turn off server syncing to stop this object with fighting positions
            ServerSync ss = (ServerSync)collectable.gameObject.FindComponent(typeof(ServerSync));
            if (ss) ss.Enable(false);

            Transform p;
            if (weapon.isLeftWeapon)
            {
                p = GetEquipPoint(leftHandler, collectable.targetEquipPoint);
                if (p)
                {
                    if (leftWeapon && leftWeapon.gameObject != collectable.gameObject)
                        ClientRemoveLeftWeapon();
                    if (rightWeapon)
                        ClientRemoveRightWeapon();
                }
            }
            else
            {
                p = GetEquipPoint(rightHandler, collectable.targetEquipPoint);
                if (p)
                {
                    if (rightWeapon && rightWeapon.gameObject != collectable.gameObject)
                        ClientRemoveRightWeapon();
                    if (leftWeapon)
                        ClientRemoveLeftWeapon();
                }
            }
            base.EquipShooterWeapon(collectable);
        }
        #endregion

        #region Melee Weapons
        protected override void EquipMeleeWeapon(vCollectableStandalone collectable)
        {
            if ((!NetworkServer.active && !NetworkClient.active) || NetworkServer.active)
            {
                NetworkIdentity ni = (NetworkIdentity)collectable.gameObject.FindComponent(typeof(NetworkIdentity));
                if (ni) // prevent invalid setup objects from being picked up
                {
                    // Tell newly connecting clients this object needs to have it's parent requested by the server.
                    vMeleeWeaponNewtorkCalls mwnc = (vMeleeWeaponNewtorkCalls)collectable.gameObject.FindComponent(typeof(vMeleeWeaponNewtorkCalls));
                    if (mwnc != null)
                    {
                        mwnc.hasParent = true;
                    }

                    // Turn off server syncing to stop this object with fighting positions
                    ServerSync ss = (ServerSync)collectable.gameObject.FindComponent(typeof(ServerSync));
                    if (ss) ss.Enable(false);

                    nc.Rpc_EquipMeleeWeapon(ni.netId); // tell the other clients to equip this weapon
                    base.EquipMeleeWeapon(collectable); // equip this weapon yourself
                }
            }
        }

        [Client]
        public virtual void ClientEquipMeleeWeapon(vCollectableStandalone collectable)
        {
            // Remove any previously equipped objects
            var weapon = collectable.weapon.GetComponent<vMeleeWeapon>();
            if (!weapon)
            {
                return;
            }

            // Turn off server syncing to stop this object with fighting positions
            ServerSync ss = (ServerSync)collectable.gameObject.FindComponent(typeof(ServerSync));
            if (ss) ss.Enable(false);
            
            if (weapon.meleeType != vMeleeType.OnlyDefense)
            {
                if (rightWeapon && rightWeapon.gameObject != collectable.gameObject)
                    ClientRemoveRightWeapon();
                if (collectable.twoHandWeapon || leftWeapon && leftWeapon.twoHandWeapon)
                    ClientRemoveLeftWeapon();
            }
            if (weapon.meleeType != vMeleeType.OnlyAttack && weapon.meleeType != vMeleeType.AttackAndDefense)
            {
                if (leftWeapon && leftWeapon.gameObject != collectable.gameObject)
                    ClientRemoveLeftWeapon();
                if (collectable.twoHandWeapon || rightWeapon && rightWeapon.twoHandWeapon)
                    ClientRemoveRightWeapon();
            }

            // equip the object
            base.EquipMeleeWeapon(collectable);
        }
        #endregion
        #endregion

        #region Enabling Syncing
        protected virtual IEnumerator ReEnableSync(ServerSync ss)
        {
            yield return new WaitForSeconds(0.1f);
            ss.SmoothEnable();
        }
        #endregion
    }
}
