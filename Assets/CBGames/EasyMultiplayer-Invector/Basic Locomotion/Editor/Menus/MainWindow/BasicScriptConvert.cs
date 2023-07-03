using Common.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.EditorCoroutines.Editor;
using UnityEngine;

namespace EMI.Editors.Windows
{
    public class BasicScriptConvert
    {
        public virtual IEnumerator Run(Action callbackFunction = null, bool callInvoke = true, Action<string, string, float> progressUpdate = null)
        {

            string progress_title = "Converting Basic Scripts";
            string dataPath = Application.dataPath.Replace('/', Path.DirectorySeparatorChar);
            string inv_basic_folder = FilesUtil.FindFolderPath("Invector-3rdPersonController/Basic Locomotion");

            #region Convert vCharacter.cs
            progressUpdate(progress_title, "Converting vCharacter.cs...", 0.07f);
            yield return new EditorWaitForSeconds(0.01f);
            string vCharacter = FilesUtil.FindFilePath("vCharacter.cs", inv_basic_folder);
            FilesUtil.ModifyFileAtPath(vCharacter, new List<FilesUtil.Modification>()
                {
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "public bool ragdolled { get; set; }" },
                        modifiedLines: new List<string>() {
                            "//public bool ragdolled { get; set; }",
                            "public bool _ragdolled = false;",
                            "public virtual bool ragdolled { get { return _ragdolled; } set { _ragdolled = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    )
                }
            );
            #endregion

            #region Convert vThirdPersonInput.cs
            progressUpdate?.Invoke(progress_title, "Converting vThirdPersonInput.cs...", 0.14f);
            yield return new EditorWaitForSeconds(0.01f);
            string vThirdPersonInput = FilesUtil.FindFilePath("vThirdPersonInput.cs", inv_basic_folder);
            FilesUtil.ModifyFileAtPath(vThirdPersonInput, new List<FilesUtil.Modification>()
                {
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "public Camera cameraMain" },
                        modifiedLines: new List<string>() {
                            "//public Camera cameraMain",
                            "public virtual Camera cameraMain"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "private bool _toogleWalk;" },
                        modifiedLines: new List<string>() {
                            "//private bool _toogleWalk;",
                            "protected bool _toogleWalk;"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "[HideInInspector] public bool lockInput;" },
                        modifiedLines: new List<string>() {
                            "//[HideInInspector] public bool lockInput;",
                            "protected bool _lockInput = false;",
                            "[HideInInspector] public virtual bool lockInput { get { return _lockInput; } set { _lockInput = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    )
                }
            );
            #endregion

            #region Convert vHeadTrack.cs
            progressUpdate?.Invoke(progress_title, "Converting vHeadTrack.cs...", 0.21f);
            yield return new EditorWaitForSeconds(0.01f);
            string vHeadTrack = FilesUtil.FindFilePath("vHeadTrack.cs", inv_basic_folder);
            FilesUtil.ModifyFileAtPath(vHeadTrack, new List<FilesUtil.Modification>()
                {
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "Vector3 GetLookPoint()" },
                        modifiedLines: new List<string>() {
                            "//Vector3 GetLookPoint()",
                            "protected virtual Vector3 GetLookPoint()"
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
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "bool lookConditions" },
                        modifiedLines: new List<string>() {
                            "//bool lookConditions",
                            "protected virtual bool lookConditions"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "private float temporaryLookTime;" },
                        modifiedLines: new List<string>() {
                            "//private float temporaryLookTime;",
                            "protected float temporaryLookTime;"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "Vector3 headPoint { get { return transform.position + (transform.up * headHeight); } }" },
                        modifiedLines: new List<string>() {
                            "//Vector3 headPoint { get { return transform.position + (transform.up * headHeight); } }",
                            "protected Vector3 headPoint { get { return transform.position + (transform.up * headHeight); } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "private Transform simpleTarget;" },
                        modifiedLines: new List<string>() {
                            "//private Transform simpleTarget;",
                            "protected Transform simpleTarget;"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "bool TargetIsOnRange(Vector3 direction)" },
                        modifiedLines: new List<string>() {
                            "//bool TargetIsOnRange(Vector3 direction)",
                            "protected bool TargetIsOnRange(Vector3 direction)"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "private Vector3 temporaryLookPoint;" },
                        modifiedLines: new List<string>() {
                            "//private Vector3 temporaryLookPoint;",
                            "protected Vector3 temporaryLookPoint;"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "Vector2 GetTargetAngle(Vector3 direction)" },
                        modifiedLines: new List<string>() {
                            "//Vector2 GetTargetAngle(Vector3 direction)",
                            "protected Vector2 GetTargetAngle(Vector3 direction)"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "void SmoothValues(float _headWeight = 0, float _bodyWeight = 0, float _x = 0, float _y = 0)" },
                        modifiedLines: new List<string>() {
                            "//void SmoothValues(float _headWeight = 0, float _bodyWeight = 0, float _x = 0, float _y = 0)",
                            "protected void SmoothValues(float _headWeight = 0, float _bodyWeight = 0, float _x = 0, float _y = 0)"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "void SortTargets()" },
                        modifiedLines: new List<string>() {
                            "//void SortTargets()",
                            "protected void SortTargets()"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "private float yRotation, xRotation;" },
                        modifiedLines: new List<string>() {
                            "//private float yRotation, xRotation;",
                            "protected float yRotation, xRotation;"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    )
                }
            );
            #endregion

            #region Convert vGenericAction.cs
            progressUpdate?.Invoke(progress_title, "Converting vGenericAction.cs...", 0.28f);
            yield return new EditorWaitForSeconds(0.01f);
            string vGenericAction = FilesUtil.FindFilePath("vGenericAction.cs", inv_basic_folder);
            FilesUtil.ModifyFileAtPath(vGenericAction, new List<FilesUtil.Modification>()
                {
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "private void HandleColliders()" },
                        modifiedLines: new List<string>() {
                            "//private void HandleColliders()",
                            "protected virtual void HandleColliders()"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "private void TriggerActionEventsInput()" },
                        modifiedLines: new List<string>() {
                            "//private void TriggerActionEventsInput()",
                            "protected virtual void TriggerActionEventsInput()"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "private void CancelButtonTimer()" },
                        modifiedLines: new List<string>() {
                            "//private void TriggerActionEventsInput()",
                            "protected virtual void CancelButtonTimer()"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "[vReadOnly] public vTriggerGenericAction triggerAction;" },
                        modifiedLines: new List<string>() {
                            "//[vReadOnly] public vTriggerGenericAction triggerAction;",
                            "[vReadOnly] protected vTriggerGenericAction _triggerAction;",
                            "public virtual vTriggerGenericAction triggerAction { get { return _triggerAction; } set { _triggerAction = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "protected vTriggerGenericAction GetNearAction()" },
                        modifiedLines: new List<string>() {
                            "//protected vTriggerGenericAction GetNearAction()",
                            "protected virtual vTriggerGenericAction GetNearAction()"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "protected vTriggerGenericAction GetNearAction()" },
                        modifiedLines: new List<string>() {
                            "//protected vTriggerGenericAction GetNearAction()",
                            "protected virtual vTriggerGenericAction GetNearAction()"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    )
                }
            );
            #endregion

            #region Convert vTutorialTextTrigger.cs
            progressUpdate?.Invoke(progress_title, "Converting vTutorialTextTrigger.cs...", 0.35f);
            yield return new EditorWaitForSeconds(0.01f);
            string tutorialTextTrigger = FilesUtil.FindFilePath("vTutorialTextTrigger.cs", inv_basic_folder);
            FilesUtil.ModifyFileAtPath(tutorialTextTrigger, new List<FilesUtil.Modification>()
                {
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "private void OnTriggerEnter(Collider other)" },
                        modifiedLines: new List<string>() {
                            "//private void OnTriggerEnter(Collider other)",
                            "protected virtual void OnTriggerEnter(Collider other)"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "private void OnTriggerExit(Collider other)" },
                        modifiedLines: new List<string>() {
                            "//private void OnTriggerExit(Collider other)",
                            "protected virtual void OnTriggerExit(Collider other)"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    )
                }
            );
            #endregion

