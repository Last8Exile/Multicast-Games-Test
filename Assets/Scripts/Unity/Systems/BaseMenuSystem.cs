using System;

using UnityEngine;

namespace Scripts.Unity.Systems
{
    public abstract class BaseMenuSystem<T> : BaseSystem<T> where T : BaseMenuSystem<T>
    {
        [field: NonSerialized] public bool IsVisible { get; private set; }

        public void SetVisible(bool isVisible)
        {
            if (IsVisible == isVisible)
                return;

            var menuInstance = GetMenuInstance();
            if (isVisible && menuInstance == null)
                menuInstance = CreateMenuInstance();

            OnBeforeSetVisisble(isVisible);
            IsVisible = isVisible;
            if (menuInstance)
                menuInstance.gameObject.SetActive(isVisible);
            OnAfterSetVisisble(isVisible);
        }

        protected abstract MonoBehaviour GetMenuInstance();
        protected abstract MonoBehaviour CreateMenuInstance();

        protected virtual void OnBeforeSetVisisble(bool isVisible) { }
        protected virtual void OnAfterSetVisisble(bool isVisible) { }
    }
}
