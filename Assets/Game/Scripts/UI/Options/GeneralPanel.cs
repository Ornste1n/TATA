using Game.Scripts.UI.Options.Base;

namespace Game.Scripts.UI.Options
{
    public class GeneralPanel : Option
    {
        public GeneralPanel(OptionsWindow window, string name, string button) : base(window, name, button)
        {
        }

        public override void Save()
        {
        }

        public override void Reset()
        {
        }
    }
}