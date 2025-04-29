using UnityEngine.UIElements;
using UnityEngine.Localization;
using System.Collections.Generic;
using System.Threading.Tasks;
using Game.Scripts.UI.Options.Base;
using UnityEngine.Localization.Settings;

namespace Game.Scripts.UI.Options
{
    public class GeneralPanel : Option
    {
        public GeneralPanel(OptionsWindow window, string name, string button) : base(window, name, button)
        {
            InitializeCollection();
            InitLanguage();
        }

        public override Task Save()
        {
            return Task.CompletedTask;
        }

        public override void Reset()
        {
        }

        private void InitLanguage()
        {
            DropdownField language = OptionsWindow.Root.Q<DropdownField>("language");
            List<Locale> locales = LocalizationSettings.AvailableLocales.Locales;
            
            language.value = LocalizationSettings.SelectedLocale.LocaleName;
            language.choices = new List<string>(locales.Count);
            
            foreach (Locale locale in locales)
                language.choices.Add(locale.LocaleName);
            
            SubscribeChangedCallback(language, ChangeLanguage);
        }

        private void ChangeLanguage()
        {
            
        }
    }
}