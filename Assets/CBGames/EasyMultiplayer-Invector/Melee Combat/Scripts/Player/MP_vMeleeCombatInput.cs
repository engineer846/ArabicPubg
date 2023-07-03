using EMI.Player;
using Mirror;
using UnityEngine;

namespace Invector.vCharacterController
{
    [RequireComponent(typeof(MeleeNetworkCalls))]
    public class MP_vMeleeCombatInput : vMeleeCombatInput
    {
        #region Properties
        protected MeleeNetworkCalls nc = null;
        [HideInInspector] public bool allowClientCameraControl = false;
        
        [Header("Custom Exposed Inputs")]
        public GenericInput walkToggle = new GenericInput("CapsLock", "", "");
        protected bool finishedInitializing = false;

        public override Camera cameraMain
        {
            get
            {
                if (nc != null && (!nc.hasAuthority || (NetworkServer.active && !NetworkClient.active))) // networked player
                {
                    if (nc.clientCam != null)
                    {
                        cc.rotateTarget = nc.clientCam.transform;
                    }
                }
                else if (!_cameraMain && !withoutMainCamera) // owning player
                {
                    if (!Camera.main)
                    {
                        Debug.Log("Missing a Camera with the tag MainCamera, please add one.");
                        withoutMainCamera = true;
                    }
                    else
                    {
                        if (_cameraMain == null && (nc == null || (nc != null && nc.hasAuthority)))
                            _cameraMain = Camera.main;

                        if (_cameraMain)
                        {
                            cc.rotateTarget = _cameraMain.transform;
                        }
                    }
                }
                return _cameraMain;
            }
            set
            {
                _cameraMain = value;
            }
        }

        [HideInInspector]
        public override bool lockInput
        {
            get
            {
                return _lockInput;
            }
            set
            {
                _lockInput = value;
                if (nc.hasAuthority)
                {
                    if (NetworkServer.active)
                        nc.lockInput = value;
                    else
                        nc.Cmd_LockInput(value);
                }
            }
        }
        #endregion

        #region Initialization
        protected virtual void Awake()
        {
            nc = GetComponent<MeleeNetworkCalls>();
            cc = GetComponent<MP_vThirdPersonController>(); //To prevent errors with it finding a character controller.
            walkToggle.keyboard = toggleWalk.ToString();
        }
        protected override void Start()
        {
            // Send keyboard presses over the network (this also sets up receiving them if networked player)
            horizontalInput.SetNetworkCalls(nc, "HorizontalInput");
            verticallInput.SetNetworkCalls(nc, "VerticalInput");
            sprintInput.SetNetworkCalls(nc, "SprintInput", VerifySprinting);
            crouchInput.SetNetworkCalls(nc, "CrouchInput", VerifyCrouching);
            strafeInput.SetNetworkCalls(nc, "StrafeInput");
            jumpInput.SetNetworkCalls(nc, "JumpInput");
            rollInput.SetNetworkCalls(nc, "RollInput");
            walkToggle.SetNetworkCalls(nc, "WalkToggle", VerifyWalk);

            // Camera Movements
            rotateCameraXInput.SetNetworkCalls(nc, "RotateCameraXInput");
            rotateCameraYInput.SetNetworkCalls(nc, "RotateCameraYInput");
            cameraZoomInput.SetNetworkCalls(nc, "CameraZoomInput");

            // Melee Inputs
            weakAttackInput.SetNetworkCalls(nc, "WeakAttackInput");
            strongAttackInput.SetNetworkCalls(nc, "StrongAttackInput");
            blockInput.SetNetworkCalls(nc, "BlockInput");

            base.Start();

            finishedInitializing = true;
        }
        #endregion

        #region Camera
        protected virtual void OnDestroy()
        {
            if (nc.clientCam != null) Destroy(nc.clientCam);
        }
        public override void FindHUD()
        {
            if (nc == null || (nc != null && nc.hasAuthority))
                base.FindHUD();
        }
        public override void FindCamera()
        {
            if (nc == null || (nc != null && nc.hasAuthority && NetworkClient.active))
            {
                base.FindCamera();
            }
        }

        public override void CameraInput()
        {
            if (NetworkClient.active && nc != null && nc.hasAuthority || (!nc.hasAuthority && allowClientCameraControl))
            {
                base.CameraInput();
            }
        }
        public override void UpdateCameraStates()
        {
            if (NetworkClient.active && nc != null && nc.hasAuthority || (!nc.hasAuthority && allowClientCameraControl))
            {
                base.UpdateCameraStates();
            }
        }
        public override void ChangeCameraState(string cameraState, bool useLerp = true)
        {
            if (NetworkClient.active && nc != null && nc.hasAuthority || (!nc.hasAuthority && allowClientCameraControl))
            {
                base.ChangeCameraState(cameraState, useLerp);
            }
        }
        public override void ResetCameraAngle()
        {
            if (NetworkClient.active && nc != null && nc.hasAuthority || (!nc.hasAuthority && allowClientCameraControl))
            {
                base.ResetCameraAngle();
            }
        }
        public override void ChangeCameraStateWithLerp(string cameraState)
        {
            if (NetworkClient.active && nc != null && nc.hasAuthority || (!nc.hasAuthority && allowClientCameraControl))
            {
                base.ChangeCameraStateWithLerp(cameraState);
            }
        }
        public override void ChangeCameraStateNoLerp(string cameraState)
        {
            if (NetworkClient.active && nc != null && nc.hasAuthority || (!nc.hasAuthority && allowClientCameraControl))
            {
                base.ChangeCameraStateNoLerp(cameraState);
            }
        }
        public override void ResetCameraState()
        {
            if (NetworkClient.active && nc != null && nc.hasAuthority || (!nc.hasAuthority && allowClientCameraControl))
            {
                base.ResetCameraState();
            }
        }
        #endregion

