using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Dsu.Framework
{
    public class SceneLoaderBase : MonoBehaviour
    {
        private static SceneLoaderBase _loaderObject;

        public static void LoadScene(string sceneName)
        {
            _loaderObject?._LoadScene(sceneName);
        }

        private void Awake()
        {
            _loaderObject = this;
        }

        public void _LoadScene(string sceneName)
        {
            StartCoroutine(LoadSceneAsync(sceneName));
        }

        private IEnumerator LoadSceneAsync(string sceneName)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

            // Optional: Prevent the scene from activating immediately
            operation.allowSceneActivation = false;

            // Show loading progress
            while (!operation.isDone) {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);

                OnProgress(progress);

                // Activate the scene when loading is complete
                if (operation.progress >= 0.9f) {
                    operation.allowSceneActivation = true;
                }

                yield return null;
            }
        }
        protected virtual void OnProgress(float progress)
        {
        }
    }//class SceneLoaderBase
}