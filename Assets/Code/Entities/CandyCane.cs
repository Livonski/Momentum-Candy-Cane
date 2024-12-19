using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandyCane : MonoBehaviour, ICollidable, IInitializable, IDestroyable
{
    Vector2Int _gridPosition;
    public void OnCollision(GameObject obj)
    {
        if (obj.GetComponent<Vehicle>() == null)
            return;
        Debug.Log("Candy eaten");
        TurnManager.Instance.EnqueueDestruction(gameObject);
        gameObject.SetActive(false);
    }

    public void OnRemove()
    {
        GameObject.FindGameObjectWithTag("Map").GetComponent<Map>().RemoveObject(gameObject, _gridPosition);
        Destroy(gameObject);
    }

    public void OnInitialize(InitializationData data)
    {
        _gridPosition = data.GridPosition;
        transform.name = data.DefaultName + data.ID;
    }

    public void TakeDamage(int damage)
    {

    }
}
