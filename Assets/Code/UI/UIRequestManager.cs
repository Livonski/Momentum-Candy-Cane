using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRequestManager : MonoBehaviour
{
    public static UIRequestManager Instance;

    [SerializeField] private HighlightArea _leftHighlightArea;
    [SerializeField] private HighlightArea _rightHighlightArea;
    [SerializeField] private float _offset;

    private Action<TurnDirection> _directionCallback;
    private Action<Vector2Int> _positionCallback;

    private void Awake()
    {
        Instance = this;
    }

    public void RequestDirectionChoice(Action<TurnDirection> onChosen, Vector2 playerPosition, Vector2Int forward)
    {
        _directionCallback = onChosen;
        Vector2 forward2D = new Vector2(forward.x, forward.y);
        _leftHighlightArea.PositionArea(playerPosition, forward2D, _offset);
        _rightHighlightArea.PositionArea(playerPosition, forward2D, _offset);
        _leftHighlightArea.BeginListening();
        _rightHighlightArea.BeginListening();

        StartCoroutine(WaitForClickAndDecideDirection(playerPosition, forward));
    }

    public void RequestPositionChoise(Action<Vector2Int> onChosen)
    {
        _positionCallback = onChosen;
        StartCoroutine(WaitForClickAndDecidePosition());
    }

    private IEnumerator WaitForClickAndDecideDirection(Vector2 playerPosition, Vector2Int forward)
    {
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        Vector3 worldClickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 clickDir = (Vector2)worldClickPos - playerPosition;

        float side = forward.x * clickDir.y - forward.y * clickDir.x;
        TurnDirection chosenDirection = side > 0 ? TurnDirection.Left : TurnDirection.Right;

        Debug.Log($"playerPosition{playerPosition}, forward{forward}, worldClickPos{worldClickPos}, clickDir{clickDir}, side{side}, chosenDirection{chosenDirection.ToString()}");

        _directionCallback?.Invoke(chosenDirection);
        _directionCallback = null;

        _leftHighlightArea.StopListening();
        _rightHighlightArea.StopListening();
    }

    private IEnumerator WaitForClickAndDecidePosition()
    {
        bool walidPosition = false;
        while(!walidPosition)
        {
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

            Vector2Int choosenPosition = Vector2Int.one;
            Vector3 worldClickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Map map = GameObject.FindGameObjectWithTag("Map").GetComponent<Map>();
            if (map.IsInsideMap(worldClickPos))
            {
                choosenPosition = map.WorldToMapPosition(worldClickPos);
                _positionCallback.Invoke(choosenPosition);
                _positionCallback = null;
                walidPosition = true;
            }
        }
    }
}
