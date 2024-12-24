using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private int _christmasSpirit;
    [SerializeField] private Hand _hand;
    [SerializeField] private Movable _movable;

    [SerializeField] private float _moveWeight = 1.0f;
    [SerializeField] private float _attackWeight = 0.5f;

    private Movable _playerMovable;
    private FinishLine _finishLine;
    private Map _map;

    private void Start()
    {
        TurnManager.Instance.AddAI(this);
        _hand = GetComponent<Hand>();
        _hand.EnableAI();

        _movable = GetComponent<Movable>();

        _map = GameObject.FindGameObjectWithTag("Map").GetComponent<Map>();

        //This is bad
        _finishLine = GameObject.Find("Finish0").GetComponent<FinishLine>();
    }

    public void TakeTurn()
    {
        List<Card> playableCards = _hand.GetCards().FindAll(c => c.GetCardData().SpiritCost <= _christmasSpirit);

        float turnSide = 0;

        Card bestCard = ChooseBestCard(playableCards, out turnSide);

        if (bestCard != null)
        {
            _christmasSpirit -= bestCard.GetCardData().SpiritCost;
            Debug.Log($"{transform.name} thinks best card is {bestCard.GetCardData().CardName}, Remainig christmas spirit: {_christmasSpirit}");
            _playerMovable ??= GameObject.FindGameObjectWithTag("Player").GetComponent<Movable>();

            TurnDirection direction = turnSide == 0 ? TurnDirection.None : (turnSide == -1 ? TurnDirection.Left : TurnDirection.Right);
            AdditionalContext additionalContext = new AdditionalContext(_playerMovable._gridPosition, direction);

            _hand.PlayCard(bestCard, additionalContext);
        }
    }

    public void OnNewTurn()
    {
        _hand.DrawCard();
        _hand.IncreaseChristmasSpirit(1);
        _christmasSpirit++;
    }

    private Card ChooseBestCard(List<Card> cards, out float turnSide)
    {
        Card bestCard = null;
        float bestValue = 0f;
        turnSide = 0;

        //I'm assuming that finish line is accesible from the right
        int distanceToFinish = _map.GetDistance(_movable._gridPosition + _movable.Forward, _finishLine.GridPosition + new Vector2Int(1,0));
        Debug.Log($"{transform.name} distance to finish: {distanceToFinish}");

        int currentDistance = _map.GetDistance(_movable._gridPosition + _movable.Velocity(), _finishLine.GridPosition + new Vector2Int(1, 0));

        foreach (var card in cards)
        {
            float value = EvaluateCard(card, currentDistance, out turnSide);
            Debug.Log($"{transform.name} thinks that {card.GetCardData().CardName} has {value} score");


            if (value > bestValue)
            {
                bestValue = value;
                bestCard = card;
            }
        }
        return bestCard;
    }

    private float EvaluateCard(Card card, int currentDistance, out float turnSide)
    {
        float positionWeight = 1f;
        turnSide = 0f;

        //Evaluating moving forward/backward
        List<CardEffectData> moveEffects = card.GetCardData().Effects.FindAll(e => e.Type == EffectType.Move || e.Type == EffectType.MoveBackwards);
        if (moveEffects.Count > 0)
        {
            int acceleration = 0;
            foreach(var move in moveEffects)
            {
                acceleration += move.Value;
            }
            Vector2Int positionAfterEffect = _movable.PredictPosition(acceleration, 0, 0);
            int newDistance = _map.GetDistance(positionAfterEffect, _finishLine.GridPosition + new Vector2Int(1, 0));
            Debug.Log($"positionAfterEffect: {positionAfterEffect} newDistance: {newDistance}, oldDistance: {currentDistance}");
            positionWeight = newDistance == -1 ? -10f : currentDistance / newDistance;
        }
        //Evaluating turning
        List<CardEffectData> turnEffects = card.GetCardData().Effects.FindAll(e => e.Type == EffectType.Turn);
        if(turnEffects.Count > 0)
        {
            int turnStrength = 0;
            foreach (var turn in turnEffects)
            {
                turnStrength += turn.Value;
            }
            //left turn
            Vector2Int leftPosition = _movable.PredictPosition(0, -1, turnStrength);
            int distanceLeft = _map.GetDistance(leftPosition, _finishLine.GridPosition + new Vector2Int(1, 0));

            //right turn
            Vector2Int rightPosition = _movable.PredictPosition(0, 1, turnStrength);
            int distanceRight = _map.GetDistance(leftPosition, _finishLine.GridPosition + new Vector2Int(1, 0));

            Debug.Log($"leftPosition: {leftPosition}, rightPosition: {rightPosition}, distanceLeft: {distanceLeft}, distanceRight: {distanceRight}, oldDistance: {currentDistance}");

            if(distanceLeft == -1 && distanceRight == -1)
            {
                positionWeight = -10f; 
            }
            else if (distanceLeft == -1 || distanceRight < distanceLeft)
            {
                positionWeight = currentDistance / distanceRight;
                turnSide = 1;
            }
            else if (distanceRight == -1 || distanceLeft < distanceRight)
            {
                positionWeight = currentDistance / distanceRight;
                turnSide = -1;
            }
            else
            {
                positionWeight = currentDistance / distanceRight;
                turnSide = 1;
                Debug.Log("Distance left == right?");
            }
        }

        float result = card.GetCardData().MoveValue * _moveWeight * positionWeight +
                       card.GetCardData().AttackValue * _attackWeight;

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
