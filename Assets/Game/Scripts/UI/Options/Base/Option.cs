using System;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace Game.Scripts.UI.Options.Base
{
    public abstract class Option : IDisposable
    {
        protected HashSet<Action> ChangedCallbacks;
        private Dictionary<IEventHandler, Action> _elementsCallbacks;
        
        public Button Button { get; }
        public VisualElement Panel { get; }
        protected OptionsWindow OptionsWindow { get; }
        
        public abstract void Save();
        public abstract void Reset();

        public virtual bool HasChanged() => ChangedCallbacks?.Count != 0;

        public Option(OptionsWindow window, string name, string button)
        {
            OptionsWindow = window;
            
            Button = OptionsWindow.Root.Q<Button>(button);
            Panel = OptionsWindow.Root.Q<VisualElement>(name);
        }
        
        protected virtual void InitializeCollection()
        {
            ChangedCallbacks = new HashSet<Action>(4);
            _elementsCallbacks = new Dictionary<IEventHandler, Action>(4);
        }
                
        protected void SubscribeChangedCallback(VisualElement element, Action callback)
        {
            element.RegisterCallback<ChangeEvent<string>>(RegisterChangedCallback);
            _elementsCallbacks.Add(element, callback);
        }

        protected void SubscribeCallback(VisualElement element, Action callback)
        {
            element.RegisterCallback<ClickEvent>(RegisterCallback);
            _elementsCallbacks.Add(element, callback);
        }

        private void RegisterChangedCallback(ChangeEvent<string> evt)
        {
            Action callback = _elementsCallbacks[evt.currentTarget];
            ChangedCallbacks.Add(callback);
        }

        private void RegisterCallback(ClickEvent evt)
        {
            Action callback = _elementsCallbacks[evt.currentTarget];
            ChangedCallbacks.Add(callback);
        }

        public virtual void Dispose()
        {
            ChangedCallbacks.Clear();
            _elementsCallbacks.Clear();

            ChangedCallbacks = null;
            _elementsCallbacks = null;
        }
    }
}