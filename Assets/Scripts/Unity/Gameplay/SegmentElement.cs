using Scripts.Unity.Extensions;

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using static Scripts.Unity.Gameplay.WordMatchGame;

namespace Scripts.Unity.Gameplay
{
    public class SegmentElement : MonoBehaviour, IInitializePotentialDragHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private GraphicRaycaster _raycaster;
        [SerializeField] private Image _background;
        [SerializeField] private ColorRef _normalColor;
        [SerializeField] private ColorRef _acceptedDragColor;

        [SerializeField] private Transform _container;
        [SerializeField] private CharacterElement _prefab;
        [NonSerialized] private List<CharacterElement> _items;

        public Segment Segment => _segment;
        [NonSerialized] private Segment _segment;

        [NonSerialized] private bool _isDummyDragObject;

        private void Awake()
        {
            _items = new();
        }

        public void SetAsDummyDragObject()
        {
            _isDummyDragObject = true;
            _raycaster.enabled = false;
            var fitter = gameObject.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        public void SetSegment(Segment segment)
        {
            Assert.IsNull(_segment);
            _segment = segment;
            var length = segment.Text.Length;
            while (_items.Count < length)
                _items.Add(Instantiate(_prefab, _container).RemoveCloneFromName());
            for (int i = 0; i < _items.Count; i++)
            {
                var item = _items[i];
                var isUsed = i < length;
                if (isUsed)
                    item.SetCharacter(segment.Text[i]);
                item.gameObject.SetActive(isUsed);
            }
        }

        public void Clear()
        {
            _segment = null;
        }


        [NonSerialized] private Vector2 _beginDragPointerPosition;
        [NonSerialized] private Vector2 _beginDragPosition;
        [NonSerialized] private bool _canDrop;
        [NonSerialized] private bool _dragStartedInWord;
        [NonSerialized] private int _dropCharacterIndex;

        void IInitializePotentialDragHandler.OnInitializePotentialDrag(PointerEventData eventData)
        {
            _beginDragPointerPosition = eventData.position;
            _beginDragPosition = transform.position;
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            _dragStartedInWord = _segment.CurrentWord != null;

            var delta = eventData.position - _beginDragPointerPosition;
            if (!_dragStartedInWord && Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                eventData.pointerDrag = null;
                var scroll = GetComponentInParent<ScrollRect>();
                if (scroll)
                {
                    scroll.OnInitializePotentialDrag(eventData);
                    scroll.OnBeginDrag(eventData);
                    eventData.pointerDrag = scroll.gameObject;
                }
                return;
            }

            _canvas.enabled = false;
            _raycaster.enabled = false;
            SetCanDrop(false);

            _dropCharacterIndex = 0;
            for (var i = 0; i < _segment.Text.Length; i++)
            {
                var item = _items[i];
                if (RectTransformUtility.RectangleContainsScreenPoint(item.transform as RectTransform, eventData.position))
                {
                    _dropCharacterIndex = i;
                    break;
                }
            }


            if (_dragStartedInWord)
                _segment.Game.RemoveSegmet(_segment);

            var dummy = _segment.Game.DummyDragSegmentElement;
            dummy.SetSegment(_segment);
            dummy.gameObject.SetActive(true);
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            var dummy = _segment.Game.DummyDragSegmentElement;
            dummy.transform.position = _beginDragPosition + (eventData.position - _beginDragPointerPosition);
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            var dummy = _segment.Game.DummyDragSegmentElement;
            dummy.Clear();
            dummy.gameObject.SetActive(false);

            _canvas.enabled = true;
            _raycaster.enabled = true;
            if (!_canDrop && _dragStartedInWord)
            {
                transform.SetParent(_segment.Game.SegmentElementsContainer);
                transform.SetAsLastSibling();
            }
            SetCanDrop(false);
        }

        public void SetCanDrop(bool canDrop)
        {
            _canDrop = canDrop;
            if (_isDummyDragObject)
                _background.color = (canDrop ? _acceptedDragColor : _normalColor).Color;
            else
                _segment.Game.DummyDragSegmentElement.SetCanDrop(canDrop);
        }

        public int GetDropCharacterIndex()
        {
            return _dropCharacterIndex;
        }
    }
}
