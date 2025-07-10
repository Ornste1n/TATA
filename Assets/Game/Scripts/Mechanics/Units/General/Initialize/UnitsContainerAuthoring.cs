using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using UnityEngine.Localization;
using System.Collections.Generic;
using Game.Scripts.Mechanics.Units.Builder;
using Game.Scripts.Mechanics.Units.General.Components;

namespace Game.Scripts.Mechanics.Units.General.Initialize
{
    public class UnitsContainerAuthoring : MonoBehaviour
    {
        [SerializeField] private LocalizedStringTable _unitsTable;
        [SerializeField] private List<UnitObject> _units = new();
        
        public class UnitsContainerAuthoringBaker : Baker<UnitsContainerAuthoring>
        {
            public override void Bake(UnitsContainerAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                
                BlobBuilder builder = new BlobBuilder(Allocator.Temp);
                ref UnitBlobRoot root = ref builder.ConstructRoot<UnitBlobRoot>();
                BlobBuilderArray<UnitBlob> units = builder.Allocate(ref root.Units, authoring._units.Count);
                
                for (int i = 0; i < units.Length; i++)
                {
                    UnitObject uo = authoring._units[i]; 
                    
                    units[i] = new UnitBlob
                    {
                        Id = uo.Id,
                        IconIndex = i,
                        BaseHealth = uo.Health,
                        NameKey = uo.NameRef.Key,
                    };
                }
                
                BlobAssetReference<UnitBlobRoot> blobRef = builder.CreateBlobAssetReference<UnitBlobRoot>(Allocator.Persistent);
                AddBlobAsset(ref blobRef, out _);
                builder.Dispose();
                
                AddComponent(entity, new UnitsCatalogBlobRef { Catalog = blobRef });
                AddComponent(entity, new UnitsLocalizationTable
                {
                    Key = authoring._unitsTable.TableReference.TableCollectionName
                });
                    
                DynamicBuffer<UnitSpriteCatalogElement> icons = AddBuffer<UnitSpriteCatalogElement>(entity);
                foreach (UnitObject uo in authoring._units)
                    icons.Add(new UnitSpriteCatalogElement { Sprites = uo.Icon });
            }
        }
    }
}