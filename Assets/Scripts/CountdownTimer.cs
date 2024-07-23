using UnityEngine;
using System.Collections;
using UnityEngine.UI; // Include for using Unity's standard UI components

public class CountdownTimer : MonoBehaviour
{
    public Text countdownText; // Reference to the standard UI Text component
    private float currentTime = 0f;
    private float startingTime = 3f; // Start at 3 seconds

    void Start()
    {
        currentTime = startingTime;
        StartCoroutine(CountdownRoutine());
    }

    IEnumerator CountdownRoutine()
    {
        while (currentTime > 0)
        {
            countdownText.text = Mathf.Ceil(currentTime).ToString(); // Update the text display
            yield return new WaitForSeconds(1f); // Wait for a second
            currentTime--; // Decrease current time
        }

        countdownText.text = "GO!"; // Change text at the end of the countdown
        yield return new WaitForSeconds(1f); // Wait for a second to show "GO!"
        Destroy(countdownText.gameObject); // Destroy the text GameObject
    }
}
