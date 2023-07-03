using EMI.Object;
using EMI.Player;
using Invector.vCamera;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Invector.vCharacterController
{
    public class MP_vLockOn : vLockOn
    {
        #region Properties
        protected Team myTeam = null;
        protected MeleeNetworkCalls nc = null;
        protected vThirdPersonCamera cam = null;
        protected override bool inTarget
        {
            get
            {
                return _inTarget;
            }
            set
            {
                _inTarget = value;
                #if UNITY_SERVER || UNITY_EDITOR
                if (NetworkServer.active)
                {
                    nc.lockedOn = _inTarget;
                }
                #endif
            }
        }
        protected override Transform target
        {
            get
            {
                return _target;
            }
            set
            {
                _target = value;
                if (NetworkClient.active && nc && nc.hasAuthority)
                {
                    if (_target && _target.GetComponent<NetworkIdentity>())
                    {
                        nc.Cmd_SetClientCamLockTarget(_target.GetComponent<NetworkIdentity>().netId);
                    }
                    else if (_target == null)
                    {
                        nc.Cmd_SetClientCamLockTarget(999999999);
                    }
                }
            }
        }
        #endregion

        #region Initialization
        protected virtual IEnumerator WaitForCamera()
        {
            yield return new WaitUntil(() => nc.ownerCam != null);
            cam = nc.ownerCam;
        }
        protected override void Start()
        {
            myTeam = GetComponent<Team>();
            nc = GetComponent<MeleeNetworkCalls>();
            if (nc.hasAuthority)
            {
                StartCoroutine(WaitForCamera());
            }

            // LockOn Inputs
            lockOnInput.SetNetworkCalls(nc, "LockOnInput");
            nexTargetInput.SetNetworkCalls(nc, "NextTargetInput");
            previousTargetInput.SetNetworkCalls(nc, "PreviousTargetInput");

            base.Start();
        }
        #endregion

        #region Target Finding
        /// <summary>
        /// This will prevent targeting yourself, as well as anyone that isn't an enemy team.
        /// </summary>
        protected override List<Transform> GetPossibleTargets()
        {
            if (tpCamera != null && tpCamera.mainTarget != null)
            {
                watcher = tpCamera.mainTarget;
            }
            else
            {
                watcher = transform;
            }

            var listPrimary = new List<Transform>();
            var targets = Physics.SphereCastAll(watcher.position, range, watcher.forward, .01f);
            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i].transform.root.gameObject.Equals(this))
                    continue;
                if (myTeam != null)
                {
                    Team team = targets[i].transform.root.GetComponent<Team>();
                    if (team != null && !myTeam.IsEnemy(team.teamName))
                        continue;
                }
                var hitOther = targets[i];
                if (tagsToFind.vToList().Contains(hitOther.transform.tag))
                {
                    if (isCharacterAlive(hitOther.transform.GetComponent<Transform>()))
                    {
                        RaycastHit hit;
                        var boundPoints = BoundPoints(hitOther.collider);
                        for (int a = 0; a < boundPoints.Length; a++)
                        {
                            var point = boundPoints[a];
                            if (Physics.Linecast(transform.position, point, out hit, layerOfObstacles))
                            {
                                if (hit.transform == hitOther.transform)
                                {
                                    listPrimary.Add(hitOther.transform);
                                    if (showDebug)
                                    {
                                        Debug.DrawLine(transform.position, point, Color.green, 2);
                                    }

                                    break;
                                }
                                else if (showDebug)
                                {
                                    Debug.DrawLine(transform.position, point, Color.red, 2);
                                }
                            }
                            else
                            {
                                listPrimary.Add(hitOther.transform);
                                if (showDebug)
                                {
                                    Debug.DrawLine(transform.position, point, Color.green, 2);
                                }

                                break;
                            }
                        }
                    }
                }
            }
            SortTargets(ref listPrimary);
            return listPrimary;
        }
        #endregion

        #region Additions
        public virtual bool IsLockedOn()
        {
            return inTarget;
        }
        [Client]
        public virtual void SetLockOn(bool value)
        {
            inTarget = value;
        }
        #endregion
    }
}
