using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 _originalPosition;
    private Transform _originalParent;
    private int _originalSiblingIdex;

    private CanvasGroup _canvasGroup;
    private RectTransform _rectTransform;
    private Canvas _canvas;

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

        _canvasGroup.blocksRaycasts = false;
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.transform as RectTransform,
            eventData.position,
            _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera,
            out Vector2 pos);

        _rectTransform.anchoredPosition = pos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //TODO: check where card dropped and play card or move it to deck
        // if (overDropZone) { MoveCardToDeck(); } else { ReturnToHand(); }

        _rectTransform.anchoredPosition = _originalPosition;
        _canvasGroup.blocksRaycasts = true;
    }
}
