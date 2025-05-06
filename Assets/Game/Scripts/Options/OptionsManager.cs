using System;
using UnityEngine;
using Game.Scripts.Data;
using System.Threading.Tasks;
using Game.Scripts.Data.Saver;

namespace Game.Scripts.Options
{
    public static class OptionsManager
    {
        private static OptionModel _model;
        
        public static GraphicsOption Graphics { get; private set; }
        public static AudioOption Audio { get; private set; }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static async void LoadConfig()
        {
            Audio = new AudioOption();
            Graphics = new GraphicsOption();
            
            try
            {
                _model = await JsonSaverAsync.Instance.LoadAsync<OptionModel>(Paths.Options);

                if (_model == null)
                    throw new NullReferenceException();
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Option Exception: Model is invalid");

                _model = new OptionModel()
                {
                    Audio = Audio.CreateDefaultModel(),
                    Graphics = Graphics.CreateDefaultModel()
                };
                
                Debug.LogWarning("Create Default Model");
                await SaveAll();
            }
            
            Audio.Initialize(_model.Audio);
            Graphics.Initialize(_model.Graphics);
        }

        public static async Task SaveAll()
        {
            await JsonSaverAsync.Instance.SaveAsync(Paths.Options, _model);
        }
    }
}