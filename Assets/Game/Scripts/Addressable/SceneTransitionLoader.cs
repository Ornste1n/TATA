using Game.Scripts.Scenes;
using System.Threading.Tasks;

namespace Game.Scripts.Addressable
{
    public class SceneTransitionLoader : CachedObjectLoader
    {
        private const string Label = "Scene Transition";
        public async Task<SceneTransitionElement> Load() => await LoadIternal<SceneTransitionElement>(Label);

        public void Unload() => UnloadInternal();
    }
}