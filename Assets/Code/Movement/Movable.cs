using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Movable : MonoBehaviour, IInitializable
{
    public Vector2Int _gridPosition { get; private set; }
    [SerializeField] private Vector2Int _velocity;
    private Map _map;
    private Queue<Vector2Int> _moveQueue;

    public void OnInitialize(InitializationData data)
    {
        _gridPosition = data.GridPosition;
        _velocity = data.InitialSpeed;
        transform.name = data.DefaultName + data.ID;

        _map = GameObject.FindGameObjectWithTag("Map").GetComponent<Map>();
        TurnManager.Instance.AddMovable(this);

        _moveQueue = new Queue<Vector2Int>();
    }

    public void AddVelocity(Vector2Int velocity)
    {
        if(velocity == null)
        {
            Debug.LogWarning($"Trying to add null velocity to {transform.name}");
            return;
        }

        _velocity += velocity;
    }

    public void Move()
    {
        //TODO: check for collisions
        Vector2Int nextPosition = _gridPosition + _velocity;
        Debug.Log($"Executing move, next position: {nextPosition}");
        MoveTo(nextPosition);
    }

    private void MoveTo(Vector2Int position)
    {
        if (!_map.IsEmptyTile(position))
        {
            Debug.Log("Collision!");
            _velocity = Vector2Int.zero;
            _moveQueue.Clear();
            return;
            //TODO: call object that we collided with
        }

        if (_map.IsInsideMap(position))
        {
            _map.MoveObject(gameObject, _gridPosition, position);
            _gridPosition = position;
            transform.position = _map.MapToWorldPosition(_gridPosition);
        }
    }

    public void MoveBy(int points)
    {
        if(_moveQueue == new Queue<Vector2Int>())
            return;
        int remainingPoints = points;
        while (remainingPoints > 0 && _moveQueue.Count > 0)
        {
            Vector2Int nextPoint = _moveQueue.Dequeue();
            Debug.Log($"Dequeuing point {nextPoint}, remaining points: {remainingPoints}");
            MoveTo(nextPoint);
            remainingPoints--;
        }
    }

    public int CalculateMovePoints()
    {
        int movePoints = 0;
        _moveQueue.Clear();

        int x0 = _gridPosition.x;
        int y0 = _gridPosition.y;
        int x1 = _gridPosition.x + _velocity.x;
        int y1 = _gridPosition.y + _velocity.y;

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = (x0 < x1) ? 1 : -1;
        int sy = (y0 < y1) ? 1 : -1;

        int err = dx - dy;

        while (true)
        {
            if(x0 != _gridPosition.x && y0 != _gridPosition.y)
            {
                _moveQueue.Enqueue(new Vector2Int(x0, y0));
                movePoints++;
                Debug.Log($"Queueing new point at {x0}:{y0}");
            }

            if (x0 == x1 && y0 == y1)
                break;

            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
        return movePoints;
    }

}