        #region Rotation
        public override void ControlRotation()
        {
            if ((nc == null || (nc != null && nc.hasAuthority) && cameraMain && !lockUpdateMoveDirection) ||
                nc != null && !nc.hasAuthority && nc.clientCam && !lockUpdateMoveDirection)
            {
                if (!cc.keepDirection)
                {
                    if ((nc == null || (nc != null && nc.hasAuthority && NetworkClient.active)) || (!NetworkClient.active && !NetworkServer.active))
                    {
                        cc.UpdateMoveDirection(cameraMain.transform);
                    }
                    else if (nc != null && !nc.hasAuthority && NetworkClient.active || NetworkServer.active)
                    {
                        if (nc.clientCam) // sometimes this initalizes faster than the camera can be made so we have to have this check to prevent crashes
                            cc.UpdateMoveDirection(nc.clientCam.transform);
                    }
                }
            }

            if ((nc == null || (nc != null && nc.hasAuthority)) && tpCamera != null && tpCamera.lockTarget && cc.isStrafing)
            {
                cc.RotateToPosition(tpCamera.lockTarget.position);          // rotate the character to a specific target
            }
            else if (nc != null && !nc.hasAuthority && nc.clientCam != null && nc.clientCam.GetComponent<ClientCamera>().lockTarget && cc.isStrafing)
            {
                cc.RotateToPosition(nc.clientCam.GetComponent<ClientCamera>().lockTarget.position); // rotate the character to a specific target that this client sees
            }
            else
            {
                cc.ControlRotationType();                                   // handle the controller rotation type (strafe or free)
            }
        }
        #endregion

        #region HUD
        public override void UpdateHUD()
        {
            if (NetworkClient.active && nc.hasAuthority)
            {
                base.UpdateHUD();
            }
        }
        #endregion

        #region Walking
        public virtual bool IsWalking()
        {
            return cc.alwaysWalkByDefault;
        }
        public override void MoveInput()
        {
            if (!lockMoveInput)
            {
                // gets input
                cc.input.x = horizontalInput.GetAxisRaw();
                cc.input.z = verticallInput.GetAxisRaw();
            }

            if (walkToggle.GetButtonDown())
            {
                cc.alwaysWalkByDefault = !cc.alwaysWalkByDefault;
            }

            cc.ControlKeepDirection();
        }
        #endregion

        #region Buttons
        public override void SetLockAllInput(bool value)
        {
            base.SetLockAllInput(value);
            if (NetworkClient.active && nc && nc.hasAuthority)
            {
                nc.Cmd_LockAllInput(value);
            }
        }
        #endregion

        #region Button Callbacks
        /// <summary>
        /// Callable ONLY by the server.
        /// This is meant to be a callback function that will make sure all the clients 
        /// are in the correct crouching state. Sometimes you can spam the crouch button
        /// and some key presses are lost which results in a desync with the server. This
        /// verifys that the last crouch state ALWAYS matches the server.
        /// </summary>
        [ServerCallback]
        public virtual void VerifyCrouching()
        {
            #if UNITY_SERVER || UNITY_EDITOR
            if (NetworkServer.active && finishedInitializing)
                nc.isCrouching = GetComponent<vThirdPersonController>().isCrouching;
            #endif
        }

        /// <summary>
        /// Callable ONLY by the server.
        /// This is meant ot be a callback function that will make sure all the clients
        /// are in the correct sprinting state. Sometimes you can spam the sprint button
        /// and some key presses are lost which results in a desync with the server. This
        /// verifies that the last sprint state ALWAYS matches the server.
        /// </summary>
        [ServerCallback]
        public virtual void VerifySprinting()
        {
            #if UNITY_SERVER || UNITY_EDITOR
            if (NetworkServer.active && finishedInitializing)
                nc.isSprinting = GetComponent<vThirdPersonController>().isSprinting;
            #endif
        }

        /// <summary>
        /// Callable ONLY by the server.
        /// This is meant ot be a callback function that will make sure all the clients
        /// are in the correct walk state. Sometimes you can spam the walk button
        /// and some key presses are lost which results in a desync with the server. This
        /// verifies that the last walk state ALWAYS matches the server.
        /// </summary>
        [ServerCallback]
        public virtual void VerifyWalk()
        {
            #if UNITY_SERVER || UNITY_EDITOR
            if (NetworkServer.active && finishedInitializing)
                nc.isWalking = cc.alwaysWalkByDefault;
            #endif
        }
        #endregion
    }
}
