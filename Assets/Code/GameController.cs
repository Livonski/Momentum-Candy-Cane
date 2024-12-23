using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] private TestObjectSpawnData[] _testObjects;
    [SerializeField] private Map _map;
    [SerializeField] private Player _player;
    [SerializeField] private int _initialCards;

    [SerializeField] private EndMenu _endMenu;
    private void Start()
    {
        if (_map == null)
            _map = GameObject.FindGameObjectWithTag("Map").GetComponent<Map>();
        _map.GenerateMap();
        SpawnTestObjects();
        DrawInitialCards();
    }

    public void OnPlayerKilled(int totalCandiesEaten)
    {
        _endMenu.ShowEndScreen("You lost", totalCandiesEaten);
    }

    public void OnPlayerWin(int totalCandiesEaten)
    {
        _endMenu.ShowEndScreen("You win", totalCandiesEaten);
    }

    private void SpawnTestObjects()
    {
        foreach (var data in _testObjects)
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
