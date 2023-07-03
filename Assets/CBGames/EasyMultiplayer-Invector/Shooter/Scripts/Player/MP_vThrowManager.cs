using EMI.Managers;
using EMI.Player;
using EMI.Utils;
using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Invector.vCharacterController
{
    /// <summary>
    /// Make throwing animations, rotations, and grenade placement all exact over the network.
    /// </summary>
    public class MP_vThrowManager : vThrowManager
    {
        #region Properties
        protected ShooterNetworkCalls nc = null;
        protected override Vector3 thirdPersonAimPoint
        {
            get
            {
                if ((!NetworkClient.active && !NetworkServer.active) || (NetworkClient.active && nc && nc.hasAuthority) || !nc)
                {
                    return throwStartPoint.position + tpInput.cameraMain.transform.forward * throwMaxForce;
                }
                else
                {
                    return throwStartPoint.position + nc.clientCam.transform.forward * throwMaxForce;
                }
            }
        }
        public override Vector3 aimPoint
        {
            get
            {
                switch (cameraStyle)
                {
                    case CameraStyle.ThirdPerson: return thirdPersonAimPoint;
                    case CameraStyle.TopDown: return topdownAimPoint;
                    case CameraStyle.SideScroll: return sideScrollAimPoint;
                }
                if ((!NetworkClient.active && !NetworkServer.active) || !nc || (NetworkClient.active && nc && nc.hasAuthority))
                {
                    return throwStartPoint.position + tpInput.cameraMain.transform.forward * throwMaxForce;
                }
                else
                {
                    return throwStartPoint.position + nc.clientCam.transform.forward * throwMaxForce;
                }
            }
        }
        public override int currentThrowObject 
        { 
            get 
            {
                if (NetworkServer.active || !nc || (!NetworkServer.active && !NetworkClient.active))
                    return _currentThrowObject;
                else
                    return nc.currentThrowObject;
            }
            set 
            { 
                _currentThrowObject = value;
                #if UNITY_SERVER || UNITY_EDITOR
                if (NetworkServer.active && nc)
                {
                    nc.currentThrowObject = value;
                }
                #endif
            } 
        }
        public override int maxThrowObjects 
        { 
            get 
            {
                if (NetworkServer.active || !nc || (!NetworkServer.active && !NetworkClient.active))
                    return _maxThrowObjects;
                else
                    return nc.maxThrowObjects;
            } 
            set 
            { 
                _maxThrowObjects = value;
                #if UNITY_SERVER || UNITY_EDITOR
                if (NetworkServer.active && nc)
                {
                    nc.maxThrowObjects = value;
                }
                #endif
            }
        }
        bool aimInputState = false;
        #endregion

        #region Initilization
        protected override IEnumerator Start()
        {
            nc = (ShooterNetworkCalls)gameObject.FindComponent(typeof(ShooterNetworkCalls));
            throwInput.SetNetworkCalls(nc, "ThrowManager_ThrowInput");
            aimThrowInput.SetNetworkCalls(nc, "ThrowManager_AimThrowInput", true);
            if (NetworkServer.active && nc)
            {
                nc.currentThrowObject = _currentThrowObject;
                nc.maxThrowObjects = _maxThrowObjects;
            }
            yield return base.Start();
        }
        #endregion

        #region Additions
        /// <summary>
        /// This prevents loops that might be caused by duplicate/late network calls. 
        /// This is made use of by the ShooterNetworkCalls hook.
        /// </summary>
        /// <returns></returns>
        public virtual bool WaitingToThrow()
        {
            return isAiming;
        }

        /// <summary>
        /// This function is used to force a client object to get back in sync with 
        /// the server if for any reason they get desynced.
        /// </summary>
        public virtual void StartThrow()
        {
            isAiming = false;
            isThrowInput = true;
        }
        public virtual bool GetIsAiming()
        {
            return isAiming;
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Make the movement and rotation visible over the network so it doesn't look like a grenade appears 
        /// out of nowhere for people on the other end of the network.
        /// </summary>
        protected override void MoveAndRotate()
        {
            if (isAiming || inThrow)
            {
                tpInput.MoveInput();
                switch (cameraStyle)
                {
                    case CameraStyle.ThirdPerson:
                        Vector3 camDir = Vector3.zero;
                        if ((!NetworkClient.active && !NetworkServer.active) || (NetworkClient.active && nc && nc.hasAuthority) || !nc)
                        {
                            camDir = tpInput.cameraMain.transform.forward;
                        }
                        else
                        {
                            camDir = nc.clientCam.transform.forward;
                        }
                        tpInput.cc.RotateToDirection(camDir);
                        break;
                    case CameraStyle.TopDown:
                        var dir = aimDirection;
                        dir.y = 0;
                        tpInput.cc.RotateToDirection(dir);
                        break;
                    case CameraStyle.SideScroll:
                        ///
                        break;
                }
            }
        }
        
        /// <summary>
        /// Only draw the projectile lines if the owning player is the one initiating the throw
        /// </summary>
        protected override void DrawTrajectory()
        {
            if ((!NetworkServer.active && !NetworkClient.active) || !nc || (NetworkClient.active && nc && nc.hasAuthority))
            {
                base.DrawTrajectory();
            }
        }

        /// <summary>
        /// This is what instantiates and propels the grenade to its destination across the network
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator Launch()
        {
            if (!NetworkClient.active && !NetworkServer.active)
            {
                yield return base.Launch();
            }
            else if (NetworkClient.active && !NetworkServer.active)
            {
                inThrow = false;
                if (currentThrowObject <= 0)
                {
                    objectToThrow = null;
                }
                yield return new WaitForSeconds(exitThrowModeDelay);
                PrepareControllerToThrow(false);
                onFinishThrow.Invoke();
            }
            else // is server
            {
                yield return new WaitForSeconds(throwDelayTime);
                int prefab_index = EMI_NetworkManager.instance.spawnPrefabs.FindIndex(x => x.GetInstanceID() == objectToThrow.gameObject.GetInstanceID());
                if (prefab_index < 0)
                {
                    Debug.LogError("Tried to instantiate a none networked object: " + objectToThrow + ", this is not allowed! This object must have a NetworkIdentity and be a part of the EMI_NetworkManager spawnable prefabs list.");
                    yield return null;
                }
                else
                {
                    GameObject prefab = UnityEngine.Object.Instantiate(EMI_NetworkManager.instance.spawnPrefabs[prefab_index], startPoint, throwStartPoint.rotation);
                    SceneManager.MoveGameObjectToScene(prefab, gameObject.scene);
                    Rigidbody obj = prefab.GetComponent<Rigidbody>();

                    obj.isKinematic = false;
                    LaunchObject(obj);
                    if (ui)
                    {
                        ui.UpdateCount(this);
                    }

                    onThrowObject?.Invoke();
                    NetworkServer.Spawn(obj.gameObject);

                    yield return new WaitForSeconds(2 * lineStepPerTime);
                    var coll = obj.GetComponent<Collider>();
                    if (coll)
                    {
                        coll.isTrigger = false;
                    }

                    inThrow = false;

                    if (currentThrowObject <= 0)
                    {
                        objectToThrow = null;
                    }

                    yield return new WaitForSeconds(exitThrowModeDelay);
                    PrepareControllerToThrow(false);
                    onFinishThrow.Invoke();
                }
            }
        }
        protected override void UpdateThrowInput()
        {
            if (objectToThrow == null || !tpInput.enabled || tpInput.cc.customAction || !canUseThrow || tpInput.cc.isDead)
            {
                isAiming = false;
                inThrow = false;
                isThrowInput = false;
                return;
            }

            aimInputState = aimThrowInput.GetButtonDown();
            if (aimInputState && !isAiming && !inThrow)
            {
                PrepareControllerToThrow(true);
                tpInput.animator.CrossFadeInFixedTime(holdingAnimation, 0.2f);
                onEnableAim.Invoke();
                return;
            }
            if (aimHoldingButton && isAiming && aimThrowInput.GetButtonUp())
            {
                PrepareControllerToThrow(false);
                tpInput.animator.CrossFadeInFixedTime(cancelAnimation, 0.2f);
                onCancelAim.Invoke();
                onFinishThrow.Invoke();
            }

            if (throwInput.GetButtonDown() && isAiming && !inThrow)
            {
                isAiming = false;
                isThrowInput = true;
            }
            else if (!aimHoldingButton && aimInputState && !isThrowInput && isAiming)
            {
                PrepareControllerToThrow(false);
                tpInput.animator.CrossFadeInFixedTime(cancelAnimation, 0.2f);
                onCancelAim.Invoke();
                onFinishThrow.Invoke();
            }
        }
        #endregion
    }
}
