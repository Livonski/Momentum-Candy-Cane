using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour, IInitializable, IBlockable
{
    [SerializeField] private List<GameObject> _props; 
    [SerializeField] private int minProps = 1;        
    [SerializeField] private int maxProps = 5;
    [SerializeField] private int numberOfProps = 10;


    private Vector2Int _gridPosition;
    public void OnInitialize(InitializationData data)
    {
        _gridPosition = data.GridPosition;
        transform.name = data.DefaultName + data.ID;
        PopulateAreaWithProps();
    }

    private void PopulateAreaWithProps()
    {
        if (_props == null || _props.Count == 0)
        {
            Debug.LogWarning("_props list is empty.");
            return;
        }

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("Obstacle: SpriteRenderer does not exist.");
            return;
        }

        Bounds bounds = spriteRenderer.bounds;

        int propsToPlace = Random.Range(minProps, maxProps + 1);
        List<Vector2> positions = GenerateUniformPositions(bounds, propsToPlace);

        foreach (var position in positions)
        {
            GameObject prop = _props[Random.Range(0, _props.Count)];
            Instantiate(prop, position, Quaternion.identity, transform);
        }
    }

    private List<Vector2> GenerateUniformPositions(Bounds bounds, int count)
    {
        List<Vector2> positions = new List<Vector2>();

        int sqrtCount = Mathf.CeilToInt(Mathf.Sqrt(count));
        float cellWidth = bounds.size.x / sqrtCount;
        float cellHeight = bounds.size.y / sqrtCount;

        for (int i = 0; i < sqrtCount; i++)
        {
            for (int j = 0; j < sqrtCount; j++)
            {
                float x = bounds.min.x + (i + 0.5f) * cellWidth;
                float y = bounds.min.y + (j + 0.5f) * cellHeight;

                float offsetX = Random.Range(-cellWidth * 0.4f, cellWidth * 0.4f);
                float offsetY = Random.Range(-cellHeight * 0.4f, cellHeight * 0.4f);

                positions.Add(new Vector2(x + offsetX, y + offsetY));

                if (positions.Count >= count)
                    return positions;
            }
        }

        return positions;
    }
}
