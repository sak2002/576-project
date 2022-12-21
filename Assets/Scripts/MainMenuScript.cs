using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuScript : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject InstructionsMenu;
    public GameObject WinMenu;

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
        WinMenu.SetActive(false);
        InstructionsMenu.SetActive(true);
    }

    public void MainMenuButton()
    {
        // Show Main Menu
        MainMenu.SetActive(true);
        WinMenu.SetActive(false);
        InstructionsMenu.SetActive(false);
    }

    public void WinMenuButton() {
        // Show Win Screen
        MainMenu.SetActive(false);
        InstructionsMenu.SetActive(false);
        WinMenu.SetActive(true);
    }

    public void QuitButton()
    {
        // Quit Game
        Application.Quit();
    }
}
