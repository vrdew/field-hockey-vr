using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneTransitionmanager : MonoBehaviour
{
    public FadeScreen1 fadeScreen;

    public void GoToScene(string sceneName)
    {
        StartCoroutine(GoToSceneRoutine(sceneName));
    }

    IEnumerator GoToSceneRoutine(string sceneName)
    {
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(fadeScreen.fadeDuration);
        SceneManager.LoadScene(sceneName);
    }

    public void EndGameandNo()
    {
        // End the game, save data, and increment round
        if (DataManager.Instance != null)
        {
            DataManager.Instance.EndGame();
            DataManager.Instance.StartNewRound(); // Increment the round
        }
        else
        {
            Debug.LogWarning("DataManager instance not found. Unable to end game and save data.");
        }
        SceneManager.LoadScene("Scene1");
    }

    public void EndGameAndYes()
    {
        // End the game, save data, and increment round
        if (DataManager.Instance != null)
        {
            DataManager.Instance.EndGame();
            DataManager.Instance.StartNewRound(); // Increment the round
        }
        else
        {
            Debug.LogWarning("DataManager instance not found. Unable to end game and save data.");
        }
        SceneManager.LoadScene("Scene6");
    }

    public void QuitApps()
    {
        if (DataManager.Instance != null)
        {
            DataManager.Instance.QuitGame();
        }
        else
        {
            Debug.LogWarning("DataManager instance not found. Unable to quit game and save data.");
        }
        StartCoroutine(QuitAppRoutine());
    }

    private IEnumerator QuitAppRoutine()
    {
        yield return new WaitForSeconds(0.5f); // Give some time for the data to be saved and cleared
        DataManager.Instance.ResetRound(); // Reset the round to 1
        Application.Quit();
    }
}