using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuScript : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject InstructionsMenu;

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
    }

    public void MainMenuButton()
    {
        // Show Main Menu
        MainMenu.SetActive(true);
        InstructionsMenu.SetActive(false);
    }
    
    public void QuitButton()
    {
        // Quit Game
        Application.Quit();
    }
}
