using Common.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.EditorCoroutines.Editor;
using UnityEngine;

namespace EMI.Editors.Windows
{
    public class ShooterScriptConvert : MeleeScriptConvert
    {
        public override IEnumerator Run(Action callbackFunction = null, bool callInvoke = true, Action<string, string, float> progressUpdate = null)
        {
            #region Run MeleeScriptConvert
            yield return base.Run(null, false, progressUpdate);
            #endregion

            string progress_title = "Converting Shooter Scripts";
            string dataPath = Application.dataPath.Replace('/', Path.DirectorySeparatorChar);
            string inv_shooter_folder = FilesUtil.FindFolderPath("Invector-3rdPersonController/Shooter");

            #region Convert vShooterWeaponBase.cs
            progressUpdate?.Invoke(progress_title, "Converting vShooterWeaponBase.cs...", 0.14f);
            yield return new EditorWaitForSeconds(0.01f);
            string vShooterWeaponBase = FilesUtil.FindFilePath("vShooterWeaponBase.cs", inv_shooter_folder);
            FilesUtil.ModifyFileAtPath(vShooterWeaponBase, new List<FilesUtil.Modification>()
                {
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "public int ammo;" },
                        modifiedLines: new List<string>() {
                            "//public int ammo;",
                            "protected int _ammo;",
                            "public virtual int ammo { get { return _ammo; } set { _ammo = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "[Tooltip(\"Frequency of shots\")]" },
                        modifiedLines: new List<string>() {
                            "//[Tooltip(\"Frequency of shots\")]",
                            "[SerializeField, Tooltip(\"Frequency of shots\")]"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "public float shootFrequency;" },
                        modifiedLines: new List<string>() {
                            "//public float shootFrequency;",
                            "protected float _shootFrequency;",
                            "public virtual float shootFrequency { get { return _shootFrequency; } set { _shootFrequency = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "[UnityEngine.Serialization.FormerlySerializedAs(\"DropOffStart\")]" },
                        modifiedLines: new List<string>() {
                            "//[UnityEngine.Serialization.FormerlySerializedAs(\"DropOffStart\")]",
                            "[SerializeField, UnityEngine.Serialization.FormerlySerializedAs(\"DropOffStart\")]"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "public float minDamageDistance = 8f;" },
                        modifiedLines: new List<string>() {
                            "//public float minDamageDistance = 8f;",
                            "protected float _minDamageDistance = 8f;",
                            "public virtual float minDamageDistance { get { return _minDamageDistance; } set { _minDamageDistance = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "[UnityEngine.Serialization.FormerlySerializedAs(\"DropOffEnd\")]" },
                        modifiedLines: new List<string>() {
                            "//[UnityEngine.Serialization.FormerlySerializedAs(\"DropOffEnd\")]",
                            "[SerializeField, UnityEngine.Serialization.FormerlySerializedAs(\"DropOffEnd\")]"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "public float maxDamageDistance = 50f;" },
                        modifiedLines: new List<string>() {
                            "//public float maxDamageDistance = 50f;",
                            "protected float _maxDamageDistance = 50f;",
                            "public virtual float maxDamageDistance { get { return _maxDamageDistance; } set { _maxDamageDistance = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "[Tooltip(\"Minimum damage caused by the shot, regardless the distance\")]" },
                        modifiedLines: new List<string>() {
                            "//[Tooltip(\"Minimum damage caused by the shot, regardless the distance\")]",
                            "[SerializeField, Tooltip(\"Minimum damage caused by the shot, regardless the distance\")]"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "public int minDamage;" },
                        modifiedLines: new List<string>() {
                            "//public int minDamage;",
                            "protected int _minDamage;",
                            "public virtual int minDamage { get { return _minDamage; } set { _minDamage = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "[Tooltip(\"Maximum damage caused by the close shot\")]" },
                        modifiedLines: new List<string>() {
                            "//[Tooltip(\"Maximum damage caused by the close shot\")]",
                            "[SerializeField, Tooltip(\"Maximum damage caused by the close shot\")]"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "public int maxDamage;" },
                        modifiedLines: new List<string>() {
                            "//public int maxDamage;",
                            "protected int _maxDamage;",
                            "public virtual int maxDamage { get { return _maxDamage; } set { _maxDamage = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    )
                }
            );
            #endregion

            #region Convert vShooterWeapon.cs
            progressUpdate?.Invoke(progress_title, "Converting vShooterWeapon.cs...", 0.28f);
            yield return new EditorWaitForSeconds(0.01f);
            string vShooterWeapon = FilesUtil.FindFilePath("vShooterWeapon.cs", inv_shooter_folder);
            FilesUtil.ModifyFileAtPath(vShooterWeapon, new List<FilesUtil.Modification>()
                {
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "[Tooltip(\"Max clip size of your weapon\")]" },
                        modifiedLines: new List<string>() {
                            "//[Tooltip(\"Max clip size of your weapon\")]",
                            "[SerializeField, Tooltip(\"Max clip size of your weapon\")]"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "public int clipSize;" },
                        modifiedLines: new List<string>() {
                            "//public int clipSize;",
                            "protected int _clipSize;",
                            "public virtual int clipSize { get { return _clipSize; } set { _clipSize = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    )
                }
            );
            #endregion

            #region Convert vShooterMeleeInput.cs
            progressUpdate?.Invoke(progress_title, "Converting vShooterMeleeInput.cs...", 0.42f);
            yield return new EditorWaitForSeconds(0.01f);
            string vShooterMeleeInput = FilesUtil.FindFilePath("vShooterMeleeInput.cs", inv_shooter_folder);
            FilesUtil.ModifyFileAtPath(vShooterMeleeInput, new List<FilesUtil.Modification>()
                {
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "protected bool isUsingScopeView;" },
                        modifiedLines: new List<string>() {
                            "//protected bool isUsingScopeView;",
                            "protected bool _isUsingScopeView;",
                            "protected virtual bool isUsingScopeView { get { return _isUsingScopeView; } set { _isUsingScopeView = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "Debug.LogWarning(\"Missing the AimCanvas, drag and drop the prefab to this scene in order to Aim\", gameObject);" },
                        modifiedLines: new List<string>() {
                            "//Debug.LogWarning(\"Missing the AimCanvas, drag and drop the prefab to this scene in order to Aim\", gameObject);"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    )
                }
            );
            #endregion

            #region Convert vControlAimCanvas.cs
            progressUpdate?.Invoke(progress_title, "Converting vControlAimCanvas.cs...", 0.57f);
            yield return new EditorWaitForSeconds(0.01f);
            string vControlAimCanvas = FilesUtil.FindFilePath("vControlAimCanvas.cs", inv_shooter_folder);
            FilesUtil.ModifyFileAtPath(vControlAimCanvas, new List<FilesUtil.Modification>()
                {
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "mainCamera = Camera.main;" },
                        modifiedLines: new List<string>() {
                            "if (scopeBackgroundCamera == null) scopeBackgroundCamera = GetComponentInChildren<Camera>(true);"
                        },
                        modificationType: FilesUtil.ModiftyType.AddBefore
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "protected UnityEvent onEnableScopeCamera { get { return currentAimCanvas.onEnableScopeCamera; } }" },
                        modifiedLines: new List<string>() {
                            "//protected UnityEvent onEnableScopeCamera { get { return currentAimCanvas.onEnableScopeCamera; } }",
                            "public UnityEvent onEnableScopeCamera { get { return currentAimCanvas.onEnableScopeCamera; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "protected UnityEvent onDisableScopeCamera { get { return currentAimCanvas.onDisableScopeCamera; } }" },
                        modifiedLines: new List<string>() {
                            "//protected UnityEvent onDisableScopeCamera { get { return currentAimCanvas.onDisableScopeCamera; } }",
                            "public UnityEvent onDisableScopeCamera { get { return currentAimCanvas.onDisableScopeCamera; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    )
                }
            );
            #endregion

            #region Convert vThrowManager.cs
            progressUpdate?.Invoke(progress_title, "Converting vThrowManager.cs...", 0.71f);
            yield return new EditorWaitForSeconds(0.01f);
            string vThrowManager = FilesUtil.FindFilePath("vThrowManager.cs", inv_shooter_folder);
            FilesUtil.ModifyFileAtPath(vThrowManager, new List<FilesUtil.Modification>()
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
                        targetLines: new List<string>() { "private void MoveAndRotate()" },
                        modifiedLines: new List<string>() {
                            "//private void MoveAndRotate()",
                            "protected virtual void MoveAndRotate()"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "private bool isAiming;" },
                        modifiedLines: new List<string>() {
                            "//private bool isAiming;",
                            "protected bool isAiming;"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "private bool inThrow;" },
                        modifiedLines: new List<string>() {
                            "//private bool inThrow;",
                            "protected bool inThrow;"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "vThirdPersonInput tpInput;" },
                        modifiedLines: new List<string>() {
                            "//vThirdPersonInput tpInput;",
                            "protected vThirdPersonInput tpInput;"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "Vector3 thirdPersonAimPoint" },
                        modifiedLines: new List<string>() {
                            "//Vector3 thirdPersonAimPoint",
                            "protected virtual Vector3 thirdPersonAimPoint"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "Vector3 topdownAimPoint" },
                        modifiedLines: new List<string>() {
                            "//Vector3 topdownAimPoint",
                            "protected Vector3 topdownAimPoint"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "Vector3 sideScrollAimPoint" },
                        modifiedLines: new List<string>() {
                            "//Vector3 sideScrollAimPoint",
                            "protected virtual Vector3 sideScrollAimPoint"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "public int currentThrowObject;" },
                        modifiedLines: new List<string>() {
                            "//public int currentThrowObject;",
                            "[SerializeField] protected int _currentThrowObject = 1;",
                            "public virtual int currentThrowObject { get { return _currentThrowObject; } set { _currentThrowObject = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "public int maxThrowObjects = 6;" },
                        modifiedLines: new List<string>() {
                            "//public int maxThrowObjects = 6;",
                            "[SerializeField] protected int _maxThrowObjects = 6;",
                            "public virtual int maxThrowObjects { get { return _maxThrowObjects; } set { _maxThrowObjects = value; } }"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "void DrawTrajectory()" },
                        modifiedLines: new List<string>() {
                            "//void DrawTrajectory()",
                            "protected virtual void DrawTrajectory()"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "private bool isThrowInput;" },
                        modifiedLines: new List<string>() {
                            "//private bool isThrowInput;",
                            "protected bool isThrowInput;"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "void LaunchObject(Rigidbody projectily)" },
                        modifiedLines: new List<string>() {
                            "//void LaunchObject(Rigidbody projectily)",
                            "protected virtual void LaunchObject(Rigidbody projectily)"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    )
                }
            );
            #endregion

            #region Convert vThrowCollectable.cs
            progressUpdate?.Invoke(progress_title, "Converting vThrowCollectable.cs...", 0.85f);
            yield return new EditorWaitForSeconds(0.01f);
            string vThrowCollectable = FilesUtil.FindFilePath("vThrowCollectable.cs", inv_shooter_folder);
            FilesUtil.ModifyFileAtPath(vThrowCollectable, new List<FilesUtil.Modification>()
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
                        targetLines: new List<string>() { "public void UpdateThrowObj(Rigidbody throwObj)" },
                        modifiedLines: new List<string>() {
                            "//public void UpdateThrowObj(Rigidbody throwObj)",
                            "public virtual void UpdateThrowObj(Rigidbody throwObj)"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "vThrowManager throwManager;" },
                        modifiedLines: new List<string>() {
                            "//vThrowManager throwManager;",
                            "protected vThrowManager throwManager;"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    )
                }
            );
            #endregion

            #region Convert vExplosive.cs
            progressUpdate?.Invoke(progress_title, "Converting vExplosive.cs...", 0.95f);
            yield return new EditorWaitForSeconds(0.01f);
            string vExplosive = FilesUtil.FindFilePath("vExplosive.cs", inv_shooter_folder);
            FilesUtil.ModifyFileAtPath(vExplosive, new List<FilesUtil.Modification>()
                {
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "private List<GameObject> collidersReached;" },
                        modifiedLines: new List<string>() {
                            "//private List<GameObject> collidersReached;",
                            "protected List<GameObject> collidersReached;"
                        },
                        modificationType: FilesUtil.ModiftyType.Replace
                    ),
                    new FilesUtil.Modification(
                        targetLines: new List<string>() { "private float GetPercentageForce(float distance, float value)" },
                        modifiedLines: new List<string>() {
                            "//private float GetPercentageForce(float distance, float value)",
                            "protected float GetPercentageForce(float distance, float value)"
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
