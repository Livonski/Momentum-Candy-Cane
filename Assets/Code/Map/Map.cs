using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    private MapObject[,] _map;
    [SerializeField] private Vector2Int _mapSize;
    [SerializeField] private GameObject _blankTile;

    private void Start()
    {
        GenerateBlankMap(_mapSize);
    }

    private void GenerateBlankMap(Vector2Int mapSize)
    {
        _map = new MapObject[mapSize.x, mapSize.y];
        Vector2 tileSize = _blankTile.GetComponent<SpriteRenderer>().bounds.size;
        Vector3 halfMapSize = new Vector3(tileSize.x * mapSize.x / 2, tileSize.y * _mapSize.y / 2);

        for (int y = 0; y < mapSize.x; y++)
        {
            for (int x = 0; x < mapSize.x; x++)
            {
                Vector3 position = transform.position + new Vector3(x * tileSize.x, y * tileSize.y) - halfMapSize;
                GameObject newTile = Instantiate(_blankTile, position, Quaternion.identity, transform);
                MapObject newMapObject = newTile.GetComponent<MapObject>();
                newMapObject.OnIntitalize(new Vector2Int(x,y), y * mapSize.x + x);

                _map[x, y] = newMapObject;
            }
        }
    }
}
