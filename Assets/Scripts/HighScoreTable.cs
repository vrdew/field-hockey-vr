using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighscoreTable : MonoBehaviour
{
    [SerializeField] private List<GoalAttemptUI> goalAttemptsUI; // List to hold the references to the TextMeshPro fields
    private List<GoalAttempt> goalAttemptsData; // Private list to hold the goal attempts data

    public void SetGoalAttemptsData(List<GoalAttempt> goalAttempts) // Public method to set data
    {
        goalAttemptsData = goalAttempts;
        ShowHighscoreTable(); // Populate the table after setting the data
    }

    public void SetGoalAttemptsDataForRound(int roundNumber)
    {
        goalAttemptsData = DataManager.Instance.GetGoalAttemptsByRound(roundNumber);
        ShowHighscoreTable();
    }

    private void ShowHighscoreTable()
    {
        if (goalAttemptsData == null || goalAttemptsData.Count == 0)
        {
            Debug.LogWarning("No goal attempts data provided!");
            return;
        }

        for (int i = 0; i < goalAttemptsData.Count; i++)
        {
            if (i >= goalAttemptsUI.Count)
            {
                Debug.LogWarning("Not enough UI elements to display all goal attempts.");
                break;
            }

            GoalAttemptUI attemptUI = goalAttemptsUI[i];
            GoalAttempt attempt = goalAttemptsData[i];

            attemptUI.attemptNo.text = (i + 1).ToString(); // Correct the attempt number display
            attemptUI.goalPos.text = attempt.goalPosition.ToString(); // Display goal position
            attemptUI.reflexTime.text = attempt.reflexTime.ToString("F2");
            attemptUI.isSaved.text = attempt.isSaved ? "Yes" : "No";
            attemptUI.errorDistance.text = attempt.errorDistance.ToString("F2");
            attemptUI.bodyArea.text = attempt.bodyArea; // Display body area
        }
    }
}


[System.Serializable]
public class GoalAttemptUI
{
    public TextMeshProUGUI attemptNo;
    public TextMeshProUGUI goalPos;
    public TextMeshProUGUI reflexTime;
    public TextMeshProUGUI isSaved;
    public TextMeshProUGUI errorDistance;
    public TextMeshProUGUI bodyArea;
}
