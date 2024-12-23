using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    private MapObject[,] _map;
    [SerializeField] private Vector2Int _mapSize;
    [SerializeField] private GameObject _blankTile;
    [SerializeField] private GameObject _snowGO;
    private Vector2 _tileSize;

    public void GenerateMap()
    {
        GenerateBlankMap(_mapSize);
    }

    public void SpawnObject(GameObject obj, InitializationData data)
    {
        if(!IsInsideMap(data.GridPosition))
        {
            Debug.Log($"{obj.transform.name} position({data.GridPosition}) is outside of map bounds");
            return;
        }
        Vector3 position = _map[data.GridPosition.x, data.GridPosition.y].transform.position;
        GameObject newObj = Instantiate(obj, position, Quaternion.identity);
        _map[data.GridPosition.x, data.GridPosition.y].AddObject(newObj);

        IInitializable initializable = newObj.GetComponent<IInitializable>();
        initializable?.OnInitialize(data);
    }

    public bool IsInsideMap(Vector2Int position)
    {
        return position.x < _mapSize.x && position.y < _mapSize.y && position.x >= 0 && position.y >= 0;
    }

    public bool IsInsideMap(Vector3 worldPosition)
    {
        Vector3 halfMapSize = new Vector3(_tileSize.x * _mapSize.x * 0.5f, _tileSize.y * _mapSize.y * 0.5f);
        Vector3 localPos = worldPosition - transform.position + halfMapSize;

        int x = Mathf.FloorToInt(localPos.x / _tileSize.x);
        int y = Mathf.FloorToInt(localPos.y / _tileSize.y);

        return (x >= 0 && x < _mapSize.x && y >= 0 && y < _mapSize.y);
    }

    public bool IsEmptyTile(Vector2Int position)
    {
        if(!IsInsideMap(position))
            return false;
        return _map[position.x,position.y]._objects.Count == 0;
    }

    public Vector3 MapToWorldPosition(Vector2Int position)
    {
        return _map[position.x, position.y].transform.position;
    }

    public Vector2Int WorldToMapPosition(Vector3 worldPosition)
    {
        Vector3 halfMapSize = new Vector3(_tileSize.x * _mapSize.x * 0.5f, _tileSize.y * _mapSize.y * 0.5f);
        Vector3 localPos = worldPosition - transform.position + halfMapSize;

        int x = Mathf.FloorToInt(localPos.x / _tileSize.x);
        int y = Mathf.FloorToInt(localPos.y / _tileSize.y);

        return new Vector2Int(x, y);
    }

    public void MoveObject(GameObject obj, Vector2Int prevPos, Vector2Int newPos)
    {
        if(!IsInsideMap(prevPos) || !(IsInsideMap(newPos)))
        {
            Debug.LogError($"Moving {obj.transform.name}, from({prevPos}) or to({newPos}) position outside of map");
            return;
        }
        _map[prevPos.x,prevPos.y].RemoveObject(obj);
        _map[newPos.x, newPos.y].AddObject(obj);
    }

    public void RemoveObject(GameObject obj, Vector2Int position)
    {
        if(!IsInsideMap(position))
        {
            Debug.LogError($"Truing to remove {obj.transform.name} from map, but it's position({position}) is outside of map");
        }
        _map[position.x, position.y].RemoveObject(obj);
    }

    public List<GameObject> GetObjectsInTile(Vector2Int position)
    {
        if(!IsInsideMap(position))
        {
            Debug.LogError($"Trying to get objects in tile({position}) that is outside of map bounds");
            return new List<GameObject>();
        }
        List<GameObject> objects = _map[position.x, position.y]._objects;
        if (objects == null)
        {
            Debug.LogError($"Tile({position}) objects list is not set");
            return new List<GameObject>();
        }
        return objects;
    }

    public MapObject GetTile(Vector2Int position)
    {
        if(!IsInsideMap(position))
        {
            Debug.LogError($"Map: Trying to get tile from poition({position}) outside of map bounds");
            return null;
        }
        return _map[position.x, position.y];
    }

    private void GenerateBlankMap(Vector2Int mapSize)
    {
        _map = new MapObject[mapSize.x, mapSize.y];
        _tileSize = _blankTile.GetComponent<SpriteRenderer>().bounds.size;
        Vector3 halfMapSize = new Vector3(_tileSize.x * mapSize.x / 2, _tileSize.y * _mapSize.y / 2);

        for (int y = 0; y < mapSize.y; y++)
        {
            for (int x = 0; x < mapSize.x; x++)
            {
                Vector3 position = transform.position + new Vector3(x * _tileSize.x, y * _tileSize.y) - halfMapSize;
                GameObject newTile = Instantiate(_blankTile, position, Quaternion.identity, transform);
                MapObject newMapObject = newTile.GetComponent<MapObject>();
                InitializationData initializationData = new InitializationData(new Vector2Int(x, y), y * mapSize.x + x);
                newMapObject.OnInitialize(initializationData);

                _map[x, y] = newMapObject;

            }
        }
    }
}
