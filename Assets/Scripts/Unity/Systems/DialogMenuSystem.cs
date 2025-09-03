using Cysharp.Threading.Tasks;

using Scripts.Unity.Systems;
using Scripts.Unity.Tools;

using System;
using System.Threading;

using UnityEngine;

namespace Scripts.Unity
{
    public struct DialogOptions
    {
        public string Title;
        public string Message;
        public string Ok;
        public string Cancel;
    }

    public struct DialogResult
    {
        public bool IsOk;
    }


    public class DialogMenuSystem : BaseSystem<DialogMenuSystem>
    {
        [SerializeField] private PrefabContainer<DialogMenu> _dialogContainer;
        [NonSerialized] private SemaphoreSlim _semaphore;

        protected override void Awake()
        {
            base.Awake();
            _semaphore = new SemaphoreSlim(1, 1);
        }

        protected override void OnDestroy()
        {
            _semaphore?.Dispose();
            _semaphore = null;
            base.OnDestroy();
        }

        public async UniTask<DialogResult> ShowAsync(DialogOptions options)
        {
            await _semaphore.WaitAsync(Application.exitCancellationToken);
            try
            {
                var dialog = _dialogContainer.GetOrCreateInstance();
                dialog.gameObject.SetActive(true);
                var result = await dialog.ShowAsync(options);
                dialog.gameObject.SetActive(false);
                return result;
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
