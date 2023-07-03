using EMI.Object;
using Mirror;
using UnityEngine;

public class MP_vWeaponConstrain : vWeaponConstrain
{
    #region Properties
    vMeleeWeaponNewtorkCalls nc = null;
    #endregion

    #region Initilization
    protected override void Start()
    {
        nc = GetComponent<vMeleeWeaponNewtorkCalls>();
        base.Start();
    }
    #endregion

    #region Overrides
    public override void Inv_Weapon_FreezeAll(bool status)
    {
        if (NetworkServer.active)
            nc.frozen = status;
        base.Inv_Weapon_FreezeAll(status);
    }
    #endregion
}
