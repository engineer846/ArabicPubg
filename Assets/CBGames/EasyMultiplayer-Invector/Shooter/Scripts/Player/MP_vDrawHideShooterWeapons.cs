using EMI.Player;

namespace Invector.vShooter
{
    public class MP_vDrawHideShooterWeapons : vDrawHideShooterWeapons
    {
        #region Properties
        protected ShooterNetworkCalls nc = null;
        #endregion

        #region Initialization
        protected override void Start()
        {
            nc = GetComponent<ShooterNetworkCalls>();
            hideAndDrawWeaponsInput.SetNetworkCalls(nc, "DrawHideMeleeWeaponsInput");
        }
        #endregion
    }
}
