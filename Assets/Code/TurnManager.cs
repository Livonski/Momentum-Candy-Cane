using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private static TurnManager _instance;
    public static TurnManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<TurnManager>();
            }
            return _instance;
        }
    }
    private List<Movable> _movables;

    public void AddMovable(Movable movable)
    {
        _movables ??= new List<Movable>();
        _movables.Add(movable);
    }

    public void ExecuteTurn()
    {
        foreach (var movable in _movables)
        {
            movable.Move();
        }
    }


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
