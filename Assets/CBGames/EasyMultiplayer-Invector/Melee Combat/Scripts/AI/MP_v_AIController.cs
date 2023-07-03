using EMI.AI;
using Mirror;
using System.Collections;
using UnityEngine;

namespace Invector.vCharacterController.AI
{
    public class MP_v_AIController : vSimpleMeleeAI_Controller
    {
        #region Properties

        #region Health
        public override float healthRecovery 
        { 
            get 
            {
                if (NetworkServer.active || NetworkClient.active)
                {
                    return nc.healthRecovery;
                }
                else
                {
                    return _healthRecovery;
                }
            } 
            set 
            { 
                _healthRecovery = value;
                #if UNITY_SERVER || UNITY_EDITOR
                if (NetworkServer.active && nc)
                {
                    nc.healthRecovery = value;
                }
                #endif
            }
        }
        public override float currentHealthRecoveryDelay { 
            get 
            {
                if (NetworkServer.active || NetworkClient.active)
                {
                    return nc.currentHealthRecoveryDelay;
                }
                else
                {
                    return _currentHealthRecoveryDelay;
                }
            } 
            set 
            { 
                _currentHealthRecoveryDelay = value;
                #if UNITY_SERVER || UNITY_EDITOR
                if (NetworkServer.active && nc)
                {
                    nc.currentHealthRecoveryDelay = value;
                }
                #endif
            } 
        }
        public override int MaxHealth
        {
            get
            {
                if ((NetworkClient.active || NetworkServer.active) && nc)
                {
                    return nc.maxHealth;
                }
                else
                {
                    return maxHealth;
                }
            }
            protected set
            {
                maxHealth = value;
                #if UNITY_SERVER || UNITY_EDITOR
                if (NetworkServer.active && nc)
                {
                    nc.maxHealth = value;
                }
                #endif
            }
        }
        public override float currentHealth
        {
            get
            {
                if ((!NetworkClient.active && !NetworkServer.active) || nc == null)
                {
                    return _currentHealth;
                }
                else if (_currentHealth < 10)
                {
                    return nc.currentHealth;
                }
                else
                {
                    return _currentHealth;
                }
            }
            protected set
            {
                if (_currentHealth != value)
                {
                    _currentHealth = value;
                    onChangeHealth.Invoke(_currentHealth);
                    StartCoroutine(VerifyHealth());
                }
                if (_currentHealth <= 0 || (nc && nc.currentHealth <= 0 && NetworkClient.active))
                {
                    isDead = true;
                }
                else if (_currentHealth > 0 || (nc && nc.currentHealth > 0 && NetworkClient.active))
                {
                    isDead = false;
                }
                #if UNITY_SERVER || UNITY_EDITOR
                if (NetworkServer.active && nc && nc.currentHealth != value)
                {
                    nc.currentHealth = value;
                }
                #endif
            }
        }
        public override bool isDead
        {
            get
            {
                if ((!NetworkClient.active && !NetworkServer.active) || nc == null)
                {
                    return _isDead;
                }
                else if (NetworkClient.active || NetworkServer.active)
                {
                    return nc.isDead;
                }
                else
                {
                    return _isDead;
                }
            }
            set
            {
                _isDead = value;
                #if UNITY_SERVER || UNITY_EDITOR
                if (NetworkServer.active && nc)
                {
                    nc.isDead = value;
                }
                #endif
                if (_isDead == true)
                {
                    onDead.Invoke(gameObject);
                }
            }
        }
        #endregion

