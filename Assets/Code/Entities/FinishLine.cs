using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour, ICollidable, IInitializable, IBlockable
{
    public Vector2Int GridPosition { get; private set; }
    public void OnInitialize(InitializationData data)
    {
        GridPosition = data.GridPosition;
        transform.name = data.DefaultName + data.ID;

    }

    public void OnCollision(GameObject obj)
    {
        Debug.Log($"{transform.name} got hit by {obj.name}");
        GameObject.Find("GameController").GetComponent<GameController>().OnCrossLine();
        /*if (obj.GetComponent<Player>() != null)
        {
            GameController gameController = GameObject.Find("GameController").GetComponent<GameController>();
            if (gameController != null)
            {
                gameController.OnPlayerWin();
            }
        }*/
    }
}
