using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private TextMeshProUGUI _spiritCost;

    public void Setup(CardData cardData)
    {
        if (cardData == null) return;

        if(_image != null && cardData.CardImage != null)
            _image.sprite = cardData.CardImage;

        if(_name != null)
            _name.text = cardData.CardName;

        if(_description != null)
            _description.text = cardData.Description;

        if(_spiritCost != null)
            _spiritCost.text = cardData.SpiritCost.ToString();
    }

}
