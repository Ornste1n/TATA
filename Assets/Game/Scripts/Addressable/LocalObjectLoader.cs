using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Scripts.Addressable
{
    public abstract class CachedObjectLoader
    {
        private GameObject _cachedObject;

        protected async Task<T> LoadIternal<T>(string assetKey, Transform parent = null, bool active = true)
        {
            var handle = Addressables.InstantiateAsync(assetKey, parent);
            _cachedObject = await handle.Task;

            if (!_cachedObject.TryGetComponent(out T component)) throw new NullReferenceException();

            _cachedObject.SetActive(active);

            return component;
        }

        protected void UnloadInternal()
        {
            if (_cachedObject == null) return;

            _cachedObject.SetActive(false);
            Addressables.ReleaseInstance(_cachedObject);
            _cachedObject = null;
        }
    }
    
    public static class LocalObjectLoader
    {
        public static async Task<T> LoadIternal<T>(string assetKey)
        {
            var handle = Addressables.LoadAssetAsync<T>(assetKey);
            T result = await handle.Task;
            return result;
        }
    }
}