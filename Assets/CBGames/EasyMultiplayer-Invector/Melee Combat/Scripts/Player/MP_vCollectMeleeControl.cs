using EMI.Object;
using EMI.Player;
using EMI.Utils;
using Invector.vCharacterController.vActions;
using Mirror;
using System.Collections;
using UnityEngine;

namespace Invector.vMelee
{
    public class MP_vCollectMeleeControl : vCollectMeleeControl
    {
        #region Properties
        protected MeleeNetworkCalls nc = null;
        #endregion

        #region Initialization
        protected override void Start()
        {
            nc = GetComponent<MeleeNetworkCalls>();

            // Unequip Inputs
            unequipRightInput.SetNetworkCalls(nc, "UnequipRightInput");
            unequipLeftInput.SetNetworkCalls(nc, "UnequipLeftInput");

            // original code (modified)
            meleeManager = GetComponent<vMeleeManager>();
            if (controlDisplayPrefab && GetComponent<NetworkIdentity>().hasAuthority && NetworkClient.active)
            {
                currentDisplay = Instantiate(controlDisplayPrefab);
            }
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
        protected override void EquipMeleeWeapon(vCollectableStandalone collectable)
        {
            if ((!NetworkServer.active && !NetworkClient.active) || NetworkServer.active)
            {
                // Turn off server syncing to stop this object with fighting positions
                ServerSync ss = (ServerSync)collectable.gameObject.FindComponent(typeof(ServerSync));
                if (ss) ss.Enable(false);

                NetworkIdentity ni = (NetworkIdentity)collectable.gameObject.FindComponent(typeof(NetworkIdentity));
                if (ni) // prevent invalid setup objects from being picked up
                {
                    nc.Rpc_EquipMeleeWeapon(ni.netId); // tell the other clients to equip this weapon
                    base.EquipMeleeWeapon(collectable); // equip this weapon yourself
                }
            }
        }

        [Client]
        public virtual void ClientEquipMeleeWeapon(vCollectableStandalone collectable)
        {
            // Turn off server syncing to stop this object with fighting positions
            ServerSync ss = (ServerSync)collectable.gameObject.FindComponent(typeof(ServerSync));
            if (ss) ss.Enable(false);

            // Remove any previously equipped objects
            var weapon = collectable.weapon.GetComponent<vMeleeWeapon>();
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

        #region Enabling Syncing
        protected virtual IEnumerator ReEnableSync(ServerSync ss)
        {
            yield return new WaitForSeconds(0.1f);
            ss.SmoothEnable();
        }
        #endregion
    }
}
