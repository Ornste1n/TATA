using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

namespace Game.Scripts.Scenes
{
    public class SceneTransitionElement : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;

        private Label _continueLabel;
        private InputAction _spaceAction;

        public event Action OnSpacePerformed;

        public void ShowLabel()
        {
            _spaceAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/space");
            _spaceAction.performed += SpacePerformed;
            _spaceAction.Enable();
            
            _continueLabel = _uiDocument.rootVisualElement.Q<Label>("continue-label");
            _continueLabel.RemoveFromClassList("hidden");
        }

        private void SpacePerformed(InputAction.CallbackContext _)
        {
            _spaceAction.performed -= SpacePerformed;
            OnSpacePerformed?.Invoke();
        }

        private void OnDestroy()
        {
            if(_spaceAction == null) return;
            
            _spaceAction.Disable();
            _spaceAction.Dispose();
        }
    }
}