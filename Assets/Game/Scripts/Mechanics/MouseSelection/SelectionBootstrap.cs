using UnityEngine;

namespace Game.Scripts.Mechanics.MouseSelection
{
    public class SelectionBootstrap : MonoBehaviour
    {
        [SerializeField] private Material _shaderSelectionMaterial;

        private SelectionAreaDrawer _drawer;
        
        private void Awake()
        {
            _drawer = new SelectionAreaDrawer(_shaderSelectionMaterial);
        }

        private void OnDestroy()
        {
            _drawer.Dispose();
        }
    }
}