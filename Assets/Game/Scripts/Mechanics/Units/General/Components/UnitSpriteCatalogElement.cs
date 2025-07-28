using UnityEngine;
using Unity.Entities;

namespace Game.Scripts.Mechanics.Units.General.Components
{
    public struct UnitSpriteCatalogElement : IBufferElementData
    {
        public readonly uint UnitId;
        public readonly UnityObjectRef<Sprite> Sprite;

        public UnitSpriteCatalogElement(uint id, Sprite sprite)
        {
            UnitId = id;
            Sprite = sprite;
        }
    }

    public struct SkillSpriteCatalogElement : IBufferElementData
    {
        public readonly UnityObjectRef<Sprite> Sprite;

        public SkillSpriteCatalogElement(Sprite sprite)
        {
            Sprite = sprite;
        }
    }
}