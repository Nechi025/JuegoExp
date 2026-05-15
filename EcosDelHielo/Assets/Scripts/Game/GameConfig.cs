using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Game/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Glaciers")]
        [Min(1)] public int   glacierClicksPerIceBall = 1;
        [Min(1)] public int   glacierMaxClicks        = 50;

        [Header("Funnel")]
        [Min(1)] public int   purifyClicksRequired    = 3;
        [Min(1)] public int   cubesPerBottle          = 1;

        [Header("Customers")]
        [Min(0.5f)] public float customerPatienceTime  = 10f;
        [Min(0.5f)] public float customerSpawnInterval = 5f;
        [Min(1f)]   public float bottleTimeout         = 12f;

        [Header("Difficulty")]
        [Min(1f)] public float difficultyRampDuration  = 120f;

        [Header("Mistakes & Score")]
        [Min(1)]  public int   maxMistakes             = 5;
        [Min(0)]  public int   scorePerDelivery        = 100;
        [Min(0f)] public float scorePerSecond          = 1f;
    }
}
