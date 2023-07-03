using EMI.Object;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EMI.Utils
{
    public static class PointsUtil
    {
        public enum PointType { RespawnPoint, NetworkSpawnPoint, JumpToPoint };
        
        /// <summary>
        /// Will return the point GameObject that matches all the past in criteria. If it can't match
        /// everything then it will return null.
        /// </summary>
        /// <param name="pointType"></param>
        /// <param name="pointName"></param>
        /// <param name="teamName"></param>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        public static GameObject GetPoint(PointType pointType, string pointName = "", string teamName = "", string sceneName = "")
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                if (string.IsNullOrEmpty(pointName))
                {
                    if (string.IsNullOrEmpty(teamName))
                    {
                        // no scene, pointName, or teamName provided, just return a random point in the "active" scene
                        List<GameObject> points = GameObject.FindGameObjectsWithTag(GetTag(pointType)).ToList();
                        if (points.Count > 0)
                            return points[Random.Range(0, points.Count)];
                    }
                    else
                    {
                        // no scene name or point name provided. Find a random point that is on the target teamName
                        List<GameObject> points = GameObject.FindGameObjectsWithTag(GetTag(pointType)).ToList().FindAll(x => x.GetComponent<Team>() && x.GetComponent<Team>().teamName == teamName);
                        if (points.Count > 0)
                            return points[Random.Range(0, points.Count)];
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(teamName))
                    {
                        // No scene name or team name provided, find the point with the pointname in the "active" scene
                        return GameObject.FindGameObjectsWithTag(GetTag(pointType)).ToList().Find(x => x.name == pointName);
                    }
                    else
                    {
                        // No scene name provided, find the point with the name on the target team in the "active" scene
                        foreach (GameObject target in GameObject.FindGameObjectsWithTag(GetTag(pointType)))
                        {
                            Team team = target.GetComponent<Team>();
                            if (!team) continue;
                            if (team.teamName == teamName && target.name == pointName)
                                return target;
                        }
                    }
                }
            }
            else
            {
                if(string.IsNullOrEmpty(pointName))
                {
                    if (string.IsNullOrEmpty(teamName))
                    {
                        // no pointName or teamName provided. However, scene name was. So find a random point in the target scene.
                        List<GameObject> points = GetAllPoints(pointType, sceneName);
                        if (points.Count > 0)
                            return points[Random.Range(0, points.Count)];
                    }
                    else
                    {
                        // no pointName provided. However, teamName and sceneName were. Find random point on target team in target scene.
                        List<GameObject> points = GetAllPoints(pointType, sceneName).FindAll(x => x.GetComponent<Team>() && x.GetComponent<Team>().teamName == teamName);
                        if (points.Count > 0)
                            return points[Random.Range(0, points.Count)];
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(teamName))
                    {
                        // sceneName and pointName provided, teamName was not. Find the point with the name in the target scene
                        return GetAllPoints(pointType, sceneName).Find(x => x.name == pointName);
                    }
                    else
                    {
                        // sceneName, pointName, and teamName were provided. Finds the target pointName, on the target team, in the target scene
                        List<GameObject> points = GetAllPoints(pointType, sceneName).FindAll(x => x.name == pointName);
                        return points.Find(x => x.GetComponent<Team>() && x.GetComponent<Team>().teamName == teamName);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets all the points of type in the target scene (if loaded).
        /// </summary>
        /// <param name="pointType"></param>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        public static List<GameObject> GetAllPoints(PointType pointType, string sceneName)
        {
            List<GameObject> points = new List<GameObject>();
            foreach(GameObject target in SceneManager.GetSceneByName(sceneName).GetRootGameObjects())
            {
                points.AddRange(target.FindGameObjectsWithTag(GetTag(pointType)));
            }
            return points;
        }

        /// <summary>
        /// Returns the target tag name for the specified pointType
        /// </summary>
        /// <param name="pointType"></param>
        /// <returns></returns>
        public static string GetTag(PointType pointType)
        {
            switch (pointType)
            {
                case PointType.JumpToPoint:
                    return "JumpPoint";
                case PointType.NetworkSpawnPoint:
                    return "NetworkSpawnPoint";
                case PointType.RespawnPoint:
                    return "NetworkRespawnPoint";
                default:
                    return null;
            }
        }
    }
}
