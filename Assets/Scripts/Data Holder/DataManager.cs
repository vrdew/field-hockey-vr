using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    private List<GoalAttempt> goalAttempts = new List<GoalAttempt>();
    private static int currentRound = 1;
    private int currentAttempt = 0;
    private const int attemptsPerRound = 10;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int CurrentRound => currentRound;

    public void StartNewRound()
    {
        currentRound++;
        currentAttempt = 0;
    }

    public void AddGoalAttempt(GoalAttempt attempt)
    {
        currentAttempt++;
        if (currentAttempt > attemptsPerRound)
        {
            StartNewRound();
            currentAttempt = 1;
        }

        attempt.roundNumber = currentRound;
        attempt.attemptNumber = currentAttempt;
        goalAttempts.Add(attempt);
        SaveData();
    }

    public List<GoalAttempt> GetGoalAttempts()
    {
        return new List<GoalAttempt>(goalAttempts);
    }

    public List<GoalAttempt> GetGoalAttemptsByRound(int roundNumber)
    {
        return goalAttempts.FindAll(attempt => attempt.roundNumber == roundNumber);
    }

    private void SaveData()
    {
        SaveLoadManager.SaveData(goalAttempts);
    }

    private void LoadData()
    {
        goalAttempts = SaveLoadManager.LoadData();
        if (goalAttempts.Count > 0)
        {
            int highestRound = 1;
            foreach (var attempt in goalAttempts)
            {
                if (attempt.roundNumber > highestRound)
                {
                    highestRound = attempt.roundNumber;
                }
            }
            currentRound = highestRound;
        }
    }

    public void EndGame()
    {
        SaveLoadManager.MoveDataToHistory();
        goalAttempts.Clear();
        currentAttempt = 0;
    }

    public void QuitGame()
    {
        SaveLoadManager.ExportHistoryToDownloads();
        SaveLoadManager.ClearHistory();
        SaveLoadManager.ClearData();
    }

    public void ResetRound()
    {
        currentRound = 1;
    }
}