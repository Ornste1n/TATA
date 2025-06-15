using Game.Scripts.Data.Saver;

namespace Game.Scripts.Options
{
    public abstract class Option<T> where T : IJsonSerializable
    {
        protected T Model { get; private set; }

        public void Initialize(T model)
        {
            Model = model;
            PresetOptions();
        }

        protected virtual void PresetOptions() {}

        public abstract T CreateDefaultModel();
    }
}