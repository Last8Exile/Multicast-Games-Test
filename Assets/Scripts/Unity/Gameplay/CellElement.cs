using System;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Scripts.Unity.Gameplay
{
    public class CellElement : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Transform _segmentContainer;
        [SerializeField] private Image _backround;
        [NonSerialized] private WordElement _wordElement;
        [NonSerialized] private int _characterIndex;


        public void SetCurrentWord(WordElement wordElement, int characterIndex)
        {
            Assert.IsNull(_wordElement);
            _wordElement = wordElement;
            _characterIndex = characterIndex;
        }

        public void Clear()
        {
            _wordElement = null;
        }


        void IDropHandler.OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag && eventData.pointerDrag.TryGetComponent<SegmentElement>(out var segmentElement))
            {
                var currentWord = _wordElement.CurrentWord;
                var game = currentWord.Game;

                var targetFirstCharacterIndex = _characterIndex - segmentElement.GetDropCharacterIndex();
                if (game.CanDropSegment(segmentElement.Segment, currentWord, targetFirstCharacterIndex))
                {
                    game.DropSegment(segmentElement.Segment, currentWord, targetFirstCharacterIndex);
                    segmentElement.transform.SetParent(_wordElement.Cells[targetFirstCharacterIndex]._segmentContainer);
                }
            }
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.dragging && eventData.pointerDrag && eventData.pointerDrag.TryGetComponent<SegmentElement>(out var segmentElement))
            {
                var currentWord = _wordElement.CurrentWord;
                var game = currentWord.Game;

                var targetFirstCharacterIndex = _characterIndex - segmentElement.GetDropCharacterIndex();
                var canDrop = game.CanDropSegment(segmentElement.Segment, currentWord, targetFirstCharacterIndex);
                segmentElement.SetCanDrop(canDrop);
            }
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            if (eventData.dragging && eventData.pointerDrag && eventData.pointerDrag.TryGetComponent<SegmentElement>(out var segmentElement))
            {
                segmentElement.SetCanDrop(false);
            }
        }
    }
}
