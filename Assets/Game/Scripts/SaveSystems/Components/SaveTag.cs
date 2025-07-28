using System;
using Unity.Entities;

namespace Game.Scripts.SaveSystems.Components
{
    public class SaveAttribute : Attribute
    {
        
    }
    
    public struct SaveEvent : IComponentData { }
}
