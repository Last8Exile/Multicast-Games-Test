using TMPro;

using UnityEngine;

namespace Scripts.Unity.UI
{
    public class WordItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;

        public void Show(string word)
        {
            _text.text = word;
            SetVisible(true);
        }

        public void Hide() => SetVisible(false);

        private void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }
    }
}
