using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class HUDController : MonoBehaviour
{
    public static HUDController instance; 

    private void Awake()
    {
        instance = this;
    }

    [Header("UI Settings")]
    [SerializeField] TMP_Text interactionText;
    [SerializeField] TMP_Text progressionBar;
    [SerializeField] Slider PlayerHealthBar;
    [SerializeField] TMP_Text MoneyText;
    [SerializeField] TMP_Text WaveNumberText;
    [SerializeField] TMP_Text RemainingZombiesText;
    [SerializeField] TMP_Text MoneyMultiplierText;
    [SerializeField] TMP_Text ZombieStatMultiplierText;
    [SerializeField] Slider BaseHealthBar;
    [SerializeField] TMP_Text WeaponText;

    private PlayerHealth playerHealth;
    private PlayerEconomy playerEconomy;
    private BaseHealth baseHealth;
    private GameManager gameManager;
    private WeaponManager weaponManager;

    public float waveTimer;
    private bool isReloading;
    private float remainingTime; // Temps restant pour la prochaine vague
    private bool isWaveTimerActive = false;

    private void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
        playerEconomy = FindObjectOfType<PlayerEconomy>();
        baseHealth = FindObjectOfType<BaseHealth>();
        gameManager = FindObjectOfType<GameManager>();
        weaponManager = FindObjectOfType<WeaponManager>();
    }

    private void Update(){
        UpdatePlayerInfo();
        UpdateBaseInfo();
        UpdateGameInfo();
        UpdateWeaponInfo();
    }

    private void UpdatePlayerInfo()
    {
        PlayerHealthBar.value = playerHealth.GetCurrentHealth();
        MoneyText.text = "Argent: " + playerEconomy.GetMoney();
    }

    private void UpdateBaseInfo()
    {
        BaseHealthBar.value = baseHealth.GetCurrentHealth();
        MoneyMultiplierText.text = "Drop: x" + gameManager.GetZombieStatMultiplier();
        ZombieStatMultiplierText.text = "Stats des zombies: x" + gameManager.GetZombieStatMultiplier();
    }

    private void UpdateGameInfo()
    {
        WaveNumberText.text = "Vague n°" + gameManager.GetWaveCounter();

        if(gameManager.GetIsWaveActive())
        {
            // Afficher le nombre de zombie restant
            RemainingZombiesText.text = "Zombies restants: " + gameManager.GetZombiesRemaining() + "/" + gameManager.GetZombiesPerWave();
        } 
        else
        {
            if (!isWaveTimerActive)
            {
                StartWaveTimer(gameManager.GetTimeBetweenWaves());
            }
        }
    }

    private void UpdateWeaponInfo()
    {
        if(weaponManager.GetIsReloading())
        {
            WeaponText.text = "Rechargement...";
        }
        else
        {
            WeaponText.text = weaponManager.GetCurrentAmmo() + "/" + weaponManager.GetMagazineSize();
        }
    }

    public void StartWaveTimer(float waveDuration)
    {
        isWaveTimerActive = true;
        remainingTime = waveDuration;
        StartCoroutine(WaveCountdown());
    }

    private IEnumerator WaveCountdown()
    {
        while (remainingTime > 0)
        {
            if (remainingTime > 60)
            {
                int minutes = Mathf.FloorToInt(remainingTime / 60);
                int seconds = Mathf.FloorToInt(remainingTime % 60);
                RemainingZombiesText.text = "Prochaine vague dans: " + minutes + "min" + seconds + "s";
            }
            else
            {
                RemainingZombiesText.text = "Prochaine vague dans: " + Mathf.CeilToInt(remainingTime) + "s";
            }

            remainingTime -= 1f; // Réduire le temps restant
            yield return new WaitForSeconds(1f); // Attendre 1 seconde
        }

        RemainingZombiesText.text = "Vague active !"; // Affichage quand la vague commence
        isWaveTimerActive = false;
    }

    public void EnableInteractiontext(string text)
    {
        interactionText.text = text + " (F)"; 
        interactionText.gameObject.SetActive(true);
    } 
    public void DisableInteractiontext()
    {
        interactionText.gameObject.SetActive(false);
    } 
}
