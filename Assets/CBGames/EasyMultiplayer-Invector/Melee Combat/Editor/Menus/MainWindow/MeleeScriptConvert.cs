using Common.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.EditorCoroutines.Editor;
using UnityEngine;

namespace EMI.Editors.Windows
{
    public class MeleeScriptConvert : BasicScriptConvert
    {
        public override IEnumerator Run(Action callbackFunction = null, bool callInvoke = true, Action<string, string, float> progressUpdate = null)
        {
            #region Run BasicScriptConvert
            yield return base.Run(null, false, progressUpdate);
            #endregion

            string progress_title = "Converting Melee Scripts";
            string dataPath = Application.dataPath.Replace('/', Path.DirectorySeparatorChar);
            string melee_folder = FilesUtil.FindFolderPath("Invector-3rdPersonController/Melee Combat");
            string inv_folder = FilesUtil.FindFolderPath("Invector-3rdPersonController/ItemManager");

            #region Convert vItemManagerEditor.cs
            progressUpdate?.Invoke(progress_title, "Converting vItemManagerEditor.cs...", 0.07f);
            yield return new EditorWaitForSeconds(0.01f);
            string vItemManagerEditor = FilesUtil.FindFilePath("vItemManagerEditor.cs", inv_folder);
            FilesUtil.ModifyFileAtPath(vItemManagerEditor, new List<FilesUtil.Modification>()
                {
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "[CustomEditor(typeof(vItemManager))]" },
                        modifiedLines: new List<string>() {
                            "[CustomEditor(typeof(vItemManager), true)]"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    )
                }
            );
            #endregion

            #region Convert vItemManager.cs
            progressUpdate?.Invoke(progress_title, "Converting vItemManager.cs...", 0.14f);
            yield return new EditorWaitForSeconds(0.01f);
            string vItemManager = FilesUtil.FindFilePath("vItemManager.cs", inv_folder);
            FilesUtil.ModifyFileAtPath(vItemManager, new List<FilesUtil.Modification>()
                {
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "IEnumerator Start()" },
                        modifiedLines: new List<string>() {
                            "//IEnumerator Start()",
                            "protected virtual IEnumerator Start()"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "private Animator animator;" },
                        modifiedLines: new List<string>() {
                            "//private Animator animator;",
                            "protected Animator animator;"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    )
                }
            );
            #endregion

