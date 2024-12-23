using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Movable : MonoBehaviour, IInitializable
{
    public Vector2Int _gridPosition { get; private set; }
    public Vector2Int Forward { get; private set; }
    public int Momentum { get; private set; }

    [SerializeField] private Vector2Int _velocity;

    private float _moveDelay;
    private Map _map;
    private ICollidable _collidable;
    private Queue<Vector2Int> _moveQueue;

    private static readonly List<Vector2Int> Directions = new List<Vector2Int>
    {
        new Vector2Int( 0,  1),  // up
        new Vector2Int( 1,  1),  // up-right
        new Vector2Int( 1,  0),  // right
        new Vector2Int( 1, -1),  // down-right
        new Vector2Int( 0, -1),  // down
        new Vector2Int(-1, -1),  // down-left
        new Vector2Int(-1,  0),  // left
        new Vector2Int(-1,  1)   // up-left
    };

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

        Momentum = 0;

        if(_velocity != Vector2Int.zero)
        {
            RecalculateForward();
        }
        else
        {
            Forward = new Vector2Int(-1, 0);
            RotateToForward();
        }
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
    public void AccelerateBackward(int velocity)
    {
        Debug.Log($"Accelerating backward by {velocity}");
        Vector2Int acceleration = Forward * -velocity;
        _velocity += acceleration;
    }

    public void Turn(float side, int numRotations)
    {
        int currentIndex = Directions.IndexOf(Forward);
        if (currentIndex < 0)
        {
            Debug.LogWarning("Forward not found in directions list.");
            return;
        }

        int delta = (side == 1) ? numRotations : -numRotations;
        int newIndex = (currentIndex + delta) % Directions.Count;
        if (newIndex < 0)
        {
            newIndex += Directions.Count; 
        }

        Forward = Directions[newIndex];
        _velocity = Forward;
        RotateToForward();
    }

    public void Move()
    {
        Vector2Int nextPosition = _gridPosition + _velocity;
        Debug.Log($"Executing move, next position: {nextPosition}");
        MoveTo(nextPosition);
    }

    private IEnumerator MoveTo(Vector2Int position)
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
            yield break;

        if (_map.IsInsideMap(position))
        {
            Vector3 currentPosition = transform.position;
            Vector3 nextPosition = _map.MapToWorldPosition(position) + new Vector3(0,0,-1);

            _map.MoveObject(gameObject, _gridPosition, position);
            _gridPosition = position;

            yield return StartCoroutine(SmoothMove(currentPosition, nextPosition, TurnManager.Instance._moveDelay));
            //RecalculateForward();
        }
    }

    public IEnumerator MoveBy(int points)
    {
        if(_moveQueue.Count == 0)
            yield break;

        int remainingPoints = points;

        while (remainingPoints > 0 && _moveQueue.Count > 0)
        {
            Vector2Int nextPoint = _moveQueue.Dequeue();
            Debug.Log($"Dequeuing point {nextPoint}, remaining points: {remainingPoints}");

            yield return StartCoroutine(MoveTo(nextPoint));

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
        Momentum = movePoints;
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
           movePoints.Add(new Vector2Int(x0, y0));

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
        Momentum = movePoints.Count - 1;
        return movePoints;
    }

    private IEnumerator SmoothMove(Vector3 startPosition, Vector3 endPosition, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
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
        //This is suboptimal, but really convenient. So I'll use this for now...
        List<Vector2Int> points = CalculateMovement();

        if (points.Count > 1)
        {
            Forward = points[1] - _gridPosition;
        }
        else
        {
            Forward = new Vector2Int(-1, 0);
        }

        RotateToForward();
        Debug.Log($"{transform.name} new forward: {Forward}");
    }
    private void RotateToForward()
    {
        Vector3 newRotation = transform.rotation.eulerAngles;
        newRotation.z = Mathf.Atan2(Forward.y, Forward.x) * Mathf.Rad2Deg + 180;
        transform.rotation = Quaternion.Euler(newRotation);
    }
}
