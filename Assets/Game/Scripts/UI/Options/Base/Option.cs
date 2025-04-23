namespace Game.Scripts.UI.Options.Base
{
    public abstract class Option
    {
        public abstract void Save();
        public abstract void Reset();
        public abstract bool HasChanged();
    }
}