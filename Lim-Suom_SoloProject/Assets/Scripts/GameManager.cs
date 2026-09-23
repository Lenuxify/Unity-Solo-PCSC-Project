using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public PlayerController player;
    public TextMeshProUGUI weaponName;
    public TextMeshProUGUI magText;
    public TextMeshProUGUI ammoText;

    public Image healthBar;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        weaponName = GameObject.Find("WeaponName").GetComponent<TextMeshProUGUI>();
        magText = GameObject.Find("MagText").GetComponent<TextMeshProUGUI>();
        ammoText = GameObject.Find("AmmoText").GetComponent<TextMeshProUGUI>();

        healthBar = GameObject.Find("HealthBar").GetComponent<Image>();
    }

    void Update()
    {
        healthBar.fillAmount = (float)player.health / (float)player.maxHealth;

        if (player.currentWeapon)
        {
            weaponName.text = player.currentWeapon.name;
            magText.text = "Mag: " + player.currentWeapon.mag + "/" + player.currentWeapon.magSize;
            ammoText.text = "Ammo: " + player.currentWeapon.ammo + "/" + player.currentWeapon.maxAmmo;

        }
        else
        {
            weaponName.text = "";
            magText.text = "";
            ammoText.text = "";
        }
    }
}
