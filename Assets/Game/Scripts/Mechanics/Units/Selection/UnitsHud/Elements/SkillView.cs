using System;
using UnityEngine.UIElements;

namespace Game.Scripts.Mechanics.Units.Selection.UnitsHud.Elements
{
    public class SkillView : IDisposable
    {
        private readonly VisualElement m_Element;

        private int _skillIndexInData;
        private Action<int> _onClicked;
        
        public SkillView(VisualElement element)
        {
            m_Element = element;
        }

        public void Subscribe(Action<int> clickEvent)
        {
            _onClicked = clickEvent;
            m_Element.RegisterCallback<ClickEvent>(HandleClick);
        }
        
        private void HandleClick(ClickEvent clickEvent) => _onClicked?.Invoke(_skillIndexInData);

        public void ActivateSKill(int index, StyleBackground imageStyle, string activeStyle)
        {
            _skillIndexInData = index;
            m_Element.style.backgroundImage = imageStyle;
            m_Element.AddToClassList(activeStyle);
        }

        public void Disable(string activeStyle)
        {
            m_Element.style.backgroundImage = default;
            m_Element.RemoveFromClassList(activeStyle);
        }

        public void Dispose()
        {
            m_Element.UnregisterCallback<ClickEvent>(HandleClick);
        }
    }
}