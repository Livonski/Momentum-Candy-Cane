using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour, ICollidable, IEntity, IBlockable, IDestroyable
{
    [SerializeField] private Movable _movable;
    [SerializeField] private int _maxHP;
    [SerializeField] private int _currentHP;

    private void Start()
    {
        _movable = GetComponent<Movable>();
        _currentHP = _maxHP;
    }

    public void OnCollision()
    {
        Debug.Log($"Collision! {transform.name}");
        _currentHP -= 1;
        if(_currentHP <= 0)
        {
            TurnManager.Instance.EnqueueDestruction(gameObject);
            gameObject.SetActive(false);
        }
        _movable.Stop();
    }

    public void OnRemove()
    {
        GameObject.FindGameObjectWithTag("Map").GetComponent<Map>().RemoveObject(gameObject, _movable._gridPosition);
        Destroy(gameObject);
    }
}
