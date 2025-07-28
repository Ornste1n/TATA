using System;

namespace Game.Scripts.Mechanics.Units.Builder
{
    public abstract class SkillBase : IDisposable
    {
        public abstract void Initialize();
        public abstract void Execute();

        public virtual void Dispose()
        {
            
        }
    }
}