            #region Convert vHealthController.cs
            progressUpdate?.Invoke(progress_title, "Converting vHealthController.cs...", 0.42f);
            yield return new EditorWaitForSeconds(0.01f);
            string vHealthController = FilesUtil.FindFilePath("vHealthController.cs", inv_basic_folder);
            FilesUtil.ModifyFileAtPath(vHealthController, new List<FilesUtil.Modification>()
                {
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "public float currentHealth" },
                        modifiedLines: new List<string>() {
                            "//public float currentHealth",
                            "public virtual float currentHealth"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "public float currentHealthRecoveryDelay;" },
                        modifiedLines: new List<string>() {
                            "//public float currentHealthRecoveryDelay;",
                            "protected float _currentHealthRecoveryDelay;",
                            "public virtual float currentHealthRecoveryDelay { get { return _currentHealthRecoveryDelay;  } set { _currentHealthRecoveryDelay = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "public bool isDead" },
                        modifiedLines: new List<string>() {
                            "//public bool isDead",
                            "public virtual bool isDead"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "public int MaxHealth" },
                        modifiedLines: new List<string>() {
                            "//public int MaxHealth",
                            "public virtual int MaxHealth"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "public float healthRecovery = 0f;" },
                        modifiedLines: new List<string>() {
                            "//public float healthRecovery = 0f;",
                            "[SerializeField] protected float _healthRecovery = 0;",
                            "public virtual float healthRecovery { get { return _healthRecovery; } set { _healthRecovery = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                }
            );
            #endregion

            #region Convert vThirdPersonMotor.cs
            progressUpdate?.Invoke(progress_title, "Converting vThirdPersonMotor.cs...", 0.49f);
            yield return new EditorWaitForSeconds(0.01f);
            string vThirdPersonMotor = FilesUtil.FindFilePath("vThirdPersonMotor.cs", inv_basic_folder);
            FilesUtil.ModifyFileAtPath(vThirdPersonMotor, new List<FilesUtil.Modification>()
                {
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "public bool isStrafing" },
                        modifiedLines: new List<string>() {
                            "//public bool isStrafing",
                            "public virtual bool isStrafing"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    )
                }
            );
            #endregion

            #region Convert vThirdPersonCamera.cs
            progressUpdate?.Invoke(progress_title, "Converting vThirdPersonCamera.cs...", 0.56f);
            yield return new EditorWaitForSeconds(0.01f);
            string vThirdPersonCamera = FilesUtil.FindFilePath("vThirdPersonCamera.cs", inv_basic_folder);
            FilesUtil.ModifyFileAtPath(vThirdPersonCamera, new List<FilesUtil.Modification>()
                {
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "internal float switchRight;" },
                        modifiedLines: new List<string>() {
                            "//internal float switchRight;",
                            "internal float _switchRight;",
                            "internal virtual float switchRight { get { return _switchRight; } set { _switchRight = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    )
                }
            );
            #endregion

            #region Convert vSimpleDoor.cs
            progressUpdate?.Invoke(progress_title, "Converting vSimpleDoor.cs...", 0.63f);
            yield return new EditorWaitForSeconds(0.01f);
            string vSimpleDoor = FilesUtil.FindFilePath("vSimpleDoor.cs", inv_basic_folder);
            FilesUtil.ModifyFileAtPath(vSimpleDoor, new List<FilesUtil.Modification>()
                {
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "public bool startOpened;" },
                        modifiedLines: new List<string>() {
                            "//public bool startOpened;",
                            "[SerializeField] protected bool _startOpened;",
                            "public virtual bool startOpened { get { return _startOpened; } set { _startOpened = value; }}"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "public bool autoOpen = true;" },
                        modifiedLines: new List<string>() {
                            "//public bool autoOpen = true;",
                            "[SerializeField] protected bool _autoOpen = true;",
                            "public virtual bool autoOpen { get { return _autoOpen; } set { _autoOpen = value; }}"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "public bool autoClose = true;" },
                        modifiedLines: new List<string>() {
                            "//public bool autoClose = true;",
                            "[SerializeField] protected bool _autoClose = true;",
                            "public virtual bool autoClose { get { return _autoClose; } set { _autoClose = value; }}"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    )
                }
            );
            #endregion

