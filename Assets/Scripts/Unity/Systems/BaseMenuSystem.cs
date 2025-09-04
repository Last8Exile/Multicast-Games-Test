using Scripts.Unity.Tools;

using System;

using UnityEngine;

namespace Scripts.Unity.Systems
{
    public abstract class BaseMenuSystem<T> : BaseSystem where T: MonoBehaviour
    {
        [SerializeField] private PrefabContainer<T> _menuContainer;
        protected T _menuInstance => _menuContainer.Instance;
        [field: NonSerialized] public bool IsVisible { get; private set; }

        public void SetVisible(bool isVisible)
        {
            if (IsVisible == isVisible)
                return;

            var menuInstance = _menuInstance;
            if (isVisible && menuInstance == null)
                menuInstance = _menuContainer.GetOrCreateInstance();

            OnBeforeSetVisisble(isVisible);
            IsVisible = isVisible;
            if (menuInstance)
                menuInstance.gameObject.SetActive(isVisible);
            OnAfterSetVisisble(isVisible);
        }

        protected virtual void OnBeforeSetVisisble(bool isVisible) { }
        protected virtual void OnAfterSetVisisble(bool isVisible) { }
    }
}
