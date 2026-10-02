using JetBrains.Annotations;
using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    /* Bugs:
     * Need to rework interactRay because its margainally hard to grab a weapon
     * Player does a secondary jump after releasing spacebar late
     */

    [Header("Damage Relationships")]
    public bool isAttacking = false;
    public bool canTakeEnemyDamage = true;
    public bool tookHazardDamage = false;
    public bool tookEnemyDamage = false;

    // track last damage taken from enemy damage. crucial for onCollisionStay damage.
    public int enemyDamage;

    // contactTime is used to check how long something has collided with the player.
    public float hazardContactTime;
    public float enemyContactTime;
    public float damageTime = 1;

    [Header("Player Stats")]
    public int health = 100;
    public int maxHealth = 100;
    public float speed = 5.0f;
    public float stamina = 100f;
    public float maxStamina = 100f;
    public float jumpHeight = 2;

    [Header("Sprint Handling")]
    public bool toggleSprint = true;
    public bool canSprint = true;
    public bool isSprinting = false;
    public bool sprintStop = false;
    public bool staminaStop = false;
    public bool regenStamina = false;
    public float sprintBoost = 1.5f;
    public float sprintCooldown = 2;
    public float staminaRegen = 10;
    public float staminaCooldown = 2;
    public float staminaCost = 20f;

    [Header("Weapon Handling")]
    public int heldWeaponID = -1;

    [Header("Other")]
    // Sprint handling
    public float jumpDetectDistance = 1;
    public float interactDistance = 6;
    public float iFrameLength = 1;
    public float fov;

    CinemachineCamera cineCam;
    CinemachinePositionComposer cineCamComposer;
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
    public GameManager gameManager;
    private Enemy enemy;

    Vector2 moveInput;

    void Start()
    {
        // Fetch General Components into an variable
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();


        // Initialize Camera
        playerCam = Camera.main;
        cineCamComposer = GameObject.Find("CinemachineCamera").GetComponent<CinemachinePositionComposer>();

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


        // SPRINTING
        if(isSprinting)
        {
            if(moveInput.y == 1 && stamina > 0)
            {
                {
                    tempMove.z *= sprintBoost;
                    stamina -= staminaCost * Time.deltaTime;

                    if (stamina < 0)
                        stamina = 0;

                    StopCoroutine("StaminaReset");
                }
            }
            else
            {
                canSprint = false;
                isSprinting = false;
                StartCoroutine("StaminaReset");
            }
        }

        if(!isSprinting)
        {
            if(!regenStamina && !staminaStop && stamina < maxStamina)
            {
                StartCoroutine("StaminaReset");
            }
            if(!canSprint && !sprintStop)
            {
                StartCoroutine("SprintReset");
            }

            // Regen Stamina
            if(regenStamina)
            {
                stamina += staminaRegen * Time.deltaTime;

                // Prevent stamina overfill
                if(stamina >= maxStamina)
                {
                    stamina = maxStamina;
                    regenStamina = false;
                }
            }
        }

        // Weapon Slow Debuff. Flat slow down of 20% universally for all heavy weapons. (or until i make a minigun or smth idek)
        if(currentWeapon)
            if(currentWeapon.heavy == true)
            {
                tempMove.z *= 0.8f;
            }

        rb.linearVelocity = (tempMove.x * transform.right) +
                            (tempMove.y * transform.up) +
                            (tempMove.z * transform.forward); 
    }

// INPUT ACTIONS
    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

    }

    public void Sprint(InputAction.CallbackContext context)
    {
        if(canSprint)
        {
            if (toggleSprint)
            {
                isSprinting = !isSprinting;
            }
            else if (!toggleSprint)
            {
                isSprinting = context.ReadValueAsButton();

                if (!isSprinting)
                    canSprint = false;
            }
        }
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
        cineCamComposer.TargetOffset.x *= -1;
    }

    public void Reload()
    {
        if (currentWeapon)
            if (!currentWeapon.isReloading)
                currentWeapon.reload();
    }

    public void changeFireMode()
    {
        if(currentWeapon)
        {
            if(currentWeapon.fireModes >= 2)
            {
                if(currentWeapon.weaponID ==  1)
                {
                    // Call changeFireMode function from Rifle.
                    currentWeapon.GetComponent<Rifle>().changeFireMode();
                }
            }
        }
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
                    heldWeaponID = currentWeapon.weaponID;
                    Debug.Log("Weapon ID: " +  heldWeaponID);
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

        heldWeaponID = -1;
    }

    public void Aim(InputAction.CallbackContext context)
    {
        if (currentWeapon)
            if (currentWeapon.Aimable)
                cineCam.Lens.FieldOfView -= 30;
    }


    private void OnCollisionEnter(Collision collision)
    {
        // Ammo Refill
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
            if (!tookHazardDamage)
                TakeDamage(10);
        }

        // Get base enemy class script then put damageDealt into enemyDamage variable. Then, use the enemyDamage as the damage to take in TakeDamage func
        if (collision.transform.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemyDamage = enemy.damageDealt;
            TakeDamage(enemyDamage);
        }

        if(collision.gameObject.tag == "LevelEnd")
        {
            gameManager.LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }


    // DAMAGE SYSTEM

    public void TakeDamage(int damage)
    {
        health -= damage;
    }


    // If player stays colliding:
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Hazard" && !tookHazardDamage)
        {
            hazardContactTime += Time.deltaTime;

            if (hazardContactTime > damageTime)
                StartCoroutine("HazardDamageCooldown");
        }

        if (collision.gameObject.tag == "Enemy" && !tookEnemyDamage)
        {
            enemyContactTime += Time.deltaTime;

            if (enemyContactTime > damageTime)
                StartCoroutine("EnemyDamageCooldown");
        }
    }

    public void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Hazard")
        {
            /*
            if (tookHazardDamage)
            {
                StopCoroutine("HazardDamageCooldown");
                tookHazardDamage = false;
            }
            */
            hazardContactTime = 0;
        }
        if (collision.gameObject.tag == "Enemy")
        {
            enemyContactTime = 0;
        }
    }


    // IEnumerators
    IEnumerator HazardDamageCooldown()
    {
        tookHazardDamage = true;

        yield return new WaitForSeconds(iFrameLength);
        health -= 10;
        hazardContactTime = 0;
        tookHazardDamage = false;
    }

    IEnumerator EnemyDamageCooldown()
    {
        tookEnemyDamage = true;

        yield return new WaitForSeconds(iFrameLength);
        TakeDamage(enemyDamage);
        enemyContactTime = 0;
        tookEnemyDamage = false;
    }

    IEnumerator SprintReset()
    {
        sprintStop = true;

        yield return new WaitForSeconds(sprintCooldown);

        canSprint = true;
        sprintStop = false;
    }

    IEnumerator StaminaReset()
    {
        staminaStop = true;

        yield return new WaitForSeconds(staminaCooldown);

        regenStamina = true;
        staminaStop = false;
    }
}