            #region Convert vEventWithDelay.cs
            progressUpdate?.Invoke(progress_title, "Converting vEventWithDelay.cs...", 0.70f);
            yield return new EditorWaitForSeconds(0.01f);
            string vEventWithDelay = FilesUtil.FindFilePath("vEventWithDelay.cs", inv_basic_folder);
            FilesUtil.ModifyFileAtPath(vEventWithDelay, new List<FilesUtil.Modification>()
                {
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "private void Start()" },
                        modifiedLines: new List<string>() {
                            "//private void Start()",
                            "protected virtual void Start()"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "public void DoEvents()" },
                        modifiedLines: new List<string>() {
                            "//public void DoEvents()",
                            "public virtual void DoEvents()"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "public void DoEvent(int index)" },
                        modifiedLines: new List<string>() {
                            "//public void DoEvent(int index)",
                            "public virtual void DoEvent(int index)"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "public void DoEvent(string name)" },
                        modifiedLines: new List<string>() {
                            "//public void DoEvent(string name)",
                            "public virtual void DoEvent(string name)"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    )
                }
            );
            #endregion

            #region Convert vFadeCanvas.cs
            progressUpdate?.Invoke(progress_title, "Converting vFadeCanvas.cs...", 0.77f);
            yield return new EditorWaitForSeconds(0.01f);
            string vFadeCanvas = FilesUtil.FindFilePath("vFadeCanvas.cs", inv_basic_folder);
            FilesUtil.ModifyFileAtPath(vFadeCanvas, new List<FilesUtil.Modification>()
                {
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "private void Start()" },
                        modifiedLines: new List<string>() {
                            "//private void Start()",
                            "protected virtual void Start()"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "public void FadeIn()" },
                        modifiedLines: new List<string>() {
                            "//public void FadeIn()",
                            "public virtual void FadeIn()"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "public void FadeOut()" },
                        modifiedLines: new List<string>() {
                            "//public void FadeOut()",
                            "public virtual void FadeOut()"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "public void AlphaFull()" },
                        modifiedLines: new List<string>() {
                            "//public void AlphaFull()",
                            "public virtual void AlphaFull()"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "public void AlphaZero()" },
                        modifiedLines: new List<string>() {
                            "//public void AlphaZero()",
                            "public virtual void AlphaZero()"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "private void InitilizeFadeEffect()" },
                        modifiedLines: new List<string>() {
                            "//private void InitilizeFadeEffect()",
                            "protected virtual void InitilizeFadeEffect()"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    )
                }
            );
            #endregion

            #region Convert vHitDamageParticle.cs
            progressUpdate?.Invoke(progress_title, "Converting vHitDamageParticle.cs...", 0.84f);
            yield return new EditorWaitForSeconds(0.01f);
            string vHitDamageParticle = FilesUtil.FindFilePath("vHitDamageParticle.cs", inv_basic_folder);
            FilesUtil.ModifyFileAtPath(vHitDamageParticle, new List<FilesUtil.Modification>()
                {
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "public void OnReceiveDamage(vDamage damage)" },
                        modifiedLines: new List<string>() {
                            "//public void OnReceiveDamage(vDamage damage)",
                            "public virtual void OnReceiveDamage(vDamage damage)"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    )
                }
            );
            #endregion

            #region vLadderAction.cs
            progressUpdate?.Invoke(progress_title, "Converting vLadderAction.cs...", 0.91f);
            yield return new EditorWaitForSeconds(0.01f);
            string vLadderAction = FilesUtil.FindFilePath("vLadderAction.cs", inv_basic_folder);
            FilesUtil.ModifyFileAtPath(vLadderAction, new List<FilesUtil.Modification>()
                {
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "protected bool isUsingLadder;" },
                        modifiedLines: new List<string>() {
                            "//protected bool isUsingLadder;",
                            "protected bool _isUsingLadder;",
                            "protected virtual bool isUsingLadder { get { return _isUsingLadder; } set { _isUsingLadder = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    )
                }
            );
            #endregion

            #region vObjectDamage.cs
            progressUpdate?.Invoke(progress_title, "Converting vObjectDamage.cs...", 0.95f);
            yield return new EditorWaitForSeconds(0.01f);
            string vObjectDamage = FilesUtil.FindFilePath("vObjectDamage.cs", inv_basic_folder);
            FilesUtil.ModifyFileAtPath(vObjectDamage, new List<FilesUtil.Modification>()
                {
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "private List<Collider> targets;" },
                        modifiedLines: new List<string>() {
                            "//private List<Collider> targets;",
                            "protected List<Collider> targets;"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    )
                }
            );
            #endregion

            #region Callback
            progressUpdate(progress_title, "Completing...", 1f);
            yield return new EditorWaitForSeconds(0.01f);
            if (callInvoke) callbackFunction?.Invoke();
            #endregion

            yield return null;
        }
    }
}
