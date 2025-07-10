using System;
using Unity.Entities;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Game.Scripts.Mechanics.Units.General;
using Game.Scripts.Mechanics.Units.Selection.UnitsHud.Elements;

namespace Game.Scripts.Mechanics.Units.Selection.UnitsHud
{
    [UpdateInGroup(typeof(UpdateSelectionSystem))]
    public partial class UnitsPanelSystem : SystemBase
    {
        private const string HiddenStyle = "hidden";
        private const string SelectedPage = "unitsPageSelected";
        
        private bool _initialized;
        private UnitView[] _unitViews;
        private VisualElement[] _unitPages;
        
        private Dictionary<uint, UnitPanelData> _unitData;
        
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
        
        public void Initialize
        (
            Dictionary<uint, UnitPanelData> data,
            VisualElement container, 
            UnitInfoPanel infoPanel, 
            VisualElement[] pages, 
            UnitView[] views
        )
        {
            _unitData = data;
            _unitViews = views;
            _unitPages = pages;
            _unitInfoPanel = infoPanel;
            _unitsContainer = container;
            _unitsSelected = container.parent;
            
            foreach (UnitView view in views)
                view.SetActive(false, HiddenStyle);
            
            foreach (VisualElement page in pages)
                page.RegisterCallback<ClickEvent>(ChangeListElement);
            
            _initialized = true;
            container.RegisterCallback<ClickEvent>(HandleUnitViewEvent);
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

            if (lastIndex == 1 && !poolIsOver)
            {
                ShowUnitView(0);
                return;
            }
            
            if(_unitInfoPanel.IsActive)
                SetActiveInfoPanel(false);
            
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
            Dictionary<uint, UnitPanelData> unitPanelS = _unitData;

            foreach ((UnitAspect Aspect, RefRW<UnitSelectionTag> Tag) unit in SystemAPI.Query<UnitAspect, RefRW<UnitSelectionTag>>())
            {
                unit.Tag.ValueRW.Group = page;

                if (page == 0)
                    unitViews[i++].ActivateOrUpdate(hiddenStyle, unitPanelS[unit.Aspect.Id].Image, unit.Aspect.Entity);

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

        private void HandleUnitViewEvent(ClickEvent clickEvent)
        {
            VisualElement clicked = (VisualElement)clickEvent.target;
            int index = _unitsContainer.IndexOf(clicked);
            ShowUnitView(index);
        }

        private void ShowUnitView(int index)
        {
            Entity entity = _unitViews[index].Entity;
            
            if(_unitInfoPanel.IsActive && _unitInfoPanel.Entity == entity) return;
            
            UnitAspect unit = SystemAPI.GetAspect<UnitAspect>(entity);
            
            _unitInfoPanel.Show(entity, _unitData[unit.Id], unit.Damageable);
            SetActiveInfoPanel(true);
        }

        private void SetActiveInfoPanel(bool active)
        {
            _unitInfoPanel.SetActive(active, HiddenStyle);
            if(active)
                _unitsSelected.AddToClassList(HiddenStyle);
            else
                _unitsSelected.RemoveFromClassList(HiddenStyle);
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
            Dictionary<uint, UnitPanelData> unitPanel = _unitData;
            
            foreach ((UnitAspect Aspect, RefRO<UnitSelectionTag> Tag) unit in SystemAPI.Query<UnitAspect, RefRO<UnitSelectionTag>>())
            {
                if(unit.Tag.ValueRO.Group != index) continue;

                unitViews[i].ActivateOrUpdate(hiddenStyle, unitPanel[unit.Aspect.Id].Image, unit.Aspect.Entity);
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
            if(!_initialized) return;
            
            base.OnDestroy();
            
            foreach (VisualElement page in _unitPages)
                page.UnregisterCallback<ClickEvent>(ChangeListElement);
            
            _unitsContainer.UnregisterCallback<ClickEvent>(HandleUnitViewEvent);
        }
    }
}