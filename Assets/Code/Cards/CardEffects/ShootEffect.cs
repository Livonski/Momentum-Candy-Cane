using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootEffect : ICardEffect
{
    private GameObject _projectile;
    public ShootEffect(CardEffectData data)
    {
        _projectile = data.SpawnableGO;
    }

    public void ApplyEffect(CardContext context)
    {
        if (context == null)
            return;
        context.AwaitPositionChoice((position) =>
        {
            Vector2Int projectileVelocity = _projectile.GetComponent<Projectile>().CalculateVelocity(context._movable._gridPosition, position, out Vector2Int projectilePosition);
            InitializationData projectileData = new InitializationData(projectilePosition, 0, "Snowball", projectileVelocity.x, projectileVelocity.y);
            Debug.Log($"Projectile shooter position: {context._movable._gridPosition}, projectile velocity: {projectileVelocity}, projectile initial position: {projectilePosition}");
            GameObject.FindGameObjectWithTag("Map").GetComponent<Map>().SpawnObject(_projectile, projectileData);
            TurnManager.Instance.PredictMovements();
        });
    }
}
