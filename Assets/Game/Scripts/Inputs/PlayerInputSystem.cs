using System;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.InputSystem;
using Game.Scripts.Inputs.Components;

namespace Game.Scripts.Inputs
{
    public partial class PlayerInputSystem : SystemBase
    {
        private Entity _entityInput;
        private MyControls _controls;
        private EntityManager _entityManager;

        private Camera _mainCamera;
        
        #region Mouse
        
        private bool _dragging;
        private Vector2 _startSelectionPosition;
        private static readonly float DistanceThreshold = 5f;
        
        private readonly Plane _groundPlane = new Plane(Vector3.up, Vector3.zero);
        
        public Action OnMouseEndDrag;
        public Action<Vector2> OnMouseBeginDrag;
        
        #endregion
        
        protected override void OnCreate()
        {
            _mainCamera = Camera.main;
            
            _controls = new MyControls(); // todo InputManager.Instance.Controls;
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            
            _entityInput = _entityManager.CreateEntity();
            _entityManager.AddComponent<InputData>(_entityInput);
        }
        
        protected override void OnStartRunning() => _controls.Enable();

        protected override void OnUpdate()
        {
            ReadSelectionMouse();
        }

        private void ReadSelectionMouse()
        {
            InputAction selection = _controls.Units.Selection;
            
            if (selection.WasPressedThisFrame())
            {
                _startSelectionPosition = Mouse.current.position.value;
            }

            if (selection.IsPressed())
            {
                float distance = Vector2.Distance(_startSelectionPosition, Mouse.current.position.value);

                if (_dragging || !(distance > DistanceThreshold)) return;
                
                _dragging = true;
                OnMouseBeginDrag?.Invoke(Mouse.current.position.value);
            }

            if (!selection.WasCompletedThisFrame()) return;
            
            Vector2 currentMousePos = Mouse.current.position.value;

            if (!_dragging)
            {
                _entityManager.AddComponentData(_entityInput, new ClickMouseEvent(GetWorldPosition(currentMousePos)));
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
                
            _entityManager.AddComponentData(_entityInput, new EndDragEvent(leftTop, leftBottom, rightTop, rightBottom));
        }
        
        protected override void OnStopRunning() => _controls.Disable();

        private float3 GetWorldPosition(Vector3 position)
        {
            Ray ray = _mainCamera.ScreenPointToRay(position);
            
            if(!_groundPlane.Raycast(ray, out float distance)) return float3.zero;

            Vector3 worldVector = ray.GetPoint(distance);

            return new float3(worldVector.x, worldVector.y, worldVector.z);
        }
    }
}