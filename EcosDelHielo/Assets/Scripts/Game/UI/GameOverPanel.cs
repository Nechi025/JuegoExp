using Core.SceneSwitching;
using Core.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class GameOverPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Button          playAgainButton;
        [SerializeField] private Button          menuButton;
        [SerializeField] private string          message = "The glacier has melted...";

        private void Awake()
        {
            if (messageText != null)
                messageText.text = message;

            playAgainButton.onClick.AddListener(OnPlayAgain);
            menuButton.onClick.AddListener(OnMenu);
        }

        private void OnDestroy()
        {
            playAgainButton.onClick.RemoveListener(OnPlayAgain);
            menuButton.onClick.RemoveListener(OnMenu);
        }

        private void OnPlayAgain()
        {
            if (ServiceLocator.TryGet<ISceneSwitcher>(out var switcher) &&
                ServiceLocator.TryGet<SceneManifest>(out var manifest))
                switcher.LoadScene(manifest.game);
            else
                Debug.LogWarning("[GameOverPanel] ISceneSwitcher or SceneManifest not found.");
        }

        private void OnMenu()
        {
            if (ServiceLocator.TryGet<ISceneSwitcher>(out var switcher) &&
                ServiceLocator.TryGet<SceneManifest>(out var manifest))
                switcher.LoadScene(manifest.mainMenu);
            else
                Debug.LogWarning("[GameOverPanel] ISceneSwitcher or SceneManifest not found.");
        }
    }
}
