using System.Collections;
using Unity.Cinemachine;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    /* Bugs:
     * Need to rework interactRay because its margainally hard to grab a weapon
     * Player can do a tinier jump after releasing the spacebar, it reads any single input from the spacebar
     */

    public bool isAttacking = false;
    public bool tookHazardDamage = false;
    public bool tookEnemyDamage = false;

    public int health = 100;
    public int maxHealth = 100;
    public float speed = 5.0f;
    public float jumpHeight = 2;
    public float jumpDetectDistance = 1;
    public float interactDistance = 6;
    public float iFrameLength = 1;

    CinemachinePositionComposer cineCam;
    Camera playerCam;
    PlayerInput playerInput;
    Rigidbody rb;

    Ray jumpRay;
    Ray interactRay;
    RaycastHit interactHit;

    // Other Classes
    public Weapon currentWeapon;
    public Transform weaponSlot;
    public GameObject pickupObj;
    public Enemy enemy;

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

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
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

        // Take Damage from a Hazard on collision
            // Hazards for this game will do a flat 10 damage.
        if(collision.gameObject.tag == "Hazard")
        {
            if(!tookHazardDamage)
                health -= 10;
        }

        
        if (collision.gameObject.tag == "Enemy")
        {
            if (!tookEnemyDamage)
                health -= collision.gameObject.GetComponent<Enemy>().damageDealt;
        }
    }


    // DAMAGE SYSTEM

    // If player stays colliding:
    private void OnCollisionStay(Collision collision)  // runs every frame when physics are updated
    {
        if (collision.gameObject.tag == "Hazard" && !tookHazardDamage)
        {
            tookHazardDamage = true;
            StartCoroutine("HazardDamageCooldown");
        }

        if (collision.gameObject.tag == "Enemy" && !tookEnemyDamage)
        {
            tookEnemyDamage = true;
            StartCoroutine("EnemyDamageCooldown");
        }
    }

    IEnumerator HazardDamageCooldown()
    {
        tookHazardDamage = true;

        yield return new WaitForSeconds(iFrameLength);
        health -= 10;
        tookHazardDamage = false;
    }

    IEnumerator EnemyDamageCooldown()
    {
        tookEnemyDamage = true;

        yield return new WaitForSeconds(iFrameLength);
        tookEnemyDamage = false;
    }

    public void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.tag == "Hazard")
        {
            if (tookHazardDamage)
            {
                StopCoroutine("HazardDamageCooldown");
                tookHazardDamage = false;
            }
        }     
    }
}
