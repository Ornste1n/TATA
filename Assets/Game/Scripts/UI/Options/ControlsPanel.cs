using Game.Scripts.Inputs;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using Game.Scripts.UI.Options.Base;

namespace Game.Scripts.UI.Options
{
    public class ControlsPanel : OptionPanel
    {
        private VisualElement _currentSelected;
        
        public ControlsPanel(OptionsWindow window, string name, string button) : base(window, name, button)
        {
            InputActionAsset action = InputManager.Instance.Controls.asset;
            ScrollView scrollView = window.Root.Q<ScrollView>("controls-view");
            
            foreach (InputActionMap map in action.actionMaps)
            {
                foreach (InputAction inputAction in map.actions)
                {
                    for (var i = 0; i < inputAction.bindings.Count; i++)
                    {
                        VisualElement container = new();
                        container.AddToClassList("inputContainer");

                        Label title = new Label(inputAction.name);
                        title.AddToClassList("inputTitle");

                        string bindingDisplay = InputControlPath.ToHumanReadableString(inputAction.bindings[i].effectivePath, 
                            InputControlPath.HumanReadableStringOptions.OmitDevice);
                        
                        Label key = new Label($"[<color=#00ffff>{bindingDisplay}</color>]");
                        key.AddToClassList("inputKey");
                        key.enableRichText = true;

                        var info = new ActionBindingInfo
                        {
                            Action = inputAction,
                            BindingIndex = i,
                            KeyLabel = key
                        };
                        
                        container.Add(title);
                        container.Add(key);

                        container.userData = info;

                        scrollView.Add(container);
                        
                        container.RegisterCallback<ClickEvent>(SelectAction);
                    }
                }
            }
        }

        private void SelectAction(ClickEvent clickEvent)
        {
            VisualElement element = (VisualElement)clickEvent.currentTarget;
            element.AddToClassList("inputSelected");

            if (_currentSelected != null)
            {
                if (ReferenceEquals(element, _currentSelected))
                {
                    Rebinding();
                    return;
                }

                _currentSelected.RemoveFromClassList("inputSelected");
            }
                
            _currentSelected = element;
            _currentSelected.AddToClassList("inputSelected");
        }

        private void Rebinding()
        {
            Debug.Log("Rebinding");
            
            if (_currentSelected?.userData is not ActionBindingInfo info)
                return;

            InputAction action = info.Action;
            int bindingIndex = info.BindingIndex;
            Label keyLabel = info.KeyLabel;

            keyLabel.text = $"[<color=#00ffff>...</color>]";
            
            action.Disable();
            
            InputActionRebindingExtensions.RebindingOperation rebind = action.PerformInteractiveRebinding(bindingIndex)
                .WithCancelingThrough("<Keyboard>/escape");

            rebind.OnComplete(operation =>
                {
                    operation.Dispose();
                    action.Enable();
                    
                    string newPath = action.bindings[bindingIndex].effectivePath;
                    string newDisplay = InputControlPath.ToHumanReadableString(newPath, InputControlPath.HumanReadableStringOptions.OmitDevice);

                    keyLabel.text = $"[<color=#00ffff>{newDisplay}</color>]";
                })
                .OnCancel(op =>
                {
                    op.Dispose();
                    action.Enable();

                    string currentPath = action.bindings[bindingIndex].effectivePath;
                    string display = InputControlPath.ToHumanReadableString(currentPath, InputControlPath.HumanReadableStringOptions.OmitDevice);
                    keyLabel.text = $"[<color=#00ffff>{display}</color>]";
                });

            rebind.Start();
        }
        
        public override void Save()
        {
        }

        public override void Reset()
        {
        }

        public override void Dispose()
        {
        }

        public override bool HasChanged()
        {
            return true;
        }
    }

    public class ActionBindingInfo
    {
        public InputAction Action;
        public int BindingIndex;
        public Label KeyLabel;
    }
}