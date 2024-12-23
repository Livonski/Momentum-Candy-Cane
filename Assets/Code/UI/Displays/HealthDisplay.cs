using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthDisplay : MonoBehaviour
{
    private TextMeshProUGUI _displayText;
    [SerializeField] private string _text;
    private Vehicle _playerVehicle;

    private void Initialize()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            return;

        _playerVehicle = player.GetComponent<Vehicle>();
        _displayText = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (_playerVehicle == null)
        {
            Initialize();
        }
        else
        {
            _displayText.text = _text + _playerVehicle.CurrentHP() + "/" + _playerVehicle.MaxHP();
        }
    }
}
