using Unity.Entities;

namespace Game.Scripts.Mechanics.Units.General.Components
{
    public struct SkillBlob
    {
        public uint Id;
    }
    
    public struct SkillBlobRoot
    {
        public BlobArray<SkillBlob> Skills;
    }
    
    public struct SkillsCatalogBlobRef : IComponentData
    {
        public BlobAssetReference<SkillBlobRoot> Catalog; 
    }
}