using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class EscapeMenu : MonoBehaviour
{

    public GameObject pauseMenuObject;

    void Update()
    {
        if (Keyboard.current.escapeKey.isPressed)
        {
            Pause();
        }
    }

    public void Pause()
    {
        pauseMenuObject.SetActive(true);
       
    }

    public void Continue()
    {
        pauseMenuObject.SetActive(false);
    }

    public void Menu()
    {
        Resources.UnloadUnusedAssets();
        SceneManager.LoadScene("Scene_Menu");
    }


    public void Quit()
    {
        Application.Quit();
    }

}
