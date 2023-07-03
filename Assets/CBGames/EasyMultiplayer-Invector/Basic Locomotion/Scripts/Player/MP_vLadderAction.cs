using EMI.Object;
using EMI.Player;
using Mirror;
using UnityEngine;

namespace Invector.vCharacterController.vActions
{
    public class MP_vLadderAction : vLadderAction
    {
        #region Properties
        protected BasicNetworkCalls nc;
        #endregion

        #region Initialization
        protected override void Awake()
        {
            nc = GetComponent<BasicNetworkCalls>();
            
            base.Awake();
        }
        protected override void Start()
        {
            verticallInput.SetNetworkCalls(nc, "VerticalInput");
            enterInput.SetNetworkCalls(nc, "EnterInput");
            exitInput.SetNetworkCalls(nc, "ExitInput");
            fastClimbInput.SetNetworkCalls(nc, "FastClimbInput");
            slideDownInput.SetNetworkCalls(nc, "SlideDownInput");

            base.Start();
        }
        #endregion

        #region Triggers
        #region Reset Player Settings
        /// <summary>
        /// Only allow the server to initiate a reset for all clients
        /// </summary>
        public override void ResetPlayerSettings()
        {
            if (NetworkServer.active || (!NetworkServer.active && !NetworkClient.active))
            {
                if (nc.onLadder == true)
                    nc.onLadder = false; // make sure new joining clients don't see this client as on a ladder
                nc.Rpc_LadderResetPlayerSettings(); // tell other clients to reset
                base.ResetPlayerSettings();
            }
        }

        /// <summary>
        /// Reset in response to the server initiating a reset
        /// </summary>
        [Client]
        public virtual void Server_ResetPlayerSettings()
        {
            tpInput.cc.animator.speed = 1; // if you don't do this here you could get desynced on the animation speed
            base.ResetPlayerSettings();
            transform.position = GetComponent<ServerSync>().GetLastServerPosition();
            transform.rotation = GetComponent<ServerSync>().GetLastServerRotation();
        }
        #endregion

        #region Enter Ladder
        /// <summary>
        /// Only allow the server to initiate a ladder climb
        /// </summary>
        protected override void TriggerEnterLadder()
        {
            if (NetworkServer.active)
            {
                nc.onLadder = true;
                base.TriggerEnterLadder();
            }
        }

        /// <summary>
        /// This will be triggered when the server initiates a ladder climb
        /// </summary>
        [Client]
        public virtual void EnterLadder()
        {
            GetComponent<ServerSync>().Enable(false);
            if (NetworkClient.active && targetLadderAction)
            {
                base.TriggerEnterLadder();
            }
        }
        #endregion

        #region Exit Ladder
        /// <summary>
        /// Only the server can initate a ladder exit
        /// </summary>
        protected override void TriggerExitLadder()
        {
            if (NetworkServer.active)
            {
                // exit ladder behaviour
                inExitingLadderAnimation = tpInput.cc.baseLayerInfo.IsName("ExitLadderTop") || tpInput.cc.baseLayerInfo.IsName("ExitLadderBottom") || tpInput.cc.baseLayerInfo.IsName("QuickExitLadder");

                if (inExitingLadderAnimation)
                {
                    nc.onLadder = false;
                    tpInput.cc.animator.speed = 1;

                    if (currentLadderAction.exitMatchTarget != null && !tpInput.cc.baseLayerInfo.IsName("QuickExitLadder"))
                    {
                        if (debugMode)
                        {
                            Debug.Log("Match Target to exit..." + currentLadderAction.name + "_" + currentLadderAction.transform.parent.gameObject.name);
                        }

                        EvaluateToPosition(currentLadderAction.exitPositionXZCurve, currentLadderAction.exitPositionYCurve, currentLadderAction.exitMatchTarget.position, tpInput.cc.baseLayerInfo.normalizedTime);
                    }
                    var newRot = new Vector3(0, tpInput.animator.rootRotation.eulerAngles.y, 0);
                    EvaluateToRotation(currentLadderAction.exitRotationCurve, Quaternion.Euler(newRot), tpInput.cc.baseLayerInfo.normalizedTime);

                    if (tpInput.cc.baseLayerInfo.normalizedTime >= 0.8f)
                    {
                        // after playing the animation we reset some values
                        ResetPlayerSettings();
                    }
                }
            }
        }

        /// <summary>
        /// This is called in response to the server initiating a ladder exit
        /// </summary>
        [Client]
        public virtual void ExitLadder()
        {
            GetComponent<ServerSync>().Enable(true);
            if (NetworkClient.active && tpInput && tpInput.cc)
            {
                base.TriggerExitLadder();
            }
        }
        #endregion
        #endregion

        #region Inputs
        /// <summary>
        /// Only the server can take actions on inputs. The owner player will take actions
        /// based on what the server tells them to do (based on SyncVars & RPCs)
        /// </summary>
        protected override void EnterLadderInput()
        {
            if (NetworkServer.active)
            {
                base.EnterLadderInput();
            }
            else
            {
                enterInput.GetButtonDown(); // just send your inputs over the network, since you're the owner
            }
        }
        #endregion

        #region Visual Icons
        /// <summary>
        /// This makes it so the visual icons will only display for your player and not none owned players
        /// </summary>
        protected override void AddLadderTrigger(vTriggerLadderAction _ladderAction)
        {
            if (targetLadderAction != _ladderAction)
            {
                targetLadderAction = _ladderAction;
                if (debugMode)
                {
                    Debug.Log("TriggerStay " + targetLadderAction.name + "_" + targetLadderAction.transform.parent.gameObject.name);
                }
            }

            if (!actionTriggers.Contains(targetLadderAction))
            {
                actionTriggers.Add(targetLadderAction);
                if (nc == null || (nc != null && nc.hasAuthority)) 
                    targetLadderAction.OnPlayerEnter.Invoke();
            }
        }

        /// <summary>
        /// This makes it so the visual icons will only display for your player and not none owned players
        /// </summary>
        protected override void RemoveLadderTrigger(vTriggerLadderAction _ladderAction)
        {
            if (_ladderAction == targetLadderAction)
            {
                targetLadderAction = null;
            }

            if (actionTriggers.Contains(_ladderAction))
            {
                actionTriggers.Remove(_ladderAction);
                if (nc == null || (nc != null && nc.hasAuthority)) _ladderAction.OnPlayerExit.Invoke();
            }
        }
        #endregion
    }
}
