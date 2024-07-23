using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

public static class SaveLoadManager
{
    private const string DATA_FILE = "data.json";
    private static string currentHistoryFile;

    public static void SaveData(List<GoalAttempt> data)
    {
        string json = JsonUtility.ToJson(new SerializableList<GoalAttempt>(data));
        File.WriteAllText(GetDataPath(), json);
        Debug.Log($"Data saved to {GetDataPath()}. Data count: {data.Count}");
    }

    public static List<GoalAttempt> LoadData()
    {
        string path = GetDataPath();
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SerializableList<GoalAttempt> serializableList = JsonUtility.FromJson<SerializableList<GoalAttempt>>(json);
            Debug.Log($"Data loaded from {path}. Loaded {serializableList.list.Count} goal attempts.");
            return serializableList.list;
        }
        Debug.Log($"No data file found at {path}. Returning empty list.");
        return new List<GoalAttempt>();
    }

    public static void MoveDataToHistory()
    {
        string dataPath = GetDataPath();
        if (currentHistoryFile == null)
        {
            currentHistoryFile = GetNewHistoryPath();
        }

        if (File.Exists(dataPath))
        {
            string json = File.ReadAllText(dataPath);
            SerializableList<GoalAttempt> serializableList = JsonUtility.FromJson<SerializableList<GoalAttempt>>(json);

            bool shouldAddHeader = !File.Exists(currentHistoryFile) || new FileInfo(currentHistoryFile).Length == 0;
            StringBuilder csvContent = new StringBuilder();
            
            if (shouldAddHeader)
            {
                csvContent.AppendLine("RoundNumber,AttemptNumber,GoalPosition,ReflexTime,IsSaved,ErrorDistance,BodyArea");
            }
            
            foreach (var attempt in serializableList.list)
            {
                csvContent.AppendLine(attempt.ToCsvString());
            }

            File.AppendAllText(currentHistoryFile, csvContent.ToString());
            Debug.Log($"Data moved to history file at {currentHistoryFile}. Moved {serializableList.list.Count} goal attempts.");

            ClearData();
        }
        else
        {
            Debug.Log($"No data file found at {dataPath}. Nothing to move to history.");
        }
    }

    public static void ExportHistoryToDownloads()
    {
        if (currentHistoryFile != null && File.Exists(currentHistoryFile))
        {
            string downloadsPath = "/storage/emulated/0/Download";
            string fileName = Path.GetFileName(currentHistoryFile);
            string destinationPath = Path.Combine(downloadsPath, fileName);

            File.Copy(currentHistoryFile, destinationPath, true);
            Debug.Log($"History exported to: {destinationPath}");
        }
        else
        {
            Debug.Log("No history file found to export.");
        }
    }

    public static void ClearHistory()
    {
        if (currentHistoryFile != null && File.Exists(currentHistoryFile))
        {
            File.Delete(currentHistoryFile);
            Debug.Log("History file cleared.");
        }
        currentHistoryFile = null;
    }

    public static void ClearData()
    {
        string dataPath = GetDataPath();
        if (File.Exists(dataPath))
        {
            File.WriteAllText(dataPath, "");
            Debug.Log($"Data file cleared at {dataPath}");
        }
    }

    private static string GetDataPath()
    {
        return Path.Combine(Application.persistentDataPath, DATA_FILE);
    }

    private static string GetNewHistoryPath()
    {
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        return Path.Combine(Application.persistentDataPath, $"History_{timestamp}.csv");
    }
}

[System.Serializable]
public class SerializableList<T>
{
    public List<T> list;
    public SerializableList(List<T> list) => this.list = list;
}