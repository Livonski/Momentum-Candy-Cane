using System;
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

    public bool IsPathBlocked(Vector2Int start, Vector2Int end)
    {
        if(start == end)
            return false;
        int x0 = start.x;
        int y0 = start.y;
        int x1 = end.x;
        int y1 = end.y;

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = (x0 < x1) ? 1 : -1;
        int sy = (y0 < y1) ? 1 : -1;

        int err = dx - dy;

        while (true)
        {
            if (!(x0 == start.x && y0 == start.y))
            {
                if (IsInsideMap(new Vector2Int(x0, y0)))
                {
                    if (_map[x0, y0]._objects.Exists(obj => obj.GetComponent<IBlockable>() != null) && !_map[x0, y0]._objects.Exists(obj => obj.GetComponent<FinishLine>() != null))
                        return true;
                }
                else
                {
                    return true;
                }
            }

            if (x0 == x1 && y0 == y1)
                break;

            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }

        return false;
    }

    public int GetDistance(Vector2Int start, Vector2Int end)
    {
        if (!IsInsideMap(start) || !IsInsideMap(start))
        {
            //throw new ArgumentException($"Start({start}) or end({end}) position is out of bounds.");
            Debug.LogError($"Start({start}) or end({end}) position is out of bounds.");
        }

        Vector2Int[] directions =
        {
            new Vector2Int(1, 0),   // Right
            new Vector2Int(-1, 0),  // Left
            new Vector2Int(0, 1),   // Up
            new Vector2Int(0, -1),  // Down
            new Vector2Int(1, 1),   // Up-right
            new Vector2Int(-1, 1),  // Up-left
            new Vector2Int(1, -1),  // Down-right
            new Vector2Int(-1, -1)  // Down-left
        };

        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        PriorityQueue<Vector2Int> priorityQueue = new PriorityQueue<Vector2Int>();
        Dictionary<Vector2Int, int> costSoFar = new Dictionary<Vector2Int, int>();

        priorityQueue.Enqueue(start, 0);
        costSoFar[start] = 0;

        while (priorityQueue.Count > 0)
        {
            Vector2Int current = priorityQueue.Dequeue();

            if (current == end)
            {
                return costSoFar[current];
            }

            foreach (var direction in directions)
            {
                Vector2Int next = current + direction;

                if (!IsPositionValid(next) || visited.Contains(next))
                {
                    continue;
                }

                int newCost = costSoFar[current] + 1;
                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;
                    int priority = newCost + Heuristic(next, end);
                    priorityQueue.Enqueue(next, priority);
                }
            }

            visited.Add(current);
        }

        //No path found
        return -1;
    }

    private bool IsPositionValid(Vector2Int position)
    {
        if (position.x < 0 || position.x >= _mapSize.x ||
            position.y < 0 || position.y >= _mapSize.y)
        {
            return false;
        }

        MapObject tile = _map[position.x, position.y];
        if (tile == null || tile._objects.Exists(obj => obj.GetComponent<IBlockable>() != null))
        {
            return false;
        }

        return true;
    }

    private int Heuristic(Vector2Int a, Vector2Int b)
    {
        // Use Chebyshev distance for 8-directional movement
        return Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y));
    }

    private class PriorityQueue<T>
    {
        private List<(T Item, int Priority)> elements = new List<(T Item, int Priority)>();

        public int Count => elements.Count;

        public void Enqueue(T item, int priority)
        {
            elements.Add((item, priority));
            elements.Sort((x, y) => x.Priority.CompareTo(y.Priority));
        }

        public T Dequeue()
        {
            var bestItem = elements[0].Item;
            elements.RemoveAt(0);
            return bestItem;
        }
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
