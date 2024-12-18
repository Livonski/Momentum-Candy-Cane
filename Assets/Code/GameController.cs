using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private TestObjectSpawnData[] _testObjects;
    [SerializeField] private Map _map;
    private void Start()
    {
        if (_map == null)
            _map = GameObject.FindGameObjectWithTag("Map").GetComponent<Map>();
        _map.GenerateMap();
        SpawnTestObjects();
    }

    private void SpawnTestObjects()
    {
        foreach (var data in _testObjects)
        {
            _map.SpawnObject(data.obj, data.initializationData);
        }
    }

    [System.Serializable]
    private struct TestObjectSpawnData
    {
        public GameObject obj;
        public InitializationData initializationData;
    }
}
