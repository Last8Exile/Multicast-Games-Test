using Scripts.Unity.Gameplay;
using Scripts.Unity.Tools;

using System;
using System.Collections.Generic;

using UnityEngine;

namespace Scripts.Unity.Systems
{
    public interface ILevelResult
    {
        bool Completed { get; }
        IEnumerable<string> CompletedWords { get; }
    }

    public class GameplaySystem : BaseSystem
    {
        [SerializeField] private PrefabContainer<WordMatchGame> _wordMatchGameContainer;
        private WordMatchGame _wordMatchGameInstance => _wordMatchGameContainer.Instance;

        public ILevelData Level => _level;
        [NonSerialized] private ILevelData _level;

        public ILevelResult Result => _result;
        [NonSerialized] private ILevelResult _result;

        public void StartLevel(ILevelData levelData)
        {
            _level = levelData;

            _wordMatchGameContainer.GetOrCreateInstance();
            _wordMatchGameInstance.StartLevel();
        }

        public void EndLevel(ILevelResult levelResult)
        {
            _result = levelResult;

            if (levelResult.Completed)
                Systems<SaveSystem>.Instance.LevelCompleted(_level.Id);
            Core.Instance.EndLevel();
        }

        public void Hide()
        {
            if (_wordMatchGameInstance)
                _wordMatchGameInstance.Hide();
        }
    }
}
