using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class HighlightArea : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Which side this area represents")]
    [SerializeField] private HighlightSide _side;

    [Header("Canvas RectTransform (Screen Space - Overlay)")]
    [SerializeField] private RectTransform _canvasRect;

    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;

    [SerializeField] private Image _highlightImage;
    [SerializeField] private GameObject _textGO;
    [SerializeField] private Color _activeColor;
    [SerializeField] private Color _inactiveColor;

    private bool _isListening = false;

    private Vector2 _lastPosition = Vector2.zero;
    private Vector2 _lastForward = Vector2.zero;
    private float _lastOffset = 0f;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_highlightImage != null)
            _highlightImage.color = _inactiveColor;
        _textGO.SetActive(false);
    }

    public void PositionArea(Vector2 playerPosition, Vector2 forward, float offset)
    {
        _lastPosition = playerPosition;
        _lastForward = forward;
        _lastOffset = offset;

        Vector2 perp;
        if (_side == HighlightSide.Left)
        {
            perp = new Vector2(-forward.y, forward.x);
        }
        else
        {
            perp = new Vector2(forward.y, -forward.x);
        }
        perp.Normalize();
        Vector2 worldPos = playerPosition + perp * offset;

        Camera camera = Camera.main;
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(camera, worldPos);

        Vector2 anchoredPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvasRect,
            screenPos,
            null,
            out anchoredPos);
        _rectTransform.anchoredPosition = anchoredPos;

        float angle = Mathf.Atan2(forward.y, forward.x) * (180 / Mathf.PI) + 180;
        _rectTransform.localEulerAngles = new Vector3(0f, 0f, angle);
        _textGO.GetComponent<RectTransform>().localEulerAngles = new Vector3(0f, 0f, -angle);
    }

    public void OnCameraMove()
    {
        if (_isListening)
            PositionArea(_lastPosition, _lastForward, _lastOffset);
    }

    public void BeginListening()
    {
        _isListening = true;
        _canvasGroup.blocksRaycasts = true;
    }

    public void StopListening()
    {
        _isListening = false;
        _highlightImage.color = _inactiveColor;
        _canvasGroup.blocksRaycasts = false;
        _textGO.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_highlightImage != null  && _isListening)
        {
            _highlightImage.color = _activeColor;
            _textGO.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_highlightImage != null && _isListening)
        {
            _highlightImage.color = new Color(1, 1, 1, 0);
            _textGO.SetActive(false);
        }
    }
}

public enum HighlightSide
{
    Left,
    Right
}