using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour, ICollidable, IBlockable, IDestroyable, IDamageDealer
{
    private bool _player;

    [SerializeField] private Movable _movable;
    [SerializeField] private int _maxHP;
    [SerializeField] private int _currentHP;

    [SerializeField] private int _damageOnCollision;

    public int CandiesEaten { get; private set; }

    private void Start()
    {
        _movable = GetComponent<Movable>();
        _currentHP = _maxHP;
        _player = GetComponent<Player>() != null;
        CandiesEaten = 0;
    }

    public void OnCollision(GameObject obj)
    {
        Debug.Log($"{transform.name} got hit by {obj.name}");
        Debug.Log("Got hit by damage dealer: " + obj.GetComponent<IDamageDealer>() != null);
        if (obj.GetComponent<FinishLine>() != null)
        {
            Debug.Log($"{transform.name} crossed finish line!");
            if (_player)
            {
                GameObject.Find("GameController").GetComponent<GameController>().OnPlayerWin(CandiesEaten);
            }
            else
            {
                TakeDamage(_maxHP);
            }
            return;
        }

        if (obj.GetComponent<IDamageDealer>() != null)
        {
            TakeDamage(obj.GetComponent<IDamageDealer>().CalculateDamage());
        }
        else
        {
            TakeDamage(1);
        }
        _movable.Stop();
    }

    public void TakeDamage(int damage)
    {
        _currentHP -= damage;
        if (_currentHP <= 0)
        {
            TurnManager.Instance.EnqueueDestruction(gameObject);
            gameObject.SetActive(false);
        }
    }

    public void Heal(int amount)
    {
        if (amount < 1)
            return;
        _currentHP = Mathf.Min(_currentHP + amount, _maxHP);
    }

    public void OnRemove()
    {
        GameObject.FindGameObjectWithTag("Map").GetComponent<Map>().RemoveObject(gameObject, _movable._gridPosition);
        Destroy(gameObject);
    }

    public int CalculateDamage()
    {
        return _damageOnCollision;
    }

    public int MaxHP()
    {
        return _maxHP; 
    }

    public int CurrentHP()
    {
        return _currentHP; 
    }

    public void EatCandy()
    {
        CandiesEaten++;
    }
}
