using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUiController : MonoBehaviour
{
    [SerializeField] Button StartGameButton;

    public void StartGameButtonEvent()
    {
        StartGameButton.interactable = false;
        LoadingScreenManager.Instance.LoadScene(Scenes.Game);
    }
}
