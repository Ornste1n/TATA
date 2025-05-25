using System;
using Unity.Entities;
using UnityEngine.UIElements;
using Game.Scripts.UI.Gameplay;
using Game.Scripts.Mechanics.Units.General;

namespace Game.Scripts.Mechanics.Units.Selection.UnitsHud
{
    [UpdateInGroup(typeof(SelectionSystemGroup))]
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
    
            VisualTreeAsset unitAsset = unitsData.UnitViewPrefab.Value;
            VisualTreeAsset pageAsset = unitsData.UnitPagePrefab.Value;
            
            VisualElement root = documentData.Document.Value.rootVisualElement;
            
            VisualElement unitsPanel = root.Q<VisualElement>("units-main");
            VisualElement pageList = root.Q<VisualElement>("units-page-list");
            VisualElement unitsContainer = unitsPanel.Q<VisualElement>("units-container");
                
            UnitView[] buffer = new UnitView[unitsData.UnitsCount];
            VisualElement[] pages = new VisualElement[unitsData.PageCount];
            
            for (int i = 0; i < unitsData.UnitsCount; i++)
            {
                TemplateContainer unitTemplate = unitAsset.Instantiate();
                VisualElement unitView = unitTemplate.Q<VisualElement>("unit");

                buffer[i] = new UnitView(unitView);
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

            UnitInfoPanel infoPanel = new UnitInfoPanel(unitsPanel);
            
            UnitsPanelSystem system = World.GetExistingSystemManaged<UnitsPanelSystem>();
            system.Initialize(unitsContainer, infoPanel, pages, buffer);

            Enabled = false;
        }
    }
    
    [UpdateInGroup(typeof(SelectionSystemGroup))]
    public partial class UnitsPanelSystem : SystemBase
    {
        private const string HiddenStyle = "hidden";
        private const string SelectedPage = "unitsPageSelected";
        
        private UnitView[] _unitViews;
        private VisualElement[] _unitPages;
        
        private UnitInfoPanel _unitInfoPanel;
        private VisualElement _unitsSelected;
        private VisualElement _unitsContainer;
        
        private int _activePages;
        private int _currentActive;
        private int _currentPageIndex;
        
        protected override void OnCreate()
        {
            RequireForUpdate<UnitsSelectedEvent>();
        }
        
        public void Initialize(VisualElement container, UnitInfoPanel infoPanel, VisualElement[] pages, UnitView[] views)
        {
            _unitViews = views;
            _unitPages = pages;
            _unitInfoPanel = infoPanel;
            _unitsContainer = container;
            _unitsSelected = container.parent;
            
            foreach (UnitView view in views)
                view.SetActive(false, HiddenStyle);
            
            foreach (VisualElement page in pages)
                page.RegisterCallback<ClickEvent>(ChangeListElement);
            
            container.RegisterCallback<ClickEvent>(SelectUnitView);
        }
        
        protected override void OnUpdate()
        {
            int activePages = _activePages;
            int activeCount = _currentActive;

            UnitView[] unitViews = _unitViews;
            VisualElement[] pages = _unitPages;

            if(_currentPageIndex != 0)
                pages[_currentPageIndex].RemoveFromClassList(SelectedPage);
            
            pages[0].AddToClassList(SelectedPage);
            
            int lastIndex = DistributeUnitsToPages(ref unitViews, out int page, out bool poolIsOver);

            RemoveEventComponent();

            if (page < activePages)
                SetVisiblePages(pages, false, page + 1, activePages);
            else
                SetVisiblePages(pages, true, activePages, page);
            
            if (lastIndex > activeCount || poolIsOver) return;
            
            DeactivateUnits(unitViews, HiddenStyle, lastIndex, activeCount);
        }

        private int DistributeUnitsToPages(ref UnitView[] unitViews, out int page, out bool poolIsOver)
        {
            page = 0;
            poolIsOver = false;
            string hiddenStyle = HiddenStyle;
            
            int i = 0;
            int maxUnitsCount = unitViews.Length;
            
            foreach ((UnitAspect Aspect, RefRW<UnitSelectionTag> Tag) unit in SystemAPI.Query<UnitAspect, RefRW<UnitSelectionTag>>())
            {
                unit.Tag.ValueRW.Group = page;

                if (page == 0)
                {
                    unitViews[i].ActivateOrUpdate(hiddenStyle, unit.Aspect.Entity);
                }
                
                i++;

                if (i != maxUnitsCount) continue;
                
                i = 0;
                page++;
            }

            poolIsOver = page > 0;
            
            _activePages = page;
            _currentPageIndex = 0;
            _currentActive = poolIsOver ? maxUnitsCount : i;
            
            return i;
        }

        private void SelectUnitView(ClickEvent clickEvent)
        {
            VisualElement clicked = (VisualElement)clickEvent.target;
            int index = _unitsContainer.IndexOf(clicked);

            UnitAspect unit = SystemAPI.GetAspect<UnitAspect>(_unitViews[index].Entity);
            
            _unitInfoPanel.Show(unit.Damageable.Health, unit.Damageable.MaxHealth);
            
            _unitInfoPanel.SetActive(true, HiddenStyle);
            _unitsSelected.AddToClassList(HiddenStyle);
        }
        
        private void ChangeListElement(ClickEvent clickEvent)
        {
            VisualElement clickedElement = (VisualElement)clickEvent.currentTarget;
            int index = SwitchPage(clickedElement);
            
            if(index == -1) return;
            
            int i = 0;
            string hiddenStyle = HiddenStyle;
            int activeCount = _currentActive;
            UnitView[] unitViews = _unitViews;
            
            foreach ((UnitAspect Aspect, RefRO<UnitSelectionTag> Tag) unit in SystemAPI.Query<UnitAspect, RefRO<UnitSelectionTag>>())
            {
                if(unit.Tag.ValueRO.Group != index) continue;

                unitViews[i].ActivateOrUpdate(hiddenStyle, unit.Aspect.Entity);
                i++;
            }

            _currentActive = i;
            _currentPageIndex = index;

            if (i > activeCount) return;
            
            DeactivateUnits(unitViews, HiddenStyle, i, activeCount);
        }

        private void RemoveEventComponent()
        {
            Entity eventEntity = SystemAPI.GetSingletonEntity<UnitsSelectedEvent>();
            EntityManager.RemoveComponent<UnitsSelectedEvent>(eventEntity);
        }
        
        private int SwitchPage(VisualElement clickedElement)
        {
            VisualElement[] pages = _unitPages;
            int index = Array.IndexOf(pages, clickedElement);
            
            if(index == -1 || index == _currentPageIndex)
                return -1;
            
            pages[index].AddToClassList(SelectedPage);
            pages[_currentPageIndex].RemoveFromClassList(SelectedPage);

            return index;
        }

        private void SetVisiblePages(VisualElement[] pages, bool isVisible, int start, int end)
        {
            if (isVisible)
            {
                for (int j = start; j <= end; j++)
                    pages[j].RemoveFromClassList(HiddenStyle);
            }
            else
            {
                for (int j = start; j <= end; j++)
                    pages[j].AddToClassList(HiddenStyle);
            }
        }
        
        private void DeactivateUnits(UnitView[] array, string hiddenStyle, int start, int end)
        {
            for (int j = start; j < end; j++)
                array[j].SetActive(false, hiddenStyle);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            foreach (VisualElement page in _unitPages)
                page.UnregisterCallback<ClickEvent>(ChangeListElement);
            
            _unitsContainer.UnregisterCallback<ClickEvent>(SelectUnitView);
        }
    }
}