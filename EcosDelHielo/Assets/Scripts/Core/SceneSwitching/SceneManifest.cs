using Core.Services;
using UnityEngine;

namespace Core.SceneSwitching
{
    [CreateAssetMenu(fileName = "SceneManifest", menuName = "Core/Scene Manifest")]
    public class SceneManifest : ScriptableObject, IService
    {
        public string bootstrap = "Bootstrap";
        public string mainMenu  = "Menu";
        public string game      = "Game";
        public string gameOver  = "Game Over";
    }
}
