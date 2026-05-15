using Core.Services;
using UnityEngine;

namespace Game
{
    public class GameManager : MonoBehaviour, IService
    {
        [SerializeField] private GameConfig config;

        public GameConfig Config => config;

        private void Awake()  => ServiceLocator.Register<GameManager>(this);
        private void OnDestroy() => ServiceLocator.Unregister<GameManager>();
    }
}
