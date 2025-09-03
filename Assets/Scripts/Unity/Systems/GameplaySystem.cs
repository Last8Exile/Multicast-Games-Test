using Scripts.Unity.Systems;
using Scripts.Unity.Tools;

using System;

using UnityEngine;

namespace Scripts.Unity.Gameplay
{
    public interface ILevelResult
    {
        ILevelData LevelData { get; }
        bool Completed { get; }
    }

    public class GameplaySystem : BaseSystem<GameplaySystem>
    {
        [SerializeField] private PrefabContainer<Gameplay> _gameplayContainer;
        private Gameplay _gameplayInstance => _gameplayContainer.Instance;

        public ILevelResult Result => _result;
        [NonSerialized] private LevelResult _result;

        public void StartLevel(ILevelData levelData)
        {
            _gameplayContainer.GetOrCreateInstance();
            _result = new LevelResult() { LevelData = levelData };

            SetVisible(true);
        }

        public void Hide()
        {
            SetVisible(false);
        }

        private void SetVisible(bool visible)
        {
            if (_gameplayInstance)
                _gameplayInstance.gameObject.SetActive(visible);
        }

        private class LevelResult : ILevelResult
        {
            public ILevelData LevelData { get; set; }
            public bool Completed { get; set; }
        }
    }
}
