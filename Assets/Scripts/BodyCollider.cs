using UnityEngine;
using System.Collections;

public class BodyCollider : MonoBehaviour
{
    private BallBehavior ballSpawner;
    public AudioSource audioSource;
    private float reflexStartTime;
    private static bool collisionRecorded = false; // Track if a collision has already been recorded
    public static bool ballBlocked = false; // Track if the ball has been blocked

    [SerializeField] private float bounceSpeed = 15f;
    [SerializeField] private float upwardBounceForce = 5f;

    private void Start()
    {
        ballSpawner = FindObjectOfType<BallBehavior>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball") && !collisionRecorded && collision.gameObject.GetComponent<BlockedBall>() == null)
        {
            collisionRecorded = true; // Mark that a collision has been recorded
            ballBlocked = true; // Mark the ball as blocked

            // Record the body part
            string bodyPart = GetBodyPartName();

            // Calculate reflex time
            float reflexTime = 1000 * (Time.time - reflexStartTime);

            // Play the sound effect
            audioSource.Play();

            // Create and add the GoalAttempt
            GoalAttempt attempt = new GoalAttempt(DataManager.Instance.CurrentRound, ballSpawner.spawnCount, ballSpawner.currentGoalPosition, reflexTime, false, 0f, bodyPart);
            DataManager.Instance.AddGoalAttempt(attempt);

            // Apply a bounce to the ball
            ApplyBounceToBall(collision);

            // Mark the ball as blocked
            collision.gameObject.AddComponent<BlockedBall>();

            Destroy(collision.gameObject, 3f);

            // Reset collisionRecorded after a short delay to be ready for the next ball
            StartCoroutine(ResetCollisionRecorded());
        }
    }

    private string GetBodyPartName()
    {
        string bodyPart = gameObject.name.Replace("mixamorig:", ""); // Remove the "mixamorig:" prefix
        if (bodyPart == "Head_RotatedCollider")
        {
            bodyPart = "Head";
        }
        if (bodyPart == "Neck_RotatedCollider")
        {
            bodyPart = "Neck";
        }
        if (bodyPart == "RightHand")
        {
            bodyPart = "Hockey Stick";
        }
        if (bodyPart == "Spine1" || bodyPart == "Spine")
        {
            bodyPart = "Lower Torso";
        }
        if (bodyPart == "Spine2")
        {
            bodyPart = "Upper Torso";
        }

        // Add spaces before capital letters in the body part name
        for (int i = 1; i < bodyPart.Length; i++)
        {
            if (char.IsUpper(bodyPart[i]) && !char.IsWhiteSpace(bodyPart[i - 1]))
            {
                bodyPart = bodyPart.Insert(i, " ");
                i++;
            }
        }

        return bodyPart;
    }

    private void ApplyBounceToBall(Collision collision)
    {
        Rigidbody ballRigidbody = collision.rigidbody;
        if (ballRigidbody != null)
        {
            // Calculate bounce direction away from the goal
            Vector3 awayFromGoal = -transform.forward; // Assuming the character is facing the goal
            Vector3 upVector = Vector3.up;
            Vector3 bounceDirection = (awayFromGoal + upVector).normalized;

            // Apply the bounce force
            ballRigidbody.velocity = bounceDirection * bounceSpeed + Vector3.up * upwardBounceForce;
        }
    }

    public void RecordReflexStartTime()
    {
        reflexStartTime = Time.time;
    }

    private IEnumerator ResetCollisionRecorded()
    {
        yield return new WaitForSeconds(0.5f);
        collisionRecorded = false;
    }
}

public class BlockedBall : MonoBehaviour { }