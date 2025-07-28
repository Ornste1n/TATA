using System;
using Unity.Entities;
using UnityEngine.UIElements;
using Game.Scripts.Mechanics.Units.General.Components;

namespace Game.Scripts.Mechanics.Units.Selection.UnitsHud.Elements
{
    public class InstrumentsPanel : IDisposable
    {
        private const string ActiveSkill = "unitSkillActive";
        
        private readonly VisualElement m_Panel;
        private readonly SkillView[] _skillViews;

        private int _lastActiveSkillView;

        public InstrumentsPanel(VisualElement panel, SkillView[] skillViews)
        {
            m_Panel = panel;
            _skillViews = skillViews;
        }

        public void SubscribesSkills(Action<int> clickEvent)
        {
            foreach (SkillView skillView in _skillViews)
                skillView.Subscribe(clickEvent);
        }
        
        public void ActiveSkills(DynamicBuffer<SkillSpriteCatalogElement> skills, int startIndex, int endIndex)
        {
            int i = 0;
            
            for (int j = startIndex; j <= endIndex; j++)
                _skillViews[i++].ActivateSKill(j, new StyleBackground(skills[j].Sprite), ActiveSkill);

            _lastActiveSkillView = i;
        }

        public void DisableSkills()
        {
            for (int i = 0; i < _lastActiveSkillView; i++)
                _skillViews[i].Disable(ActiveSkill);
        }

        public void Dispose()
        {
            SkillView[] skillViews = _skillViews;
            
            foreach (SkillView skillView in skillViews)
                skillView.Dispose();
            
            Array.Clear(skillViews, 0, skillViews.Length);
        }
    }
}