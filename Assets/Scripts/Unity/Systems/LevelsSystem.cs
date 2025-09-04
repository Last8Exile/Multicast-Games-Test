using Newtonsoft.Json;

using Scripts.Unity.Extensions;

using System;
using System.Collections.Generic;

using UnityEngine;

namespace Scripts.Unity.Systems
{
    public interface ILevelData
    {
        Guid Id { get; }
        int WordLength { get; }
        IReadOnlyList<string> Words { get; }
        IReadOnlyList<string> Segments { get; }

        int Index { get; }
    }

    public class LevelData : ILevelData
    {
        public Guid Id { get; set; }
        public int WordLength { get; set; }
        public List<string> Words { get; set; }
        public List<string> Segments { get; set; }
        [JsonIgnore] public int Index { get; set; }

        IReadOnlyList<string> ILevelData.Words => Words;
        IReadOnlyList<string> ILevelData.Segments => Segments;
    }


    public class LevelsSystem : BaseSystem
    {
        [SerializeField] private List<TextAsset> _levelAssets;

        public IReadOnlyList<ILevelData> SortedLevels => _sortedLevels;
        [NonSerialized] private List<LevelData> _sortedLevels;

        [NonSerialized] private Dictionary<Guid, LevelData> _levels;

        protected override void Awake()
        {
             base.Awake();

            _sortedLevels = new();
            _levels = new();

            foreach (var levelAsset in _levelAssets.OrEmpty())
            {
                if (levelAsset == null)
                    continue;

                var json = levelAsset.text;
                if (string.IsNullOrEmpty(json))
                    continue;

                var levelData = JsonConvert.DeserializeObject<LevelData>(json);
                AddLevel(levelData);
            }

            void AddLevel(LevelData levelData)
            {
                levelData.Index = _sortedLevels.Count;
                _levels.Add(levelData.Id, levelData);
                _sortedLevels.Add(levelData);
            }
        }

        public ILevelData FirstLevel => _sortedLevels[0];
        public int LevelsCount => _sortedLevels.Count;

        public bool TryGetLevel(Guid guid, out ILevelData levelData)
        {
            if (_levels.TryGetValue(guid, out var level))
            {
                levelData = level;
                return true;
            }
            levelData = null;
            return false;
        }

        public bool TryGetNextLevel(Guid guid, out ILevelData nextLevelData)
        {
            if (_levels.TryGetValue(guid, out var level))
            {
                var nextLevelIndex = level.Index + 1;
                if (nextLevelIndex < _sortedLevels.Count)
                {
                    nextLevelData = _sortedLevels[nextLevelIndex];
                    return true;
                }
            }
            nextLevelData = null;
            return false;
        }

        public bool TryGetNextLevel(ISaveData saveData, out ILevelData nextLevelData)
        {
            if (TryGetLevel(saveData.LastCompletedLevelId, out var lastCompletedLevel) && 
                TryGetNextLevel(lastCompletedLevel.Id, out var nextLevel))
            {
                nextLevelData = nextLevel;
                return true;
            }

            nextLevelData = null;
            return false;
        }

        public int CountCompletedLevels(ISaveData saveData)
        {
            var count = 0;
            foreach (var level in _sortedLevels)
                if (saveData.TryGetLevelProgress(level.Id, out var progress) && progress.IsCompleted)
                    count++;
            return count;
        }
    }
}

