using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardDropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _highlightImage;
    [SerializeField] private Color _activeColor;
    [SerializeField] private Color _inactiveColor;

    private bool _isHighlighted = false;

    private void Start()
    {
        if(_highlightImage != null)
            _highlightImage.color = _inactiveColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_highlightImage != null && eventData.pointerDrag != null)
        {
            _highlightImage.color =_activeColor;
            _isHighlighted = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData )
    { 
        if (_highlightImage != null)
        {
            _highlightImage.color = new Color(1, 1, 1, 0);
            _isHighlighted = false;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        CardInteraction card = eventData.pointerDrag?.GetComponent<CardInteraction>();
        if (card != null && card.CardOwner != null && card.CardModel != null)
        {
            card.CardOwner.PlayCard(card.CardModel);
        }
    }
}
