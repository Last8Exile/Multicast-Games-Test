using Scripts.Unity.Extensions;

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Assertions;

using static Scripts.Unity.Gameplay.WordMatchGame;

namespace Scripts.Unity.Gameplay
{
    public class WordElement : MonoBehaviour
    {
        [SerializeField] private Transform _container;
        [SerializeField] private CellElement _prefab;
        [NonSerialized] private List<CellElement> _items;
        [NonSerialized] private CurrentWord _currentWord;

        public IReadOnlyList<CellElement> Cells => _items;
        public CurrentWord CurrentWord => _currentWord;

        private void Awake()
        {
            _items = new();
        }

        public void SetCurrentWord(CurrentWord currentWord)
        {
            Assert.IsNull(_currentWord);
            _currentWord = currentWord;
            var length = currentWord.Characters.Count;
            while (_items.Count < length)
                _items.Add(Instantiate(_prefab, _container).RemoveCloneFromName());
            for (int i = 0; i < _items.Count; i++)
            {
                var item = _items[i];
                var isUsed = i < length;
                if (isUsed)
                    item.SetCurrentWord(this, i);
                item.gameObject.SetActive(isUsed);
            }
        }

        public void Clear()
        {
            _currentWord = null;
            foreach (var item in _items)
                item.Clear();
        }
    }
}
