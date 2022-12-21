using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuScript : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject InstructionsMenu;
    public GameObject ControlsMenu;

    // Start is called before the first frame update
    void Start()
    {
        MainMenuButton();
    }

    public void PlayNowButton()
    {
        // Start playing game
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }

    public void InstructionsButton()
    {
        // Show Instructions
        MainMenu.SetActive(false);
        InstructionsMenu.SetActive(true);
        ControlsMenu.SetActive(false);
    }

    public void MainMenuButton()
    {
        // Show Main Menu
        MainMenu.SetActive(true);
        InstructionsMenu.SetActive(false);
        ControlsMenu.SetActive(false);
    }

    public void ControlsMenuButtom()
    {
        // Show Controls
        MainMenu.SetActive(false);
        InstructionsMenu.SetActive(false);
        ControlsMenu.SetActive(true);
    }

    public void QuitButton()
    {
        // Quit Game
        Application.Quit();
    }
}
