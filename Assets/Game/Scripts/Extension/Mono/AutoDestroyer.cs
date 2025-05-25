using System;
using UnityEngine;

namespace Game.Scripts.Extension.Mono
{
    public class AutoDestroyer : MonoBehaviour
    {
        [SerializeField] private UnityFunction _function;
        
        public enum UnityFunction
        {
            Awake,
            Start,
            Update,
            LateUpdate
        }

        private void TryDestroy(UnityFunction currentFunction)
        {
            if (_function == currentFunction)
                Destroy(gameObject);
        }
        
        private void Awake() => TryDestroy(UnityFunction.Awake);
        private void Start() => TryDestroy(UnityFunction.Start);
        private void Update() => TryDestroy(UnityFunction.Update);
        private void LateUpdate() => TryDestroy(UnityFunction.LateUpdate);
    }
}
