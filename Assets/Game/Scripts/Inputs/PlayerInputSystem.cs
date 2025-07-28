using System;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using Game.Scripts.UI.Gameplay;
using Game.Scripts.WorldSettings;
using Game.Scripts.Inputs.Components;

namespace Game.Scripts.Inputs
{
    public partial class PlayerInputSystem : SystemBase
    {
        private Entity _entityInput;
        private MyControls _controls;

        private Camera _mainCamera;
        private UIDocument _mainDocument;
        
        #region Mouse
        private const float DragDistanceThreshold = 5f;
        private const float RaycastDistance = 150f;
        
        private bool _dragging;
        private Vector2 _startSelectionPosition;

        public Action OnMouseEndDrag;
        public Action<float3> OnMouseClick;
        public Action<Vector2> OnMouseBeginDrag;
        #endregion
        
        #region UI
        public Action OnPaused;
        #endregion
        
        protected override void OnCreate()
        {
            _controls = new MyControls(); // todo InputManager.Instance.Controls;
            
            _entityInput = EntityManager.CreateEntity();
            EntityManager.AddComponent<InputData>(_entityInput);

            RequireForUpdate<MainCanvasData>();
            RequireForUpdate<MouseInputComponent>();
        }

        protected override void OnStartRunning()
        {
            _controls.Enable();
            _mainCamera = Camera.main;
            
            MainCanvasData data = SystemAPI.GetSingleton<MainCanvasData>();
            _mainDocument = data.Document.Value;
        }

        protected override void OnUpdate()
        {
            ReadMouse();
            ReadUIInput();
        }
        
        private void ReadMouse()
        {
            Vector2 currentMousePos = Mouse.current.position.value;
            MyControls.CameraActions cameraActions = _controls.Camera;
            
            ref MouseInputComponent input = ref SystemAPI.GetSingletonRW<MouseInputComponent>().ValueRW;
            
            input.WorldMousePosition = GetWorldPosition(currentMousePos, out Ray ray);
            input.ViewMousePosition = _mainCamera.ScreenToViewportPoint(currentMousePos); 
            input.ScreenToWorldRay = ray;
            
            input.Direction = ReadCameraScrollButtons(cameraActions);
            input.OnDragScroll = cameraActions.DragScroll.IsPressed();
            
            ReadMouseInput(currentMousePos, input.WorldMousePosition);
        }
        
        private void ReadMouseInput(float2 currentMousePos, float3 worldPosition)
        {
            InputAction selection = _controls.Units.Selection;
            
            if (selection.WasPressedThisFrame())
                _startSelectionPosition = currentMousePos;

            if (selection.IsPressed())
            {
                float distance = Vector2.Distance(_startSelectionPosition, currentMousePos);

                if (_dragging || !(distance > DragDistanceThreshold)) return;
                
                _dragging = true;
                OnMouseBeginDrag?.Invoke(currentMousePos);
            }

            if (!selection.WasCompletedThisFrame()) return;
            

            if (!_dragging)
            {
                if (IsPointerOverUI()) return;
                
                OnMouseClick?.Invoke(worldPosition);
                EntityManager.AddComponentData(_entityInput, new ClickMouseEvent());

                return;
            }
            
            _dragging = false;
            OnMouseEndDrag?.Invoke();

            Vector3 startPosition = _startSelectionPosition;
            
            float minX = Mathf.Min(startPosition.x, currentMousePos.x);
            float maxX = Mathf.Max(startPosition.x, currentMousePos.x);
            
            float minY = Mathf.Min(startPosition.y, currentMousePos.y);
            float maxY = Mathf.Max(startPosition.y, currentMousePos.y);

            float3 leftTop = GetWorldPosition(new Vector2(minX, maxY));
            float3 leftBottom = GetWorldPosition(new Vector2(minX, minY));
                
            float3 rightTop = GetWorldPosition(new Vector2(maxX, maxY));
            float3 rightBottom = GetWorldPosition(new Vector2(maxX, minY));
                
            EntityManager.AddComponentData(_entityInput, new EndDragEvent(leftTop, leftBottom, rightTop, rightBottom));
        }
        
        private void ReadUIInput()
        {
            if(_controls.Menu.Pause.WasPressedThisFrame())
                OnPaused?.Invoke();
        }
        
        private float3 ReadCameraScrollButtons(MyControls.CameraActions actions)
        {
            float3 direction = float3.zero;
            
            if (actions.ScrollDown.IsPressed())
                direction.z = -1;
            
            if (actions.ScrollUp.IsPressed())
                direction.z = 1;
            
            if (actions.ScrollLeft.IsPressed())
                direction.x = -1;
            
            if (actions.ScrollRight.IsPressed())
                direction.x = 1;

            return direction;
        }
        
        protected override void OnStopRunning() => _controls.Disable();

        private bool IsPointerOverUI()
        {
            Vector2 mousePosition = Mouse.current.position.value;
            mousePosition.y = Screen.height - mousePosition.y;
            
            IPanel panel = _mainDocument.rootVisualElement.panel;
            Vector2 screenPoint = RuntimePanelUtils.ScreenToPanel(panel, mousePosition);

            return panel.Pick(screenPoint) != null;
        }
        
        private float3 GetWorldPosition(Vector3 position)
        {
            Ray ray = _mainCamera.ScreenPointToRay(position);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, RaycastDistance, 1 << (int)WorldLayers.Ground))
                return hitInfo.point;
            
            return float3.zero;
        }

        private float3 GetWorldPosition(Vector3 position, out Ray ray)
        { 
            ray = _mainCamera.ScreenPointToRay(position);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, RaycastDistance, 1 << (int)WorldLayers.Ground))
                return hitInfo.point;
            
            return float3.zero;
        }
    }
}