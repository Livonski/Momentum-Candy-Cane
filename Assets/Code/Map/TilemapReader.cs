using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapReader : MonoBehaviour
{
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private string _outputFileName = "tilemap_data.json";

    public void ExportTilemapToJson()
    {
        List<Vector3Int> allTilePositions = new List<Vector3Int>();

        foreach (var position in _tilemap.cellBounds.allPositionsWithin)
        {
            Vector3Int localPosition = new Vector3Int(position.x, position.y, position.z);
            TileBase tile = _tilemap.GetTile(localPosition);

            if (tile != null)
            {
                allTilePositions.Add(localPosition);
            }
        }

        if (allTilePositions.Count == 0)
        {
            Debug.LogWarning("Tilemap пуст, нечего экспортировать.");
            return;
        }

        Vector3Int minPosition = GetMinPosition(allTilePositions);

        List<Vector2Int> correctedTilePositions = new List<Vector2Int>();
        foreach (var position in allTilePositions)
        {
            correctedTilePositions.Add(new Vector2Int(position.x - minPosition.x, position.y - minPosition.y));
        }

        string json = JsonUtility.ToJson(new TilemapDataWrapper { WallPositions = correctedTilePositions }, true);

        string path = Path.Combine(Application.dataPath, _outputFileName);
        File.WriteAllText(path, json);

        Debug.Log($"Tilemap экспортирован в JSON файл: {path}");
    }

    private Vector3Int GetMinPosition(List<Vector3Int> positions)
    {
        int minX = int.MaxValue;
        int minY = int.MaxValue;

        int maxX = int.MinValue;
        int maxY = int.MinValue;

        foreach (var position in positions)
        {
            if (position.x < minX) minX = position.x;
            if (position.y < minY) minY = position.y;

            if (position.x > maxX) maxX = position.x;
            if (position.y > maxY) maxY = position.y;
        }

        Debug.Log($"maxX:{maxX} maxY:{maxY}, minX:{minX} minY:{minY}");
        return new Vector3Int(minX, minY, 0);
    }
}

[System.Serializable]
public class TilemapDataWrapper
{
    public List<Vector2Int> WallPositions;
}