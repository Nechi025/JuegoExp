using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Game/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Glaciers")]
        [Min(1)]     public int   glacierClicksMin       = 5;
        [Min(1)]     public int   glacierClicksMax       = 15;
        [Min(0f)]    public float glacierPassiveDecayRate = 0.05f;
        [Min(0.001f)] public float glacierClickDamage    = 0.05f;

        [Header("Funnel")]
        [Min(1)]     public int   purifyClicksRequired   = 3;
        [Min(1)]     public int   cubesPerBottle         = 3;
        [Min(0.1f)]  public float iceCubeFallSpeed       = 2f;
        [Min(0.1f)]  public float beltMoveSpeed          = 1f;

        [Header("Customers")]
        [Min(0.5f)]  public float customerPatienceTime   = 10f;
        [Min(0.5f)]  public float customerSpawnInterval  = 5f;
        [Min(1f)]    public float bottleTimeout          = 12f;
        [Min(1)]     public int   maxCustomers           = 4;

        [Header("Difficulty")]
        [Min(1f)]    public float difficultyRampDuration = 120f;

        [Header("Mistakes & Score")]
        [Min(1)]     public int   maxMistakes            = 5;
        [Min(0)]     public int   scorePerDelivery       = 100;
        [Min(0f)]    public float scorePerSecond         = 1f;
    }
}
