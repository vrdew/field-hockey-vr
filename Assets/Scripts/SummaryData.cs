using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class GoalkeeperStatsManager : MonoBehaviour
{
    [SerializeField] private StatsUI statsUI;
    private List<GoalAttempt> goalAttemptsData;

    private void Start()
    {
        UpdateStats(DataManager.Instance.CurrentRound); // Default to the current round on start
    }

    public void UpdateStats(int roundNumber)
    {
        goalAttemptsData = DataManager.Instance.GetGoalAttemptsByRound(roundNumber);

        if (goalAttemptsData == null || goalAttemptsData.Count == 0)
        {
            Debug.LogWarning($"No goal attempts data available for round {roundNumber}!");
            return;
        }

        // Calculate stats
        float accuracy = 0;
        float avgInitiationTime = 0;
        float avgErrorDistance = 0;
        for (int i = 0; i < goalAttemptsData.Count; i++)
        {
            if (goalAttemptsData[i].isSaved)
            {
                accuracy++;
            }
            avgInitiationTime += goalAttemptsData[i].reflexTime;
            avgErrorDistance += goalAttemptsData[i].errorDistance;
        }

        accuracy = accuracy / goalAttemptsData.Count * 100;
        avgInitiationTime = avgInitiationTime / goalAttemptsData.Count;
        avgErrorDistance = avgErrorDistance / goalAttemptsData.Count;

        string mostFrequentBodyPart = goalAttemptsData
            .Where(s => s.isSaved == true && s.bodyArea != "None")
            .GroupBy(s => s.bodyArea)
            .OrderByDescending(g => g.Count())
            .FirstOrDefault()?.Key ?? "No successful saves yet";

        var regionSuccessRates = goalAttemptsData
            .GroupBy(s => s.goalPosition)
            .Select(g => new
            {
                Region = g.Key,
                SuccessRate = (float)g.Count(s => s.isSaved == true) / g.Count(),
                SavedCount = g.Count(s => s.isSaved == true)
            })
            .ToList();
        bool anySaved = regionSuccessRates.Any(r => r.SavedCount > 0);

        string mostSuccessfulRegion = anySaved
            ? regionSuccessRates.OrderByDescending(r => r.SuccessRate).FirstOrDefault()?.Region.ToString()
            : "None";
        string leastSuccessfulRegion = anySaved
            ? regionSuccessRates.OrderBy(r => r.SuccessRate).FirstOrDefault()?.Region.ToString()
            : "None";

        statsUI.responseAccuracy.text = $"Response accuracy: {accuracy:F1}%";
        statsUI.avgInitiationTime.text = $"Average initiation time: {avgInitiationTime:F2} ms";
        statsUI.avgErrorDistance.text = $"Average error distance: {avgErrorDistance:F2} cm";
        statsUI.mostFrequentBodyPart.text = $"Most frequently used body part: {mostFrequentBodyPart}";
        statsUI.mostSuccessfulRegion.text = $"Most successful region saved: {mostSuccessfulRegion}";
        statsUI.leastSuccessfulRegion.text = $"Least successful region saved: {leastSuccessfulRegion}";
    }
}

[System.Serializable]
public class StatsUI
{
    public TextMeshProUGUI responseAccuracy;
    public TextMeshProUGUI avgInitiationTime;
    public TextMeshProUGUI avgErrorDistance;
    public TextMeshProUGUI mostFrequentBodyPart;
    public TextMeshProUGUI mostSuccessfulRegion;
    public TextMeshProUGUI leastSuccessfulRegion;
}
