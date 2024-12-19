using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRequestManager : MonoBehaviour
{
    public static UIRequestManager Instance;

    private Action<TurnDirection> _directionCallback;
    private Action<Vector2Int> _positionCallback;

    private void Awake()
    {
        Instance = this;
    }

    public void RequestDirectionChoice(Action<TurnDirection> onChosen, Vector2 playerPosition, Vector2Int forward)
    {
        _directionCallback = onChosen;
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

        _directionCallback?.Invoke(chosenDirection);
        _directionCallback = null;
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