            #region Convert vItemCollectionDisplay.cs
            progressUpdate?.Invoke(progress_title, "Converting vItemCollectionDisplay.cs...", 0.21f);
            yield return new EditorWaitForSeconds(0.01f);
            string vItemCollectionDisplay = FilesUtil.FindFilePath("vItemCollectionDisplay.cs", inv_folder);
            FilesUtil.ModifyFileAtPath(vItemCollectionDisplay, new List<FilesUtil.Modification>()
                {
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "public void FadeText(string message, float timeToStay, float timeToFadeOut)" },
                        modifiedLines: new List<string>() {
                            "//public void FadeText(string message, float timeToStay, float timeToFadeOut)",
                            "public virtual void FadeText(string message, float timeToStay, float timeToFadeOut)"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    )
                }
            );
            #endregion

            #region Convert vCheckCanAddHealth.cs
            progressUpdate?.Invoke(progress_title, "Converting vCheckCanAddHealth.cs...", 0.28f);
            yield return new EditorWaitForSeconds(0.01f);
            string vCheckCanAddHealth = FilesUtil.FindFilePath("vCheckCanAddHealth.cs", inv_folder);
            FilesUtil.ModifyFileAtPath(vCheckCanAddHealth, new List<FilesUtil.Modification>()
                {
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "using System.Collections.Generic;" },
                        modifiedLines: new List<string>() {
                            "using Mirror;"
                        },
                        modificationType: FilesUtil.ModiftyType.AddBefore
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "if (itemManager)" },
                        modifiedLines: new List<string>() {
                            "//if (itemManager)",
                            "if (itemManager && ((itemManager.gameObject.GetComponent<NetworkIdentity>().hasAuthority && NetworkClient.active) || (!NetworkClient.active && !NetworkServer.active)))"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    )
                }
            );
            #endregion

            #region Convert vBreakableObject.cs
            progressUpdate?.Invoke(progress_title, "Converting vBreakableObject.cs...", 0.35f);
            yield return new EditorWaitForSeconds(0.01f);
            string vBreakableObject = FilesUtil.FindFilePath("vBreakableObject.cs", melee_folder);
            FilesUtil.ModifyFileAtPath(vBreakableObject, new List<FilesUtil.Modification>()
                {
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "IEnumerator BreakObjet()" },
                        modifiedLines: new List<string>() {
                            "//IEnumerator BreakObjet()",
                            "protected virtual IEnumerator BreakObjet()"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "private Rigidbody _rigidBody;" },
                        modifiedLines: new List<string>() {
                            "//private Rigidbody _rigidBody;",
                            "protected Rigidbody _rigidBody;"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "private Collider _collider;" },
                        modifiedLines: new List<string>() {
                            "//private Collider _collider;",
                            "protected Collider _collider;"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "private bool isBroken;" },
                        modifiedLines: new List<string>() {
                            "//private bool isBroken;",
                            "protected bool isBroken;"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    )
                }
            );
            #endregion

            #region Convert vLockOnBehaviour.cs
            progressUpdate?.Invoke(progress_title, "Converting vLockOnBehaviour.cs...", 0.42f);
            yield return new EditorWaitForSeconds(0.01f);
            string vLockOnBehaviour = FilesUtil.FindFilePath("vLockOnBehaviour.cs", melee_folder);
            FilesUtil.ModifyFileAtPath(vLockOnBehaviour, new List<FilesUtil.Modification>()
                {
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "protected Transform target;" },
                        modifiedLines: new List<string>() {
                            "//protected Transform target;",
                            "protected Transform _target;",
                            "protected virtual Transform target { get { return _target; } set { _target = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "private bool _inLockOn;" },
                        modifiedLines: new List<string>() {
                            "//private bool _inLockOn;",
                            "protected bool _inLockOn;"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "protected List<Transform> GetPossibleTargets()" },
                        modifiedLines: new List<string>() {
                            "//protected List<Transform> GetPossibleTargets()",
                            "protected virtual List<Transform> GetPossibleTargets()"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "private Transform watcher;" },
                        modifiedLines: new List<string>() {
                            "//private Transform watcher;",
                            "protected Transform watcher;"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    )
                }
            );
            #endregion

            #region Convert vLockOn.cs
            progressUpdate?.Invoke(progress_title, "Converting vLockOn.cs...", 0.49f);
            yield return new EditorWaitForSeconds(0.01f);
            string vLockOn = FilesUtil.FindFilePath("vLockOn.cs", melee_folder);
            FilesUtil.ModifyFileAtPath(vLockOn, new List<FilesUtil.Modification>()
                {
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "protected bool inTarget;" },
                        modifiedLines: new List<string>() {
                            "//protected bool inTarget;",
                            "protected bool _inTarget;",
                            "protected virtual bool inTarget { get { return _inTarget; } set { _inTarget = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    )
                }
            );
            #endregion

            #region Convert v_AIMotor.cs
            progressUpdate?.Invoke(progress_title, "Converting v_AIMotor.cs...", 0.56f);
            yield return new EditorWaitForSeconds(0.01f);
            string v_AIMotor = FilesUtil.FindFilePath("v_AIMotor.cs", melee_folder);
            if (!string.IsNullOrEmpty(v_AIMotor))
            {
                FilesUtil.ModifyFileAtPath(v_AIMotor, new List<FilesUtil.Modification>()
                {
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "protected Vector3 targetPos;" },
                        modifiedLines: new List<string>() {
                            "//protected Vector3 targetPos;",
                            "protected Vector3 _targetPos;",
                            "protected virtual Vector3 targetPos { get { return _targetPos; } set { _targetPos = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "protected bool canSeeTarget;" },
                        modifiedLines: new List<string>() {
                            "//protected bool canSeeTarget;",
                            "protected bool _canSeeTarget;",
                            "protected virtual bool canSeeTarget { get { return _canSeeTarget; } set { _canSeeTarget = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "protected Vector3 destination;" },
                        modifiedLines: new List<string>() {
                            "//protected Vector3 destination;",
                            "protected Vector3 _destination;",
                            "protected virtual Vector3 destination { get { return _destination; } set { _destination = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "protected Vector3 fwd;" },
                        modifiedLines: new List<string>() {
                            "//protected Vector3 fwd;",
                            "protected Vector3 _fwd;",
                            "protected virtual Vector3 fwd { get { return _fwd; } set { _fwd = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "protected bool isGrounded;" },
                        modifiedLines: new List<string>() {
                            "//protected bool isGrounded;",
                            "protected bool _isGrounded;",
                            "protected virtual bool isGrounded { get { return _isGrounded; } set { _isGrounded = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "protected bool isStrafing;" },
                        modifiedLines: new List<string>() {
                            "//protected bool isStrafing;",
                            "protected bool _isStrafing;",
                            "protected virtual bool isStrafing { get { return _isStrafing; } set { _isStrafing = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "protected int currentWaypoint;" },
                        modifiedLines: new List<string>() {
                            "//protected int currentWaypoint;",
                            "protected int _currentWaypoint;",
                            "protected virtual int currentWaypoint { get { return _currentWaypoint; } set { _currentWaypoint = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "protected int currentPatrolPoint;" },
                        modifiedLines: new List<string>() {
                            "//protected int currentPatrolPoint;",
                            "protected int _currentPatrolPoint;",
                            "protected virtual int currentPatrolPoint { get { return _currentPatrolPoint; } set { _currentPatrolPoint = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "protected float direction;" },
                        modifiedLines: new List<string>() {
                            "//protected float direction;",
                            "protected float _timer, _wait;",
                            "protected float _fovAngle;",
                            "protected float _direction;",
                            "protected virtual float direction { get { return _direction; } set { _direction = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "protected float timer, wait;" },
                        modifiedLines: new List<string>() {
                            "//protected float timer, wait;"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "protected float fovAngle;" },
                        modifiedLines: new List<string>() {
                            "//protected float fovAngle;",
                            "protected virtual float timer { get { return _timer; } set { _timer = value; } }",
                            "protected virtual float wait { get { return _wait; } set { _wait = value; } }",
                            "protected virtual float fovAngle { get { return _fovAngle; } set { _fovAngle = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "protected Vector3 startPosition;" },
                        modifiedLines: new List<string>() {
                            "//protected Vector3 startPosition;",
                            "protected Vector3 _startPosition;",
                            "protected virtual Vector3 startPosition { get { return _startPosition; } set { _startPosition = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        previousLines: new List<string>() { "protected bool" },
                        targetLines: new List<string>() { "isCrouched," },
                        modifiedLines: new List<string>() {
                            "//isCrouched,"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        previousLines: new List<string>() { "protected bool" },
                        targetLines: new List<string>() { "canAttack," },
                        modifiedLines: new List<string>() {
                            "//canAttack,"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        previousLines: new List<string>() { "protected bool" },
                        targetLines: new List<string>() { "isRolling;" },
                        modifiedLines: new List<string>() {
                            "//isRolling;"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "protected bool" },
                        modifiedLines: new List<string>() {
                            "//protected bool"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "public bool isBlocking { get; protected set; }" },
                        modifiedLines: new List<string>() {
                            "protected bool _isCrouched;",
                            "protected bool _canAttack;",
                            "protected bool _tryingBlock;",
                            "protected bool _isRolling;",
                            "protected virtual bool isCrouched { get { return _isCrouched; } set { _isCrouched = value; } }",
                            "protected virtual bool canAttack { get { return _canAttack; } set { _canAttack = value; } }",
                            "protected virtual bool tryingBlock { get { return _tryingBlock; } set { _tryingBlock = value; } }",
                            "protected virtual bool isRolling { get { return _isRolling; } set { _isRolling = value; } }",
                            "protected bool _isBlocking;",
                            "protected bool _isAttacking;"
                        },
                        modificationType: FilesUtil.ModiftyType.AddBefore
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "public bool isBlocking { get; protected set; }" },
                        modifiedLines: new List<string>() {
                            "//public bool isBlocking { get; protected set; }",
                            "public virtual bool isBlocking { get { return _isBlocking; } protected set { _isBlocking = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "public bool isAttacking { get; protected set; }" },
                        modifiedLines: new List<string>() {
                            "//public bool isAttacking { get; protected set; }",
                            "public bool isAttacking { get { return _isAttacking; } protected set { _isAttacking = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "public AIStates currentState = AIStates.PatrolWaypoints;" },
                        modifiedLines: new List<string>() {
                            "protected AIStates _currentState = AIStates.PatrolWaypoints;",
                            "public virtual AIStates currentState { get { return _currentState; } set { _currentState = value; } }",
                            "//public AIStates currentState = AIStates.PatrolWaypoints;"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "public bool actions;" },
                        modifiedLines: new List<string>() {
                            "//public bool actions;",
                            "protected bool _actions;",
                            "public virtual bool actions { get { return _actions; } set { _actions = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                }
                );
            }
            #endregion

            #region Convert v_AIController.cs
            progressUpdate?.Invoke(progress_title, "Converting v_AIController.cs...", 0.63f);
            yield return new EditorWaitForSeconds(0.01f);
            string v_AIController = FilesUtil.FindFilePath("v_AIController.cs", melee_folder);
            if (!string.IsNullOrEmpty(v_AIController))
            {
                FilesUtil.ModifyFileAtPath(v_AIController, new List<FilesUtil.Modification>()
                {
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "protected Vector3 moveToDestination;" },
                        modifiedLines: new List<string>() {
                            "//protected Vector3 moveToDestination;",
                            "protected Vector3 _moveToDestination;",
                            "protected virtual Vector3 moveToDestination { get { return _moveToDestination; } set { _moveToDestination = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "protected float moveToSpeed;" },
                        modifiedLines: new List<string>() {
                            "//protected float moveToSpeed;",
                            "protected float _moveToSpeed;",
                            "protected virtual float moveToSpeed { get { return _moveToSpeed; } set { _moveToSpeed = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "protected IEnumerator MeleeAttackRotine()" },
                        modifiedLines: new List<string>() {
                            "//protected IEnumerator MeleeAttackRotine()",
                            "protected virtual IEnumerator MeleeAttackRotine()"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    )
                }
                );
            }
            #endregion

            #region Convert vSimpleMeleeAI_Motor.cs
            progressUpdate?.Invoke(progress_title, "Converting vSimpleMeleeAI_Motor.cs...", 0.70f);
            yield return new EditorWaitForSeconds(0.01f);
            string vSimpleMeleeAI_Motor = FilesUtil.FindFilePath("vSimpleMeleeAI_Motor.cs", melee_folder);
            if (!string.IsNullOrEmpty(vSimpleMeleeAI_Motor))
            {
                FilesUtil.ModifyFileAtPath(vSimpleMeleeAI_Motor, new List<FilesUtil.Modification>()
                {
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "protected Vector3 targetPos;" },
                        modifiedLines: new List<string>() {
                            "//protected Vector3 targetPos;",
                            "protected Vector3 _targetPos;",
                            "protected virtual Vector3 targetPos { get { return _targetPos; } set { _targetPos = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "protected bool canSeeTarget;" },
                        modifiedLines: new List<string>() {
                            "//protected bool canSeeTarget;",
                            "protected bool _canSeeTarget;",
                            "protected virtual bool canSeeTarget { get { return _canSeeTarget; } set { _canSeeTarget = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "protected Vector3 destination;" },
                        modifiedLines: new List<string>() {
                            "//protected Vector3 destination;",
                            "protected Vector3 _destination;",
                            "protected virtual Vector3 destination { get { return _destination; } set { _destination = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "protected Vector3 fwd;" },
                        modifiedLines: new List<string>() {
                            "//protected Vector3 fwd;",
                            "protected Vector3 _fwd;",
                            "protected virtual Vector3 fwd { get { return _fwd; } set { _fwd = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "protected bool isGrounded;" },
                        modifiedLines: new List<string>() {
                            "//protected bool isGrounded;",
                            "protected bool _isGrounded;",
                            "protected virtual bool isGrounded { get { return _isGrounded; } set { _isGrounded = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "protected bool isStrafing;" },
                        modifiedLines: new List<string>() {
                            "//protected bool isStrafing;",
                            "protected bool _isStrafing;",
                            "protected virtual bool isStrafing { get { return _isStrafing; } set { _isStrafing = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "protected int currentWaypoint;" },
                        modifiedLines: new List<string>() {
                            "//protected int currentWaypoint;",
                            "protected int _currentWaypoint;",
                            "protected virtual int currentWaypoint { get { return _currentWaypoint; } set { _currentWaypoint = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "protected int currentPatrolPoint;" },
                        modifiedLines: new List<string>() {
                            "//protected int currentPatrolPoint;",
                            "protected int _currentPatrolPoint;",
                            "protected virtual int currentPatrolPoint { get { return _currentPatrolPoint; } set { _currentPatrolPoint = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "protected float direction;" },
                        modifiedLines: new List<string>() {
                            "//protected float direction;",
                            "protected float _direction;",
                            "protected virtual float direction { get { return _direction; } set { _direction = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "protected float timer, wait;" },
                        modifiedLines: new List<string>() {
                            "//protected float timer, wait;",
                            "protected float _timer, _wait;",
                            "protected virtual float timer { get { return _timer; } set { _timer = value; } }",
                            "protected virtual float wait { get { return _wait; } set { _wait = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "protected float fovAngle;" },
                        modifiedLines: new List<string>() {
                            "//protected float fovAngle;",
                            "protected float _fovAngle;",
                            "protected virtual float fovAngle { get { return _fovAngle; } set { _fovAngle = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "protected Vector3 startPosition;" },
                        modifiedLines: new List<string>() {
                            "//protected Vector3 startPosition;",
                            "protected Vector3 _startPosition;",
                            "protected virtual Vector3 startPosition { get { return _startPosition; } set { _startPosition = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        previousLines: new List<string>() { "protected bool" },
                        targetLines: new List<string>() { "isCrouched," },
                        modifiedLines: new List<string>() {
                            "//isCrouched,"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        previousLines: new List<string>() { "protected bool" },
                        targetLines: new List<string>() { "canAttack," },
                        modifiedLines: new List<string>() {
                            "//canAttack,"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        previousLines: new List<string>() { "protected bool" },
                        targetLines: new List<string>() { "tryingBlock," },
                        modifiedLines: new List<string>() {
                            "//tryingBlock,"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        previousLines: new List<string>() { "protected bool" },
                        targetLines: new List<string>() { "isRolling;" },
                        modifiedLines: new List<string>() {
                            "//isRolling;"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "protected bool" },
                        modifiedLines: new List<string>() {
                            "//protected bool"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "public bool isBlocking { get; protected set; }" },
                        modifiedLines: new List<string>() {
                            "//public bool isBlocking { get; protected set; }",
                            "protected bool _isCrouched;",
                            "protected bool _canAttack;",
                            "protected bool _tryingBlock;",
                            "protected bool _isRolling;",
                            "protected virtual bool isCrouched { get { return _isCrouched; } set { _isCrouched = value; } }",
                            "protected virtual bool canAttack { get { return _canAttack; } set { _canAttack = value; } }",
                            "protected virtual bool tryingBlock { get { return _tryingBlock; } set { _tryingBlock = value; } }",
                            "protected virtual bool isRolling { get { return _isRolling; } set { _isRolling = value; } }",
                            "protected bool _isBlocking;",
                            "protected bool _isAttacking;",
                            "public virtual bool isBlocking { get { return _isBlocking; } protected set { _isBlocking = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "public bool isAttacking { get; protected set; }" },
                        modifiedLines: new List<string>() {
                            "//public bool isAttacking { get; protected set; }",
                            "public bool isAttacking { get { return _isAttacking; } protected set { _isAttacking = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "public AIStates currentState = AIStates.PatrolWaypoints;" },
                        modifiedLines: new List<string>() {
                            "protected AIStates _currentState = AIStates.PatrolWaypoints;",
                            "public virtual AIStates currentState { get { return _currentState; } set { _currentState = value; } }",
                            "//public AIStates currentState = AIStates.PatrolWaypoints;"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "public bool actions;" },
                        modifiedLines: new List<string>() {
                            "//public bool actions;",
                            "protected bool _actions;",
                            "public virtual bool actions { get { return _actions; } set { _actions = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                }
                );
            }
            #endregion

            #region Convert vSimpleMeleeAI_Controller.cs
            progressUpdate?.Invoke(progress_title, "Converting vSimpleMeleeAI_Controller.cs...", 0.77f);
            yield return new EditorWaitForSeconds(0.01f);
            string vSimpleMeleeAI_Controller = FilesUtil.FindFilePath("vSimpleMeleeAI_Controller.cs", melee_folder);
            if (!string.IsNullOrEmpty(vSimpleMeleeAI_Controller))
            {
                FilesUtil.ModifyFileAtPath(vSimpleMeleeAI_Controller, new List<FilesUtil.Modification>()
                {
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "protected Vector3 moveToDestination;" },
                        modifiedLines: new List<string>() {
                            "//protected Vector3 moveToDestination;",
                            "protected Vector3 _moveToDestination;",
                            "protected virtual Vector3 moveToDestination { get { return _moveToDestination; } set { _moveToDestination = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "protected float moveToSpeed;" },
                        modifiedLines: new List<string>() {
                            "//protected float moveToSpeed;",
                            "protected float _moveToSpeed;",
                            "protected virtual float moveToSpeed { get { return _moveToSpeed; } set { _moveToSpeed = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "protected IEnumerator MeleeAttackRotine()" },
                        modifiedLines: new List<string>() {
                            "//protected IEnumerator MeleeAttackRotine()",
                            "protected virtual IEnumerator MeleeAttackRotine()"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    )
                }
                );
            }
            #endregion

            #region Convert vItemSlot.cs
            progressUpdate?.Invoke(progress_title, "Converting vItemSlot.cs...", 0.84f);
            yield return new EditorWaitForSeconds(0.01f);
            string vItemSlot = FilesUtil.FindFilePath("vItemSlot.cs", inv_folder);
            FilesUtil.ModifyFileAtPath(vItemSlot, new List<FilesUtil.Modification>()
                {
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "private void UpdateDisplays(vItem item)" },
                        modifiedLines: new List<string>() {
                            "//private void UpdateDisplays(vItem item)",
                            "protected virtual void UpdateDisplays(vItem item)"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    )
                }
            );
            #endregion

            #region Convert vWeaponConstrain
            progressUpdate?.Invoke(progress_title, "Converting vWeaponConstrain.cs...", 0.95f);
            yield return new EditorWaitForSeconds(0.01f);
            string vWeaponConstrain = FilesUtil.FindFilePath("vWeaponConstrain.cs", melee_folder);
            FilesUtil.ModifyFileAtPath(vWeaponConstrain, new List<FilesUtil.Modification>()
                {
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "public void Inv_Weapon_FreezeAll(bool status)" },
                        modifiedLines: new List<string>() {
                            "//public void Inv_Weapon_FreezeAll(bool status)",
                            "public virtual void Inv_Weapon_FreezeAll(bool status)"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "void Start()" },
                        modifiedLines: new List<string>() {
                            "//void Start()",
                            "protected virtual void Start()"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    )
                }
            );
            #endregion

            #region Convert vEquipArea
            progressUpdate?.Invoke(progress_title, "Converting vEquipArea.cs...", 0.97f);
            yield return new EditorWaitForSeconds(0.01f);
            string vEquipArea = FilesUtil.FindFilePath("vEquipArea.cs", inv_folder);
            FilesUtil.ModifyFileAtPath(vEquipArea, new List<FilesUtil.Modification>()
                {
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "public void OnSubmitSlot(vItemSlot slot)" },
                        modifiedLines: new List<string>() {
                            "//public void OnSubmitSlot(vItemSlot slot)",
                            "public virtual void OnSubmitSlot(vItemSlot slot)"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "public void NextEquipSlot()" },
                        modifiedLines: new List<string>() {
                            "//public void NextEquipSlot()",
                            "public virtual void NextEquipSlot()"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "public void PreviousEquipSlot()" },
                        modifiedLines: new List<string>() {
                            "//public void PreviousEquipSlot()",
                            "public virtual void PreviousEquipSlot()"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    )
                }
            );
            #endregion

            #region Callback
            progressUpdate?.Invoke(progress_title, "Completing...", 1f);
            yield return new EditorWaitForSeconds(0.01f);
            if (callInvoke) callbackFunction?.Invoke();
            #endregion
        }
    }
}
