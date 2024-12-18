[System.Serializable]
public class CardEffectData
{
    public EffectType Type;
    public int Value;
}

public enum EffectType
{
    Move,
    Turn
}

