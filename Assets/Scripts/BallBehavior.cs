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
    [Header("Prefabs & Settings")]
    [Tooltip("The physics-driven sphere prefab (no animation)")]
    public GameObject ballPrefab;

    [Tooltip("Animated FBX prefab for each GoalPosition, in enum order")]
    public List<GameObject> animationPrefabs; // 9 elements

    [Header("Per-Animation Overrides")]
    [Tooltip("Spawn positions for each animation prefab (in enum order)")]
    public Vector3[] animationSpawnPositions = new Vector3[9];
    [Tooltip("Delay before spawning ball, for each animation (in seconds)")]
    public float[] animationDelays = new float[9];

    public GameObject GoalPrefab;
    public float spawnInterval = 3f;
    public Vector3 ballScale = new Vector3(2f, 2f, 2f);
    public float travelTime = 1f;
    public float initialDelay = 3.5f;

    public AudioClip spawnSound;

    [Header("Runtime Info")]
    public int spawnCount = 0;
    public GoalPosition currentGoalPosition;
    public float ballStartTime;

    private AudioSource audioSource;
    private BoxCollider goalCollider;
    private Vector3[,] targetZones = new Vector3[9, 2];
    private List<GoalPosition> selectedTargetPositions = new List<GoalPosition>();
    private const int maxSpawnCount = 10;
    private const string JSON_FILE_NAME = "selected_targets.json";

    void Start()
    {
        // validate lists
        if (animationPrefabs == null || animationPrefabs.Count != 9)
            Debug.LogError("animationPrefabs must contain exactly 9 elements.");
        if (animationSpawnPositions.Length != 9 || animationDelays.Length != 9)
            Debug.LogError("animationSpawnPositions and animationDelays must each have length 9.");

        goalCollider = GoalPrefab.GetComponent<BoxCollider>();
        if (goalCollider == null) Debug.LogError("GoalPrefab missing BoxCollider");

        audioSource = gameObject.AddComponent<AudioSource>();
        SetupTargetZones();
        LoadSelectedTargetPositions();
        ClearSelectedTargetsFile();

        StartCoroutine(InitialDelayRoutine());
    }

    IEnumerator InitialDelayRoutine()
    {
        yield return new WaitForSeconds(initialDelay);
        StartCoroutine(SpawnBallRoutine());
    }

    IEnumerator SpawnBallRoutine()
    {
        yield return new WaitForSeconds(spawnInterval);
        while (spawnCount < maxSpawnCount)
        {
            StartCoroutine(SpawnAndShootBallRoutine());
            spawnCount++;
            yield return new WaitForSeconds(spawnInterval);
        }
        SceneManager.LoadScene("Scene8");
    }

    IEnumerator SpawnAndShootBallRoutine()
    {
        // select direction
        currentGoalPosition = selectedTargetPositions[Random.Range(0, selectedTargetPositions.Count)];
        int idx = (int)currentGoalPosition;

        // play animation
        Vector3 animPos = animationSpawnPositions[idx];
        Quaternion animRot = Quaternion.Euler(0f, 180f, 0f);
        GameObject animModel = Instantiate(animationPrefabs[idx], animPos, animRot);
        Animator anim = animModel.GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetInteger("DirIndex", idx);
            anim.SetTrigger("PlayDir");
        }
        else Debug.LogWarning("Animated prefab missing Animator");

        // wait per-animation delay
        yield return new WaitForSeconds(animationDelays[idx]);

        // spawn sphere and apply physics
        Vector3 spawnPos = transform.position;
        GameObject ball = Instantiate(ballPrefab, spawnPos, Quaternion.identity);

        if (spawnSound != null)
            audioSource.PlayOneShot(spawnSound);

        ball.transform.localScale = ballScale;
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = false;
        }
        else Debug.LogError("ballPrefab missing Rigidbody");

        // record launch time
        ballStartTime = Time.time;

        // launch velocity
        Vector3 targetPos = Vector3.Lerp(
            targetZones[idx, 0],
            targetZones[idx, 1],
            Random.value
        );
        if (rb != null) rb.velocity = (targetPos - spawnPos) / travelTime;

        // destroy fbx + ball on goal hit
        var cleanup = ball.AddComponent<AnimCleanup>();
        cleanup.animModel = animModel;

        // notify listeners
        foreach (BodyCollider bc in FindObjectsOfType<BodyCollider>())
            bc.RecordReflexStartTime();
    }

    void SetupTargetZones()
    {
        float w = goalCollider.bounds.size.x / 3f;
        float h = goalCollider.bounds.size.y / 3f;
        var min = goalCollider.bounds.min;
        var max = goalCollider.bounds.max;
        var c = goalCollider.bounds.center;
        float z = GoalPrefab.transform.position.z;
        // TopLeft
        targetZones[0, 0] = new Vector3(min.x, c.y + h, z);
        targetZones[0, 1] = new Vector3(c.x - w, max.y, z);
        // TopMiddle
        targetZones[1, 0] = new Vector3(c.x - w, c.y + h, z);
        targetZones[1, 1] = new Vector3(c.x + w, max.y, z);
        // TopRight
        targetZones[2, 0] = new Vector3(c.x + w, c.y + h, z);
        targetZones[2, 1] = new Vector3(max.x, max.y, z);
        // CenterLeft
        targetZones[3, 0] = new Vector3(min.x, c.y - h, z);
        targetZones[3, 1] = new Vector3(c.x - w, c.y + h, z);
        // Center
        targetZones[4, 0] = new Vector3(c.x - w, c.y - h, z);
        targetZones[4, 1] = new Vector3(c.x + w, c.y + h, z);
        // CenterRight
        targetZones[5, 0] = new Vector3(c.x + w, c.y - h, z);
        targetZones[5, 1] = new Vector3(max.x, c.y + h, z);
        // BottomLeft
        targetZones[6, 0] = new Vector3(min.x, min.y, z);
        targetZones[6, 1] = new Vector3(c.x - w, c.y - h, z);
        // BottomMiddle
        targetZones[7, 0] = new Vector3(c.x - w, min.y, z);
        targetZones[7, 1] = new Vector3(c.x + w, c.y - h, z);
        // BottomRight
        targetZones[8, 0] = new Vector3(c.x + w, min.y, z);
        targetZones[8, 1] = new Vector3(max.x, c.y - h, z);
    }

    void LoadSelectedTargetPositions()
    {
        string path = Path.Combine(Application.persistentDataPath, JSON_FILE_NAME);
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            var opts = JsonUtility.FromJson<SelectedTargetOptions>(json);
            foreach (var o in opts.options)
                if (System.Enum.TryParse(o, out GoalPosition gp))
                    selectedTargetPositions.Add(gp);
        }
        if (selectedTargetPositions.Count == 0)
            selectedTargetPositions.AddRange(System.Enum.GetValues(typeof(GoalPosition)) as GoalPosition[]);
    }

    void ClearSelectedTargetsFile()
    {
        string path = Path.Combine(Application.persistentDataPath, JSON_FILE_NAME);
        if (File.Exists(path)) File.Delete(path);
    }

    [System.Serializable]
    private class SelectedTargetOptions { public List<string> options; }

    private class AnimCleanup : MonoBehaviour
    {
        public GameObject animModel;

        void Start()
        {
            // Auto destroy both ball and animation model after 5 seconds
            if (animModel != null)
                Destroy(animModel, 1.5f);
            Destroy(gameObject, 1.5f);
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("goal"))
            {
                if (animModel != null)
                    Destroy(animModel);
                Destroy(gameObject);
            }
        }

        void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("goal"))
            {
                if (animModel != null)
                    Destroy(animModel);
                Destroy(gameObject);
            }
        }
    }

}
