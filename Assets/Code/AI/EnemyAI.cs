using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private int _christmasSpirit;
    [SerializeField] private Hand _hand;
    private Movable _playerMovable;

    private void Start()
    {
        TurnManager.Instance.AddAI(this);
        _hand = GetComponent<Hand>();
        _hand.EnableAI();
    }

    public void TakeTurn()
    {
        List<Card> playableCards = _hand.GetCards().FindAll(c => c.GetCardData().SpiritCost <= _christmasSpirit);

        Card bestCard = ChooseBestCard(playableCards);

        _christmasSpirit -= bestCard.GetCardData().SpiritCost;
        Debug.Log($"{transform.name} has this cards:");
        foreach (Card card in playableCards)
        {
            Debug.Log(card.GetCardData().CardName);
        }

        if (bestCard != null)
        {
            Debug.Log($"{transform.name} thinks best card is {bestCard.GetCardData().CardName}, Remainig christmas spirit: {_christmasSpirit}");
            _playerMovable ??= GameObject.FindGameObjectWithTag("Player").GetComponent<Movable>();
            AdditionalContext additionalContext = new AdditionalContext(_playerMovable._gridPosition, TurnDirection.Right);

            _hand.PlayCard(bestCard, additionalContext);
        }
    }

    public void OnNewTurn()
    {
        _hand.DrawCard();
        _hand.IncreaseChristmasSpirit(1);
        _christmasSpirit++;
    }

    private Card ChooseBestCard(List<Card> cards)
    {
        Card bestCard = null;
        float bestValue = 0f;

        foreach (var card in cards)
        {
            float value = EvaluateCard(card);

            if (value > bestValue)
            {
                bestValue = value;
                bestCard = card;
            }
        }
        return bestCard;
    }

    private float EvaluateCard(Card card)
    {
        float moveWeight = 1.0f;
        float attackWeight = 0.5f;

        float result = card.GetCardData().MoveValue * moveWeight +
                       card.GetCardData().AttackValue * attackWeight;

        return result;
    }
}
public struct AdditionalContext
{
    public Vector2Int ShootingTarget;
    public TurnDirection TurnSide;

    public AdditionalContext(Vector2Int target, TurnDirection side)
    {
        ShootingTarget = target;
        TurnSide = side;
    }
}
