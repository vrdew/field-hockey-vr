using UnityEngine;
using System.Collections;

public class Goal : MonoBehaviour
{
    private BallBehavior ballSpawner;
    public AudioSource audioSource;

    private void Start()
    {
        ballSpawner = FindObjectOfType<BallBehavior>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            BlockedBall blockedBall = other.gameObject.GetComponent<BlockedBall>();
            if (blockedBall == null)
            {
                // Ball was not blocked, record as a successful goal
                RecordGoal(other.gameObject);
            }
            // If the ball was blocked, do nothing (the attempt was already recorded in BodyCollider)

            // Destroy the ball
            Destroy(other.gameObject);
        }
    }

    private void RecordGoal(GameObject ball)
    {
        string bodyPart = "None";
        float reflexTime = 1000 * (Time.time - ballSpawner.ballStartTime);
        float errorDistance = CalculateErrorDistance(ball);

        // Play the sound effect
        audioSource.Play();

        // Create and add the GoalAttempt
        GoalAttempt attempt = new GoalAttempt(DataManager.Instance.CurrentRound, ballSpawner.spawnCount, ballSpawner.currentGoalPosition, reflexTime, true, errorDistance, bodyPart);
        DataManager.Instance.AddGoalAttempt(attempt);
    }

    private float CalculateErrorDistance(GameObject ball)
    {
        float errorDistance = float.MaxValue;
        foreach (BodyCollider bodyCollider in FindObjectsOfType<BodyCollider>())
        {
            float distance = Vector3.Distance(ball.transform.position, bodyCollider.transform.position);
            if (distance < errorDistance)
            {
                errorDistance = distance;
            }
        }
        return errorDistance;
    }
}