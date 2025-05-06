using UnityEngine.UIElements;
using System.Threading.Tasks;
using UnityEngine.Localization;
using System.Collections.Generic;
using Game.Scripts.UI.Options.Base;
using Game.Scripts.Extension.Toolkit;
using UnityEngine.Localization.Settings;

namespace Game.Scripts.UI.Options
{
    public class GeneralPanel : OptionPanel
    {
        public GeneralPanel(OptionsWindow window, string name, string button) : base(window, name, button)
        {
            InitLanguage();
        }

        public override void Save()
        {
        }

        public override void Reset()
        {
        }

        public override bool HasChanged() => true;

        public override void Dispose() { }

        private void InitLanguage()
        {
            DropdownField language = OptionsWindow.Root.Q<DropdownField>("language");
            List<Locale> locales = LocalizationSettings.AvailableLocales.Locales;
            
            language.value = LocalizationSettings.SelectedLocale.LocaleName;
            language.choices = new List<string>(locales.Count);
            
            foreach (Locale locale in locales)
                language.choices.Add(locale.LocaleName);
            
            //language.RegisterEvent<ChangeEvent<string>>(HandleCallback, _elementsCallbacks, ChangeLanguage);
        }

        private void ChangeLanguage()
        {
            
        }
    }
}