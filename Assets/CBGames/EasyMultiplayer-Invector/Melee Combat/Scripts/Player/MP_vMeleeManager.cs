using EMI.Player;
using EMI.Utils;
using Invector.vItemManager;
using Mirror;
using System.Collections.Generic;
using UnityEngine;

namespace Invector.vMelee
{
    public class MP_vMeleeManager : vMeleeManager
    {
        #region Properties
        protected MeleeNetworkCalls nc = null;
        protected MP_vItemManager im = null;
        #endregion

        #region Initialization
        protected virtual void Awake()
        {
            nc = GetComponent<MeleeNetworkCalls>();
            im = GetComponent<MP_vItemManager>();
        }
        #endregion

        //#region Setting Weapons
        ///// <summary>
        ///// Having the server send this across the network guarantees that a players weapon stance will 
        ///// always match the servers. So if a player desyncs with the server they will re-sync on the 
        ///// next update that they receive from the server (roughly every 10ms)
        ///// </summary>
        
        //public override void SetLeftWeapon(GameObject weaponObject)
        //{
        //    if (NetworkServer.active && GetComponent<vCollectMeleeControl>())
        //    {
        //        if (weaponObject != null)
        //        {
        //            NetworkIdentity ni = (NetworkIdentity)weaponObject.FindComponent(typeof(NetworkIdentity));
        //            if (!ni)
        //            {
        //                Debug.LogError(weaponObject + ": All equippable objects MUST have a NetworkIdentity on them. This object does not.");
        //            }
        //            else
        //            {
        //                nc.setleftWeapon = ni.netId;
        //            }
        //        }
        //        else
        //        {
        //            nc.setleftWeapon = 0;
        //        }
        //    }
        //    base.SetLeftWeapon(weaponObject);
        //    if (nc.hasAuthority)
        //    {
        //        if (weaponObject == null)
        //        {
        //            if (NetworkServer.active)
        //            {
        //                nc.MMleftWeapon = -1;
        //                if (!NetworkClient.active)
        //                    StartCoroutine(nc.VerifyMMLeftWeapon(nc.MMleftWeapon));
        //            }
        //            else if (NetworkClient.active)
        //                nc.Cmd_SetMMWeapon(-1, false);
        //        }
        //        else
        //        {
        //            if (NetworkServer.active)
        //            {
        //                nc.MMleftWeapon = weaponObject.GetComponent<vMeleeEquipment>().referenceItem.id;
        //                if (!NetworkClient.active)
        //                    StartCoroutine(nc.VerifyMMLeftWeapon(nc.MMleftWeapon));
        //            }
        //            else if (NetworkClient.active)
        //                nc.Cmd_SetMMWeapon(weaponObject.GetComponent<vMeleeEquipment>().referenceItem.id, false);
        //        }
        //    }
        //}
        //public override void SetRightWeapon(GameObject weaponObject)
        //{
        //    if (NetworkServer.active && GetComponent<vCollectMeleeControl>())
        //    {
        //        if (weaponObject != null)
        //        {
        //            NetworkIdentity ni = (NetworkIdentity)weaponObject.FindComponent(typeof(NetworkIdentity));
        //            if (!ni)
        //            {
        //                Debug.LogError(weaponObject + ": All equippable objects MUST have a NetworkIdentity on them. This object does not.");
        //            }
        //            else
        //            {
        //                nc.setrightWeapon = ni.netId;
        //            }
        //        }
        //        else
        //        {
        //            nc.setrightWeapon = 0;
        //        }
        //    }
        //    base.SetRightWeapon(weaponObject);
        //    if (nc.hasAuthority)
        //    {
        //        if (weaponObject == null)
        //        {
        //            if (NetworkServer.active)
        //            {
        //                nc.MMrightWeapon = -1;
        //                if (!NetworkClient.active)
        //                    StartCoroutine(nc.VerifyMMRightWeapon(nc.MMrightWeapon));
        //            }
        //            else if (NetworkClient.active)
        //                nc.Cmd_SetMMWeapon(-1, true);
        //        }
        //        else
        //        {
        //            if (NetworkServer.active)
        //            {
        //                nc.MMrightWeapon = weaponObject.GetComponent<vMeleeEquipment>().referenceItem.id;
        //                if (!NetworkClient.active)
        //                    StartCoroutine(nc.VerifyMMRightWeapon(nc.MMrightWeapon));
        //            }
        //            else if (NetworkClient.active)
        //                nc.Cmd_SetMMWeapon(weaponObject.GetComponent<vMeleeEquipment>().referenceItem.id, true);
        //        }
        //    }
        //}
        //public override void SetLeftWeapon(vMeleeWeapon weapon)
        //{
        //    if (NetworkServer.active && GetComponent<vCollectMeleeControl>())
        //    {
        //        if (weapon != null && GetComponent<vCollectMeleeControl>())
        //        {
        //            NetworkIdentity ni = (NetworkIdentity)weapon.gameObject.FindComponent(typeof(NetworkIdentity));
        //            if (!ni)
        //            {
        //                Debug.LogError(weapon + ": All equippable objects MUST have a NetworkIdentity on them. This object does not.");
        //            }
        //            else
        //            {
        //                nc.setleftWeapon = ni.netId;
        //            }
        //        }
        //        else
        //        {
        //            nc.setrightWeapon = 0;
        //        }
                
