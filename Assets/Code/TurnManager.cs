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
            if(_instance == null)
            {
                _instance = GameObject.FindObjectOfType<TurnManager>();
            }
            return _instance;
        }
    }

    [SerializeField] bool _manualSubTurns;

    private List<MovableData> _movables;
    private int _minMovePoints;
    private int _currentSubturn;

    public void AddMovable(Movable movable)
    {
        _movables ??= new List<MovableData>();
        MovableData newMovable = new MovableData(movable);
        _movables.Add(newMovable);
    }

    public void ExecuteTurn()
    {
        _minMovePoints = int.MaxValue;
        _currentSubturn = 0;

        for(int i = 0; i < _movables.Count; i++)
        {
            int movePoints = _movables[i].Movable.CalculateMovePoints();
            //I hate lists +this is ugly
            MovableData newMovable = new MovableData(_movables[i].Movable, movePoints);
            _minMovePoints = Mathf.Min(_minMovePoints, movePoints);
            _movables[i] = newMovable;
            Debug.Log($"Movable {i}, move points: {movePoints}");
        }

        Debug.Log($"Min move points: {_minMovePoints}");
        _minMovePoints = Mathf.Max(_minMovePoints, 1);
        for(int i = 0; i < _movables.Count; i++)
        {
            int numberOfMoves = (int)Mathf.Round((float)_movables[i].MovePoints / (float)_minMovePoints);
            MovableData newMovable = _movables[i];
            newMovable.NumberOfMoves = numberOfMoves;
            _movables[i] = newMovable;
            Debug.Log($"Movable {i}, number of moves: {numberOfMoves}");
        }

        if(_manualSubTurns)
            return;

        for (int i = 0; i < _minMovePoints; i++)
        {
            Debug.Log($"Sub-turn {i}");
            foreach(var moveData in _movables)
            {
                moveData.Movable.MoveBy(moveData.NumberOfMoves);
            }
        }
    }

    public void NextSubturn()
    {
        _currentSubturn++;
        if(_currentSubturn > _minMovePoints)
        {
            Debug.Log("All sub-turns processed");
            return;
        }
        foreach (var moveData in _movables)
        {
            Debug.Log($"Executing movable, number of moves: {moveData.NumberOfMoves}");
            moveData.Movable.MoveBy(moveData.NumberOfMoves);
        }
    }


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private struct MovableData
    {
        public Movable Movable;
        public int MovePoints;
        public int RemainigMovePoints;
        public int NumberOfMoves;

        public MovableData(Movable movable, int movePoints = 0, int numberOfMoves = 0)
        {
            Movable = movable;
            MovePoints = movePoints;
            RemainigMovePoints = 0;
            NumberOfMoves = numberOfMoves;
        }

        public void Reset()
        {
            MovePoints = 0;
            RemainigMovePoints = 0;
            NumberOfMoves = 0;
        }

        public void SetMovePoints(int movePoints)
        {
            MovePoints = movePoints;
        }

        public void SetNumberOfMoves(int numberOfMoves)
        {
            NumberOfMoves = numberOfMoves;
        }

        public void DecreaseRemainingMovePoints(int usedPoints)
        {
            RemainigMovePoints -= usedPoints;
        }
    }
}
