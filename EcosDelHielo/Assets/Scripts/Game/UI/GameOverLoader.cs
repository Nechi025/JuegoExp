using Core.SceneSwitching;
using Core.Services;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.UI
{
    public class GameOverLoader : MonoBehaviour
    {
        [SerializeField] private string fallbackSceneName = "Game Over";

        private void OnEnable()  => GameEventBus.OnGameOver += HandleGameOver;
        private void OnDisable() => GameEventBus.OnGameOver -= HandleGameOver;

        private void HandleGameOver()
        {
            if (ServiceLocator.TryGet<ISceneSwitcher>(out var switcher) &&
                ServiceLocator.TryGet<SceneManifest>(out var manifest))
            {
                switcher.LoadScene(manifest.gameOver);
            }
            else
            {
                SceneManager.LoadScene(fallbackSceneName);
            }
        }
    }
}
