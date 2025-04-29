using System.Threading.Tasks;
using Game.Scripts.UI.Options.Base;

namespace Game.Scripts.UI.Options
{
    public class AudioPanel : Option
    {
        public AudioPanel(OptionsWindow window, string name, string button) : base(window, name, button)
        {
        }

        public override Task Save()
        {
            return Task.CompletedTask;
        }

        public override void Reset()
        {
        }
    }
}