using Scripts.Unity.Systems;

using System;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Unity.UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private TextMeshProUGUI _playLevelText;
        [SerializeField] private TextMeshProUGUI _completedCountText;
        [SerializeField] private Button _exitButton;

        [NonSerialized] private ILevelData _levelToPlay;


        private void Awake()
        {
            _playButton.onClick.AddListener(OnPlayClick);
            _exitButton.onClick.AddListener(OnExitClick);
        }

        private void OnPlayClick()
        {
            Core.Instance.StartLevel(_levelToPlay);
        }

        private void OnExitClick() 
        {
            Core.Instance.Quit();
        }

        public void Refresh()
        {
            var saveData = Systems<SaveSystem>.Instance.Data;
            var levelSystem = Systems<LevelsSystem>.Instance;

            if (!levelSystem.TryGetNextLevel(saveData, out _levelToPlay))
                _levelToPlay = levelSystem.FirstLevel;
            _playLevelText.text = $"Level {_levelToPlay.Index + 1}";
            _completedCountText.text = $"{levelSystem.CountCompletedLevels(saveData)} / {levelSystem.LevelsCount}";
        }
    }
}