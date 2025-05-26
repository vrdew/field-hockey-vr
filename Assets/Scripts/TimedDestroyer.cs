using UnityEngine;

public class TimedDestroyer : MonoBehaviour
{
    [Tooltip("Assign the part of this GameObject to destroy (e.g., child GameObject, MeshRenderer, etc.)")]
    public GameObject targetPart;

    [Tooltip("Time in seconds before the target part is destroyed")]
    public float destroyDelay = 2f;

    void Start()
    {
        if (targetPart != null)
        {
            Destroy(targetPart, destroyDelay);
        }
        else
        {
            Debug.LogWarning("TimedDestroyer: No target part assigned to destroy.", this);
        }
    }
}
