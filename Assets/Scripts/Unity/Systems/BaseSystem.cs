using UnityEngine;

namespace Scripts.Unity.Systems
{
    public static class Systems<T> where T : BaseSystem
    {
        public static T Instance => Core.Instance.ServiceProvider.GetSerivce<T>();
    }

    public abstract class BaseSystem : MonoBehaviour
    {
        protected virtual void Awake()
        {
            Core.Instance.ServiceProvider.AddService(this);
        }
    }
}
