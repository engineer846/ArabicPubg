using EMI.Player;
using Invector.vShooter;
using Mirror;
using System;
using UnityEngine;

namespace Invector.vCharacterController
{
    public class MP_vShooterMeleeInput : vShooterMeleeInput
    {
        #region Properties
        protected MP_vShooterManager sm = null;
        protected ShooterNetworkCalls nc = null;
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

        public override vControlAimCanvas controlAimCanvas
        {
            get
            {
                if ((!NetworkServer.active && !NetworkClient.active) || nc == null || (nc && nc.hasAuthority && NetworkClient.active))
                {
                    if (!_controlAimCanvas)
                    {
                        _controlAimCanvas = FindObjectOfType<vControlAimCanvas>();
                        if (_controlAimCanvas)
                        {
                            _controlAimCanvas.Init(cc);
                        }
                    }
                }

                return _controlAimCanvas;
            }
        }
        protected override bool isUsingScopeView 
        { 
            get 
            {
                if ((NetworkClient.active && nc && !nc.hasAuthority) || (NetworkServer.active && !NetworkClient.active))
                {
                    return nc.isUsingScopeView;
                }
                else
                {
                    return _isUsingScopeView;
                }
            } 
            set 
            { 
                _isUsingScopeView = value;
                #if UNITY_SERVER || UNITY_EDITOR
                if (NetworkClient.active && nc && nc.hasAuthority) // only the owning client knows if they're looking through the scope
                {
                    nc.Cmd_SetIsUsingScopeView(Convert.ToByte(value));
                }
                #endif
            } 
        }

        protected override Vector3 targetArmAligmentDirection
        {
            get
            {
                Transform t;
                if ((!NetworkClient.active && !NetworkServer.active) || nc == null || (NetworkClient.active && nc.hasAuthority))
                {
                    t = controlAimCanvas && controlAimCanvas.isScopeCameraActive && controlAimCanvas.scopeBackgroundCamera ? controlAimCanvas.scopeBackgroundCamera.transform : cameraMain.transform;
                }
                else
                {
                    t = nc.isScopeCameraActive && nc.clientScopeCamera ? nc.clientScopeCamera.transform : nc.clientCam.transform;
                }
                
                return t.forward;
            }
        }
        #endregion

        #region Initialization
        protected virtual void Awake()
        {
            nc = GetComponent<ShooterNetworkCalls>();
            sm = GetComponent<MP_vShooterManager>();
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

            // Shooter Inputs
            aimInput.SetNetworkCalls(nc, "AimInput");
            shotInput.SetNetworkCalls(nc, "ShotInput");
            reloadInput.SetNetworkCalls(nc, "ReloadInput", VerifyReload);
            switchCameraSideInput.SetNetworkCalls(nc, "SwitchCameraSideInput");
            scopeViewInput.SetNetworkCalls(nc, "ScopeViewInput");

            base.Start();

            if (!controlAimCanvas && NetworkClient.active && nc && nc.hasAuthority)
            {
                Debug.LogWarning("Missing the AimCanvas, drag and drop the prefab to this scene in order to Aim", gameObject);
            }

            finishedInitializing = true;
        }
        #endregion

        #region ShooterMeleeInput Overrides
        #region Buttons
        public override void SetLockAllInput(bool value)
        {
            base.SetLockAllInput(value);
            if (!NetworkServer.active && NetworkClient.active && nc && nc.hasAuthority)
            {
                nc.Cmd_LockAllInput(value);
            }
        }
        #endregion

