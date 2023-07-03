using EMI.Managers;
using EMI.Utils;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EMI.Object
{
    /// <summary>
    /// Sometimmes the NetworkIdentity doesn't cut it in Hosted mode. Sometimes you want to disable objects that are not in the same scene as 
    /// the player object is in. This component takes care of this situation. It will only enable object that are in the same scene as the 
    /// player object is in. Otherwise it will disable the target gameobjects.
    /// </summary>
    public class DisableIfNotOwningClientScene : MonoBehaviour
    {
        #region Properties
        [SerializeField, Tooltip("If you want to effect all children of this object.")]
        protected bool effectAllChildren = true;
        [SerializeField, Tooltip("If you want to effect this object.")]
        protected bool effectThisObject = true;

        [SerializeField, Tooltip("The list of children that will be effected. This list will be added to if any of the above options are ticked.")]
        protected List<GameObject> children = new List<GameObject>();
        #endregion

        #region Initilization
        protected virtual void Awake()
        {
            if (!GetComponent<NetworkIdentity>() && effectThisObject)
                children.Add(gameObject);
            if (effectAllChildren)
            {
                foreach (Transform child in transform)
                {
                    children.Add(child.gameObject);
                }
            }
        }
        protected virtual void Start()
        {
            ClientConnection conn = ClientUtil.GetMyClientConnectionObject();
            if (conn)
            {
                conn.OnSceneChanged += OnClientSceneChanged;
                OnClientSceneChanged(conn.inScene);
            }
            else
            {
                StartCoroutine(WaitForClientConnection());
            }
        }
        protected virtual IEnumerator WaitForClientConnection()
        {
            yield return new WaitUntil(() => ClientUtil.GetMyClientConnectionObject() != null);
            ClientConnection conn = ClientUtil.GetMyClientConnectionObject();
            conn.OnSceneChanged += OnClientSceneChanged;
            OnClientSceneChanged(conn.inScene);
        }
        protected virtual void OnDestroy()
        {
            ClientConnection conn = ClientUtil.GetMyClientConnectionObject();
            if (conn)
                conn.OnSceneChanged -= OnClientSceneChanged;
        }
        #endregion

        #region Enable Logic
        protected virtual void OnClientSceneChanged(string sceneName)
        {
            EnableObjects(gameObject.scene.name.GetCleanSceneName() == sceneName.GetCleanSceneName());
        }
        protected virtual void EnableObjects(bool enable)
        {
            children.ForEach(x => x.SetActive(enable));
        }
        #endregion
    }
}
