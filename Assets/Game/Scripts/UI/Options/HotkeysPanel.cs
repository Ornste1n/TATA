using Game.Scripts.Inputs;
using Game.Scripts.Inputs.General;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using Game.Scripts.UI.Options.Base;

namespace Game.Scripts.UI.Options
{
    public class HotkeysPanel : OptionPanel
    {
        private Label _navigationLabel;
        private ScrollView _scrollView;
        private VisualElement _currentSelected;

        private bool _hasChanged = false;
        
        public HotkeysPanel(OptionsWindow window, string name, string button) : base(window, name, button)
        {
            InitializeControlsView();
        }

        private void InitializeControlsView()
        {
            InputActionAsset action = InputManager.Instance.Controls.asset;
            ScrollView scrollView = OptionsWindow.Root.Q<ScrollView>("hotkeys-view");

            _scrollView = scrollView;
            
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
            
            _navigationLabel = OptionsWindow.Root.Q<Label>("hotkeys-navigation-label");
            _navigationLabel.text = OptionsWindow.SelectInputSlot;
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

            string newLabel = OptionsWindow.ConfirmInputSlot;
            
            if(!ReferenceEquals(_navigationLabel.text, newLabel))
                _navigationLabel.text = newLabel;
        }

        private void Rebinding()
        {
            if (_currentSelected?.userData is not ActionBindingInfo info)
                return;

            _navigationLabel.text = OptionsWindow.InputNewBind;
            
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
                    
                    _navigationLabel.text = OptionsWindow.SelectInputSlot;
                    
                    _currentSelected.RemoveFromClassList("inputSelected");
                    _currentSelected = null;

                    if (!_hasChanged) _hasChanged = true;
                })
                .OnCancel(op =>
                {
                    op.Dispose();
                    action.Enable();

                    string currentPath = action.bindings[bindingIndex].effectivePath;
                    string display = InputControlPath.ToHumanReadableString(currentPath, InputControlPath.HumanReadableStringOptions.OmitDevice);
                    keyLabel.text = $"[<color=#00ffff>{display}</color>]";
                    
                    _navigationLabel.text = OptionsWindow.SelectInputSlot;
                    
                    _currentSelected.RemoveFromClassList("inputSelected");
                    _currentSelected = null;
                });

            rebind.Start();
        }
        
        public override void Save() => InputManager.Instance.SaveControlOverrides();

        public override void Reset() => InputManager.Instance.ResetOverrides();

        public override void Dispose()
        {
            foreach (VisualElement container in _scrollView.Children())
                container.UnregisterCallback<ClickEvent>(SelectAction);
        }

        public override bool HasChanged() => _hasChanged;
    }

    public class ActionBindingInfo
    {
        public InputAction Action;
        public int BindingIndex;
        public Label KeyLabel;
    }
}