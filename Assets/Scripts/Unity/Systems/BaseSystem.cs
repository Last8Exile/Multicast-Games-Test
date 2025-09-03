using UnityEngine;

namespace Scripts.Unity.Systems
{
    public abstract class BaseSystem<T> : MonoBehaviour where T : BaseSystem<T>
    {
        public static T Instance => _instance;
        private static T _instance;

        protected virtual void Awake()
        {
            _instance = (T)this;
        }

        protected virtual void OnDestroy()
        {
            _instance = null;
        }
    }
}
