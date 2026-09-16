using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public float speed = 5.0f;
    public float jumpHeight = 2;
    public float jumpDetectDistance = 1;

    CinemachinePositionComposer cineCam;
    Camera playerCam;
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

        // Initialize Camera
        playerCam = Camera.main;
        cineCam = GameObject.Find("CinemachineCamera").GetComponent<CinemachinePositionComposer>();

        // Set up new move vector
        moveInput = Vector2.zero;

        jumpRay = new Ray(transform.position, -transform.up);
        
    }

    private void FixedUpdate()
    {
        Quaternion playerRotation = Quaternion.identity;
        playerRotation.y = playerCam.transform.rotation.y;
        playerRotation.w = playerCam.transform.rotation.w;
        transform.rotation = playerRotation;
    }

    // Update is called once per frame
    void Update()
    {
        jumpRay.origin = transform.position;
        jumpRay.direction = -transform.up;

        Vector3 tempMove = rb.linearVelocity;

        // Make tempMove = speed. Speed = 5
        tempMove.x = (moveInput.x * speed);
        tempMove.z = (moveInput.y * speed);

        rb.linearVelocity = (tempMove.x * transform.right) +
                            (tempMove.y * transform.up) +
                            (tempMove.z * transform.forward); 
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

    public void shoulderSwap()
    {
        cineCam.TargetOffset.x *= -1;
    }
}
