using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

public class Map : MonoBehaviour
{
    private MapObject[,] _map;
    [SerializeField] private Vector2Int _mapSize;
    [SerializeField] private GameObject _blankTile;
    private Vector2 _tileSize;

    public void GenerateMap()
    {
        GenerateBlankMap(_mapSize);
    }

    public void SpawnObject(GameObject obj, Vector2Int gridPosition)
    {
        if(!IsInsideMap(gridPosition))
        {
            Debug.Log($"{obj.transform.name} position({gridPosition}) is outside of map bounds");
            return;
        }
        Vector3 position = _map[gridPosition.x, gridPosition.y].transform.position;
        GameObject newObj = Instantiate(obj, position, Quaternion.identity);
        _map[gridPosition.x,gridPosition.y].AddObject(newObj);

        IInitializable initializable = newObj.GetComponent<IInitializable>();
        initializable?.OnInitialize(gridPosition, 0);
    }

    public bool IsInsideMap(Vector2Int position)
    {
        return position.x < _mapSize.x && position.y < _mapSize.y && position.x >= 0 && position.y >= 0;
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
                newMapObject.OnInitialize(new Vector2Int(x, y), y * mapSize.x + x);

                _map[x, y] = newMapObject;
            }
        }
    }
}
