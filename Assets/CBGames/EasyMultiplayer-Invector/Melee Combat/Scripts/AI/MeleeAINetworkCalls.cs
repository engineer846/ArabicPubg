using EMI.Player;
using Invector.vCharacterController.AI;
using Mirror;
using UnityEngine;
using static Invector.vCharacterController.AI.vSimpleMeleeAI_Motor;

namespace EMI.AI
{
    public class MeleeAINetworkCalls : MeleeNetworkCalls
    {
        #region Properties
        #region MP_v_AIController
        protected MP_v_AIController ai_controller = null;
        [HideInInspector, SyncVar(hook = nameof(ActionsChanged))] public bool actions = false;
        [HideInInspector, SyncVar(hook = nameof(CurrentStateChanged))] public AIStates currentState = AIStates.PatrolWaypoints;
        [HideInInspector, SyncVar(hook = nameof(IsBlockingChanged))] public bool isBlocking = false;
        [HideInInspector, SyncVar(hook = nameof(TryingBlockChanged))] public bool tryingBlock = false;
        [HideInInspector, SyncVar(hook = nameof(CanAttackChanged))] public bool canAttack = false;
        [HideInInspector, SyncVar(hook = nameof(IsCrouchedChanged))] public bool isCrouched = false;
        [HideInInspector, SyncVar(hook = nameof(TargetPosChanged))] public Vector3 targetPos = Vector3.zero;
        [HideInInspector, SyncVar(hook = nameof(CanSeeTargetChanged))] public bool canSeeTarget = false;
        [HideInInspector, SyncVar(hook = nameof(DestinationChanged))] public Vector3 destination = Vector3.zero;
        [HideInInspector, SyncVar(hook = nameof(FwdChanged))] public Vector3 fwd = Vector3.zero;
        [HideInInspector, SyncVar(hook = nameof(AIIsStrafingChanged))] public bool ai_isStrafing = false;
        [HideInInspector, SyncVar(hook = nameof(CurrentWayPointChanged))] public int currentWaypoint = 0;
        [HideInInspector, SyncVar(hook = nameof(CurrentPatrolPointChanged))] public int currentPatrolPoint = 0;
        [HideInInspector, SyncVar(hook = nameof(DirectionChanged))] public float direction = 0;
        [HideInInspector, SyncVar(hook = nameof(TimerChanged))] public float timer = 0;
        [HideInInspector, SyncVar(hook = nameof(WaitChanged))] public float wait = 0;
        [HideInInspector, SyncVar(hook = nameof(FovAngleChanged))] public float fovAngle = 0;
        [HideInInspector, SyncVar(hook = nameof(StartPositionChanged))] public Vector3 startPosition = Vector3.zero;
        [HideInInspector, SyncVar(hook = nameof(MoveToDestinationChanged))] public Vector3 moveToDestination = Vector3.zero;
        [HideInInspector, SyncVar(hook = nameof(MoveToSpeedChanged))] public float moveToSpeed = 0;
        [HideInInspector, SyncVar(hook = nameof(CurrentTargetChanged))] public uint targetNetId = 0;
        #endregion
        #endregion

        #region Initilization
        protected override void Awake()
        {
            ai_controller = GetComponent<MP_v_AIController>();
            base.Awake();
        }
        protected override void Start()
        {
            if (NetworkClient.active)
            {
                InitializeServerValues();
            }
            base.Start();
        }
        /// <summary>
        /// Sets the AI to whatever the servers state is in.
        /// </summary>
        [Client]
        protected virtual void InitializeServerValues()
        {
            TargetPosChanged(Vector3.zero, targetPos);
            CanSeeTargetChanged(false, canSeeTarget);
            DestinationChanged(Vector3.zero, destination);
            FwdChanged(Vector3.zero, fwd);
            AIIsStrafingChanged(false, ai_isStrafing);
            CurrentWayPointChanged(0, currentWaypoint);
            CurrentPatrolPointChanged(0, currentPatrolPoint);
            DirectionChanged(0, direction);
            TimerChanged(0, timer);
            WaitChanged(0, wait);
            FovAngleChanged(0, fovAngle);
            StartPositionChanged(Vector3.zero, startPosition);
            MoveToDestinationChanged(Vector3.zero, moveToDestination);
            CurrentTargetChanged(0, targetNetId);
        }
        #endregion

