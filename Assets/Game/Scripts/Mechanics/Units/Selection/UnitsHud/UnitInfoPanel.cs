using UnityEngine.UIElements;

namespace Game.Scripts.Mechanics.Units.Selection.UnitsHud
{
    public class UnitInfoPanel
    {
        private readonly VisualElement m_Panel;
        private readonly Label _unitHealth;
        
        private readonly Label _unitLabel;
        private readonly VisualElement _unitIcon;
        
        public UnitInfoPanel(VisualElement root)
        {
            m_Panel = root.Q<VisualElement>("unit-info");
            
            _unitLabel = root.Q<Label>("unit-info-name");
            _unitHealth = root.Q<Label>("unit-info-health");
            _unitIcon = root.Q<VisualElement>("unit-info-icon");
            
        }

        public void Show(float health, float maxHealth)
        {
            _unitHealth.text = $"{health}/{maxHealth}";
        }
        
        public void SetActive(bool active, string hiddenStyle)
        {
            if(active)
                m_Panel.RemoveFromClassList(hiddenStyle);
            else
                m_Panel.AddToClassList(hiddenStyle);
        }
    }
}