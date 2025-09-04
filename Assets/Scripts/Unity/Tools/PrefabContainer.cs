using Scripts.Unity.Extensions;

using System;

using UnityEngine;

using UObject = UnityEngine.Object;

namespace Scripts.Unity.Tools
{
    [Serializable]
    public class PrefabContainer<T> where T : MonoBehaviour
    {
        [SerializeField] private T _prefab;
        [NonSerialized] private T _instance;

        public T Instance => _instance == null ? null : _instance;

        public T GetOrCreateInstance()
        {
            if (_instance == null)
                _instance = UObject.Instantiate(_prefab).RemoveCloneFromName();
            return _instance;
        }

        public void DestroyInstance()
        {
            if (_instance)
                UObject.DestroyImmediate(_instance);
        }
    }
}
