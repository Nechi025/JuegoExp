using Core.SceneSwitching;
using Core.Services;
using DG.Tweening;
using UnityEngine;

namespace Core.Bootstrap
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] private SceneManifest scenes;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            if (!DOTween.instance)
                DOTween.Init(recycleAllByDefault: true, useSafeMode: true)
                    .SetCapacity(200, 50);
            ServiceLocator.Register<SceneManifest>(scenes);
        }

        private void Start()
        {
            if (!ServiceLocator.TryGet<ISceneSwitcher>(out var switcher))
            {
                Debug.LogError("[Bootstrap] ISceneSwitcher is not registered. " +
                               "Ensure SceneSwitcher is a child of PersistentSystems in the Bootstrap scene.");
                return;
            }
            switcher.LoadScene(scenes.mainMenu);
        }
    }
}
