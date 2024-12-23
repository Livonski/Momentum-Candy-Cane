using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CandiesDisplay : MonoBehaviour
{
    private TextMeshProUGUI _displayText;
    [SerializeField] private string _text;
    private Vehicle _playerVehicle;

    public void SetValue(int value)
    {
        _displayText = GetComponent<TextMeshProUGUI>();
        _displayText.text = _text + value;
    }

    private void Initialize()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            return;
        _playerVehicle = player.GetComponent<Vehicle>();
        _displayText = GetComponent<TextMeshProUGUI>();
    }

    //I'll probably need to make some event on points change to remove this horrible update
    private void Update()
    {
        if (_playerVehicle == null)
        {
            Initialize();
        }
        else
        {
            _displayText.text = _text + _playerVehicle.CandiesEaten;
        }
    }
}
