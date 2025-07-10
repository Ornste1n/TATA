using System;
using UnityEngine;

namespace Game.Scripts.Extension.Mono
{
    [Serializable]
    public class SerializableMonoScript
    {
        [SerializeField] private string _typeName = null;
        private Type _type = null;
    
        public Type Type
        {
            get
            {
                if (_type == null)
                {
                    if (string.IsNullOrEmpty(_typeName))
                    {
                        return null;
                    }

                    _type = Type.GetType(_typeName);
                }

                return _type;
            }
            set
            {
                _type = value;
                if (_type == null)
                {
                    _typeName = "";
                }
                else
                {
                    _typeName = _type.AssemblyQualifiedName;
                }
            }
        }
    }

    [Serializable]
    public class SerializableMonoScript<T> : SerializableMonoScript where T : class
    {
        public T AddToGameObject(GameObject go)
        {
            var type = Type;
            if (type != null)
            {
                return (T)(object)go.AddComponent(type);
            }

            return default(T);
        }
    }
}