        #region Aim Positioning
        protected override void CheckAimConditions()
        {
            if (!shooterManager)
            {
                return;
            }

            int weaponSide = 0;
            if ((!NetworkClient.active && !NetworkServer.active) || nc == null || (nc && nc.hasAuthority && NetworkClient.active))
            {
                weaponSide = tpCamera.switchRight < 0 ? -1 : 1;
            }
            else
            {
                weaponSide = nc.switchRight < 0 ? -1 : 1;
            }

            if (CurrentActiveWeapon == null)
            {
                aimConditions = false;
                return;
            }

            if (shooterManager.useCheckAim == false || !IsAiming)
            {
                aimConditions = true;
                return;
            }
            if (animator.IsInTransition(0)) return;
            Vector3 startPoint = Vector3.zero;
            Vector3 endPoint = Vector3.zero;

            UpdateCheckAimPoints(ref startPoint, ref endPoint);
            if (Vector3.Distance(startPoint, AimPosition) < Vector3.Distance(startPoint, endPoint))
                aimConditions = false;
            var _ray = new Ray(startPoint, ((endPoint) - startPoint).normalized);

            if (Physics.SphereCast(_ray, shooterManager.checkAimRadius, out checkCanAimHit, (endPoint - startPoint).magnitude, shooterManager.blockAimLayer))
            {
                aimConditions = false;
            }
            else
            {
                aimConditions = true;
            }

            //aimWeight = Mathf.Lerp(aimWeight, aimConditions ? 1 : 0, 10 * Time.deltaTime);
        }
        protected override void UpdateAimPosition()
        {
            if (!shooterManager)
            {
                return;
            }

            if (CurrentActiveWeapon == null)
            {
                return;
            }

            // Fixed their ternary hell they wrote and turned it into something actually readable. Why would you ever write this - people, don't ever do this...
            //var camT = isUsingScopeView && controlAimCanvas && controlAimCanvas.scopeBackgroundCamera ? //Check if is using canvas scope view
            //        CurrentActiveWeapon.zoomScopeCamera ? /* if true, check if weapon has a zoomScopeCamera, 
            //    if true...*/
            //        CurrentActiveWeapon.zoomScopeCamera.transform : controlAimCanvas.scopeBackgroundCamera.transform :
            //        /*else*/cameraMain.transform;

            Transform camT = null;
            if ((NetworkClient.active && nc && nc.hasAuthority) || nc == null || (!NetworkClient.active && !NetworkServer.active)) // if not networked, or owning player
            {
                if (isUsingScopeView && controlAimCanvas && controlAimCanvas.scopeBackgroundCamera)
                {
                    if (CurrentActiveWeapon.zoomScopeCamera)
                    {
                        camT = CurrentActiveWeapon.zoomScopeCamera.transform;
                        if (nc) nc.UpdateZoomCamera(camT);
                    }
                    else
                    {
                        camT = controlAimCanvas.scopeBackgroundCamera.transform;
                        if (nc) nc.UpdateScopeCamera(camT);
                    }
                }
                else
                {
                    camT = cameraMain.transform;
                }
            }
            else // not owning player and networked.
            {
                if (nc.isUsingScopeView && nc.clientScopeCamera)
                {
                    if (nc.clientZoomScopeCamera)
                    {
                        camT = nc.clientZoomScopeCamera.transform;
                    }
                    else
                    {
                        camT = nc.clientScopeCamera.transform;
                    }
                }
                else
                {
                    camT = nc.clientCam.transform;
                }
            }


            var origin1 = camT.position;
            // Why is this ever needed? Seems like something that should be removed...
            //if (!(controlAimCanvas && controlAimCanvas.isScopeCameraActive && controlAimCanvas.scopeBackgroundCamera))
            //{
            //    origin1 = camT.position;
            //}

            var vOrigin = origin1;
            vOrigin += controlAimCanvas && controlAimCanvas.isScopeCameraActive && controlAimCanvas.scopeBackgroundCamera ? camT.forward : Vector3.zero;
            AimPosition = camT.position + camT.forward * 100f;
            //aimAngleReference.transform.eulerAngles = new Vector3(aimAngleReference.transform.eulerAngles.x, transform.eulerAngles.y, aimAngleReference.transform.eulerAngles.z);
            if (!isUsingScopeView)
            {
                lastAimDistance = 100f;
            }

            if (shooterManager.raycastAimTarget && CurrentActiveWeapon.raycastAimTarget)
            {
                RaycastHit hit;
                Ray ray = new Ray(vOrigin, camT.forward);
                float fcp = ((NetworkClient.active && nc && nc.hasAuthority) || nc == null) ? cameraMain.farClipPlane : nc.clientCamFarClipPlane;
                if (Physics.Raycast(ray, out hit, fcp, shooterManager.damageLayer))
                {
                    if (hit.collider.transform.IsChildOf(transform))
                    {
                        var collider = hit.collider;
                        var hits = Physics.RaycastAll(ray, fcp, shooterManager.damageLayer);
                        float dist = fcp;
                        for (int i = 0; i < hits.Length; i++)
                        {
                            if (hits[i].distance < dist && hits[i].collider.gameObject != collider.gameObject && !hits[i].collider.transform.IsChildOf(transform))
                            {
                                dist = hits[i].distance;
                                hit = hits[i];
                            }
                        }
                    }

                    if (hit.collider)
                    {
                        if (!isUsingScopeView)
                        {
                            lastAimDistance = Vector3.Distance(camT.position, hit.point);
                        }

                        AimPosition = hit.point;
                    }
                }
                if (shooterManager.showCheckAimGizmos)
                {
                    Debug.DrawLine(ray.origin, AimPosition);
                }
            }
            if (isAimingByInput && ((!NetworkClient.active && !NetworkServer.active) || nc == null || (NetworkClient.active && nc && nc.hasAuthority)))
            {
                shooterManager.CameraSway();
            }
        }
        protected override void UpdateCheckAimPoints(ref Vector3 start, ref Vector3 end)
        {
            if (CurrentActiveWeapon)
            {
                float checkAimSmooth = shooterManager.checkAimOffsetSmooth;
                ///Lerp offsets 
                checkCanAimOffsetStartX = Mathf.Lerp(checkCanAimOffsetStartX, IsCrouching ? shooterManager.checkAimCrouchedOffsetStartX : shooterManager.checkAimStandingOffsetStartX, checkAimSmooth * Time.deltaTime);
                checkCanAimOffsetStartY = Mathf.Lerp(checkCanAimOffsetStartY, IsCrouching ? shooterManager.checkAimCrouchedOffsetStartY : shooterManager.checkAimStandingOffsetStartY, checkAimSmooth * Time.deltaTime);
                checkCanAimOffsetEndX = Mathf.Lerp(checkCanAimOffsetEndX, IsCrouching ? shooterManager.checkAimCrouchedOffsetEndX : shooterManager.checkAimStandingOffsetEndX, checkAimSmooth * Time.deltaTime);
                checkCanAimOffsetEndY = Mathf.Lerp(checkCanAimOffsetEndY, IsCrouching ? shooterManager.checkAimCrouchedOffsetEndY : shooterManager.checkAimStandingOffsetEndY, checkAimSmooth * Time.deltaTime);

                /// Make original points to check aim 
                Vector3 startPoint = aimAngleReference.transform.TransformPoint(upperArmPosition);
                Vector3 endPoint = aimAngleReference.transform.TransformPoint(muzzlePosition);
                Vector3 forward = aimAngleReference.transform.InverseTransformDirection(muzzleForward);

                ///Apply offsets
                Vector3 newStartPoint = Vector3.zero;
                Vector3 newEndPoint = Vector3.zero;
                if ((!NetworkClient.active && !NetworkServer.active) || nc == null || (NetworkClient.active && nc.hasAuthority))
                {
                    newStartPoint = startPoint + cameraMain.transform.right * (checkCanAimOffsetStartX * (tpCamera.switchRight > 0 ? 1 : -1)) + cameraMain.transform.up * checkCanAimOffsetStartY;
                    newEndPoint = endPoint + cameraMain.transform.right * (checkCanAimOffsetEndX * (tpCamera.switchRight > 0 ? 1 : -1)) + cameraMain.transform.up * checkCanAimOffsetEndY + forward * shooterManager.checkAimOffsetZ;
                }
                else
                {
                    newStartPoint = startPoint + nc.clientCam.transform.right * (checkCanAimOffsetStartX * (nc.switchRight > 0 ? 1 : -1)) + nc.clientCam.transform.up * checkCanAimOffsetStartY;
                    newEndPoint = endPoint + nc.clientCam.transform.right * (checkCanAimOffsetEndX * (nc.switchRight > 0 ? 1 : -1)) + nc.clientCam.transform.up * checkCanAimOffsetEndY + forward * shooterManager.checkAimOffsetZ;
                }

                start = newStartPoint;
                end = newEndPoint;
            }
        }
        #endregion

