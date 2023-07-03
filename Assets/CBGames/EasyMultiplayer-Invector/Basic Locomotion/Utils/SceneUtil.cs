using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace EMI.Utils
{
    public class SceneUtil
    {
        /// <summary>
        /// This will return a list of gameobjects that have a certain tag name in your 
        /// desired scene. If you don't supply a scene name it will look in every loaded
        /// scene. If it still can't find anything it will return null.
        /// </summary>
        /// <param name="tagName">The tag name to search for</param>
        /// <param name="sceneName">The scene name to search in.</param>
        /// <returns>A list of found gameobjects</returns>
        public static List<GameObject> FindGameObjectsWithTag(string tagName, string sceneName = "")
        {
            List<GameObject> retVal = new List<GameObject>();
            if (string.IsNullOrEmpty(sceneName))
            {
                GetAllLoadedScenes().ForEach(scene =>
                {
                    scene.GetRootGameObjects().ToList().ForEach(go =>
                    {
                        retVal.AddRange(go.FindGameObjectsWithTag(tagName));
                    });
                });
            }
            else
            {
                SceneManager.GetSceneByName(sceneName).GetRootGameObjects().ToList().ForEach(go =>
                {
                    retVal.AddRange(go.FindGameObjectsWithTag(tagName));
                });
            }
            return retVal;
        }

        /// <summary>
        /// Will return a list of all scenes that are current loaded.
        /// </summary>
        /// <returns>List of loaded scenes.</returns>
        public static List<Scene> GetAllLoadedScenes()
        {
            List<Scene> loadedScenes = new List<Scene>();
            int countLoaded = SceneManager.sceneCount;
            for (int i = 0; i < countLoaded; i++)
            {
                loadedScenes.Add(SceneManager.GetSceneAt(i));
            }
            return loadedScenes;
        }
    }
}
