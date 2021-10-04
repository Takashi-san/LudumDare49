using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    public void RestartGame()
    {
        GameManager.Instance.ReloadScene();
    }

    public void BackToMainMenu()
    {
        GameManager.Instance.LoadScene("MainMenu");
    }
}
