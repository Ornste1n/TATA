using System;
using UnityEngine.UIElements;

namespace Game.Scripts.UI.Options.Base
{
    public abstract class OptionPanel : IDisposable
    {
        public Button Button { get; }
        public VisualElement Panel { get; }
        protected OptionsWindow OptionsWindow { get; }

        public OptionPanel(OptionsWindow window, string name, string button)
        {
            OptionsWindow = window;
            
            Button = OptionsWindow.Root.Q<Button>(button);
            Panel = OptionsWindow.Root.Q<VisualElement>(name);
        }

        public abstract void Save();
        public abstract void Reset();
        public abstract void Dispose();
        public abstract bool HasChanged();
    }
}