using Scripts.Unity.Systems;

using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Unity.UI
{
    public class EndLevelMenu : MonoBehaviour
    {
        [SerializeField] private Button _nextLevelButton;
        [SerializeField] private Button _mainMenuButton;

        private void Awake()
        {
            _nextLevelButton.onClick.AddListener(OnNextLevelClick);
            _mainMenuButton.onClick.AddListener(OnMainMenuClick);
        }

        private void OnEnable()
        {
            
        }

        private void OnNextLevelClick()
        {
            if (LevelsSystem.Instance.TryGetNextLevel(SaveSystem.Instance.Data, out var nextLevelData))
                Core.Instance.StartLevel(nextLevelData);
        }

        private void OnMainMenuClick()
        {
            Core.Instance.MainMenu();
        }
    }
}
