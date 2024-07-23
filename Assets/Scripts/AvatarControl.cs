using UnityEngine;
using UnityEngine.Animations.Rigging;
using System.Collections.Generic;

public class AvatarControl : MonoBehaviour
{
    [Header("Smoothing Settings")]
    public int windowSize = 5;
    public float kalmanQ = 0.00001f;
    public float kalmanR = 0.01f;

    [Header("IK Targets")]
    public Transform leftHandTarget;
    public Transform rightHandTarget;
    public Transform leftFootTarget;
    public Transform rightFootTarget;

    private Queue<Vector3> leftHandPositions = new Queue<Vector3>();
    private Queue<Vector3> rightHandPositions = new Queue<Vector3>();
    private Queue<Vector3> leftFootPositions = new Queue<Vector3>();
    private Queue<Vector3> rightFootPositions = new Queue<Vector3>();

    private KalmanFilter kalmanFilterX = new KalmanFilter();
    private KalmanFilter kalmanFilterY = new KalmanFilter();
    private KalmanFilter kalmanFilterZ = new KalmanFilter();

    void Update()
    {
        // Update smoothed positions for hands and feet
        Vector3 smoothedLeftHandPosition = GetSmoothedPosition(leftHandTarget.position, leftHandPositions);
        Vector3 smoothedRightHandPosition = GetSmoothedPosition(rightHandTarget.position, rightHandPositions);
        Vector3 smoothedLeftFootPosition = GetSmoothedPosition(leftFootTarget.position, leftFootPositions);
        Vector3 smoothedRightFootPosition = GetSmoothedPosition(rightFootTarget.position, rightFootPositions);

        // Apply Kalman filter for additional smoothing
        smoothedLeftHandPosition = ApplyKalmanFilter(smoothedLeftHandPosition);
        smoothedRightHandPosition = ApplyKalmanFilter(smoothedRightHandPosition);
        smoothedLeftFootPosition = ApplyKalmanFilter(smoothedLeftFootPosition);
        smoothedRightFootPosition = ApplyKalmanFilter(smoothedRightFootPosition);

        // Update IK targets with smoothed positions
        leftHandTarget.position = smoothedLeftHandPosition;
        rightHandTarget.position = smoothedRightHandPosition;
        leftFootTarget.position = smoothedLeftFootPosition;
        rightFootTarget.position = smoothedRightFootPosition;
    }

    Vector3 GetSmoothedPosition(Vector3 currentPosition, Queue<Vector3> positionQueue)
    {
        positionQueue.Enqueue(currentPosition);

        if (positionQueue.Count > windowSize)
        {
            positionQueue.Dequeue();
        }

        Vector3 smoothedPosition = Vector3.zero;
        foreach (Vector3 pos in positionQueue)
        {
            smoothedPosition += pos;
        }
        smoothedPosition /= positionQueue.Count;

        return smoothedPosition;
    }

    Vector3 ApplyKalmanFilter(Vector3 position)
    {
        position.x = kalmanFilterX.Update(position.x);
        position.y = kalmanFilterY.Update(position.y);
        position.z = kalmanFilterZ.Update(position.z);
        return position;
    }
}

public class KalmanFilter
{
    private float q;
    private float r;
    private float p = 1, x = 0, k;

    public KalmanFilter(float q = 0.00001f, float r = 0.01f)
    {
        this.q = q;
        this.r = r;
    }

    public float Update(float measurement)
    {
        p = p + q;
        k = p / (p + r);
        x = x + k * (measurement - x);
        p = (1 - k) * p;
        return x;
    }
}