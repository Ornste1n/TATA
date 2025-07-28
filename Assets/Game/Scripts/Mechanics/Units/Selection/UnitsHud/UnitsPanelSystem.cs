using System;
using Unity.Entities;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Game.Scripts.Extension.ECS;
using Game.Scripts.Mechanics.Units.General;
using Game.Scripts.Mechanics.Units.General.Components;
using Game.Scripts.Mechanics.Units.Selection.UnitsHud.Elements;

namespace Game.Scripts.Mechanics.Units.Selection.UnitsHud
{
    [UpdateInGroup(typeof(UpdateSelectionSystem))]
    public partial class UnitsPanelSystem : SystemBase
    {
        private const string HiddenStyle = "hidden";
        private const string SelectedPage = "unitsPageSelected";

        #region Visual Elements

        private bool _initialized;
        private int _activePages;
        private int _currentActive;
        private int _currentPageIndex;
        
        private UnitView[] _unitViews;
        private VisualElement[] _unitPages;
        
        private UnitInfoPanel _unitInfoPanel;
        private InstrumentsPanel _instrumentsPanel;
        
        private VisualElement _unitsSelected;
        private VisualElement _unitsContainer;

        #endregion

        #region Events

        public Action<int> TriedUseSkill;

        #endregion
        
        #region Resoureces

        private BlobAssetReference<UnitBlobRoot> _unitsBlob;
        private Dictionary<uint, StyleBackground> _unitsSprites;

        #endregion
        
        protected override void OnCreate()
        {
            RequireForUpdate<UnitsSelectedEvent>();
        }

        public void Initialize(Dictionary<uint, StyleBackground> sprites, VisualElement container,
            UnitInfoPanel infoPanel, InstrumentsPanel instPanel, VisualElement[] pages, UnitView[] views)
        {
            _unitViews = views;
            _unitPages = pages;
            _unitsSprites = sprites;
            _unitInfoPanel = infoPanel;
            _unitsContainer = container;
            _instrumentsPanel = instPanel;
            _unitsSelected = container.parent;
            
            foreach (UnitView view in views)
                view.SetActive(false, HiddenStyle);
            
            foreach (VisualElement page in pages)
                page.RegisterCallback<ClickEvent>(ChangeListElement);
            
            _initialized = true;
            instPanel.SubscribesSkills(TriedUseSkill);
            container.RegisterCallback<ClickEvent>(HandleUnitViewEvent);
            _unitsBlob = SystemAPI.GetSingleton<UnitsCatalogBlobRef>().Catalog;
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

            if (_unitInfoPanel.IsActive)
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
            
            Dictionary<uint, StyleBackground> sprites = _unitsSprites;
            
            foreach ((UnitAspect Aspect, RefRW<UnitSelectionTag> Tag) unit in SystemAPI.Query<UnitAspect, RefRW<UnitSelectionTag>>())
            {
                unit.Tag.ValueRW.Group = page;

                if (page == 0)
                    unitViews[i++].ActivateOrUpdate(hiddenStyle, sprites[unit.Aspect.Id], unit.Aspect.Entity);

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
            ref UnitBlobRoot blobRoot = ref _unitsBlob.Value;

            if (!BlobUtility.TryGetUnit(ref blobRoot, unit.Id, out UnitBlob blob)) return;

            Entity entityBuffer = SystemAPI.GetSingletonEntity<SkillSpriteCatalogElement>();
            DynamicBuffer<SkillSpriteCatalogElement> skills = SystemAPI.GetBuffer<SkillSpriteCatalogElement>(entityBuffer);

            if (blob.StartSkillBaseIndex != -1)
                _instrumentsPanel.ActiveSkills(skills, blob.StartSkillBaseIndex, blob.EndSkillBaseIndex);
            else
                _instrumentsPanel.DisableSkills();
            
            _unitInfoPanel.Show(entity, blob.LocalizeName.ToString(), _unitsSprites[unit.Id], unit.Damageable);
            SetActiveInfoPanel(true);
        }

        private void SetActiveInfoPanel(bool active)
        {
            _unitInfoPanel.SetActive(active, HiddenStyle);
            if (active)
            {
                _unitsSelected.AddToClassList(HiddenStyle);
            }
            else
            {
                _instrumentsPanel.DisableSkills();
                _unitsSelected.RemoveFromClassList(HiddenStyle);
            }
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
           
            Dictionary<uint, StyleBackground> sprites = _unitsSprites;
            
            foreach ((UnitAspect Aspect, RefRO<UnitSelectionTag> Tag) unit in SystemAPI.Query<UnitAspect, RefRO<UnitSelectionTag>>())
            {
                if(unit.Tag.ValueRO.Group != index) continue;
                
                unitViews[i++].ActivateOrUpdate(hiddenStyle, sprites[unit.Aspect.Id], unit.Aspect.Entity);
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
            
            _instrumentsPanel.Dispose();
            _unitsContainer.UnregisterCallback<ClickEvent>(HandleUnitViewEvent);
        }
    }
}