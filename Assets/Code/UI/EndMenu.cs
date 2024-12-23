using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _endText;
    [SerializeField] private GameObject _menuGO;
    [SerializeField] private CandiesDisplay _candiesDisplay;
    public void ShowEndScreen(string endText, int totalCandiesEaten)
    {
        _menuGO.SetActive(true);
        _endText.text = endText;
        _candiesDisplay.SetValue(totalCandiesEaten);
    }
}
