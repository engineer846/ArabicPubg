using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EMI.Utils
{
    public static class Extensions
    {
        #region Quaternion
        /// <summary>
        /// Will return true if the angle is within an acceptable distance to the other angle
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="otherAngle"></param>
        /// <param name="closeEnough"></param>
        /// <returns></returns>
        public static bool CloseEnough(this Quaternion angle, Quaternion otherAngle, float closeEnough = 1f)
        {
            return ((angle.eulerAngles - otherAngle.eulerAngles).sqrMagnitude <= (closeEnough * closeEnough));
        }
        #endregion

        #region Vector3
        /// <summary>
        /// Will return true if the Vector3 point is within an accetable distance to the other point
        /// </summary>
        /// <param name="point"></param>
        /// <param name="otherValue"></param>
        /// <param name="closeEnoughValue"></param>
        /// <returns></returns>
        public static bool CloseEnough(this Vector3 point, Vector3 otherValue, float closeEnoughValue = 0.0001f)
        {
            return (otherValue - point).sqrMagnitude < closeEnoughValue;
        }
        #endregion

        #region Scenes
        public static string GetCleanSceneName(this string input)
        {
            if (input == null) return "";
            return input.Split('/').Last().Replace(".unity", "");
        }
        #endregion

        #region Tags
        /// <summary>
        /// Recursively searches this gameobject for any child that is tagged with the target tag
        /// </summary>
        /// <param name="target">The gameobject to start searching all children in</param>
        /// <param name="tag">The tag to search for</param>
        /// <returns>List of gameobjects (children or root) that have this tag</returns>
        public static List<GameObject> FindGameObjectsWithTag(this GameObject target, string tag)
        {
            List<GameObject> targetChildren = new List<GameObject>();
            tag = tag.Trim();
            if (target.tag == tag)
            {
                targetChildren.Add(target);
            }
            for (int i = 0; i < target.transform.childCount; i++)
            {
                Transform targetChild = target.transform.GetChild(i);
                if (targetChild.tag == tag) //This child object has the tag you're looking for
                {
                    targetChildren.Add(targetChild.gameObject);
                }
                if (targetChild.childCount > 0) //This child has more children in it
                {
                    targetChildren.AddRange(targetChild.gameObject.FindGameObjectsWithTag(tag)); //recursively search this child and so on
                }    
            }
            return targetChildren;
        }
        #endregion

    }
}
