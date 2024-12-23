using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] private Map _map;
    [SerializeField] private Player _player;
    [SerializeField] private int _initialCards;

    [SerializeField] private int _totalVehicles = 0;
    [SerializeField] private int _totalCrossedLine = 0;

    [SerializeField] private EndMenu _endMenu;

    [SerializeField] private TestObjectSpawnData[] _objects;
    [SerializeField] private GameObject _wallGameObject;
    private List<Vector2Int> _wallPositions;

    private void Start()
    {
        if (_map == null)
            _map = GameObject.FindGameObjectWithTag("Map").GetComponent<Map>();
        _map.GenerateMap();

        LoadWallData();
        SpawnWalls();
        SpawnObjects();
        DrawInitialCards();
    }

    public void OnPlayerKilled(int totalCandiesEaten)
    {
        _endMenu.ShowEndScreen("You lost", totalCandiesEaten);
    }

    public void OnPlayerWin(int totalCandiesEaten)
    {
        _totalCrossedLine++;
        _endMenu.ShowEndScreen($"You win, you came {_totalCrossedLine}/{_totalVehicles}", totalCandiesEaten);
    }

    public void OnCrossLine()
    {
        _totalCrossedLine++;
    }

    private void LoadWallData()
    {
        string path = Path.Combine(Application.dataPath, "walls.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            var wallDataWrapper = JsonUtility.FromJson<WallDataWrapper>(json);
            _wallPositions = wallDataWrapper.WallPositions;
        }
        else
        {
            Debug.LogError("Файл walls.json не найден");
            _wallPositions = new List<Vector2Int>();
        }
    }

    private void SpawnWalls()
    {
        int ID = 0;
        foreach (var position in _wallPositions)
        {
            var initializationData = new InitializationData(position,ID,"Wall");
            _map.SpawnObject(_wallGameObject, initializationData);
            ID++;
        }
    }

    private void SpawnObjects()
    {
        foreach (var data in _objects)
        {
            _map.SpawnObject(data.obj, data.initializationData);
        }
    }

    private void DrawInitialCards()
    {
        //TODO: make so not only player drags cards on start
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        _player.DrawCards(_initialCards);
    }

    [System.Serializable]
    private struct TestObjectSpawnData
    {
        public GameObject obj;
        public InitializationData initializationData;
    }
}


[System.Serializable]
public class WallDataWrapper
{
    public List<Vector2Int> WallPositions;
}