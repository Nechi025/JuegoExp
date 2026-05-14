using System;
using Core.Services;
using Core.Transitions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.SceneSwitching
{
    public class SceneSwitcher : MonoBehaviour, ISceneSwitcher
    {
        private bool _isLoading;

        private void Awake()
        {
            ServiceLocator.Register<ISceneSwitcher>(this);
        }

        public void LoadScene(string sceneName)
        {
            PerformLoad(() => SceneManager.LoadSceneAsync(sceneName));
        }

        public void LoadScene(int sceneIndex)
        {
            PerformLoad(() => SceneManager.LoadSceneAsync(sceneIndex));
        }

        public void ReloadCurrentScene()
        {
            LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void PerformLoad(Func<AsyncOperation> loadOperation)
        {
            if (_isLoading) return;
            _isLoading = true;

            if (ServiceLocator.TryGet<ITransitionManager>(out var transition))
            {
                transition.FadeToBlack(() =>
                {
                    AsyncOperation op;
                    try
                    {
                        op = loadOperation();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[SceneSwitcher] Scene load failed: {e.Message}");
                        _isLoading = false;
                        transition.FadeFromBlack();
                        return;
                    }
                    op.completed += _ => transition.FadeFromBlack(() => _isLoading = false);
                });
            }
            else
            {
                try
                {
                    loadOperation();
                }
                finally
                {
                    _isLoading = false;
                }
            }
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<ISceneSwitcher>();
        }
    }
}
