using Cysharp.Threading.Tasks;

using System;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Unity
{
    public class DialogMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _titleLabel;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private GameObject _messageLabel;
        [SerializeField] private TextMeshProUGUI _messageText;
        [SerializeField] private Button _okButton;
        [SerializeField] private TextMeshProUGUI _okText;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private TextMeshProUGUI _cancelText;
        [NonSerialized] private UniTaskCompletionSource<DialogResult> _tcs;

        private void Awake()
        {
            _okButton.onClick.AddListener(OnOkClick);
            _cancelButton.onClick.AddListener(OnCancelClick);
        }

        private void OnDestroy()
        {
            SetCanceled();
        }

        public UniTask<DialogResult> ShowAsync(DialogOptions options)
        {
            SetCanceled();
            _tcs = new();

            _titleLabel.SetActive(!string.IsNullOrEmpty(options.Title));
            _titleText.text = options.Title;

            _messageLabel.SetActive(!string.IsNullOrEmpty(options.Message));
            _messageText.text = options.Message;

            _okButton.gameObject.SetActive(!string.IsNullOrEmpty(options.Ok));
            _okText.text = options.Ok;

            _cancelButton.gameObject.SetActive(!string.IsNullOrEmpty(options.Cancel));
            _cancelText.text = options.Cancel;

            return _tcs.Task;
        }

        private void OnOkClick() => SetResult(new DialogResult { IsOk = true });

        private void OnCancelClick() => SetResult(new DialogResult { IsOk = false });

        private void SetResult(DialogResult result)
        {
            _tcs?.TrySetResult(result);
            _tcs = null;
        }

        private void SetCanceled()
        {
            _tcs?.TrySetCanceled();
            _tcs = null;
        }
    }
}
