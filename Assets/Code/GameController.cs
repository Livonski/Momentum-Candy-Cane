using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private TestObjectSpawnData[] _testObjects;
    [SerializeField] private Map _map;
    [SerializeField] private Player _player;
    [SerializeField] private int _initialCards;
    private void Start()
    {
        if (_map == null)
            _map = GameObject.FindGameObjectWithTag("Map").GetComponent<Map>();
        _map.GenerateMap();
        SpawnTestObjects();
        DrawInitialCards();
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
