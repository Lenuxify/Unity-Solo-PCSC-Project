using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    PlayerController player;

    public GameObject projectile;
    public Transform firePoint;
    public Camera fireDirection;

    [Header("Meta Attributes")]
    public bool canFire = true;
    public bool holdToAttack = true;
    public bool isReloading = false;
    public int weaponID;
    public string weaponName;

    // Proj = Projectile, rof = Rate of Fire
    [Header("Weapon Stats")]
    public float projLifespan;
    public float projVelocity;
    public float reloadCooldown;
    public float rof;
    public int fireModes;
    public int currentFiremode;
    public int mag;
    public int magSize;

    [Header("Ammo Stats")]
    public int ammo;
    public int maxAmmo;
    public int ammoRefill;



    void Start()
    {
        fireDirection = Camera.main;
    }

    public void equip()
    {
        player.currentWeapon = this;

        transform.SetPositionAndRotation(player.weaponSlot.position, player.weaponSlot.rotation);
        transform.SetParent(player.weaponSlot); // weapon slot becomes parent

        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().isTrigger = true;
    }

    public void unequip()
    {
        player.currentWeapon = null;

        transform.SetParent(null);
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Collider>().isTrigger = false;

        this.player = null;
    }

    public void fire()
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

    public void reload()
    {
        if (mag >= magSize)
            return;

        isReloading = true;
        canFire = false;

        int reloadCount = magSize - mag;

        if(ammo < reloadCount)
        {
            mag += ammo;
            ammo = 0;
        }

        else
        {
            mag += reloadCount;
            ammo -= reloadCount;
        }

        StartCoroutine("reloadingCooldown");
    }

    public void recoil()
    {

    }

    public void switchFireMode()
    {

    }

    IEnumerator fireCooldown()
    {
        yield return new WaitForSeconds(rof); // wait what number is rof before executing what comes after this line

        if(mag > 0)
            canFire = true;
    }

    IEnumerator reloadingCooldown()
    {
        yield return new WaitForSeconds(reloadCooldown);

        isReloading = false;
        canFire = true;

    }

    //IEnumerator burstDuration()
    //{
//
    //}
}
