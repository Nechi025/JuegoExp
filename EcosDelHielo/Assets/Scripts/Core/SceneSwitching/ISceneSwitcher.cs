using Core.Services;

namespace Core.SceneSwitching
{
    public interface ISceneSwitcher : IService
    {
        void LoadScene(string sceneName);
        void LoadScene(int sceneIndex);
        void ReloadCurrentScene();
    }
}