        #region Hooks
        #region MP_v_AIController
        protected virtual void ActionsChanged(bool oldValue, bool newValue)
        {
            if (NetworkServer.active) return;
            actions = newValue;
        }
        protected virtual void CurrentStateChanged(AIStates oldState, AIStates newState)
        {
            if (NetworkServer.active) return;
            ai_controller.currentState = newState;
        }
        protected virtual void IsBlockingChanged(bool oldValue, bool newValue)
        {
            if (NetworkServer.active) return;
            ai_controller.SetIsBlocking(newValue);
        }
        protected virtual void TryingBlockChanged(bool oldValue, bool newValue)
        {
            if (NetworkServer.active) return;
            ai_controller.SetTryingBlock(newValue);
        }
        protected virtual void CanAttackChanged(bool oldValue, bool newValue)
        {
            if (NetworkServer.active) return;
            ai_controller.SetCanAttack(newValue);
        }
        protected virtual void IsCrouchedChanged(bool oldValue, bool newValue)
        {
            if (NetworkServer.active) return;
            ai_controller.isCrouching = newValue;
        }
        protected virtual void TargetPosChanged(Vector3 oldPos, Vector3 newPos)
        {
            if (NetworkServer.active) return;
            ai_controller.SetTargetPos(newPos);
        }
        protected virtual void CanSeeTargetChanged(bool oldValue, bool newValue)
        {
            if (NetworkServer.active) return;
            ai_controller.SetCanSeeTarget(newValue);
        }
        protected virtual void DestinationChanged(Vector3 oldDes, Vector3 newDes)
        {
            if (NetworkServer.active) return;
            ai_controller.SetDestination(newDes);
        }
        protected virtual void FwdChanged(Vector3 oldValue, Vector3 newVlaue)
        {
            if (NetworkServer.active) return;
            ai_controller.SetFwd(newVlaue);
        }
        protected virtual void AIIsStrafingChanged(bool oldValue, bool newValue)
        {
            if (NetworkServer.active) return;
            ai_controller.SetStraging(newValue);
        }
        protected virtual void CurrentWayPointChanged(int oldPoint, int newPoint)
        {
            if (NetworkServer.active) return;
            ai_controller.SetCurrentWaypoint(newPoint);
        }
        protected virtual void CurrentPatrolPointChanged(int oldPoint, int newPoint)
        {
            if (NetworkServer.active) return;
            ai_controller.SetCurrentPatrolPoint(newPoint);
        }
        protected virtual void DirectionChanged(float oldDir, float newDir)
        {
            if (NetworkServer.active) return;
            ai_controller.SetDirection(newDir);
        }
        protected virtual void TimerChanged(float oldTime, float newTime)
        {
            if (NetworkServer.active) return;
            ai_controller.SetTimer(newTime);
        }
        protected virtual void WaitChanged(float oldWait, float newWait)
        {
            if (NetworkServer.active) return;
            ai_controller.SetWait(newWait);
        }
        protected virtual void FovAngleChanged(float oldAngle, float newAngle)
        {
            if (NetworkServer.active) return;
            ai_controller.SetFov(newAngle);
        }
        protected virtual void StartPositionChanged(Vector3 oldValue, Vector3 newValue)
        {
            if (NetworkServer.active) return;
            ai_controller.SetStartPos(newValue);
        }
        protected virtual void MoveToDestinationChanged(Vector3 oldDest, Vector3 newDest)
        {
            if (NetworkServer.active) return;
            ai_controller.SetMoveToDestination(newDest);
        }
        protected virtual void MoveToSpeedChanged(float oldValue, float newValue)
        {
            if (NetworkServer.active) return;
            ai_controller.SetMoveToSpeed(newValue);
        }
        protected virtual void CurrentTargetChanged(uint oldVlaue, uint newValue)
        {
            if (NetworkServer.active) return;
            ai_controller.ClientSetCurrentTarget(newValue);
        }
        #endregion
        #endregion

        #region RPC
        #region ClientRpc
        [ClientRpc(includeOwner = false)]
        public virtual void Rpc_AIAttack()
        {
            ai_controller.ClientMeleeAttack();
        }
        #endregion
        #endregion
    }
}
