using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public enum TargetZone
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

public class SemiCircularShooter : MonoBehaviour
{
    public GameObject projectilePrefab;
    public GameObject shooterPrefab;
    public GameObject targetPrefab;
    public float spawnInterval = 3f;
    public float shootingForce = 10f;
    public Vector3 projectileScale = new Vector3(2f, 2f, 2f);
    public float delayBeforeShoot = 0;
    public AudioClip spawnSound;
    private BoxCollider targetCollider;
    public TargetZone currentTargetZone;
    private AudioSource audioSource;
    public int spawnCount = 0;
    private const int maxSpawnCount = 10;
    public float projectileStartTime;
    public float travelTime = 1f;
    public float initialDelay = 3.5f;
    private Vector3[,] targetZones = new Vector3[9, 2];

    // Semi-circular spawning variables
    public float semiCircleRadius = 10f;
    public float semiCircleAngle = 180f;
    public Vector3 semiCircleCenter;

    void Start()
    {
        targetCollider = targetPrefab.GetComponent<BoxCollider>();
        if (targetCollider == null)
        {
            Debug.LogError("TargetPrefab does not have a BoxCollider component.");
            return;
        }

        SetupTargetZones();
        audioSource = gameObject.AddComponent<AudioSource>();

        // Set the semi-circle center relative to the target
        semiCircleCenter = targetPrefab.transform.position - targetPrefab.transform.forward * semiCircleRadius;

        StartCoroutine(InitialDelayRoutine());
    }

    IEnumerator InitialDelayRoutine()
    {
        yield return new WaitForSeconds(initialDelay);
        StartCoroutine(SpawnShooterRoutine());
    }

    void SetupTargetZones()
    {
        float thirdWidth = targetCollider.bounds.size.x / 3;
        float thirdHeight = targetCollider.bounds.size.y / 3;

        targetZones[(int)TargetZone.TopLeft, 0] = new Vector3(targetCollider.bounds.min.x, targetCollider.bounds.center.y + thirdHeight, targetPrefab.transform.position.z);
        targetZones[(int)TargetZone.TopLeft, 1] = new Vector3(targetCollider.bounds.center.x - thirdWidth, targetCollider.bounds.max.y, targetPrefab.transform.position.z);

        targetZones[(int)TargetZone.TopMiddle, 0] = new Vector3(targetCollider.bounds.center.x - thirdWidth, targetCollider.bounds.center.y + thirdHeight, targetPrefab.transform.position.z);
        targetZones[(int)TargetZone.TopMiddle, 1] = new Vector3(targetCollider.bounds.center.x + thirdWidth, targetCollider.bounds.max.y, targetPrefab.transform.position.z);

        targetZones[(int)TargetZone.TopRight, 0] = new Vector3(targetCollider.bounds.center.x + thirdWidth, targetCollider.bounds.center.y + thirdHeight, targetPrefab.transform.position.z);
        targetZones[(int)TargetZone.TopRight, 1] = new Vector3(targetCollider.bounds.max.x, targetCollider.bounds.max.y, targetPrefab.transform.position.z);

        targetZones[(int)TargetZone.CenterLeft, 0] = new Vector3(targetCollider.bounds.min.x, targetCollider.bounds.center.y - thirdHeight, targetPrefab.transform.position.z);
        targetZones[(int)TargetZone.CenterLeft, 1] = new Vector3(targetCollider.bounds.center.x - thirdWidth, targetCollider.bounds.center.y + thirdHeight, targetPrefab.transform.position.z);

        targetZones[(int)TargetZone.Center, 0] = new Vector3(targetCollider.bounds.center.x - thirdWidth, targetCollider.bounds.center.y - thirdHeight, targetPrefab.transform.position.z);
        targetZones[(int)TargetZone.Center, 1] = new Vector3(targetCollider.bounds.center.x + thirdWidth, targetCollider.bounds.center.y + thirdHeight, targetPrefab.transform.position.z);

        targetZones[(int)TargetZone.CenterRight, 0] = new Vector3(targetCollider.bounds.center.x + thirdWidth, targetCollider.bounds.center.y - thirdHeight, targetPrefab.transform.position.z);
        targetZones[(int)TargetZone.CenterRight, 1] = new Vector3(targetCollider.bounds.max.x, targetCollider.bounds.center.y + thirdHeight, targetPrefab.transform.position.z);

        targetZones[(int)TargetZone.BottomLeft, 0] = new Vector3(targetCollider.bounds.min.x, targetCollider.bounds.min.y, targetPrefab.transform.position.z);
        targetZones[(int)TargetZone.BottomLeft, 1] = new Vector3(targetCollider.bounds.center.x - thirdWidth, targetCollider.bounds.center.y - thirdHeight, targetPrefab.transform.position.z);

        targetZones[(int)TargetZone.BottomMiddle, 0] = new Vector3(targetCollider.bounds.center.x - thirdWidth, targetCollider.bounds.min.y, targetPrefab.transform.position.z);
        targetZones[(int)TargetZone.BottomMiddle, 1] = new Vector3(targetCollider.bounds.center.x + thirdWidth, targetCollider.bounds.center.y - thirdHeight, targetPrefab.transform.position.z);

        targetZones[(int)TargetZone.BottomRight, 0] = new Vector3(targetCollider.bounds.center.x + thirdWidth, targetCollider.bounds.min.y, targetPrefab.transform.position.z);
        targetZones[(int)TargetZone.BottomRight, 1] = new Vector3(targetCollider.bounds.max.x, targetCollider.bounds.center.y - thirdHeight, targetPrefab.transform.position.z);
    }

