using Newtonsoft.Json;

using Scripts.Unity.Extensions;

using System;
using System.Collections.Generic;

namespace Scripts.Unity.Systems
{
    public interface ISaveData
    {
        Guid LastCompletedLevelId { get; }
        bool TryGetLevelProgress(Guid levelId, out ILevelProgress progress);
    }

    public interface ILevelProgress
    {
        Guid LevelId { get; }
        bool IsCompleted { get; }
    }

    public class SaveSystem : BaseSystem
    {
        [NonSerialized] private bool _isLoaded;
        [NonSerialized] private string _key;

        public ISaveData Data => _data;
        [NonSerialized] private SaveData _data;

        public void Load(string key)
        {
            _key = key;
            DoPrefsAction(PrefsAction.Get);
            _data ??= new SaveData();
            _isLoaded = true;
        }

        public void LevelCompleted(Guid levelId)
        {
            var data = _data;

            data.LastCompletedLevelId = levelId;
            var levels = data.Levels;
            if (!levels.TryGetValue(levelId, out var levelProgress))
                levelProgress = levels[levelId] = new();
            levelProgress.IsCompleted = true;

            Save();
        }

        private void Save()
        {
            if (_isLoaded)
                DoPrefsAction(PrefsAction.Set);
        }

        private void DoPrefsAction(PrefsAction action)
        {
            PlayerPrefsExtensions.PrefsJson(_key, action, ref _data);
        }

        private class SaveData : ISaveData
        {
            public Guid LastCompletedLevelId { get; set; }
            public Dictionary<Guid, LevelProgress> Levels { get; set; } = new();

            public bool TryGetLevelProgress(Guid levelId, out ILevelProgress progress)
            {
                if (Levels.TryGetValue(levelId, out var levelProgress))
                {
                    levelProgress.LevelId = levelId;
                    progress = levelProgress;
                    return true;
                }
                progress = null;
                return false;
            }
        }

        private class LevelProgress : ILevelProgress 
        {
            [JsonIgnore] public Guid LevelId { get; set; }
            public bool IsCompleted { get; set; }
        }
    }
}