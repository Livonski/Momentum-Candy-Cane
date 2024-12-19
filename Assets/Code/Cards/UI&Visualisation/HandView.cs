using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandView : MonoBehaviour
{
    [SerializeField] private Transform _cardParent;
    [SerializeField] private GameObject _cardPrefab;

    [SerializeField] private float _arcAngle;
    [SerializeField] private float _arcRadius;

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

        if (cards.Count == 0) return;

        int count = cards.Count;
        float startAngle = -_arcAngle / 2f;
        float angleStep = count == 1 ? 0 : _arcAngle / (count - 1);

        for (int i = 0; i < count; i++)
        {
            GameObject cardGO = Instantiate(_cardPrefab, _cardParent);
            CardView cardView = cardGO.GetComponent<CardView>();
            if (cardView != null)
            {
                cardView.Setup(cards[i].GetCardData());
            }

            float angle = startAngle + angleStep * i;
            float rad = angle * Mathf.Deg2Rad;
            Vector3 pos = new Vector3(Mathf.Sin(rad) * _arcRadius, Mathf.Cos(rad) * _arcRadius, 0f);

            cardGO.transform.localPosition = pos;
            cardGO.transform.localEulerAngles = new Vector3(0f, 0f, -angle);
        }
    }
}
