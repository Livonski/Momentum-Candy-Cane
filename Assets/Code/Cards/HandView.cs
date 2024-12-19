using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandView : MonoBehaviour
{
    [SerializeField] private Transform _cardParent;
    [SerializeField] private GameObject _cardPrefab;

    private Hand _playerHand;

    public void Initialize(Hand hand)
    {
        _playerHand = hand;
        _playerHand.OnHandChanged += UpdateHandView;

        UpdateHandView(_playerHand.GetCards());            
    }

    private void UpdateHandView(List<Card> cards)
    {
        foreach(Transform child in _cardParent)
        {
            Destroy(child.gameObject);
        }

        foreach(var card in cards)
        {
            GameObject cardGO = Instantiate(_cardPrefab, _cardParent);
            CardView cardView = cardGO.GetComponent<CardView>();

            if (cardView != null)
                cardView.Setup(card.GetCardData());
        }
    }
}
