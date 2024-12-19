using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private Hand _hand;
    private HandView _handView;
    private Button _drawCardButton;
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
    }
}
