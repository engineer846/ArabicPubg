using Invector.vItemManager;
using Mirror;
using System.Collections.Generic;
using UnityEngine;

namespace EMI.Utils
{
    public static class MeleeExtensions
    {
        #region Child Trees
        public static Transform GetChildFromTree(this Transform staringParent, SyncList<byte> tree)
        {
            Transform target = null;
            target = staringParent;
            for (int i = tree.Count - 1; i >= 0; i--)
            {
                target = target.GetChild(tree[i]);
            }
            return target;
        }
        public static Transform GetChildFromTree(this Transform staringParent, List<byte> tree)
        {
            Transform target = null;
            target = staringParent;
            for(int i = tree.Count-1; i >= 0; i--)
            {
                target = target.GetChild(tree[i]);
            }
            return target;
        }
        public static List<byte> BuildChildTree(this Transform startingChild)
        {
            List<byte> tree = new List<byte>();
            if (startingChild.parent == null)
            {
                return tree;
            }
            else
            {
                tree.Add((byte)startingChild.GetSiblingIndex());
                tree = startingChild.parent.transform.BuildChildTree(tree);
            }
            return tree;
        }
        public static SyncList<byte> BuildChildTree(this Transform startingChild, bool SyncList)
        {
            SyncList<byte> tree = new SyncList<byte>();
            if (startingChild.parent == null)
            {
                return tree;
            }
            else
            {
                tree.Add((byte)startingChild.GetSiblingIndex());
                tree = startingChild.parent.transform.BuildChildTree(tree, true);
            }
            return tree;
        }
        public static List<byte> BuildChildTree(this Transform startingChild, List<byte> tree)
        {
            if (startingChild.parent == null)
            {
                return tree;
            }
            else
            {
                tree.Add((byte)startingChild.GetSiblingIndex());
                tree = startingChild.parent.transform.BuildChildTree(tree);
            }
            return tree;
        }
        public static SyncList<byte> BuildChildTree(this Transform startingChild, SyncList<byte> tree, bool SyncList)
        {
            if (startingChild.parent == null)
            {
                return tree;
            }
            else
            {
                tree.Add((byte)startingChild.GetSiblingIndex());
                tree = startingChild.parent.transform.BuildChildTree(tree, true);
            }
            return tree;
        }
        #endregion

        #region Components
        /// <summary>
        /// This will find the first enabled component that exists on this gameobject tree.
        /// </summary>
        /// <param name="go">GameObject to search</param>
        /// <param name="comp">Component type to search for</param>
        /// <returns></returns>
        public static Component FindComponent(this GameObject go, System.Type compType)
        {
            if (go.GetComponent(compType))
            {
                return go.GetComponent(compType);
            }
            else if (go.GetComponentInParent(compType))
            {
                return go.GetComponentInParent(compType);
            }
            else if (go.GetComponentInChildren(compType))
            {
                return go.GetComponentInChildren(compType);
            }
            return null;
        }
        #endregion
    }
}
