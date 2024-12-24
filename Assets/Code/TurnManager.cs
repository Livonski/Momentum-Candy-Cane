using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    [SerializeField] private bool _manualSubTurns;
    [SerializeField] public float _moveDelay;
    [SerializeField] private Color _movementhighlightColor;

    //private Player _player;

    private List<MovableData> _movables;
    private int _minMovePoints;
    private int _currentSubturn;
    private bool _processingTurn;

    private Queue<GameObject> _destructionQueue;

    private void OnLevelWasLoaded(int level)
    {
        _movables = new List<MovableData>();
        _minMovePoints = 0;
        _currentSubturn = 0;
        _processingTurn = false;
    }

    public void AddMovable(Movable movable, int priority)
    {
        _movables ??= new List<MovableData>();
        MovableData newMovable = new MovableData(movable, priority);
        _movables.Add(newMovable);
        Debug.Log("Adding new movable");
    }

    public float CurrentMoveDelay()
    {
        return _moveDelay; 
    }

    public void ExecuteTurnEditor()
    {
        if(!_processingTurn)
        {
            _processingTurn = true;
            StartCoroutine(ExecuteTurn());
        }
    }

    public IEnumerator ExecuteTurn()
    {
        //TODO move some things into their own methods

        _minMovePoints = int.MaxValue;
        _currentSubturn = 0;

        for(int i = 0; i < _movables.Count; i++)
        {
            int movePoints = _movables[i].Movable.CalculateMovePoints();

            //I hate lists +this is ugly
            MovableData newMovable = new MovableData(_movables[i].Movable, _movables[i].Priority, movePoints);
            _minMovePoints = Mathf.Min(_minMovePoints, movePoints);
            _movables[i] = newMovable;
            Debug.Log($"Movable {i}, move points: {movePoints}");
        }

        Debug.Log($"Min move points: {_minMovePoints}");
        _minMovePoints = Mathf.Max(_minMovePoints, 1);
        for(int i = 0; i < _movables.Count; i++)
        {
            int numberOfMoves = (int)Mathf.Round((float)_movables[i].MovePoints / (float)_minMovePoints);
            if(_movables[i].MovePoints == _minMovePoints)
                numberOfMoves = _minMovePoints;
            MovableData newMovable = _movables[i];
            newMovable.NumberOfMoves = numberOfMoves;
            _movables[i] = newMovable;
            Debug.Log($"Movable {i}, number of moves: {numberOfMoves}");
        }

        _movables.Sort((a,b) => b.Priority.CompareTo(a.Priority));

        if(_manualSubTurns)
            yield return null;

        for (int i = 0; i < _minMovePoints; i++)
        {
            Debug.Log($"Sub-turn {i}");
            foreach(var moveData in _movables)
            {
                yield return StartCoroutine(moveData.Movable.MoveBy(moveData.NumberOfMoves));
            }
            DestroyObjects();
        }
        PredictMovements();

        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().OnNewTurn();
        _processingTurn = false;
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
        DestroyObjects();
    }

    public void EnqueueDestruction(GameObject obj)
    {
        _destructionQueue ??= new Queue<GameObject>();
        _destructionQueue.Enqueue(obj);
    }

    private void DestroyObjects()
    {
        if (_destructionQueue == null)
            return;
        while (_destructionQueue.Count > 0)
            _destructionQueue.Dequeue().GetComponent<IDestroyable>().OnRemove();
    }

    public void PredictMovements()
    {
        TileHighlighter.Instance.ClearHighlights();
        for (int i = 0; i < _movables.Count; i++)
        {
            List<Vector2Int> trajectory = _movables[i].Movable.CalculateMovement();
            if(trajectory.Count <= 1)
                continue;
            TileHighlighter.Instance.HighlightTiles(trajectory, _movementhighlightColor);
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private struct MovableData
    {
        public Movable Movable;
        public int Priority;
        public int MovePoints;
        public int RemainigMovePoints;
        public int NumberOfMoves;

        public MovableData(Movable movable, int priority = 0, int movePoints = 0, int numberOfMoves = 0)
        {
            Movable = movable;
            Priority = priority;
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
