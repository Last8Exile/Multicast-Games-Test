using System;
using System.Collections.Generic;

using UnityEngine;

namespace Scripts.Unity.UI
{
    public class WordsPanel : MonoBehaviour
    {
        [SerializeField] private Transform _container;
        [SerializeField] private WordItem _prefab;

        [NonSerialized] private List<WordItem> _items;
        [NonSerialized] private int _activeItems;

        private void Awake()
        {
            _items = new();
        }

        public void Add(string word)
        {
            WordItem item;

            if (_activeItems == _items.Count)
            {
                item = Instantiate(_prefab, _container);
                item.transform.SetAsLastSibling();
                _items.Add(item);
            }
            else
            {
                item = _items[_activeItems];
            }

            item.Show(word);
            _activeItems++;
        }

        public void Clear()
        {
            for (int i = 0; i < _activeItems; i++)
                _items[i].Hide();
            _activeItems = 0;
        }
    }
}
