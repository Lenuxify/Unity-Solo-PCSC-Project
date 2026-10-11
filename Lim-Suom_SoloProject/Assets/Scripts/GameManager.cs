using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public PlayerController player;
    public Weapon weapon;
    public MachineGun machineGun;

    public GameObject pauseMenu;
    public GameObject deathScreen;

    public TextMeshProUGUI weaponName;
    public TextMeshProUGUI magText;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI bandageText;

    public Image healthBar;
    public Image staminaBar;

    public int enemyCount = 0;
    public int currentLevel;

    public bool paused = false;

    void Start()
    {
        Time.timeScale = 1;

        // initalize everything after the current scene is NOT the main menu
        if(SceneManager.GetActiveScene().buildIndex != 0)
        {
            player = GameObject.Find("Player").GetComponent<PlayerController>();
            currentLevel = SceneManager.GetActiveScene().buildIndex;

            pauseMenu = GameObject.FindGameObjectWithTag("Pause");
            deathScreen = GameObject.FindGameObjectWithTag("Death");

            weaponName = GameObject.Find("WeaponName").GetComponent<TextMeshProUGUI>();
            magText = GameObject.Find("MagText").GetComponent<TextMeshProUGUI>();
            ammoText = GameObject.Find("AmmoText").GetComponent<TextMeshProUGUI>();
            bandageText = GameObject.Find("BandageText").GetComponent<TextMeshProUGUI>();

            healthBar = GameObject.Find("CurrentHealth").GetComponent<Image>();
            staminaBar = GameObject.Find("CurrentStamina").GetComponent<Image>();

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            pauseMenu.SetActive(false);
            deathScreen.SetActive(false);

            enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        }
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            if (player.isDead == true)
            {
                deathScreen.SetActive(true);
                paused = true;
            }

            if (paused)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                Time.timeScale = 0;

                if(player.isDead == false)
                    pauseMenu.SetActive(true);
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;

                Time.timeScale = 1;

                pauseMenu.SetActive(false);
            }


            healthBar.fillAmount = (float)player.health / (float)player.maxHealth;
            staminaBar.fillAmount = (float)player.stamina / (float)player.maxStamina; 

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

            if (player.heldBandages > 0)
                bandageText.text = "Bandages: " + player.heldBandages;
            else
                bandageText.text = "";
        }
    }

    public void pause()
    {
        paused = !paused;

        Cursor.visible = paused;

        if(paused)
        {
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
        }

        pauseMenu.SetActive(paused);
    }

    public void LoadLevel(int levelID)
    {
        if (levelID >= SceneManager.sceneCountInBuildSettings)
            Debug.Log("Scene ID too high: " + levelID);
        else
            SceneManager.LoadScene(levelID);
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(currentLevel);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
