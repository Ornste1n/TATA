using Unity.Entities;
using UnityEngine.UIElements;
using Game.Scripts.Mechanics.Units.General.Components;

namespace Game.Scripts.Mechanics.Units.Selection.UnitsHud.Elements
{
    public class UnitInfoPanel
    {
        private readonly VisualElement m_Panel;
        private readonly Label _unitHealth;
        
        private readonly Label _unitLabel;
        private readonly VisualElement _unitIcon;

        public Entity Entity { get; private set; }
        public bool IsActive { get; private set; }
        
        public UnitInfoPanel(VisualElement root)
        {
            m_Panel = root.Q<VisualElement>("unit-info");
            
            _unitLabel = root.Q<Label>("unit-info-name");
            _unitHealth = root.Q<Label>("unit-info-health");
            _unitIcon = root.Q<VisualElement>("unit-info-icon");
        }

        public void Show(Entity entity, UnitPanelData data, Damageable damageable)
        {
            Entity = entity;
            _unitLabel.text = data.Name;
            _unitIcon.style.backgroundImage = data.Image;
            _unitHealth.text = $"{damageable.Health}/{damageable.MaxHealth}";
        }
        
        public void SetActive(bool active, string hiddenStyle)
        {
            IsActive = active;
            
            if(active)
                m_Panel.RemoveFromClassList(hiddenStyle);
            else
                m_Panel.AddToClassList(hiddenStyle);
        }
    }
}