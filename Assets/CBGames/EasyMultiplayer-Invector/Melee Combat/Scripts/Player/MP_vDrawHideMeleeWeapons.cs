using EMI.Player;

namespace Invector
{
    public class MP_vDrawHideMeleeWeapons : vDrawHideMeleeWeapons
    {
        #region Properties
        protected MeleeNetworkCalls nc = null;
        #endregion

        #region Initialization
        protected override void Start()
        {
            nc = GetComponent<MeleeNetworkCalls>();
            hideAndDrawWeaponsInput.SetNetworkCalls(nc, "DrawHideMeleeWeaponsInput");
        }
        #endregion
    }
}
