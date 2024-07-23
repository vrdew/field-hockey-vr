using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;

public enum GoalPosition
{
    TopLeft,
    TopMiddle,
    TopRight,
    CenterLeft,
    Center,
    CenterRight,
    BottomLeft,
    BottomMiddle,
    BottomRight
}

public class BallBehavior : MonoBehaviour
{
    public GameObject ballPrefab;
    public GameObject GoalPrefab;
    public float spawnInterval = 3f;
    public float shootingForce = 10f;
    public Vector3 ballScale = new Vector3(2f, 2f, 2f);
    public float delayBeforeShoot = 0;
    public AudioClip spawnSound;
    private BoxCollider goalCollider;
    public GoalPosition currentGoalPosition;
    private AudioSource audioSource;
    public int spawnCount = 0;
    private const int maxSpawnCount = 10;
    public float ballStartTime;
    public float travelTime = 1f;
    public float initialDelay = 3.5f;
    private Vector3[,] targetZones = new Vector3[9, 2];
    private List<GoalPosition> selectedTargetPositions = new List<GoalPosition>();
    private const string JSON_FILE_NAME = "selected_targets.json";

    void Start()
    {
        goalCollider = GoalPrefab.GetComponent<BoxCollider>();
        if (goalCollider == null)
        {
            Debug.LogError("GoalPrefab does not have a BoxCollider component.");
            return;
        }

        SetupTargetZones();
        audioSource = gameObject.AddComponent<AudioSource>();
        LoadSelectedTargetPositions();
        ClearSelectedTargetsFile();

        StartCoroutine(InitialDelayRoutine());
    }

    void LoadSelectedTargetPositions()
    {
        string filePath = Path.Combine(Application.persistentDataPath, JSON_FILE_NAME);
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            SelectedTargetOptions selectedOptions = JsonUtility.FromJson<SelectedTargetOptions>(json);
            foreach (string option in selectedOptions.options)
            {
                if (System.Enum.TryParse(option, out GoalPosition goalPosition))
                {
                    selectedTargetPositions.Add(goalPosition);
                }
            }
            Debug.Log($"Loaded {selectedTargetPositions.Count} target positions from JSON file.");
        }

