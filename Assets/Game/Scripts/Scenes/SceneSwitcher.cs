using System;
using Latios;
using UnityEngine;
using Unity.Entities;
using System.Threading;
using System.Threading.Tasks;
using Game.Scripts.Addressable;
using UnityEngine.SceneManagement;

namespace Game.Scripts.Scenes
{
    public static class SceneSwitcher
    {
        private const string MenuScene = "Menu";
        
        public static async Task LoadGameSceneAsync(string sceneName, CancellationToken token)
        {
            if(string.IsNullOrWhiteSpace(sceneName))
                throw new ArgumentException($"Scene name is empty ('{sceneName}').");
            
            SceneTransitionLoader loader = new SceneTransitionLoader();
            SceneTransitionElement element = await loader.Load();

            LatiosWorld gameWorld = Bootstrap.CreateGameWorld();
            gameWorld.simulationSystemGroup.Enabled = false;
            gameWorld.presentationSystemGroup.Enabled = false;
            
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            operation!.allowSceneActivation = false;
            
            while (operation.progress < 0.9f)
            {
                token.ThrowIfCancellationRequested();
                await Awaitable.NextFrameAsync(token);
            }
            
            element.ShowLabel();
            
            try
            {
                element.OnSpacePerformed += AllowSwitchScene;
                await operation;
                
                gameWorld.initializationSystemGroup.Update();
                gameWorld.simulationSystemGroup.Enabled = true;
                gameWorld.presentationSystemGroup.Enabled = true;
            }
            finally
            {
                element.OnSpacePerformed -= AllowSwitchScene;
                loader.Unload();
            }

            void AllowSwitchScene() => operation.allowSceneActivation = true;
        }

        public static async Task LoadMenuSceneAsync(CancellationToken token)
        {
            SceneTransitionLoader loader = new SceneTransitionLoader();
            await loader.Load();

            Bootstrap.GameWorld.Dispose();
            
            await SceneManager.LoadSceneAsync(MenuScene);
            loader.Unload();
        }
    }
}
