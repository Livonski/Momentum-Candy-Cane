using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private int _christmasSpirit;
    [SerializeField] private Hand _hand;
    [SerializeField] private Movable _movable;
    [SerializeField] private Vehicle _vehicle;

    [SerializeField] private float _moveWeight = 1.0f;
    [SerializeField] private float _attackWeight = 0.5f;
    [SerializeField] private int _idealCardsAmount = 5;

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
        _playerMovable ??= GameObject.FindGameObjectWithTag("Player").GetComponent<Movable>();

        float turnSide = 0;

        Card bestCard = ChooseBestCard(playableCards, out turnSide);

        if (bestCard != null)
        {
            _christmasSpirit -= bestCard.GetCardData().SpiritCost;
            Debug.Log($"{transform.name} thinks best card is {bestCard.GetCardData().CardName}, Remainig christmas spirit: {_christmasSpirit}");

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
        int currentDistance = _map.GetDistance(_movable._gridPosition + _movable.Velocity(), _finishLine.GridPosition + new Vector2Int(1, 0));
        bool isPathBlocked = _map.IsPathBlocked(_movable._gridPosition, _movable._gridPosition + _movable.Velocity());
        currentDistance = isPathBlocked ? -1 : currentDistance;
        Debug.Log($"{transform.name} distance to finish: {currentDistance}, position: {_movable._gridPosition}, velocity: {_movable.Velocity()}, path blocked: {isPathBlocked}");

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
        float reachableTargerWeight = 1f;
        float drawValue = 0f;
        float healValue = 0f;

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
            bool isPathBlocked = _map.IsPathBlocked(_movable._gridPosition, positionAfterEffect);
            Debug.Log($"positionAfterEffect: {positionAfterEffect} newDistance: {newDistance}, oldDistance: {currentDistance}, isPathBlocked: {isPathBlocked}");
            positionWeight = newDistance == -1 ? -10f : currentDistance / newDistance;
            positionWeight = isPathBlocked ? -1 : positionWeight;
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
            bool isLeftPathBlocked = _map.IsPathBlocked(_movable._gridPosition, leftPosition);

            //right turn
            Vector2Int rightPosition = _movable.PredictPosition(0, 1, turnStrength);
            int distanceRight = _map.GetDistance(rightPosition, _finishLine.GridPosition + new Vector2Int(1, 0));
            bool isRightPathBlocked = _map.IsPathBlocked(_movable._gridPosition, rightPosition);

            Debug.Log($"leftPosition: {leftPosition}, rightPosition: {rightPosition}, distanceLeft: {distanceLeft}, distanceRight: {distanceRight}, oldDistance: {currentDistance}, isLeftPathBlocked: {isLeftPathBlocked}, isRightPathBlocked: {isRightPathBlocked}");

            if(distanceLeft == -1 && distanceRight == -1)
            {
                positionWeight = -10f; 
            }
            else if (distanceLeft == -1 || (distanceRight < distanceLeft && distanceRight != -1))
            {
                //positionWeight = 1 - (float)currentDistance / (float)distanceRight;
                positionWeight = 1 - (float)distanceRight / (float)currentDistance;
                turnSide = 1;
            }
            else if (distanceRight == -1 || (distanceLeft < distanceRight && distanceLeft != -1))
            {
                //positionWeight = 1 - (float)currentDistance / (float)distanceRight;
                positionWeight = 1 - (float)distanceLeft / (float)currentDistance;
                turnSide = -1;
            }
            else
            {
                positionWeight = 1 - (float)distanceRight / (float)currentDistance;
                turnSide = 1;
                Debug.Log("Distance left == right?");
            }
        }
        //Evaluating Healing
        List<CardEffectData> healEffects = card.GetCardData().Effects.FindAll(e => e.Type == EffectType.Heal);
        if (healEffects.Count > 0)
        {
            int totalHealAmount = 0;
            foreach (var heal in healEffects)
            {
                totalHealAmount += heal.Value;
            }
            drawValue = totalHealAmount * (1 - _vehicle.CurrentHP() / _vehicle.MaxHP());
        }
        //Evaluating Shooting
        List<CardEffectData> shootEffects = card.GetCardData().Effects.FindAll(e => e.Type == EffectType.Shoot);
        if(shootEffects.Count > 0)
        {
            bool targetReachable = true;
            foreach (var shoot in shootEffects)
            {
                Vector2Int direction = _movable._gridPosition - _movable.CalculateMovement()[1];
                Debug.Log($"Direction: {direction}, ShooterPos: {_movable._gridPosition}, TargetPos: {_playerMovable._gridPosition}");
                if(_map.IsPathBlocked(_movable._gridPosition + direction, _playerMovable._gridPosition - direction))
                    targetReachable = false;
            }
            reachableTargerWeight = targetReachable ? 1 : 0;
        }

        //Evaluating Drawing new card
        List<CardEffectData> drawEffects = card.GetCardData().Effects.FindAll(e => e.Type == EffectType.DrawCards);
        if(drawEffects.Count > 0)
        {
            int totalDrawAmount = 0;
            foreach (var draw in drawEffects)
            {
                totalDrawAmount += draw.Value;
            }
            drawValue = totalDrawAmount * (1 - _hand.GetCards().Count / _idealCardsAmount);
        }


        float result = card.GetCardData().MoveValue * _moveWeight * positionWeight +
                       card.GetCardData().AttackValue * _attackWeight * reachableTargerWeight + drawValue + healValue;

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