        #region vSimpleMeleeAI_Motor
        public override bool actions 
        { 
            get 
            { 
                if (NetworkClient.active || NetworkServer.active)
                {
                    return nc.actions;
                }
                else
                {
                    return _actions;
                }
            } 
            set 
            { 
                _actions = value;
                #if UNITY_SERVER || UNITY_EDITOR
                if (NetworkServer.active)
                {
                    nc.actions = value;
                }
                #endif
            } 
        }
        public override AIStates currentState 
        { 
            get 
            { 
                if (NetworkServer.active || NetworkClient.active)
                {
                    return nc.currentState;
                }
                else
                {
                    return _currentState;
                }
            } 
            set 
            { 
                _currentState = value;
                #if UNITY_SERVER || UNITY_EDITOR
                if (NetworkServer.active)
                {
                    nc.currentState = value;
                }
                #endif
            } 
        }
        protected override bool isCrouched 
        { 
            get 
            {
                if (NetworkClient.active || NetworkServer.active)
                {
                    return nc.isCrouched;
                }
                else
                {
                    return _isCrouched;
                }
            } 
            set 
            { 
                _isCrouched = value;
                #if UNITY_SERVER || UNITY_EDITOR
                if (NetworkServer.active)
                {
                    nc.isCrouched = value;
                }
                #endif
            } 
        }
        protected override bool canAttack 
        { 
            get 
            {
                if (NetworkServer.active || NetworkClient.active)
                {
                    return nc.canAttack;
                }
                else
                {
                    return _canAttack;
                }
            } 
            set 
            { 
                _canAttack = value;
                #if UNITY_SERVER || UNITY_EDITOR
                if (NetworkServer.active)
                {
                    nc.canAttack = value;
                }
                #endif
            } 
        }
        protected override bool tryingBlock 
        { 
            get 
            {
                if (NetworkServer.active || NetworkClient.active)
                {
                    return nc.tryingBlock;
                }
                else
                {
                    return _tryingBlock;
                }
            } 
            set 
            { 
                _tryingBlock = value;
                #if UNITY_SERVER || UNITY_EDITOR
                if (NetworkServer.active)
                {
                    nc.tryingBlock = value;
                }
                #endif
            }
        }
        //protected override bool isRolling { get { return _isRolling; } set { _isRolling = value; } }
        public override bool isBlocking 
        { 
            get 
            {
                if (NetworkServer.active || NetworkClient.active)
                {
                    return nc.isBlocking;
                }
                else
                {
                    return _isBlocking;
                }
            } 
            protected set { 
                _isBlocking = value;
                #if UNITY_SERVER || UNITY_EDITOR
                if (NetworkServer.active)
                {
                    nc.isBlocking = value;
                }
                #endif
            }
        }
        protected override Vector3 targetPos 
        { 
            get 
            {
                if (NetworkServer.active || NetworkClient.active)
                {
                    return nc.targetPos;
                }
                else
                {
                    return _targetPos;
                }
            } 
            set 
            { 
                _targetPos = value;
                #if UNITY_SERVER || UNITY_EDITOR
                if (NetworkServer.active && nc)
                {
                    nc.targetPos = value;
                }
                #endif
            } 
        }
        protected override bool canSeeTarget 
        { 
            get 
            {
                if (NetworkServer.active || NetworkClient.active)
                {
                    return nc.canSeeTarget;
                }
                else
                {
                    return _canSeeTarget;
                }
            } 
            set
            { 
                _canSeeTarget = value;
                #if UNITY_SERVER || UNITY_EDITOR
                if (NetworkServer.active && nc)
                {
                    nc.canSeeTarget = value;
                }
                #endif
            } 
        }
        protected override Vector3 destination 
        { 
            get 
            {
                if (NetworkServer.active || NetworkClient.active)
                {
                    return nc.destination;
                }
                else
                {
                    return _destination;
                }
            } 
            set 
            { 
                _destination = value;
                #if UNITY_SERVER || UNITY_EDITOR
                if (NetworkServer.active && nc)
                {
                    nc.destination = value;
                }
                #endif
            }
        }
        protected override Vector3 fwd 
        { 
            get 
            {
                if (NetworkServer.active || NetworkClient.active)
                {
                    return nc.fwd;
                }
                else
                {
                    return _fwd;
                }
            } 
            set 
            { 
                _fwd = value;
                #if UNITY_SERVER || UNITY_EDITOR
                if (NetworkServer.active && nc)
                {
                    nc.fwd = value;
                }
                #endif
            }
        }
        //protected override bool isGrounded { get { return _isGrounded; } set { _isGrounded = value; } }
        protected override bool isStrafing 
        { 
            get 
            {
                if (NetworkServer.active || NetworkClient.active)
                {
                    return nc.ai_isStrafing;
                }
                else
                {
                    return _isStrafing;
                }
            } 
            set 
            { 
                _isStrafing = value;
                #if UNITY_SERVER || UNITY_EDITOR
                if (NetworkServer.active && nc)
                {
                    nc.ai_isStrafing = value;
                }
                #endif
            }
        }
        protected override int currentWaypoint 
        { 
            get 
            {
                if (NetworkServer.active || NetworkClient.active)
                {
                    return nc.currentWaypoint;
                }
                else
                {
                    return _currentWaypoint;
                }
            } 
            set 
            { 
                _currentWaypoint = value;
                #if UNITY_SERVER || UNITY_EDITOR
                if (NetworkServer.active && nc)
                {
                    nc.currentWaypoint = value;
                }
                #endif
            }
        }
        protected override int currentPatrolPoint 
        { 
            get 
            {
                if (NetworkServer.active || NetworkClient.active)
                {
                    return nc.currentPatrolPoint;
                }
                else
                {
                    return _currentPatrolPoint;
                }
            } 
            set 
            { 
                _currentPatrolPoint = value;
                #if UNITY_SERVER || UNITY_EDITOR
                if (NetworkServer.active && nc)
                {
                    nc.currentPatrolPoint = value;
                }
                #endif
            }
        }
        protected override float direction 
        { 
            get 
            {
                if (NetworkServer.active || NetworkClient.active)
                {
                    return nc.direction;
                }
                else
                {
                    return _direction;
                }
            } 
            set 
            { 
                _direction = value;
                #if UNITY_SERVER || UNITY_EDITOR
                if (NetworkServer.active && nc)
                {
                    nc.direction = value;
                }
                #endif
            }
        }
        protected override float timer 
        { 
            get 
            {
                if (NetworkServer.active || NetworkClient.active)
                {
                    return nc.timer;
                }
                else
                {
                    return _timer;
                } 
            } 
            set 
            { 
                _timer = value;
                #if UNITY_SERVER || UNITY_EDITOR
                if (NetworkServer.active && nc)
                {
                    nc.timer = value;
                }
                #endif
            }
        }
        protected override float wait 
        { 
            get 
            {
                if (NetworkServer.active || NetworkClient.active)
                {
                    return nc.wait;
                }
                else
                {
                    return _wait;
                }
            } 
            set 
            { 
                _wait = value;
                #if UNITY_SERVER || UNITY_EDITOR
                if (NetworkServer.active && nc)
                {
                    nc.wait = value;
                }
                #endif
            }
        }
        protected override float fovAngle 
        { 
            get 
            {
                if (NetworkServer.active || NetworkClient.active)
                {
                    return nc.fovAngle;
                }
                else
                {
                    return _fovAngle;
                }
            } 
            set 
            { 
                _fovAngle = value;
                #if UNITY_SERVER || UNITY_EDITOR
                if (NetworkServer.active && nc)
                {
                    nc.fovAngle = value;
                }
                #endif
            }
        }
        protected override Vector3 startPosition 
        { 
            get 
            {
                if (NetworkServer.active || NetworkClient.active)
                {
                    return nc.startPosition;
                }
                else
                {
                    return _startPosition;
                }
            } 
            set 
            { 
                _startPosition = value;
            #if UNITY_SERVER || UNITY_EDITOR
                if (NetworkServer.active && nc)
                {
                    nc.startPosition = value;
                }
            #endif
            }
        }

