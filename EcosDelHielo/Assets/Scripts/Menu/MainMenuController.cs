using Core.SceneSwitching;
using Core.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button exitButton;

        private ISceneSwitcher _switcher;
        private SceneManifest  _manifest;

        private void Start()
        {
            if (!ServiceLocator.TryGet<ISceneSwitcher>(out _switcher))
            {
                Debug.LogError("[MainMenuController] ISceneSwitcher not registered.");
                return;
            }

            if (!ServiceLocator.TryGet<SceneManifest>(out _manifest))
            {
                Debug.LogError("[MainMenuController] SceneManifest not registered.");
                return;
            }

            playButton.onClick.AddListener(OnPlayClicked);
            exitButton.onClick.AddListener(OnExitClicked);
        }

        private void OnDestroy()
        {
            playButton.onClick.RemoveListener(OnPlayClicked);
            exitButton.onClick.RemoveListener(OnExitClicked);
        }

        private void OnPlayClicked() => _switcher.LoadScene(_manifest.game);

        private void OnExitClicked() => Application.Quit();
    }
}
