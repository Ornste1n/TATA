using UnityEngine;
using Unity.Entities;
using UnityEngine.UIElements;

namespace Game.Scripts.Mechanics.Units.Selection.UnitsHud
{
    public class UnitsPanelAuthoring : MonoBehaviour
    {
        [SerializeField] private int _maxUnitsSize;
        [SerializeField] private VisualTreeAsset _unitPrefab;
        [Space] 
        [SerializeField] private int _maxSkillSize;
        [SerializeField] private VisualTreeAsset _unitSkillPrefab;
        [Space]
        [SerializeField] private int _maxPageSize;
        [SerializeField] private VisualTreeAsset _unitPagePrefab;
        
        public class CanvasAuthoringBaker : Baker<UnitsPanelAuthoring>
        {
            public override void Bake(UnitsPanelAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new UnitsPanelData()
                {
                    PageCount = authoring._maxPageSize,
                    UnitsCount = authoring._maxUnitsSize,
                    SkillCount = authoring._maxSkillSize,
                    UnitViewPrefab = authoring._unitPrefab,
                    UnitPagePrefab = authoring._unitPagePrefab,
                    UnitSkillPrefab = authoring._unitSkillPrefab,
                });
            }
        }
    }

    public struct UnitsPanelData : IComponentData
    {
        public int PageCount;
        public int UnitsCount;
        public int SkillCount;
        public UnityObjectRef<VisualTreeAsset> UnitViewPrefab;
        public UnityObjectRef<VisualTreeAsset> UnitPagePrefab;
        public UnityObjectRef<VisualTreeAsset> UnitSkillPrefab;
    }
}