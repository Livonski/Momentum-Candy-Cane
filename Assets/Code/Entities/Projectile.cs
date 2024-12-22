using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, ICollidable, IDestroyable, IDamageDealer, IBlockable
{
    [SerializeField] private Movable _movable;
    [SerializeField] private int _damage;
    [SerializeField] private int _maxVelocity;

    private void Start()
    {
        _movable = GetComponent<Movable>();
    }

    public void OnCollision(GameObject obj)
    {
        Debug.Log($"{transform.name} got hit by {obj.name}");
        TakeDamage(1);
        _movable.Stop();
    }

    public void TakeDamage(int damage)
    {
        TurnManager.Instance.EnqueueDestruction(gameObject);
        gameObject.SetActive(false);
    }

    public void OnRemove()
    {
        GameObject.FindGameObjectWithTag("Map").GetComponent<Map>().RemoveObject(gameObject, _movable._gridPosition);
        Destroy(gameObject);
    }

    public int CalculateDamage()
    {
        return _damage;
    }

    public Vector2Int CalculateVelocity(Vector2Int position, Vector2Int direction, out Vector2Int initialPosition)
    {
        //It is possible to spawn a projectile on top of some other object
        //In this case objects doesn't collide with each other
        Vector2Int velocity = CalculateMovePoints(position, direction, out initialPosition);
        return velocity;
    }

    private Vector2Int CalculateMovePoints(Vector2Int position, Vector2Int direction, out Vector2Int initialPosition)
    {
        int movePoints = 0;
        bool succes = false;

        Vector2Int velocity = Vector2Int.zero;
        initialPosition = position;

        int x0 = position.x;
        int y0 = position.y;
        int x1 = direction.x;
        int y1 = direction.y;

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = (x0 < x1) ? 1 : -1;
        int sy = (y0 < y1) ? 1 : -1;

        int err = dx - dy;

        while (!succes)
        {
            if(!(x0 == position.x && y0 == position.y))
            {
                movePoints++;
                if(movePoints == _maxVelocity)
                {
                    velocity = new Vector2Int(x0 - position.x, y0 - position.y);
                    succes = true;
                    return velocity;
                }
            }
            if (movePoints == 1)
                initialPosition = new Vector2Int(x0, y0);

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
        velocity = new Vector2Int(x0 - position.y, y0 - position.y);
        return velocity;
    }
}
