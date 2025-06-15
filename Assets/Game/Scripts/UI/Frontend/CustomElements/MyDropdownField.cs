using System;
using UnityEngine.UIElements;

namespace Game.Scripts.UI.Frontend.CustomElements
{
    public class MyDropdownField : DropdownField
    {
        [Obsolete("Obsolete")]
        public new class UxmlFactory : UxmlFactory<MyDropdownField, UxmlTraits> { }

        public MyDropdownField()
        {
            label = string.Empty;

            var labelElement = this.Q<Label>("unity-label");
            
            if (labelElement != null)
                labelElement.style.display = DisplayStyle.None;
        }
    }
}