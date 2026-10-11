using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MachineGun : Weapon
{
    // WEAPON ID: 3

    [Header("LMG Exclusive")]
    public bool overheated = false;
    public float overheatMeter = 0;
    public float maxOverheat = 100;
    public float overheatRate = 10;
    public float coolingDownRate = 5;
    public float overheatCooldown = 3;

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        fireDirection = Camera.main;
    }

    // Barrel Overheat System, overheat still carries between reloads making continous fire punishing. 
    // DISABLED, IT KEEPS BREAKING AND I DONT GOT TIME TO FIX THAT DURING BETA PHASE
    /*
    public void Update()
    {
        if(player.currentWeapon)
            if (player.heldWeaponID == 3)
                if (Mouse.current.leftButton.isPressed && !overheated && !isReloading && mag > 0)
                {
                    overheatMeter += overheatRate * Time.deltaTime;
                }
                else
                {
                    if(player.heldWeaponID != 3)
                    { 
                    }

                    if (overheatMeter > 100)
                        overheatMeter = 100;

                    if (overheatMeter < 0)
                        overheatMeter = 0;

                    if (overheatMeter > 0)
                        overheatMeter -= coolingDownRate * Time.deltaTime;
                }

                if (overheatMeter >= 100)
                {
                    canFire = false;
                    overheated = true;
                }

                if (overheatMeter <= 50 && overheated)
                {
                    canFire = true;
                    overheated = false;
                }
    }
    */

    public new void fire()
    {
        if(canFire && !isReloading && mag > 0)
        {
            GameObject p = Instantiate(projectile, firePoint.position, firePoint.rotation);
            p.GetComponent<Rigidbody>().AddForce(fireDirection.transform.forward * projVelocity); // add force relative to camera direction * velocity
            Destroy(p, projLifespan);
            canFire = false;
            mag--;
            StartCoroutine("fireCooldown");
        }
    }

    IEnumerator fireCooldown()
    {
        yield return new WaitForSeconds(rof);

        if(!overheated)
            if (mag > 0)
                canFire = true;
    }
}
