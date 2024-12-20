using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TileHighlighter : MonoBehaviour
{
    public static TileHighlighter Instance;

    [SerializeField] private Map _map;
    private List<MapObject> _highlightedTiles = new List<MapObject>();

    private void Awake()
    {
        Instance = this;
    }

    public void HighlightTiles(List<Vector2Int> positions, Color highlightColor)
    {
        foreach (var tile in positions)
        {
            HighlightTile(tile, highlightColor);
        }
    }

    public void HighlightTile(Vector2Int position, Color highlightColor)
    {
        MapObject tile = _map.GetTile(position);
        if (tile != null)
        {
            tile.Highlight(highlightColor);
            _highlightedTiles.Add(tile);
        }
    }

    public void ClearHighlights()
    {
        foreach (MapObject obj in _highlightedTiles)
        {
            obj.Highlight(Color.white);
        }
        _highlightedTiles.Clear();
    }
}
