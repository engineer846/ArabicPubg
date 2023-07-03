using EMI.Object;
using EMI.Player;
using EMI.Utils;
using Invector;
using Invector.vCharacterController;
using Invector.vCharacterController.AI;
using Invector.vItemManager;
using Invector.vMelee;
using Invector.vShooter;
using Mirror;
using UnityEditor;
using UnityEngine;

namespace EMI.Menus
{
    public partial class ObjectConverter
    {
        partial void ConvertShooterAI(GameObject target) { }
        partial void ConvertShooterObject(GameObject target)
        {
            ReplaceComponent(target, typeof(UseItemEventTrigger), typeof(MP_UseItemEventTrigger));
            ReplaceComponent(target, typeof(vExplosive), typeof(MP_vExplosive));
            ReplaceComponent(target, typeof(vShooterEquipment), typeof(MP_vShooterEquipment));
            ReplaceComponent(target, typeof(vWeaponConstrain), typeof(MP_vWeaponConstrain));

            if (!target.GetComponent<vShooterEquipment>() && !target.GetComponent<vMeleeEquipment>() && !target.GetComponent<vMeleeWeapon>())
            {
                if (target.GetComponent<vShooterWeapon>() && !target.GetComponentInParent<vThirdPersonController>() && !target.GetComponentInParent<vSimpleMeleeAI_Controller>())
                {
                    ReplaceComponent(target, typeof(vShooterWeapon), typeof(MP_vShooterWeapon));
                    if (!target.GetComponent<vShooterWeaponNetworkCalls>())
                    {
                        target.AddComponent<vShooterWeaponNetworkCalls>();
                        target.GetComponent<vShooterWeaponNetworkCalls>().syncInterval = 0;
                    }
                    if (!target.GetComponent<ServerSync>())
                    {
                        target.AddComponent<ServerSync>();
                        ServerSync ss = target.GetComponent<ServerSync>();
                        ss.GetType().GetField("interpolation", EditorUtils.allBindingValues).SetValue(ss, 0.223f);
                        ss.GetType().GetField("forceSnapDistance", EditorUtils.allBindingValues).SetValue(ss, 1f);
                        ss.syncInterval = 0;
                    }
                    if (!target.GetComponent<NetworkIdentity>())
                    {
                        target.AddComponent<NetworkIdentity>();
                    }
                }
            }
            if (target.GetComponentInChildren<vThrowManager>(true))
            {
                GameObject childTarget = target.GetComponentInChildren<vThrowManager>(true).gameObject;
                if (PrefabUtility.GetPrefabInstanceStatus(childTarget) != PrefabInstanceStatus.NotAPrefab)
                {
                    GameObject instanceRoot = PrefabUtility.GetNearestPrefabInstanceRoot(childTarget);
                    PrefabUtility.UnpackPrefabInstance(instanceRoot, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                }
                ReplaceComponent(childTarget, typeof(vThrowManager), typeof(MP_vThrowManager));
            }
            ReplaceComponent(target, typeof(vThrowCollectable), typeof(MP_vThrowCollectable));
            if (target.GetComponent<vAmmoStandalone>() && !target.GetComponent<NetworkIdentity>())
            {
                target.AddComponent<NetworkIdentity>();
            }
        }
        partial void ConvertShooterCharacter(GameObject target)
        {
            ReplaceComponent(target, typeof(vWeaponConstrain), typeof(MP_vWeaponConstrain));
            ReplaceComponent(target, typeof(vShooterMeleeInput), typeof(MP_vShooterMeleeInput));
            ReplaceComponent(target, typeof(vShooterManager), typeof(MP_vShooterManager));
            ReplaceComponent(target, typeof(vCollectShooterMeleeControl), typeof(MP_vCollectShooterMeleeControl));
            ReplaceComponent(target, typeof(vLockOnShooter), typeof(MP_vLockOnShooter));
            ReplaceComponent(target, typeof(vDrawHideShooterWeapons), typeof(MP_vDrawHideShooterWeapons));
            if (target.GetComponentInChildren<vThrowManager>(true))
            {
                GameObject childTarget = target.GetComponentInChildren<vThrowManager>(true).gameObject;
                ReplaceComponent(childTarget, typeof(vThrowManager), typeof(MP_vThrowManager));
            }
            ReplaceComponent(target, typeof(vExplosive), typeof(MP_vExplosive));
            if (target.GetComponent<MP_vShooterMeleeInput>() ||
                target.GetComponent<MP_vShooterManager>() ||
                target.GetComponent<MP_vCollectShooterMeleeControl>() ||
                target.GetComponent<MP_vLockOnShooter>() ||
                target.GetComponent<MP_vDrawHideShooterWeapons>() ||
                target.GetComponentInChildren<MP_vThrowManager>(true))
            {
                if (!target.GetComponent<ShooterNetworkCalls>())
                {
                    target.AddComponent<ShooterNetworkCalls>();
                }
                target.GetComponent<ShooterNetworkCalls>().syncInterval = 0;
            }
        }
    }
}
