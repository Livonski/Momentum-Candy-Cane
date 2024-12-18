using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, ICollidable, IDestroyable, IDamageDealer, IBlockable
{
    [SerializeField] private Movable _movable;
    [SerializeField] private int _damage;

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
}
