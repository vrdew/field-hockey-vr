using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public float groundDistance = 0.1f; // Distance to check for ground
    public LayerMask groundMask; // Layer(s) to consider as ground
    public float gravityForce = 9.81f; // Force to apply when not grounded

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component is required on this game object.");
            enabled = false;
            return;
        }
    }

    void FixedUpdate()
    {
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, groundDistance, groundMask);

        if (isGrounded)
        {
            // Snap to ground if very close
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, groundDistance + 0.1f, groundMask))
            {
                transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); // Zero out vertical velocity
            }
        }
        else
        {
            // Apply gravity when not grounded
            rb.AddForce(Vector3.down * gravityForce, ForceMode.Acceleration);
        }
    }

    void OnDrawGizmos()
    {
        // Visualize the ground check ray in the Scene view
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundDistance);
    }
}