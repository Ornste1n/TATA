using Game.Scripts.Mechanics.Units.Builder;
using Unity.Entities;
using UnityEngine.Localization.Tables;
using UnityEngine.Localization.Settings;
using Game.Scripts.Mechanics.Units.General.Components;

namespace Game.Scripts.Mechanics.Units.General.Initialize
{
    [UpdateInGroup(typeof(InitializeUnitsSystemGroup))]
    public partial class UnitsInitializeSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<UnitsLocalizationTable>();
        }

        protected override void OnUpdate()
        {
            UnitsLocalizationTable table = SystemAPI.GetSingleton<UnitsLocalizationTable>();
            StringTable localTable = LocalizationSettings.StringDatabase.GetTable(table.Key.ToString());

            BlobAssetReference<UnitBlobRoot> blobRef = SystemAPI.GetSingleton<UnitsCatalogBlobRef>().Catalog;
            ref UnitBlobRoot blob = ref blobRef.Value;
            
            for (int i = 0; i < blob.Units.Length; i++)
            {
                ref UnitBlob unit = ref blob.Units[i];
                StringTableEntry entry = localTable.GetEntry(unit.NameKey.Value);

                unit.LocalizeName = entry.GetLocalizedString();
            }
            
            SkillsArray array = SystemAPI.ManagedAPI.GetSingleton<SkillsArray>();
            foreach (SkillBase arraySkill in array.Skills)
                arraySkill.Initialize();

            Enabled = false;
        }
    }
}