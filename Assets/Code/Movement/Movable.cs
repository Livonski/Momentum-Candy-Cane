using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Movable : MonoBehaviour, IInitializable
{
    public Vector2Int _gridPosition { get; private set; }
    public Vector2Int Forward { get; private set; }
    [SerializeField] private Vector2Int _velocity;

    private float _moveDelay;
    private Map _map;
    private ICollidable _collidable;
    private Queue<Vector2Int> _moveQueue;

    public void OnInitialize(InitializationData data)
    {
        _gridPosition = data.GridPosition;
        _velocity = data.InitialSpeed;
        transform.name = data.DefaultName + data.ID;

        _map = GameObject.FindGameObjectWithTag("Map").GetComponent<Map>();
        _collidable = GetComponent<ICollidable>();

        TurnManager.Instance.AddMovable(this);
        _moveDelay = TurnManager.Instance.CurrentMoveDelay();

        _moveQueue = new Queue<Vector2Int>();

        Forward = new Vector2Int(-1,0);
    }

    public void AddVelocity(Vector2Int velocity)
    {
        if(velocity == null)
        {
            Debug.LogWarning($"Trying to add null velocity to {transform.name}");
            return;
        }
        _velocity += velocity;
        RecalculateForward();
    }

    public void AccelerateForward(int velocity)
    {
        Debug.Log($"Accelerating forward by {velocity}");
        Vector2Int acceleration = Forward * velocity;
        AddVelocity(acceleration);
    }

    public void Turn(float side)
    {
        //Currently it rotates object by 90 degrees which is fine but i probably need to make
        //an option to rotate 45 degrees

        if(side == 1)
        {
            Debug.Log("Turning right");
            _velocity = new Vector2Int(Forward.y, -Forward.x);
        }
        else
        {
            Debug.Log("Turning left");
            _velocity = new Vector2Int(-Forward.y, Forward.x);
        }
        Forward = _velocity;
    }

    public void Move()
    {
        Vector2Int nextPosition = _gridPosition + _velocity;
        Debug.Log($"Executing move, next position: {nextPosition}");
        MoveTo(nextPosition);
    }

    private void MoveTo(Vector2Int position)
    {
        List<GameObject> objectsInTile = _map.GetObjectsInTile(position);
        bool blocked = false;
        foreach (GameObject obj in objectsInTile)
        {
            if(obj.GetComponent<IBlockable>() != null)
            {
                _collidable.OnCollision(obj);
                blocked = true;
            }
            obj.GetComponent<ICollidable>()?.OnCollision(gameObject);
        }

        if (blocked)
            return;


        if (_map.IsInsideMap(position))
        {
            StopAllCoroutines();
            Vector3 currentPosition = transform.position;
            Vector3 nextPosition = _map.MapToWorldPosition(position) + new Vector3(0,0,-1);

            _map.MoveObject(gameObject, _gridPosition, position);
            _gridPosition = position;

            StartCoroutine(SmoothMove(currentPosition, nextPosition, 0.25f));
            //RecalculateForward();
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
            //Debug.Log($"Dequeuing point {nextPoint}, remaining points: {remainingPoints}");
            MoveTo(nextPoint);
            remainingPoints--;
        }
    }

    public void Stop()
    {
        _velocity = Vector2Int.zero;
        _moveQueue.Clear();
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
            if(!(x0 == _gridPosition.x && y0 == _gridPosition.y))
            {
                _moveQueue.Enqueue(new Vector2Int(x0, y0));
                movePoints++;
                //Debug.Log($"Queueing new point at {x0}:{y0}");
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

    public List<Vector2Int> CalculateMovement()
    {
        List<Vector2Int> movePoints = new List<Vector2Int>();

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
            //if (!(x0 == _gridPosition.x && y0 == _gridPosition.y))
            //{
                movePoints.Add(new Vector2Int(x0, y0));
                //Debug.Log($"Queueing new point at {x0}:{y0}");
            //}

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

    private IEnumerator SmoothMove(Vector3 startPosition, Vector3 endPosition, float duration)
    {
        float elapsedTime = 0f;
        while(elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.position = Vector3.Lerp(startPosition, endPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = endPosition;
    }

    private void RecalculateForward()
    {
        //For some reason this thing is not working properly
        Vector2Int[] directions =
        {
            new Vector2Int(0, 1),
            new Vector2Int(1, 1),
            new Vector2Int(1, 0),
            new Vector2Int(1, -1),
            new Vector2Int(0, -1),
            new Vector2Int(-1, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(-1, 1)
        };

        Vector2Int bestDirection = directions[0];
        float bestScore = float.MinValue;

        foreach (Vector2Int direction in directions)
        {
            float score = Vector2.Dot(_velocity, direction);
            if(score > bestScore)
            {
                bestDirection = direction;
                bestScore = score;
            }
        }
        Forward = bestDirection;
        Debug.Log($"{transform.name} new forward: {Forward}");
    }
}
