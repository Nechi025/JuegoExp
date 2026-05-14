using Game;
using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class GameHUD : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI mistakesText;

        [SerializeField] private ScoreManager   scoreManager;
        [SerializeField] private MistakeManager mistakeManager;

        private void Update()
        {
            scoreText.text    = $"SCORE {scoreManager.Score:D6}";
            timeText.text     = $"⏱ {FormatTime(scoreManager.TimeAlive)}";
            mistakesText.text = $"MISTAKES {mistakeManager.CurrentMistakes}";
        }

        private static string FormatTime(float seconds)
        {
            int m = (int)(seconds / 60);
            int s = (int)(seconds % 60);
            return $"{m:D2}:{s:D2}";
        }
    }
}
