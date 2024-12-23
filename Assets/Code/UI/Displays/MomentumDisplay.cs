using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MomentumDisplay : MonoBehaviour
{
    private TextMeshProUGUI _displayText;
    [SerializeField] private string _text;
    private Movable _playerMovable;

    private void Initialize()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            return;

        _playerMovable = player.GetComponent<Movable>();
        _displayText = GetComponent<TextMeshProUGUI>();
    }

    //I'll probably need to make some event on points change to remove this horrible update
    private void Update()
    {
        if (_playerMovable == null)
        {
            Initialize();
        }
        else
        {
            _displayText.text = _text + _playerMovable.Momentum;
        }
    }
}