    IEnumerator SpawnShooterRoutine()
    {
        while (spawnCount < maxSpawnCount)
        {
            SpawnAndShoot();
            yield return new WaitForSeconds(spawnInterval);
            spawnCount++;
        }
        SceneManager.LoadScene("Scene8");
    }

    void SpawnAndShoot()
    {
        Vector3 spawnPosition = GetRandomPointOnSemiCircle();
        GameObject shooter = Instantiate(shooterPrefab, spawnPosition, Quaternion.identity);
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

        if (spawnSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(spawnSound);
        }

        projectile.transform.localScale = projectileScale;
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
        if (projectileRb != null)
        {
            projectileRb.isKinematic = true;
            projectileRb.useGravity = false;
        }
        else
        {
            Debug.LogError("Projectile does not have a Rigidbody component.");
        }

        StartCoroutine(ShootAfterDelay(shooter, projectile));
    }

    IEnumerator ShootAfterDelay(GameObject shooter, GameObject projectile)
    {
        yield return new WaitForSeconds(delayBeforeShoot);

        currentTargetZone = (TargetZone)Random.Range(0, 9);
        Vector3 targetPosition = Vector3.Lerp(targetZones[(int)currentTargetZone, 0], targetZones[(int)currentTargetZone, 1], Random.value);
        Vector3 directionToTarget = (targetPosition - projectile.transform.position).normalized;

        // Rotate shooter to face the target
        shooter.transform.rotation = Quaternion.LookRotation(directionToTarget);

        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
        if (projectileRb != null)
        {
            projectileStartTime = Time.time;
            projectileRb.isKinematic = false;
            projectileRb.useGravity = false;

            // Calculate and apply velocity
            Vector3 velocity = (targetPosition - projectile.transform.position) / travelTime;
            projectileRb.velocity = velocity;

            // Record reflex start time for all body colliders
            foreach (BodyCollider bodyCollider in FindObjectsOfType<BodyCollider>())
            {
                bodyCollider.RecordReflexStartTime();
            }
        }
        else
        {
            Debug.LogError("Projectile does not have a Rigidbody component.");
        }
    }

    Vector3 GetRandomPointOnSemiCircle()
    {
        float angle = Random.Range(0, semiCircleAngle) * Mathf.Deg2Rad;
        float x = Mathf.Cos(angle) * semiCircleRadius;
        float z = Mathf.Sin(angle) * semiCircleRadius;
        return semiCircleCenter + new Vector3(x, 0, z);
    }
}