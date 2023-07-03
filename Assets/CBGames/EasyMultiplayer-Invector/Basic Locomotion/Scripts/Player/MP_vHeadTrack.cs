using EMI.Player;
using Mirror;
using UnityEngine;

namespace Invector.vCharacterController
{
    public class MP_vHeadTrack : vHeadTrack
    {
        #region Properties
        protected BasicNetworkCalls nc = null;
        protected override bool lookConditions
        {
            get
            {
                if (!cameraMain && nc && nc.HasAuthority() && NetworkClient.active)
                {
                    cameraMain = Camera.main;
                }
                if (!nc || (NetworkClient.active && nc && nc.HasAuthority()))
                {
                    return head != null && (followCamera && cameraMain != null) || (!followCamera && (currentLookTarget || simpleTarget)) || temporaryLookTime > 0;
                }
                else if (nc)
                {
                    return head != null && (followCamera && nc.clientCam != null) || (!followCamera && (currentLookTarget || simpleTarget)) || temporaryLookTime > 0;
                }
                else
                {
                    return false;
                }
            }
        }
        protected MP_vThirdPersonInput tpi = null;
        #endregion

        #region Initialization
        protected virtual void Awake()
        {
            nc = GetComponent<BasicNetworkCalls>();
            tpi = GetComponent<MP_vThirdPersonInput>();
        }
        #endregion

        protected override Vector3 GetLookPoint()
        {
            var distanceToLook = 100;

            if (animator == null)
            {
                return Vector3.zero;
            }

            if (lookConditions && !IgnoreHeadTrack())
            {
                var dir = transform.forward;
                if (temporaryLookTime <= 0)
                {
                    var lookPosition = headPoint + (transform.forward * distanceToLook);
                    if (followCamera)
                    {
                        if (nc.HasAuthority() && NetworkClient.active)
                        {
                            lookPosition = (cameraMain.transform.position + (cameraMain.transform.forward * distanceToLook));
                        }
                        else if (nc.clientCam)
                        {
                            lookPosition = (nc.clientCam.transform.position + (nc.clientCam.transform.forward * distanceToLook));
                        }
                    }

                    dir = lookPosition - headPoint;
                    if ((followCamera && !alwaysFollowCamera) || !followCamera)
                    {
                        if (simpleTarget != null)
                        {
                            dir = simpleTarget.position - headPoint;
                            if (currentLookTarget && currentLookTarget == lastLookTarget)
                            {
                                currentLookTarget.ExitLook(this);
                                lastLookTarget = null;
                            }
                        }
                        else if (currentLookTarget != null && (currentLookTarget.ignoreHeadTrackAngle || TargetIsOnRange(currentLookTarget.lookPoint - headPoint)) && currentLookTarget.IsVisible(headPoint, obstacleLayer))
                        {
                            dir = currentLookTarget.lookPoint - headPoint;
                            if (currentLookTarget != lastLookTarget)
                            {
                                currentLookTarget.EnterLook(this);
                                lastLookTarget = currentLookTarget;
                            }
                        }
                        else if (currentLookTarget && currentLookTarget == lastLookTarget)
                        {
                            currentLookTarget.ExitLook(this);
                            lastLookTarget = null;
                        }
                    }
                }
                else
                {
                    dir = temporaryLookPoint - headPoint;
                    temporaryLookTime -= Time.deltaTime;
                    if (currentLookTarget && currentLookTarget == lastLookTarget)
                    {
                        currentLookTarget.ExitLook(this);
                        lastLookTarget = null;
                    }
                }

                var angle = GetTargetAngle(dir);
                if (cancelTrackOutOfAngle && (lastLookTarget == null || !lastLookTarget.ignoreHeadTrackAngle))
                {
                    if (TargetIsOnRange(dir))
                    {
                        if (animator.GetBool("IsStrafing") && !IsAnimatorTag("Upperbody Pose"))
                        {
                            SmoothValues(strafeHeadWeight, strafeBodyWeight, angle.x, angle.y);
                        }
                        else if (animator.GetBool("IsStrafing") && IsAnimatorTag("Upperbody Pose"))
                        {
                            SmoothValues(aimingHeadWeight, aimingBodyWeight, angle.x, angle.y);
                        }
                        else
                        {
                            SmoothValues(freeHeadWeight, freeBodyWeight, angle.x, angle.y);
                        }
                    }
                    else
                    {
                        SmoothValues();
                    }
                }
                else
                {
                    if (animator.GetBool("IsStrafing") && !IsAnimatorTag("Upperbody Pose"))
                    {
                        SmoothValues(strafeHeadWeight, strafeBodyWeight, angle.x, angle.y);
                    }
                    else if (animator.GetBool("IsStrafing") && IsAnimatorTag("Upperbody Pose"))
                    {
                        SmoothValues(aimingHeadWeight, aimingBodyWeight, angle.x, angle.y);
                    }
                    else
                    {
                        SmoothValues(freeHeadWeight, freeBodyWeight, angle.x, angle.y);
                    }
                }
                if (targetsInArea.Count > 1)
                {
                    SortTargets();
                }
            }
            else
            {
                SmoothValues();
                if (targetsInArea.Count > 1)
                {
                    SortTargets();
                }
            }

            var rotA = Quaternion.AngleAxis(yRotation, transform.up);
            var rotB = Quaternion.AngleAxis(xRotation, transform.right);
            var finalRotation = (rotA * rotB);
            var lookDirection = finalRotation * transform.forward;
            return headPoint + (lookDirection * distanceToLook);
        }
        
    }
}
