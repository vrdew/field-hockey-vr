using UnityEngine;
using System.Collections.Generic;

public class AvatarControl : MonoBehaviour
{
    [Header("References (all under AvatarRig)")]
    public Transform avatarRoot;        // Drag the AvatarRig transform here
    public Transform leftController;    // Drag XR Rig ▶ Camera Offset ▶ Left Controller
    public Transform rightController;   // Drag XR Rig ▶ Camera Offset ▶ Right Controller
    public Transform leftHandTarget;    // Your empty target under AvatarRig
    public Transform rightHandTarget;   // Your empty target under AvatarRig

    [Header("Smoothing (optional)")]
    public bool useSmoothing = false;
    public int windowSize = 5;
    private Queue<Vector3> leftQueue = new Queue<Vector3>();
    private Queue<Vector3> rightQueue = new Queue<Vector3>();

    void LateUpdate()
    {
        // 1) read controller world-space
        Vector3 worldL = leftController.position;
        Vector3 worldR = rightController.position;

        // 2) convert into Avatar-local space
        Vector3 localL = avatarRoot.InverseTransformPoint(worldL);
        Vector3 localR = avatarRoot.InverseTransformPoint(worldR);

        // 3) optional moving-average smoothing
        if (useSmoothing)
        {
            localL = Smooth(localL, leftQueue);
            localR = Smooth(localR, rightQueue);
        }
        else
        {
            leftQueue.Clear();
            rightQueue.Clear();
        }

        // 4) apply to your IK targets **locally**  
        leftHandTarget.localPosition = localL;
        rightHandTarget.localPosition = localR;

        // 5) mirror rotation too (if your Two-Bone IK uses rotation)
        leftHandTarget.localRotation = Quaternion.Inverse(avatarRoot.rotation) * leftController.rotation;
        rightHandTarget.localRotation = Quaternion.Inverse(avatarRoot.rotation) * rightController.rotation;
    }

    Vector3 Smooth(Vector3 v, Queue<Vector3> q)
    {
        q.Enqueue(v);
        if (q.Count > windowSize) q.Dequeue();
        Vector3 sum = Vector3.zero;
        foreach (var x in q) sum += x;
        return sum / q.Count;
    }
}
