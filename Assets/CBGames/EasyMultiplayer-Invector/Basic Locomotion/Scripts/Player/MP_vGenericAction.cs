using EMI.Player;

namespace Invector.vCharacterController.vActions
{
    public class MP_vGenericAction : vGenericAction
    {
        #region Properties
        protected BasicNetworkCalls nc = null;
        public override vTriggerGenericAction triggerAction { 
            get 
            {
                return _triggerAction; 
            }
            set 
            {
                if (value == null)
                {
                    nc.UpdateInput("GenericActionInput", false);
                    _triggerAction.actionInput.SetNetworkCalls(null, "GenericActionInput");
                }
                _triggerAction = value; 
                if (_triggerAction != null && (nc == null || nc != _triggerAction.actionInput.nc))
                {
                    _triggerAction.actionInput.SetNetworkCalls(nc, "GenericActionInput");
                }
            } 
        }
        #endregion

        #region Initialization
        protected override void Awake()
        {
            nc = GetComponent<BasicNetworkCalls>();
            base.Awake();
        }
        #endregion

        #region Overrides
        protected override vTriggerGenericAction GetNearAction()
        {
            try
            {
                return base.GetNearAction();
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}
