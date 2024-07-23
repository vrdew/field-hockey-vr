using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighscoreTableManager : MonoBehaviour
{
    [SerializeField] private HighscoreTable highscoreTable;

    private void Start()
    {
        // Get the current round number and set the highscore table data
        int currentRound = DataManager.Instance.CurrentRound;
        highscoreTable.SetGoalAttemptsDataForRound(currentRound);
    }
}
