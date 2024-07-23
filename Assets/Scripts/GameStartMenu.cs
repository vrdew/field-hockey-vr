using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameStartMenu : MonoBehaviour
{
    [Header("UI Pages")]
    public GameObject mainMenu;

    [Header("Main Menu Buttons")]
    public Button startButton;

    // Start is called before the first frame update
    void Start()
    {
        EnableMainMenu();

        //Hook events
        startButton.onClick.AddListener(StartGame);
    }


    public void StartGame()
    {
        HideAll();
        SceneManager.LoadScene("Scene2");
    }

    public void HideAll()
    {
        mainMenu.SetActive(false);
    }

    public void EnableMainMenu()
    {
        mainMenu.SetActive(true);
    }
}
