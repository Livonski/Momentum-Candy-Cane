using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject _panelGO;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            _panelGO.SetActive(!_panelGO.activeSelf);
        }
    }
}
