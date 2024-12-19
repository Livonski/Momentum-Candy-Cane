[System.Serializable]
public class CardEffectData
{
    public EffectType Type;
    public int Value;

    //Stuff for turn effect
    public float Side;
}

public enum EffectType
{
    Move,
    Turn
}

