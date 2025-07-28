using System;
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

                DynamicBuffer<UnitPrefabReference> unitsPrefabs = AddBuffer<UnitPrefabReference>(entity);
                DynamicBuffer<UnitSpriteCatalogElement> unitsIcons = AddBuffer<UnitSpriteCatalogElement>(entity);
                DynamicBuffer<SkillSpriteCatalogElement> skillsIcons = AddBuffer<SkillSpriteCatalogElement>(entity);

                List<SkillBase> skillBases = new (units.Length * 2);
                
                for (int i = 0; i < units.Length; i++)
                {
                    UnitObject uo = authoring._units[i]; 
                    unitsIcons.Add(new UnitSpriteCatalogElement(uo.Id, uo.Icon));
                    
                    int currentIndex = skillBases.Count - 1;
                    int startIndex = currentIndex - 1 > 0 ? currentIndex : 0;

                    if (uo.Skills.Count == 0)
                        startIndex = -1;
                    
                    foreach (UnitSkill unitSkill in uo.Skills)
                    {
                        Type skillType = unitSkill.SkillType;

                        if (skillType == null || Activator.CreateInstance(skillType) is not SkillBase skillBase) continue;
                        
                        skillBases.Add(skillBase);
                        skillsIcons.Add(new SkillSpriteCatalogElement(unitSkill.Icon));
                    }

                    unitsPrefabs.Add(new UnitPrefabReference{ Prefab = GetEntity(uo.Prefab, TransformUsageFlags.Dynamic) });
                    units[i] = new UnitBlob(uo.Id, uo.Health, i, startIndex, skillBases.Count - 1, uo.NameRef.Key);
                }
                
                skillBases.TrimExcess();
                
                BlobAssetReference<UnitBlobRoot> blobRef = builder.CreateBlobAssetReference<UnitBlobRoot>(Allocator.Persistent);
                AddBlobAsset(ref blobRef, out _);
                builder.Dispose();

                AddComponentObject(entity, new SkillsArray() { Skills = skillBases});
                
                AddComponent(entity, new UnitsCatalogBlobRef { Catalog = blobRef });
                AddComponent(entity, new UnitsLocalizationTable
                {
                    Key = authoring._unitsTable.TableReference.TableCollectionName
                });
            }
        }
    }
}