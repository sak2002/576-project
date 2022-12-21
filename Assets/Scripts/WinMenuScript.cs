using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WinMenuScript : MonoBehaviour
{
    public GameObject InGameMenu;
    public GameObject OutGameMenu;

    // Start is called before the first frame update
    void Start()
    {
        // InGameMenuOn();
    }
    
    public void InGameMenuOn() {
        InGameMenu.SetActive(true);
        OutGameMenu.SetActive(false);
    }

    public void OutGameMenuWin(string word, string meaning)
    {
        InGameMenu.SetActive(false);
        OutGameMenu.SetActive(true);
        GameObject winMenu = OutGameMenu.transform.Find("WinMenu").gameObject;
        TMP_Text winTitle = winMenu.transform.Find("WinTitle").gameObject.GetComponent<TMP_Text>();
        TMP_Text winWord = winMenu.transform.Find("WinWord").gameObject.GetComponent<TMP_Text>();
        TMP_Text winWordMeaning = winMenu.transform.Find("WinWordMeaning").gameObject.GetComponent<TMP_Text>();
        winTitle.text = "Congratulations!\nYOU WON!!!!";
        winWord.text = "Congratulations, you successfully completed the word: " + word;
        winWordMeaning.text = "The meaning of this word is: " + meaning;
    }

    public void OutGameMenuLose()
    {
        InGameMenu.SetActive(false);
        OutGameMenu.SetActive(true);
        GameObject winMenu = OutGameMenu.transform.Find("WinMenu").gameObject;
        TMP_Text winTitle = winMenu.transform.Find("WinTitle").gameObject.GetComponent<TMP_Text>();
        TMP_Text winWord = winMenu.transform.Find("WinWord").gameObject.GetComponent<TMP_Text>();
        TMP_Text winWordMeaning = winMenu.transform.Find("WinWordMeaning").gameObject.GetComponent<TMP_Text>();
        winTitle.text = "You Lost :(";
        winWord.text = "Click restart to play again!";
        winWordMeaning.text = "";
    }

    public void RestartButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }

    public void MainMenuButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("EntryScene");
    }
}
