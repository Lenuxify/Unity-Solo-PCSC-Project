using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public PlayerController player;

    public GameObject pauseMenu;

    public TextMeshProUGUI weaponName;
    public TextMeshProUGUI magText;
    public TextMeshProUGUI ammoText;

    public Image healthBar;

    public int enemyCount = 0;

    public bool paused = false;

    void Start()
    {
        Time.timeScale = 1;

        // initalize everything after the current scene is NOT the main menu
        if(SceneManager.GetActiveScene().buildIndex != 0)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

            pauseMenu = GameObject.FindGameObjectWithTag("Pause");

            weaponName = GameObject.Find("WeaponName").GetComponent<TextMeshProUGUI>();
            magText = GameObject.Find("MagText").GetComponent<TextMeshProUGUI>();
            ammoText = GameObject.Find("AmmoText").GetComponent<TextMeshProUGUI>();

            healthBar = GameObject.Find("CurrentHealth").GetComponent<Image>();

            pauseMenu.SetActive(false);

            enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        }
    }

    void Update()
    {
        /*
        if(paused)
        {
            // free le cursor
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            Time.timeScale = 0;

            pauseMenu.SetActive(true);
        }
        else
        {
            // prison le cursor
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            Time.timeScale = 1;

            pauseMenu.SetActive(false);
        }
        */

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
        if (levelID > SceneManager.sceneCount)
            Debug.Log("Scene ID too high: " + levelID);
        else
            SceneManager.LoadScene(levelID);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