        #endregion

        #region vSimpleMeleeAI_Controller
        protected override float moveToSpeed 
        { 
            get 
            {
                if (NetworkServer.active || NetworkClient.active)
                {
                    return nc.moveToSpeed;
                }
                else
                {
                    return _moveToSpeed;
                }
            } 
            set 
            { 
                _moveToSpeed = value;
                #if UNITY_SERVER || UNITY_EDITOR
                if (NetworkServer.active && nc)
                {
                    nc.moveToSpeed = value;
                }
                #endif
            }
        }
        protected override Vector3 moveToDestination 
        { 
            get 
            {
                if (NetworkServer.active || NetworkClient.active)
                {
                    return nc.moveToDestination;
                }
                else
                {
                    return _moveToDestination;
                }
            } 
            set 
            { 
                _moveToDestination = value;
                #if UNITY_SERVER || UNITY_EDITOR
                if (NetworkServer.active && nc)
                {
                    nc.moveToDestination = value;
                }
                #endif
            }
        }
        #endregion

        #region Reference
        protected MeleeAINetworkCalls nc = null;
        #endregion
        
        #endregion

        #region Initilization
        protected virtual void Awake()
        {
            nc = GetComponent<MeleeAINetworkCalls>();
        }
        #endregion

        #region Overrides
        #region Attack
        protected override IEnumerator MeleeAttackRotine()
        {
            if ((!NetworkClient.active && !NetworkServer.active) || NetworkServer.active)
            {
                if (!isAttacking && !actions && attackCount > 0 && !lockMovement && !isRolling)
                {
                    if (NetworkServer.active)
                    {
                        nc.Rpc_AIAttack();
                        yield return base.MeleeAttackRotine();
                    }
                }
            }
        }
        [Client]
        public virtual void ClientMeleeAttack()
        {
            if (!isAttacking && !actions && attackCount > 0 && !lockMovement && !isRolling)
            {
                StartCoroutine(base.MeleeAttackRotine());
            }
        }
        #endregion

