using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChangeButton : MonoBehaviour
{
    private enum SceneType
    {
        Game,
        MainMenu
    }
    [SerializeField] private SceneType type;

    public void ChangeScene()
    {
        if (type == SceneType.Game)
        {
            SceneLoader.Instance.LoadGame();
        }
        else
        {
            SceneLoader.Instance.LoadMainMenu();
        }
    }
}
