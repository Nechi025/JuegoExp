using Core.Services;
using UnityEngine;

namespace Core.SceneSwitching
{
    [CreateAssetMenu(fileName = "SceneManifest", menuName = "Core/Scene Manifest")]
    public class SceneManifest : ScriptableObject, IService
    {
        public string mainMenu = "MainMenu";
        public string game     = "GameScene";
    }
}
