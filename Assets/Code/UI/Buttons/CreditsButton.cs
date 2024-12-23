using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsButton : MonoBehaviour
{
    [SerializeField] private GameObject _creditsPanel;
    public void OnClick()
    {
        _creditsPanel.SetActive(!_creditsPanel.activeSelf);
    }
}
