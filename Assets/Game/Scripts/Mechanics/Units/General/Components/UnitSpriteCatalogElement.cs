using UnityEngine;
using Unity.Entities;
using UnityEngine.UIElements;

namespace Game.Scripts.Mechanics.Units.General.Components
{
    public struct UnitSpriteCatalogElement : IBufferElementData
    {
        public UnityObjectRef<Sprite> Sprites;
    }

    public struct SkillSpriteCatalogElement : IBufferElementData
    {
        public UnityObjectRef<Sprite> Sprites;
    }
}