using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace Game.Scripts.UI.Frontend
{
    public class UIAutoScaler : MonoBehaviour
    {
        [SerializeField] private List<UIDocument> _uiDocuments = new();
        [Space]
        [SerializeField] private float _minFontSize = 1f;
        
        private void FitAll()
        {
            foreach (UIDocument document in _uiDocuments)
            {
                List<Label> labels = document.rootVisualElement.Query<Label>().ToList();
                List<Button> buttons = document.rootVisualElement.Query<Button>().ToList();
                
                foreach (Label label in labels)
                    AdjustLabel(label);
                
                foreach (Button button in buttons)
                    AdjustLabel(button);
            }
        }

        private void AdjustLabel(TextElement element)
        {
            float availableWidth = element.resolvedStyle.width
                                   - element.resolvedStyle.paddingLeft
                                   - element.resolvedStyle.paddingRight
                                   - element.resolvedStyle.marginLeft
                                   - element.resolvedStyle.marginRight;

            float currentSize = element.resolvedStyle.fontSize;
            float bestFit = currentSize;

            for (float testSize = currentSize; testSize >= _minFontSize; testSize -= 0.5f)
            {
                element.style.fontSize = testSize;

                Vector2 size = element.MeasureTextSize(
                    element.text,
                    0, VisualElement.MeasureMode.Undefined,
                    0, VisualElement.MeasureMode.Undefined
                );

                if (!(size.x <= availableWidth)) continue;
                
                bestFit = testSize;
                break;
            }

            element.style.fontSize = bestFit;
        }
    }
}