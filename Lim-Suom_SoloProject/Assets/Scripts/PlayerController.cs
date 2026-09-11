using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public float speed = 5.0f;
    public float jumpHeight = 10f;

    PlayerInput playerInput;
    Rigidbody rb;

    Vector2 moveInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Fetch Components into an variable
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();

        // Set up new move vector
        moveInput = new Vector2();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 tempMovement = rb.linearVelocity;

        rb.linearVelocity = (moveInput.x * speed) + (moveInput.y * speed) + (moveInput.z * speed);
    }

    // Read context of input
    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

    }
}