        #region Health
        protected virtual IEnumerator VerifyHealth()
        {
            yield return new WaitForSeconds(0.001f);
            if (nc.currentHealth != currentHealth)
            {
                currentHealth = nc.currentHealth;
            }
        }
        /// <summary>
        /// There is a mistake in invectors code. It can potentially double invoke the onDead event. I fixed it for them.
        /// </summary>
        public override void ChangeHealth(int value)
        {
            currentHealth = value;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            HandleCheckHealthEvents();
        }

        /// <summary>
        /// There is a mistake in invectors code. It can potentially double invoke the onDead event. I fixed it for them.
        /// </summary>
        public override void AddHealth(int value)
        {
            currentHealth += value;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            HandleCheckHealthEvents();
        }
        #endregion

        #region Targets
        public override void SetCurrentTarget(Transform target)
        {
            if (NetworkServer.active || (!NetworkServer.active && !NetworkClient.active))
            {
                if (target != currentTarget.transform)
                {
                    currentTarget.transform = target;
                    currentTarget.colliderTarget = target.GetComponent<Collider>();
                    currentTarget.character = target.GetComponent<vIHealthController>();
                    if (target.GetComponent<NetworkIdentity>())
                    {
                        nc.targetNetId = target.GetComponent<NetworkIdentity>().netId;
                    }
                }
                AddTagsToDetect(target.gameObject.tag);
                sphereSensor.AddTarget(target);
            }
        }
        public virtual void ClientSetCurrentTarget(uint targetNetId)
        {
            if (targetNetId == 0) return;
            NetworkIdentity netId = FindObjectsOfType<NetworkIdentity>().vToList().Find(x => x.netId == targetNetId);
            if (netId != null)
            {
                Transform target = netId.transform;
                if (target != currentTarget.transform)
                {
                    currentTarget.transform = target;
                    currentTarget.colliderTarget = target.GetComponent<Collider>();
                    currentTarget.character = target.GetComponent<vIHealthController>();
                }
                AddTagsToDetect(target.gameObject.tag);
                sphereSensor.AddTarget(target);
            }
        }
        #endregion
        #endregion

        #region Additions
        [Client]
        public virtual void SetIsBlocking(bool value)
        {
            isBlocking = value;
        }
        [Client]
        public virtual void SetTryingBlock(bool value)
        {
            tryingBlock = value;
        }
        [Client]
        public virtual void SetCanAttack(bool value)
        {
            canAttack = value;
        }
        [Client]
        public virtual void SetTargetPos(Vector3 pos)
        {
            targetPos = pos;
        }
        [Client]
        public virtual void SetCanSeeTarget(bool value)
        {
            canSeeTarget = value;
        }
        [Client]
        public virtual void SetDestination(Vector3 destination)
        {
            this.destination = destination;
        }
        [Client]
        public virtual void SetFwd(Vector3 fwd)
        {
            this.fwd = fwd;
        }
        [Client]
        public virtual void SetStraging(bool value)
        {
            isStrafing = value;
        }
        [Client]
        public virtual void SetCurrentWaypoint(int value)
        {
            currentWaypoint = value;
        }
        [Client]
        public virtual void SetCurrentPatrolPoint(int value)
        {
            currentPatrolPoint = value;
        }
        [Client]
        public virtual void SetDirection(float value)
        {
            direction = value;
        }
        [Client]
        public virtual void SetTimer(float time)
        {
            timer = time;
        }
        [Client]
        public virtual void SetWait(float time)
        {
            wait = time;
        }
        [Client]
        public virtual void SetFov(float value)
        {
            fovAngle = value;
        }
        [Client]
        public virtual void SetStartPos(Vector3 value)
        {
            startPosition = value;
        }
        [Client]
        public virtual void SetMoveToDestination(Vector3 value)
        {
            moveToDestination = value;
        }
        [Client]
        public virtual void SetMoveToSpeed(float value)
        {
            moveToSpeed = value;
        }
        #endregion
    }
}
