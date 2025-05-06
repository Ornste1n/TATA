using Game.Scripts.Data.Saver;

namespace Game.Scripts.Options
{
    public abstract class Option<T> where T : IJsonSerializable
    {
        protected T Model { get; private set; }

        public void Initialize(T model)
        {
            Model = model;
            HandleOptions();
        }

        protected abstract void HandleOptions();

        public abstract T CreateDefaultModel();
    }
}