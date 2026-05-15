using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.UI
{
    public class GameOverPanel : MonoBehaviour
    {
        [SerializeField] private CanvasGroup     fadeOverlay;
        [SerializeField] private CanvasGroup     panelGroup;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Button          playAgainButton;
        [SerializeField] private Button          menuButton;

        [Space]
        [SerializeField] private string menuSceneName  = "EcosdelHielo";
        [SerializeField] private string defaultMessage = "The glacier has melted...";

        private void Awake()
        {
            fadeOverlay.alpha             = 0f;
            fadeOverlay.blocksRaycasts    = false;
            panelGroup.alpha              = 0f;
            panelGroup.interactable       = false;
            panelGroup.blocksRaycasts     = false;

            playAgainButton.onClick.AddListener(OnPlayAgain);
            menuButton.onClick.AddListener(OnMenu);

            if (messageText != null && !string.IsNullOrEmpty(defaultMessage))
                messageText.text = defaultMessage;
        }

        private void OnEnable()  => GameEventBus.OnGameOver += HandleGameOver;
        private void OnDisable() => GameEventBus.OnGameOver -= HandleGameOver;

        private void HandleGameOver()
        {
            fadeOverlay.blocksRaycasts = true;
            DOTween.To(() => fadeOverlay.alpha, x => fadeOverlay.alpha = x, 1f, 1.5f)
                   .SetEase(Ease.InQuad)
                   .OnComplete(ShowContent);
        }

        private void ShowContent()
        {
            panelGroup.interactable   = true;
            panelGroup.blocksRaycasts = true;
            DOTween.To(() => panelGroup.alpha, x => panelGroup.alpha = x, 1f, 0.5f);
        }

        private void OnPlayAgain() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        private void OnMenu()      => SceneManager.LoadScene(menuSceneName);
    }
}
