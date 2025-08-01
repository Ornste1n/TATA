using Unity.Entities;
using UnityEngine.UIElements;

namespace Game.Scripts.Mechanics.Units.Selection.UnitsHud.Elements
{
    public class UnitView
    {
        private readonly VisualElement m_Element;
        
        public Entity Entity { get; private set; }
        public bool IsActive { get; private set; }
        
        public UnitView(VisualElement element)
        {
            m_Element = element;
        }

        public void ActivateOrUpdate(string hiddenStyle, StyleBackground style = default, Entity entity = default)
        {
            if (!IsActive) SetActive(true, hiddenStyle);

            if (style != default) m_Element.style.backgroundImage = style;
            
            if(entity == default) return;

            Entity = entity;
        }
        
        public void SetActive(bool active, string hiddenStyle)
        {
            if(active)
                m_Element.RemoveFromClassList(hiddenStyle);
            else
                m_Element.AddToClassList(hiddenStyle);
            
            IsActive = active;
        }
    }
}