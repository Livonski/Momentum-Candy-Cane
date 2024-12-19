using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 _originalPosition;
    private Vector3 _originalRotation;
    private Transform _originalParent;
    private int _originalSiblingIdex;
    private Vector2 _dragOffset;

    private CanvasGroup _canvasGroup;
    private RectTransform _rectTransform;
    private Canvas _canvas;

    public Card CardModel;
    public Hand CardOwner;

    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _rectTransform = GetComponent<RectTransform>();

        _canvas = GetComponentInParent<Canvas>();

        if (_canvas == null)
            Debug.LogError("CardInteraction: parent Canvas not found!");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _originalSiblingIdex = transform.GetSiblingIndex();
        transform.SetAsLastSibling();
        _rectTransform.localScale = Vector3.one * 1.2f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _rectTransform.localScale = Vector3.one;
        transform.SetSiblingIndex(_originalSiblingIdex);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _originalParent = transform.parent;
        _originalPosition = _rectTransform.anchoredPosition;
        _originalRotation = _rectTransform.rotation.eulerAngles;

        _canvasGroup.blocksRaycasts = false;
        transform.SetAsLastSibling();
        _rectTransform.rotation = Quaternion.Euler(0,0,0);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.transform as RectTransform,
            eventData.position,
            _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera,
            out Vector2 localPoint);

        _dragOffset = _rectTransform.anchoredPosition - localPoint;
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.transform as RectTransform,
            eventData.position,
            _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera,
            out Vector2 pos);

        _rectTransform.anchoredPosition = pos + _dragOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //TODO: check where card dropped and play card or move it to deck
        // if (overDropZone) { MoveCardToDeck(); } else { ReturnToHand(); }

        _canvasGroup.blocksRaycasts = true;
        if (transform.parent == _originalParent)
        {
            _rectTransform.anchoredPosition = _originalPosition;
            _rectTransform.rotation = Quaternion.Euler(_originalRotation);
        }
    }
}
