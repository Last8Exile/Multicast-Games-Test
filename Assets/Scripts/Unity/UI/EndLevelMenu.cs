using Scripts.Unity.Systems;

using System;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Unity.UI
{
    public class EndLevelMenu : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _nextLevelButton;
        [SerializeField] private Button _mainMenuButton;
        [SerializeField] private WordsPanel _wordsPanel;

        [NonSerialized] private ILevelData _currentLevel;
        [NonSerialized] private ILevelData _nextLevel;

        private void Awake()
        {
            _retryButton.onClick.AddListener(OnRetryClick);
            _nextLevelButton.onClick.AddListener(OnNextLevelClick);
            _mainMenuButton.onClick.AddListener(OnMainMenuClick);
        }

        private void OnRetryClick()
        {
            StartLevel(_currentLevel);
        }

        private void OnNextLevelClick()
        {
            StartLevel(_nextLevel);
        }

        private void StartLevel(ILevelData levelData)
        {
            if (levelData != null)
                Core.Instance.StartLevel(levelData);
            else
                Core.Instance.MainMenu();
        }

        private void OnMainMenuClick()
        {
            Core.Instance.MainMenu();
        }

        public void Refresh()
        {
            var levelSystem = Systems<LevelsSystem>.Instance;
            var gameplaySystem = Systems<GameplaySystem>.Instance;

            var levelResult = gameplaySystem.Result;

            _currentLevel = gameplaySystem.Level;
            var hasNextLevel = levelSystem.TryGetNextLevel(_currentLevel.Id, out _nextLevel);

            _titleText.text = levelResult.Completed ? "VICTORY" : "DEFEAT";
            _retryButton.gameObject.SetActive(!levelResult.Completed);
            _nextLevelButton.gameObject.SetActive(levelResult.Completed && hasNextLevel);

            _wordsPanel.Clear();
            foreach (var word in levelResult.CompletedWords)
                _wordsPanel.Add(word);
        }
    }
}
