using System.IO;
using UnityEngine;
using Game.Scripts.Data;
using UnityEngine.InputSystem;

namespace Game.Scripts.Inputs.General
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }
        public MyControls Controls { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            Initialize();
        }

        private void Initialize()
        {
            Controls = new MyControls();
            LoadControlOverrides();
            Controls.Enable();
        }
        
        private void LoadControlOverrides()
        {
            if (File.Exists(Paths.Inputs))
            {
                string json = File.ReadAllText(Paths.Inputs);
                Controls.LoadBindingOverridesFromJson(json);
            }
            else
            {
                Debug.Log("No saved control bindings found. Using defaults.");
            }
        }

        public void SaveControlOverrides()
        {
            string json = Controls.SaveBindingOverridesAsJson();
            File.WriteAllText(Paths.Inputs, json);
        }

        public void ResetOverrides()
        {
            Controls.RemoveAllBindingOverrides();
        }
        
        private void OnDestroy()
        {
            if (Controls != null)
            {
                Controls.Disable();
                Controls.Dispose();
            }
        }
    }
}