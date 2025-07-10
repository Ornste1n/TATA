using Unity.Entities;
using Unity.Collections;

namespace Game.Scripts.Mechanics.Units.General.Components
{
    public struct UnitBlob
    {
        public uint Id;
        public int  BaseHealth;
        public int  IconIndex;
        public FixedString64Bytes NameKey;
        public FixedString64Bytes LocalizeName;
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