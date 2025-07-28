using Unity.Entities;
using Unity.Collections;

namespace Game.Scripts.Mechanics.Units.General.Components
{
    public struct UnitBlob
    {
        public readonly uint Id;
        public readonly int  BaseHealth;
        
        public readonly int BuffersIndex;
        
        public readonly int EndSkillBaseIndex;
        public readonly int StartSkillBaseIndex;
        
        public readonly FixedString64Bytes NameKey;
        public FixedString64Bytes LocalizeName;

        public UnitBlob(uint id, int health, int buffersIndex, int startSkillBaseIndex, int endSkillBaseIndex,  FixedString64Bytes nameKey)
        {
            Id = id;
            NameKey = nameKey;
            BaseHealth = health;
            LocalizeName = default;
            BuffersIndex = buffersIndex;
            EndSkillBaseIndex = endSkillBaseIndex;
            StartSkillBaseIndex = startSkillBaseIndex;
        }

        public override string ToString()
        {
            return
                @$"               Id: {Id}
               NameKey: {NameKey}
               BaseHealth: {BaseHealth}
               LocalizeName: {LocalizeName}
               BuffersIndex: {BuffersIndex}
               EndSkillBaseIndex: {EndSkillBaseIndex}
               StartSkillBaseIndex: {StartSkillBaseIndex}";
        }
    }
    
    public struct UnitBlobRoot
    {
        public BlobArray<UnitBlob> Units;
    }

    public struct UnitsLocalizationTable : IComponentData
    {
        public FixedString64Bytes Key;
    }
    
    public struct UnitsCatalogBlobRef : IComponentData
    {
        public BlobAssetReference<UnitBlobRoot> Catalog; 
    }
}