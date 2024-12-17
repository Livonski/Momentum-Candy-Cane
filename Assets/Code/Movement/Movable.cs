using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movable : MonoBehaviour, IInitializable
{
    public Vector2Int _gridPosition { get; private set; }
    [SerializeField] private Vector2Int _velocity;
    private Map _map;

    public void OnInitialize(Vector2Int gridPosition, int ID)
    {
        _gridPosition = gridPosition;
        _map = GameObject.FindGameObjectWithTag("Map").GetComponent<Map>();
        TurnManager.Instance.AddMovable(this);

        //Debug value
        _velocity = new Vector2Int(1,1);
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
        if(_map.IsInsideMap(nextPosition))
        {
            _gridPosition = nextPosition;
            transform.position = _map.MapToWorldPosition(_gridPosition);
        }
    }
}
