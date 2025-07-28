using Unity.Entities;
using UnityEngine.UIElements;
using Game.Scripts.UI.Gameplay;
using System.Collections.Generic;
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
        }

        protected override void OnUpdate()
        {
            MainCanvasData documentData = SystemAPI.GetSingleton<MainCanvasData>();
            UnitsPanelData unitsData = SystemAPI.GetSingleton<UnitsPanelData>();

            VisualElement root = documentData.Document.Value.rootVisualElement;

            VisualElement unitsPanel = root.Q<VisualElement>("units-main");
            VisualElement pageList = root.Q<VisualElement>("units-page-list");
            VisualElement unitsContainer = unitsPanel.Q<VisualElement>("units-container");
            VisualElement instrumentsPanel = root.Q<VisualElement>("instruments-panel");
            VisualElement skillsContent = instrumentsPanel.Q<VisualElement>("skills-content");

            VisualElement[] pages = BuildPages(unitsData, pageList);
            UnitView[] unitBuffer = BuildUnits(unitsData, unitsContainer);
            SkillView[] skillViews = BuildSkills(unitsData, skillsContent);

            UnitInfoPanel infoPanel = new UnitInfoPanel(unitsPanel);
            InstrumentsPanel instPanel = new InstrumentsPanel(instrumentsPanel, skillViews);
            Dictionary<uint, StyleBackground> spriteMap = GetAndClearSprites();

            UnitsPanelSystem system = World.GetExistingSystemManaged<UnitsPanelSystem>();
            system.Initialize(spriteMap, unitsContainer, infoPanel, instPanel, pages, unitBuffer);

            Enabled = false;
        }

        private UnitView[] BuildUnits(UnitsPanelData data, VisualElement container)
        {
            UnitView[] unitBuffer = new UnitView[data.UnitsCount];
            VisualTreeAsset unitAsset = data.UnitViewPrefab.Value;

            for (int i = 0; i < data.UnitsCount; i++)
            {
                TemplateContainer unitTemplate = unitAsset.Instantiate();
                VisualElement unitView = unitTemplate.Q<VisualElement>("unit");

                unitBuffer[i] = new UnitView(unitView);
                container.Add(unitView);
            }

            return unitBuffer;
        }

        private VisualElement[] BuildPages(UnitsPanelData data, VisualElement container)
        {
            VisualElement[] pages = new VisualElement[data.PageCount];
            VisualTreeAsset pageAsset = data.UnitPagePrefab.Value;

            for (int i = 0; i < data.PageCount; i++)
            {
                TemplateContainer template = pageAsset.Instantiate();
                VisualElement page = template.Q<VisualElement>("unit-page");

                page.Q<Label>("page-number").text = $"{i + 1}";
                page.AddToClassList("hidden");
                container.Add(page);

                pages[i] = page;
            }

            return pages;
        }

        private SkillView[] BuildSkills(UnitsPanelData data, VisualElement container)
        {
            SkillView[] skills = new SkillView[data.SkillCount];
            VisualTreeAsset skillAsset = data.UnitSkillPrefab.Value;

            for (int i = 0; i < data.SkillCount; i++)
            {
                TemplateContainer template = skillAsset.Instantiate();
                VisualElement unitView = template.Q<VisualElement>("unit-skill");
                container.Add(unitView);

                skills[i] = new SkillView(unitView);
            }
            
            return skills;
        }

        private Dictionary<uint, StyleBackground> GetAndClearSprites()
        {
            Entity catalogEntity = SystemAPI.GetSingletonEntity<UnitsCatalogBlobRef>();
            DynamicBuffer<UnitSpriteCatalogElement> unitsSprites = SystemAPI.GetBuffer<UnitSpriteCatalogElement>(catalogEntity);

            Dictionary<uint, StyleBackground> sprites = new(unitsSprites.Length);
            for (int i = 0; i < unitsSprites.Length; i++)
            {
                sprites.Add(unitsSprites[i].UnitId, new StyleBackground(unitsSprites[i].Sprite.Value));
            }

            EntityManager.RemoveComponent<UnitSpriteCatalogElement>(catalogEntity);
            return sprites;
        }
    }
}