        #region Camera
        public override void ScopeViewInput()
        {
            if ((!NetworkServer.active && !NetworkClient.active) || nc == null || (nc && nc.hasAuthority && NetworkClient.active))
                base.ScopeViewInput();
        }
        public override void EnableScopeView()
        {
            if ((!NetworkServer.active && !NetworkClient.active) || nc == null || (nc && nc.hasAuthority && NetworkClient.active))
                base.EnableScopeView();
        }
        public override void DisableScopeView()
        {
            if ((!NetworkServer.active && !NetworkClient.active) || nc == null || (nc && nc.hasAuthority && NetworkClient.active))
                base.DisableScopeView();
        }
        #endregion

        #region Head Positioning
        protected override void UpdateHeadTrackLookPoint()
        {
            if ((!NetworkClient.active && !NetworkServer.active) || nc == null || (NetworkClient.active && nc.hasAuthority))
            {
                base.UpdateHeadTrackLookPoint();
            }
            else
            {
                if (IsAiming && !isUsingScopeView)
                {
                    headTrack.SetTemporaryLookPoint(nc.clientCam.transform.position + nc.clientCam.transform.forward * 10, 0.1f);
                }
            }
        }
        #endregion
        #endregion

        #region MeleeCombatInput Overrides
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

        #region Button Callbacks
        /// <summary>
        /// Callable ONLY by the server.
        /// This is meant to be a callback function that will make sure all the clients 
        /// are in the correct reloading state. Sometimes you can spam the reload button
        /// and some key presses are lost which results in a desync with the server. This
        /// verifys that the last reloading state ALWAYS matches the server.
        /// </summary>
        [ServerCallback]
        protected virtual void VerifyReload()
        {
            #if UNITY_SERVER || UNITY_EDITOR
            if (NetworkServer.active && sm.isReloadingWeapon != (bool)nc.GetValue("ReloadInput", typeof(bool)))
                nc.UpdateInput("ReloadInput", sm.isReloadingWeapon);
            #endif
        }

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

        #endregion
    }
}
