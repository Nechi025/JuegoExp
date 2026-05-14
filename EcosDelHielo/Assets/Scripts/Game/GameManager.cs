using Core.Services;
using UnityEngine;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameConfig config;

        private void Awake()
        {
            ServiceLocator.Register<GameConfig>(config);
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<GameConfig>();
        }
    }
}
