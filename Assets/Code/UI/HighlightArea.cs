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

    private bool _isHighlighted = false;
    private bool _isListening = false;

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
        //Currently this stuff is not working properly
        //I'll need to rethink it later
        //The idea is to shift an object to the player position
        //But i think rotation is messing things up
        /*Vector2 perp;
        if (_side == HighlightSide.Left)
        {
            perp = new Vector2(-forward.y, forward.x);
        }
        else
        {
            perp = new Vector2(forward.y, -forward.x);
        }
        perp.Normalize();

        float halfWidth = _rectTransform.rect.width * _rectTransform.lossyScale.x * 0.5f;
        float halfHeight = _rectTransform.rect.height * _rectTransform.lossyScale.x * 0.5f;
        Vector2 worldPos = playerPosition + perp * offset;

        Camera camera = Camera.main;
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(camera, worldPos);
        screenPos += perp * _rectTransform.rect.height;

        Vector2 anchoredPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvasRect,
            screenPos,
            null,
            out anchoredPos);

        _rectTransform.anchoredPosition = anchoredPos;*/

        float angle = Mathf.Atan2(forward.x, forward.y) * 180 / Mathf.PI - 90;
        _rectTransform.localEulerAngles = new Vector3(0f, 0f, angle);

        //TODO: figure out how I want to change text position to be perfectly centered
        _textGO.GetComponent<RectTransform>().localEulerAngles = new Vector3(0f, 0f, -angle);
        //Debug.Log($"{_side.ToString()} highlight area: perp:{perp}, worldPos: {worldPos}, screenPos: {screenPos}, anchoredPos: {anchoredPos}, playerPos: {playerPosition}. forward: {forward}");
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
        _isHighlighted = false;
        _canvasGroup.blocksRaycasts = false;
        _textGO.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_highlightImage != null  && _isListening)
        {
            _highlightImage.color = _activeColor;
            _isHighlighted = true;
            _textGO.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_highlightImage != null && _isListening)
        {
            _highlightImage.color = new Color(1, 1, 1, 0);
            _isHighlighted = false;
            _textGO.SetActive(false);
        }
    }
}

public enum HighlightSide
{
    Left,
    Right
}