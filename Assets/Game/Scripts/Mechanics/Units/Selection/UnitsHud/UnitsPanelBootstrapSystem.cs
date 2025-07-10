using UnityEngine;
using Unity.Entities;
using UnityEngine.UIElements;
using Game.Scripts.UI.Gameplay;
using System.Collections.Generic;
using UnityEngine.Localization.Tables;
using UnityEngine.Localization.Settings;
using Game.Scripts.Mechanics.Units.General.Components;
using Game.Scripts.Mechanics.Units.General.Initialize;
using Game.Scripts.Mechanics.Units.Selection.UnitsHud.Elements;

namespace Game.Scripts.Mechanics.Units.Selection.UnitsHud
{
    [UpdateInGroup(typeof(InitializeUnitsSystemGroup))]
    [UpdateAfter(typeof(UnitsInitializeSystem))]
    public partial class UnitsPanelBootstrapSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<MainCanvasData>();
            RequireForUpdate<UnitsPanelData>();
            RequireForUpdate<UnitsLocalizationTable>();
        }
    
        protected override void OnUpdate()
        {
            MainCanvasData documentData = SystemAPI.GetSingleton<MainCanvasData>();
            UnitsPanelData unitsData = SystemAPI.GetSingleton<UnitsPanelData>();

            VisualTreeAsset unitAsset = unitsData.UnitViewPrefab.Value;
            VisualTreeAsset pageAsset = unitsData.UnitPagePrefab.Value;
            VisualTreeAsset skillAsset = unitsData.UnitSkillPrefab.Value;
            
            VisualElement root = documentData.Document.Value.rootVisualElement;
            
            VisualElement unitsPanel = root.Q<VisualElement>("units-main");
            VisualElement pageList = root.Q<VisualElement>("units-page-list");
            VisualElement unitsContainer = unitsPanel.Q<VisualElement>("units-container");
            
            VisualElement instrumentsPanel = root.Q<VisualElement>("instruments-panel");
            VisualElement skillsContent = instrumentsPanel.Q<VisualElement>("skills-content");
                
            UnitView[] unitBuffer = new UnitView[unitsData.UnitsCount];
            VisualElement[] pages = new VisualElement[unitsData.PageCount];
            
            for (int i = 0; i < unitsData.UnitsCount; i++)
            {
                TemplateContainer unitTemplate = unitAsset.Instantiate();
                VisualElement unitView = unitTemplate.Q<VisualElement>("unit");

                unitBuffer[i] = new UnitView(unitView);
                unitsContainer.Add(unitView);
            }

            for (int i = 0; i < unitsData.PageCount; i++)
            {
                TemplateContainer template = pageAsset.Instantiate();
                VisualElement page = template.Q<VisualElement>("unit-page");
                
                page.Q<Label>("page-number").text = $"{i + 1}";
                page.AddToClassList("hidden");
                pageList.Add(page);
                
                pages[i] = page;
            }

            for (int i = 0; i < unitsData.SkillCount; i++)
            {
                TemplateContainer template = skillAsset.Instantiate();
                VisualElement unitView = template.Q<VisualElement>("unit-skill");

                //unitBuffer[i] = new UnitView(unitView);
                skillsContent.Add(unitView);
            }
            
            UnitInfoPanel infoPanel = new UnitInfoPanel(unitsPanel);
            UnitsPanelSystem system = World.GetExistingSystemManaged<UnitsPanelSystem>();

            UnitsLocalizationTable table = SystemAPI.GetSingleton<UnitsLocalizationTable>();
            StringTable localTable = LocalizationSettings.StringDatabase.GetTable(table.Key.ToString());
            
            Entity catalogEntity = SystemAPI.GetSingletonEntity<UnitsCatalogBlobRef>();
            BlobAssetReference<UnitBlobRoot> blobRef = SystemAPI.GetSingleton<UnitsCatalogBlobRef>().Catalog;
            ref UnitBlobRoot blob = ref blobRef.Value;

            DynamicBuffer<UnitSpriteCatalogElement> icons = SystemAPI.GetBuffer<UnitSpriteCatalogElement>(catalogEntity);
            Dictionary<uint, UnitPanelData> unitPanels = new Dictionary<uint, UnitPanelData>(blob.Units.Length);
            
            for (int i = 0; i < blob.Units.Length; i++)
            {
                ref readonly UnitBlob unit = ref blob.Units[i];
                Sprite sprite = icons[unit.IconIndex].Sprites.Value;
                StringTableEntry entry = localTable.GetEntry(unit.NameKey.Value);
                
                unitPanels.TryAdd(unit.Id, new UnitPanelData()
                {
                    Name = entry.GetLocalizedString(),
                    Image = new StyleBackground(sprite),
                });
            }
            
            system.Initialize(unitPanels, unitsContainer, infoPanel, pages, unitBuffer);
            Enabled = false;
        }
    }

    public class UnitPanelData
    {
        public string Name;
        public StyleBackground Image;
    }
}