using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Scripts.UI.Frontend.CustomElements
{
    public class CustomSlider<T> where T : Enum
    {
        private readonly Slider m_Slider;
        private readonly Label m_ValueLabel;
        
        private readonly VisualElement m_Root;
        private readonly VisualElement m_Dragger;
        
        private VisualElement m_Bar;
        private VisualElement m_NewDragger;

        public Slider Slider => m_Slider;
        public T Type { get; }
        
        public CustomSlider(VisualElement root, T type, string sliderName, string valueLabelName, float value)
        {
            Type = type;
            m_Root = root;
            m_Slider = m_Root.Q<Slider>(sliderName);
            m_ValueLabel = m_Root.Q<Label>(valueLabelName);
            m_Dragger = m_Slider.Q<VisualElement>("unity-dragger");
            SetCustomElements();
            
            m_Slider.RegisterCallback<ChangeEvent<float>>(SliderValueChanged);
            m_NewDragger.RegisterCallback<GeometryChangedEvent>(SetStartDragPosition);

            m_Slider.value = value;
        }

        private void SetCustomElements()
        {
            m_Bar = new VisualElement { name = "Bar" };
            m_Bar.AddToClassList("bar");
            
            m_Dragger.Add(m_Bar);

            m_NewDragger = new VisualElement() { name = "NewDragger" };
            m_NewDragger.AddToClassList("newdragger");
            m_NewDragger.pickingMode = PickingMode.Ignore;
            
            m_Slider.Add(m_NewDragger);
        }

        private void SetStartDragPosition(GeometryChangedEvent evt)
        {
            UpdateDragger();
            m_NewDragger.UnregisterCallback<GeometryChangedEvent>(SetStartDragPosition);
        }

        private void SliderValueChanged(ChangeEvent<float> value)
        {
            UpdateDragger();
            m_ValueLabel.text = $"{value.newValue:F0}%";
        }

        private void UpdateDragger()
        {
            Vector2 dist = new Vector2
            (
                (m_NewDragger.layout.width - m_Dragger.layout.width) / 2,
                (m_NewDragger.layout.height - m_Dragger.layout.height) / 2
            );
            
            Vector2 position = m_Dragger.parent.LocalToWorld(m_Dragger.transform.position);
            m_NewDragger.transform.position = m_NewDragger.parent.WorldToLocal(position - dist);
        }
    }
}