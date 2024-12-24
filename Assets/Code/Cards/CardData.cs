using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Card")]
public class CardData : ScriptableObject
{
    public string CardName;
    public Sprite CardImage;
    public int SpiritCost;
    public List<CardEffectData> Effects;
    public int Rarity;
    public string Description;
    public AudioClip PlayClip;

    public float AttackValue;
    public float MoveValue;
}
