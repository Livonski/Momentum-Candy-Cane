using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChristmasSpiritDisplay : MonoBehaviour
{
    private TextMeshProUGUI _displayText;
    [SerializeField] private string _text;
    private Hand _playerHand;
    private Player _player;

    private void Initialize()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        _player = player.GetComponent<Player>();
        _playerHand = player.GetComponent<Hand>();
        _displayText = GetComponent<TextMeshProUGUI>();
    }

    //I'll probably need to make some event on points change to remove this horrible update
    private void Update()
    {
        if (_player == null)
        {
            Initialize();
        }
        else
        {
            string sign = _player.GetChristmasSpiritRegen() > 0 ? "+" : "";
            _displayText.text = _text + _playerHand._christmasSpirit + "(" + sign + _player.GetChristmasSpiritRegen() + ")";
        }
    }
}
