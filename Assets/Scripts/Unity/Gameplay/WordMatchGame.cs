using Cysharp.Threading.Tasks;

using Scripts.Unity.Extensions;
using Scripts.Unity.Systems;
using Scripts.Unity.Tools;

using System;
using System.Collections.Generic;
using System.Linq;

using TMPro;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Scripts.Unity.Gameplay
{
    public class WordMatchGame : MonoBehaviour
    {
        // Visual
        [SerializeField] private GraphicRaycaster _graphicRaycaster;
        [SerializeField] private Button _surrenderButton;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private Transform _dragContainer;

        [SerializeField] private GameObject _resultLabel;
        [SerializeField] private Image _resultBackground;
        [SerializeField] private TextMeshProUGUI _resultText;
        [SerializeField] private ColorRef _victoryColor;
        [SerializeField] private ColorRef _defeatColor;

        [SerializeField] private Transform _worldElementsContainer;
        [SerializeField] private WordElement _wordElementPrefab;
        [NonSerialized] private List<WordElement> _wordElements;

        public Transform SegmentElementsContainer => _segmentElementsContainer;
        [SerializeField] private Transform _segmentElementsContainer;
        [SerializeField] protected SegmentElement _segmentElementPrefab;
        [NonSerialized] protected List<SegmentElement> _segmentElements;

        public SegmentElement DummyDragSegmentElement => _dummyDragSegmentElement;
        [NonSerialized] private SegmentElement _dummyDragSegmentElement;

        // Logic
        [NonSerialized] private ILevelData _levelData;
        [NonSerialized] private LevelResult _levelResult;
        [NonSerialized] private HashSet<TargetWord> _incompleteWords;
        [NonSerialized] private List<CurrentWord> _currentWords;
        [NonSerialized] private List<Segment> _segments;

        private void Awake()
        {
            _wordElements = new();
            _segmentElements = new();

            _incompleteWords = new();
            _currentWords = new();
            _segments = new();

            _surrenderButton.onClick.AddListener(OnSusrrenderClick);
        }

        private void OnSusrrenderClick()
        {
            EndLevel(completed: false);
        }

        public void StartLevel()
        {
            Clear();

            Assert.IsTrue(_incompleteWords.Count == 0);
            Assert.IsTrue(_currentWords.Count == 0);
            Assert.IsTrue(_segments.Count == 0);

            gameObject.SetActive(true);

            _levelData = Systems<GameplaySystem>.Instance.Level;
            
            var wordLength = _levelData.WordLength;
            var wordCount = _levelData.Words.Count;
            var segmentCount = _levelData.Segments.Count;

            foreach (var word in _levelData.Words)
            {
                var targetWord = ClassPool<TargetWord>.Get();
                targetWord.Init(word);
                _incompleteWords.Add(targetWord);

                var currentWord = ClassPool<CurrentWord>.Get();
                currentWord.Init(this, _currentWords.Count, wordLength);
                _currentWords.Add(currentWord);
            }

            foreach (var segmentText in _levelData.Segments)
            {
                var segment = ClassPool<Segment>.Get();
                segment.Init(this, segmentText);
                _segments.Add(segment);
            }
            _segments.Shuffle();

            while (_wordElements.Count < wordCount)
                _wordElements.Add(Instantiate(_wordElementPrefab, _worldElementsContainer).RemoveCloneFromName());
            for (int i = 0; i < _wordElements.Count; i++)
            {
                var wordElement = _wordElements[i];
                var isUsed = i < wordCount;
                if (isUsed)
                    wordElement.SetCurrentWord(_currentWords[i]);
                wordElement.gameObject.SetActive(isUsed);
            }

            while (_segmentElements.Count < segmentCount)
                _segmentElements.Add(Instantiate(_segmentElementPrefab, _segmentElementsContainer).RemoveCloneFromName());
            for (int i = 0; i < _segmentElements.Count; i++)
            {
                var segmentElement = _segmentElements[i];
                var isUsed = i < segmentCount;
                if (isUsed)
                    segmentElement.SetSegment(_segments[i]);
                segmentElement.gameObject.SetActive(isUsed);
            }

            if (_dummyDragSegmentElement == null)
            {
                _dummyDragSegmentElement = Instantiate(_segmentElementPrefab, _dragContainer);
                _dummyDragSegmentElement.SetAsDummyDragObject();
                _dummyDragSegmentElement.gameObject.SetActive(false);
            }

            _graphicRaycaster.enabled = true;
        }

        private void EndLevel(bool completed)
        {
            _levelResult = new LevelResult
            {
                Completed = completed,
                CompletedWords = completed ?
                        _currentWords.Select(c => c.CompletedWord.Text).ToList()
                        : Enumerable.Empty<string>(),
            };

            // Async is needed to unwind stack from gameplay and UI operations (to not interrupt them).

            EndLevelAsync().Forget();
            async UniTaskVoid EndLevelAsync()
            {
                await UniTask.NextFrame();
                
                _graphicRaycaster.enabled = false;
                _resultBackground.color = (_levelResult.Completed ? _victoryColor : _defeatColor).Color;
                _resultText.text = _levelResult.Completed ? "VICTORY" : "DEFEAT";
                _resultLabel.gameObject.SetActive(true);

                await UniTask.Delay(TimeSpan.FromSeconds(1));
                
                Systems<GameplaySystem>.Instance.EndLevel(_levelResult);
            }
        }

        private void Clear()
        {
            void Release<T>(ICollection<T> collection) where T : class, new()
            {
                foreach (var item in collection)
                    ClassPool<T>.Release(item);
                collection.Clear();
            }

            Release(_incompleteWords);
            Release(_currentWords);
            Release(_segments);

            foreach (var item in _wordElements)
                item.Clear();
            foreach (var item in _segmentElements)
            {
                item.transform.SetParent(_segmentElementsContainer);
                item.Clear();
            }

            _scrollRect.horizontalNormalizedPosition = 0;

            _resultLabel.gameObject.SetActive(false);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            Clear();
        }

        public bool CanDropSegment(Segment segment, CurrentWord targetWord, int targetFirstCharacterIndex)
        {
            Assert.IsNull(segment.CurrentWord);
            if (targetFirstCharacterIndex < 0 || targetFirstCharacterIndex + segment.Text.Length > targetWord.Characters.Count)
                return false;
            return IsEmptyCharacters(targetWord.Characters, targetFirstCharacterIndex, segment.Text.Length);
        }

        public void DropSegment(Segment segment, CurrentWord targetWord, int targetFirstCharacterIndex)
        {
            Assert.IsNull(segment.CurrentWord);

            segment.CurrentWord = targetWord;
            segment.FirstCharacterIndex = targetFirstCharacterIndex;
            CopyCharacters(targetWord.Characters, segment.FirstCharacterIndex, segment.Text);

            TryCompleteWord(targetWord);
        }

        private void TryCompleteWord(CurrentWord currentWord)
        {
            Assert.IsNull(currentWord.CompletedWord);

            foreach (var incompeteWord in _incompleteWords)
            {
                if (currentWord.Characters.SequenceEqual(incompeteWord.Text))
                {
                    _incompleteWords.Remove(incompeteWord);
                    currentWord.CompletedWord = incompeteWord;
                    break;
                }
            }      

            if (_incompleteWords.Count == 0)
            {
                EndLevel(completed: true);
            }
        }

        public void RemoveSegmet(Segment segment)
        {
            Assert.IsNotNull(segment.CurrentWord);

            var currentWord = segment.CurrentWord;
            segment.CurrentWord = null;
            ClearCharacters(currentWord.Characters, segment.FirstCharacterIndex, segment.Text.Length);

            if (currentWord.CompletedWord != null)
                UncompleteWord(currentWord);
        }

        private void UncompleteWord(CurrentWord currentWord)
        {
            Assert.IsNotNull(currentWord.CompletedWord);

            var completedWord = currentWord.CompletedWord;
            currentWord.CompletedWord = null;

            _incompleteWords.Add(completedWord);
        }

        private bool IsEmptyCharacters(List<char> characters, int start, int length)
        {
            for (int i = 0; i < length; i++)
                if (characters[start + i] != default)
                    return false;
            return true;
        }

        private void CopyCharacters(List<char> characters, int start, string text)
        {
            for (int i = 0; i < text.Length; i++)
                characters[start + i] = text[i];
        }

        private void ClearCharacters(List<char> characters, int start, int length)
        {
            for (int i = 0; i < length; i++)
                characters[start + i] = default;
        }

        public class TargetWord : IPoolable
        {
            public string Text;

            public void Init(string text)
            {
                Text = text;
            }

            public void Clear()
            {
                Text = null;
            }
        }

        public class CurrentWord : IPoolable
        {
            public WordMatchGame Game;
            public int Index;
            public List<char> Characters = new();
            public TargetWord CompletedWord;

            public void Init(WordMatchGame game, int index, int wordLength)
            {
                Assert.IsNull(Game);
                Assert.IsNull(CompletedWord);
                Assert.IsTrue(Characters.Count == 0);

                Game = game;
                Index = index;
                for (int i = 0; i < wordLength; i++)
                    Characters.Add(default);
            }

            public void Clear()
            {
                Game = null;
                Characters.Clear();
                CompletedWord = null;
            }
        }

        public class Segment : IPoolable
        {
            public WordMatchGame Game;

            public string Text;
            public CurrentWord CurrentWord;
            public int FirstCharacterIndex;

            public void Init(WordMatchGame game, string text)
            {
                Assert.IsNull(Game);
                Assert.IsNull(CurrentWord);
                Game = game;
                Text = text;
            }

            public void Clear()
            {
                Game = null;
                Text = null;
                CurrentWord = null;
            }
        }

        private class LevelResult : ILevelResult
        {
            public bool Completed { get; set; }
            public IEnumerable<string> CompletedWords { get; set; }
        }
    }
}
