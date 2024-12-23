using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private Hand _hand;
    [SerializeField] private int _christmasSpiritRegen;

    private HandView _handView;
    private Button _drawCardButton;
    Vehicle _playerVehicle;
    void Start()
    {
        _hand = GetComponent<Hand>();
        //I'm making an assuption that only one HandView exists on scene, let's hope it's true
        _handView = FindAnyObjectByType<HandView>();
        //This is bad, but I can live with that
        _drawCardButton = GameObject.Find("DrawCardButton").GetComponent<Button>();

        if (_hand != null && _handView != null)
        {
            _handView.Initialize(_hand);
        }
        else
        {
            Debug.LogError("Player: hand or handView references are not set");
        }

        if(_drawCardButton != null)
        {
            _drawCardButton.onClick.AddListener(_hand.DrawCard);
        }
        else
        {
            Debug.LogWarning("Player: _drawCardButton is not set");
        }

        _playerVehicle = GetComponent<Vehicle>();
        GameObject.Find("CameraTarget").GetComponent<CameraTarget>().JumpToPlayer(transform.position);
    }

    public int GetChristmasSpiritRegen()
    {
        return _christmasSpiritRegen;
    }

    public void OnNewTurn()
    {
        DrawCards(1);
        _hand.IncreaseChristmasSpirit(_christmasSpiritRegen);
    }

    public void DrawCards(int amount)
    {
        _hand ??= gameObject.GetComponent<Hand>();
        for (int i = 0; i < amount; i++) 
        {
            _hand.DrawCard();
        }
    }

    private void OnDestroy()
    {
        GameController gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameController.OnPlayerKilled(_playerVehicle.CandiesEaten);
    }
}
