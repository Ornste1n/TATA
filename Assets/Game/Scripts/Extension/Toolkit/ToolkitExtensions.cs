using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Game.Scripts.Extension.Toolkit
{
    public static class ToolkitExtensions
    {
        public static void RegisterEvent<T>
        (
            this VisualElement element, 
            EventCallback<T> callback,
            Dictionary<IEventHandler, Action> handlers,
            Action action
        )
            where T : EventBase<T>, new()
        {
            element.RegisterCallback(callback);
            handlers.TryAdd(element, action);
        }
    }
}