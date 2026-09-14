using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public float speed = 5.0f;
    public float jumpHeight = 2;
    public float jumpDetectDistance = 1;

    Ray jumpRay;
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
        moveInput = Vector2.zero;

        jumpRay = new Ray(transform.position, -transform.up);
        
    }

    // Update is called once per frame
    void Update()
    {
        jumpRay.origin = transform.position;
        jumpRay.direction = -transform.up;

        Vector3 tempMove = rb.linearVelocity;

        tempMove.x = (moveInput.x * speed) * transform.right.x;
        tempMove.z = (moveInput.y * speed) * transform.forward.z;


        rb.linearVelocity = tempMove; 
    }

    // Read context of input
    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

    }

    public void Jump()
    {
        if(Physics.Raycast(jumpRay, jumpDetectDistance))
        {
            rb.AddForce(transform.up * jumpHeight, ForceMode.Impulse);
        }
    }
}