        if (selectedTargetPositions.Count == 0)
        {
            Debug.Log("No target positions were selected or file not found. Using all positions.");
            selectedTargetPositions.AddRange(System.Enum.GetValues(typeof(GoalPosition)) as GoalPosition[]);
        }
    }

    void ClearSelectedTargetsFile()
    {
        string filePath = Path.Combine(Application.persistentDataPath, JSON_FILE_NAME);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log($"Cleared selected targets file at {filePath}");
        }
    }

    IEnumerator InitialDelayRoutine()
    {
        yield return new WaitForSeconds(initialDelay);
        StartCoroutine(SpawnBallRoutine());
    }

    void SetupTargetZones()
    {
        float thirdWidth = goalCollider.bounds.size.x / 3;
        float thirdHeight = goalCollider.bounds.size.y / 3;

        targetZones[(int)GoalPosition.TopLeft, 0] = new Vector3(goalCollider.bounds.min.x, goalCollider.bounds.center.y + thirdHeight, GoalPrefab.transform.position.z);
        targetZones[(int)GoalPosition.TopLeft, 1] = new Vector3(goalCollider.bounds.center.x - thirdWidth, goalCollider.bounds.max.y, GoalPrefab.transform.position.z);

        targetZones[(int)GoalPosition.TopMiddle, 0] = new Vector3(goalCollider.bounds.center.x - thirdWidth, goalCollider.bounds.center.y + thirdHeight, GoalPrefab.transform.position.z);
        targetZones[(int)GoalPosition.TopMiddle, 1] = new Vector3(goalCollider.bounds.center.x + thirdWidth, goalCollider.bounds.max.y, GoalPrefab.transform.position.z);

        targetZones[(int)GoalPosition.TopRight, 0] = new Vector3(goalCollider.bounds.center.x + thirdWidth, goalCollider.bounds.center.y + thirdHeight, GoalPrefab.transform.position.z);
        targetZones[(int)GoalPosition.TopRight, 1] = new Vector3(goalCollider.bounds.max.x, goalCollider.bounds.max.y, GoalPrefab.transform.position.z);

        targetZones[(int)GoalPosition.CenterLeft, 0] = new Vector3(goalCollider.bounds.min.x, goalCollider.bounds.center.y - thirdHeight, GoalPrefab.transform.position.z);
        targetZones[(int)GoalPosition.CenterLeft, 1] = new Vector3(goalCollider.bounds.center.x - thirdWidth, goalCollider.bounds.center.y + thirdHeight, GoalPrefab.transform.position.z);

        targetZones[(int)GoalPosition.Center, 0] = new Vector3(goalCollider.bounds.center.x - thirdWidth, goalCollider.bounds.center.y - thirdHeight, GoalPrefab.transform.position.z);
        targetZones[(int)GoalPosition.Center, 1] = new Vector3(goalCollider.bounds.center.x + thirdWidth, goalCollider.bounds.center.y + thirdHeight, GoalPrefab.transform.position.z);

        targetZones[(int)GoalPosition.CenterRight, 0] = new Vector3(goalCollider.bounds.center.x + thirdWidth, goalCollider.bounds.center.y - thirdHeight, GoalPrefab.transform.position.z);
        targetZones[(int)GoalPosition.CenterRight, 1] = new Vector3(goalCollider.bounds.max.x, goalCollider.bounds.center.y + thirdHeight, GoalPrefab.transform.position.z);

        targetZones[(int)GoalPosition.BottomLeft, 0] = new Vector3(goalCollider.bounds.min.x, goalCollider.bounds.min.y, GoalPrefab.transform.position.z);
        targetZones[(int)GoalPosition.BottomLeft, 1] = new Vector3(goalCollider.bounds.center.x - thirdWidth, goalCollider.bounds.center.y - thirdHeight, GoalPrefab.transform.position.z);

        targetZones[(int)GoalPosition.BottomMiddle, 0] = new Vector3(goalCollider.bounds.center.x - thirdWidth, goalCollider.bounds.min.y, GoalPrefab.transform.position.z);
        targetZones[(int)GoalPosition.BottomMiddle, 1] = new Vector3(goalCollider.bounds.center.x + thirdWidth, goalCollider.bounds.center.y - thirdHeight, GoalPrefab.transform.position.z);

        targetZones[(int)GoalPosition.BottomRight, 0] = new Vector3(goalCollider.bounds.center.x + thirdWidth, goalCollider.bounds.min.y, GoalPrefab.transform.position.z);
        targetZones[(int)GoalPosition.BottomRight, 1] = new Vector3(goalCollider.bounds.max.x, goalCollider.bounds.center.y - thirdHeight, GoalPrefab.transform.position.z);
    }

    IEnumerator SpawnBallRoutine()
    {
        yield return new WaitForSeconds(spawnInterval);
        while (spawnCount < maxSpawnCount)
        {
            SpawnAndShootBall();
            yield return new WaitForSeconds(spawnInterval);
            spawnCount++;
        }
        SceneManager.LoadScene("Scene8");
    }

    public void SpawnBall()
    {
        if (spawnCount < maxSpawnCount)
        {
            StartCoroutine(DelayedSpawnAndShootBall());
            spawnCount++;
        }
    }

    IEnumerator DelayedSpawnAndShootBall()
    {
        yield return new WaitForSeconds(3f);
        SpawnAndShootBall();
    }

    void SpawnAndShootBall()
    {
        StartCoroutine(SpawnAndShootBallRoutine());
    }

    IEnumerator SpawnAndShootBallRoutine()
    {
        Vector3 spawnPosition = transform.position;
        GameObject ball = Instantiate(ballPrefab, spawnPosition, Quaternion.identity);

        if (spawnSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(spawnSound);
        }

        ball.transform.localScale = ballScale;
        Rigidbody ballRb = ball.GetComponent<Rigidbody>();
        if (ballRb != null)
        {
            ballRb.isKinematic = true;
            ballRb.useGravity = false;
        }
        else
        {
            Debug.LogError("Ball does not have a Rigidbody component.");
        }

        yield return new WaitForSeconds(delayBeforeShoot);

        currentGoalPosition = selectedTargetPositions[Random.Range(0, selectedTargetPositions.Count)];
        Vector3 targetPosition = Vector3.Lerp(targetZones[(int)currentGoalPosition, 0], targetZones[(int)currentGoalPosition, 1], Random.value);
        Vector3 directionToGoal = (targetPosition - spawnPosition).normalized;

        if (ballRb != null)
        {
            ballStartTime = Time.time;
            ballRb.isKinematic = false;
            ballRb.useGravity = false;

            Vector3 velocity = (targetPosition - spawnPosition) / travelTime;
            ballRb.velocity = velocity;

            foreach (BodyCollider bodyCollider in FindObjectsOfType<BodyCollider>())
            {
                bodyCollider.RecordReflexStartTime();
            }
        }
        else
        {
            Debug.LogError("Ball does not have a Rigidbody component.");
        }
    }

    [System.Serializable]
    private class SelectedTargetOptions
    {
        public List<string> options;
    }
}