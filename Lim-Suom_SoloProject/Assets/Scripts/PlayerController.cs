using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public bool isAttacking = false;
    public bool tookDamage = false;

    public int health = 100;
    public float speed = 5.0f;
    public float jumpHeight = 2;
    public float jumpDetectDistance = 1;
    public float interactDistance = 6;
    public float dmgCooldown = 3;

    CinemachinePositionComposer cineCam;
    Camera playerCam;
    PlayerInput playerInput;
    Rigidbody rb;

    Ray jumpRay;
    Ray interactRay;
    RaycastHit interactHit;

    // Items
    public Weapon currentWeapon;
    public Transform weaponSlot;
    public GameObject pickupObj;

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
        interactRay = new Ray(playerCam.transform.position, transform.forward);

        weaponSlot = transform.GetChild(0);
        
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
        //if(health <=0)
            // die
        
        // Ray Setups
        jumpRay.origin = transform.position;
        jumpRay.direction = -transform.up;

        interactRay.origin = playerCam.transform.position;
        interactRay.direction = playerCam.transform.forward;

        if (Physics.Raycast(interactRay, out interactHit, interactDistance)) // send output to interactHit
        {
            if (interactHit.collider.tag == "Weapon")
            {
                pickupObj = interactHit.collider.gameObject;
            }
        }
        else
            pickupObj = null;

        // Checks if button is being held down every frame, hence why it is in update.
        if (currentWeapon)
            if (currentWeapon.holdToAttack && isAttacking)
                currentWeapon.fire();

            Vector3 tempMove = rb.linearVelocity;

        // Make tempMove = speed. Speed = 5
        tempMove.x = (moveInput.x * speed);
        tempMove.z = (moveInput.y * speed);

        rb.linearVelocity = (tempMove.x * transform.right) +
                            (tempMove.y * transform.up) +
                            (tempMove.z * transform.forward); 
    }

// INPUT ACTIONS
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

    public void Reload()
    {
        if (currentWeapon)
            if (!currentWeapon.isReloading)
                currentWeapon.reload();
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if(currentWeapon)
        {
            if (currentWeapon.holdToAttack)
            {
                if (context.ReadValueAsButton())
                    isAttacking = true;
                else
                    isAttacking = false;
            }

            else if (context.ReadValueAsButton())
                currentWeapon.fire();
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.ReadValueAsButton())
        {
            if (pickupObj)
            {
                if (pickupObj.tag == "Weapon")
                {
                    pickupObj.GetComponent<Weapon>().equip(this); // gives playercontroller reference to equip func
                }
            }
        }
        else if (currentWeapon) // reload if not looking at obj, works for controller but creates two reload keys for keyboard
            Reload();
    }

    public void DropWeapon()
    {
        if(currentWeapon)
            currentWeapon.unequip();
    }


    // Ammo Refill
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Ammo")
        {

            if(currentWeapon && currentWeapon.ammo < currentWeapon.maxAmmo)
            {
                // most amount of ammo that can be refilled
                int ammoFill = currentWeapon.maxAmmo - currentWeapon.ammo;

                if (ammoFill < currentWeapon.ammoRefill)
                {
                    currentWeapon.ammo += ammoFill;
                }
                else
                {
                    currentWeapon.ammo += currentWeapon.ammoRefill;
                }

                Destroy(collision.gameObject);
            }
        }

        // Take Damage
        if(collision.gameObject.tag == "Hazard")
        {
            health -= 10;
        }
    }


    // HEALTH SYSTEM
    private void OnCollisionStay(Collision collision)  // runs every frame when physics are updated
    {
        if (tookDamage)
            StartCoroutine("damageCooldown");
    }

    IEnumerator damageCooldown()
    {
        tookDamage = true;

        yield return new WaitForSeconds(dmgCooldown);

            health -= 10;

        tookDamage = false;
    }

    public void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.tag == "Hazard")
        {
            if (tookDamage)
            {
                StopCoroutine("damageCooldown");
                tookDamage = false;
            }
        }
    }
}