        //    }
        //    base.SetLeftWeapon(weapon);
        //    if (nc.hasAuthority)
        //    {
        //        if (weapon == null)
        //        {
        //            if (NetworkServer.active)
        //            {
        //                nc.MMleftWeapon = -1;
        //                if (!NetworkClient.active)
        //                    StartCoroutine(nc.VerifyMMLeftWeapon(nc.MMleftWeapon));
        //            }
        //            else if (NetworkClient.active)
        //                nc.Cmd_SetMMWeapon(-1, false);
        //        }
        //        else
        //        {
        //            if (NetworkServer.active)
        //            {
        //                nc.MMleftWeapon = weapon.transform.GetComponent<vMeleeEquipment>().referenceItem.id;
        //                if (!NetworkClient.active)
        //                    StartCoroutine(nc.VerifyMMLeftWeapon(nc.MMleftWeapon));
        //            }
        //            else if (NetworkClient.active)
        //                nc.Cmd_SetMMWeapon(weapon.transform.GetComponent<vMeleeEquipment>().referenceItem.id, false);
        //        }
        //    }
        //}
        //public override void SetRightWeapon(vMeleeWeapon weapon)
        //{
        //    if (NetworkServer.active && GetComponent<vCollectMeleeControl>())
        //    {
        //        if (weapon != null)
        //        {
        //            NetworkIdentity ni = (NetworkIdentity)weapon.gameObject.FindComponent(typeof(NetworkIdentity));
        //            if (!ni)
        //            {
        //                Debug.LogError(weapon + ": All equippable objects MUST have a NetworkIdentity on them. This object does not.");
        //            }
        //            else
        //            {
        //                nc.setrightWeapon = ((NetworkIdentity)weapon.gameObject.FindComponent(typeof(NetworkIdentity))).netId;
        //            }
        //        }
        //        else
        //        {
        //            nc.setrightWeapon = 0;
        //        }
        //    }
        //    base.SetRightWeapon(weapon);
        //    if (nc.hasAuthority)
        //    {
        //        if (weapon == null)
        //        {
        //            if (NetworkServer.active)
        //            {
        //                nc.MMrightWeapon = -1;
        //                if (!NetworkClient.active)
        //                    StartCoroutine(nc.VerifyMMRightWeapon(nc.MMrightWeapon));
        //            }
        //            else if (NetworkClient.active)
        //                nc.Cmd_SetMMWeapon(-1, true);
        //        }
        //        else
        //        {
        //            if (NetworkServer.active)
        //            {
        //                nc.MMrightWeapon = weapon.transform.GetComponent<vMeleeEquipment>().referenceItem.id;
        //                if (!NetworkClient.active)
        //                    StartCoroutine(nc.VerifyMMRightWeapon(nc.MMrightWeapon));
        //            }
        //            else if (NetworkClient.active)
        //                nc.Cmd_SetMMWeapon(weapon.transform.GetComponent<vMeleeEquipment>().referenceItem.id, true);
        //        }
        //    }
        //}
        //#endregion
    }
}
