using Core.SceneSwitching;
using Core.Services;
using UnityEngine;

namespace Game.UI
{
    public class GameOverLoader : MonoBehaviour
    {
        private void OnEnable()  => GameEventBus.OnGameOver += HandleGameOver;
        private void OnDisable() => GameEventBus.OnGameOver -= HandleGameOver;

        private void HandleGameOver()
        {
            if (!ServiceLocator.TryGet<ISceneSwitcher>(out var switcher) ||
                !ServiceLocator.TryGet<SceneManifest>(out var manifest))
            {
                Debug.LogError("[GameOverLoader] ISceneSwitcher or SceneManifest not registered.");
                return;
            }
            switcher.LoadScene(manifest.gameOver);
        }
    }
}
