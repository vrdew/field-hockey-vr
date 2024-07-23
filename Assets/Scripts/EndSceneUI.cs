using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class EndSceneUI : MonoBehaviour
{
    [Header("UI Pages")]
    public GameObject mainMenu;

    [Header("Main Menu Buttons")]
    public Button startButton;
    public Button backButton;

    public List<Button> returnButtons;

    // Start is called before the first frame update
    void Start()
    {
        EnableMainMenu();

        //Hook events
        startButton.onClick.AddListener(StartGame);
        backButton.onClick.AddListener(GoBack);

        foreach (var item in returnButtons)
        {
            item.onClick.AddListener(EnableMainMenu);
        }
    }

    public void StartGame()
    {
        HideAll();
        SceneManager.LoadScene(2);
    }

    public void HideAll()
    {
        mainMenu.SetActive(false);
    }

    public void EnableMainMenu()
    {
        mainMenu.SetActive(true);
    }
    public void GoBack()
    {
        SceneManager.LoadScene(0);
    }
}
