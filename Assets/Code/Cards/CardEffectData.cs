using UnityEngine;

[System.Serializable]
public class CardEffectData
{
    public EffectType Type;
    public int Value;
    public GameObject SpawnableGO;
}

public enum EffectType
{
    Move,
    Turn,
    Shoot
}

