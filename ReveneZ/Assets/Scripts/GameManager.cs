using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Wave Settings")]
    public GameObject zombiePrefab; // Préfabriqué de zombie
    public Transform player; // Référence au joueur
    public int zombiesPerWave = 20; // Nombre de zombies par manche
    private int zombiesRemaining; // Compteur de zombies restants dans la manche
    public float timeBetweenWaves = 120f; // Temps entre deux manches
    private bool isWaveActive = false; // Indique si une manche est en cours
    private bool isWaitingForNextWave = false; // Protection contre le lancement multiple de la coroutine
    private int fearIncreasePerWave = 10; // Augmentation de la peur globale par vague

    [Header("Spawn Points")]
    public Vector3[] spawnPoints; // Coordonnées de spawn des zombies

    private int waveCounter = 0; // Compteur de manches
    private float moneyMultiplier = 0.9f; // Multiplicateur de récompense
    private float zombieStatMultiplier = 0.9f; // Multiplicateur de stats zombies

    private List<Vector3> shuffledSpawnPoints = new List<Vector3>();

    private void Start()
    {
        StartNewWave();
    }

    private void Update()
    {
        // Vérifie si tous les zombies sont éliminés et prépare la manche suivante
        if (zombiesRemaining <= 0 && isWaveActive && !isWaitingForNextWave)
        {
            isWaveActive = false; // Fin de la vague
            StartCoroutine(StartNextWaveAfterDelay());
        }
    }

    /// <summary>
    /// Démarre une nouvelle manche.
    /// </summary>
    private void StartNewWave()
    {
        waveCounter++;
        Debug.Log($"Starting Wave {waveCounter}");

        isWaveActive = true;
        isWaitingForNextWave = false;
        zombiesRemaining = zombiesPerWave;

        // Soigner le joueur au début de chaque manche
        player.GetComponent<PlayerHealth>().Heal();

        // Augmenter les multiplicateurs et la peur globale
        moneyMultiplier += 0.1f;
        zombieStatMultiplier += 0.1f;
        Zombie.fear += fearIncreasePerWave;

        // Mélanger les points de spawn
        ShuffleSpawnPoints();

        // Répartir et faire apparaître les zombies
        SpawnZombies();
    }

    /// <summary>
    /// Mélange les points de spawn pour obtenir une répartition aléatoire.
    /// </summary>
    private void ShuffleSpawnPoints()
    {
        shuffledSpawnPoints = new List<Vector3>(spawnPoints);
        for (int i = 0; i < shuffledSpawnPoints.Count; i++)
        {
            int randomIndex = Random.Range(i, shuffledSpawnPoints.Count);
            Vector3 temp = shuffledSpawnPoints[i];
            shuffledSpawnPoints[i] = shuffledSpawnPoints[randomIndex];
            shuffledSpawnPoints[randomIndex] = temp;
        }
    }

    /// <summary>
    /// Répartit et fait apparaître les zombies sur les points de spawn.
    /// </summary>
    private void SpawnZombies()
    {
        int[] zombiesAtSpawnPoint = new int[spawnPoints.Length];
        int maxZombiesPerSpawn = Mathf.CeilToInt((float)zombiesPerWave / spawnPoints.Length);

        for (int i = 0; i < zombiesPerWave; i++)
        {
            int randomSpawnIndex = Random.Range(0, spawnPoints.Length);

            // Vérifier qu'on ne dépasse pas la capacité max par point de spawn
            if (zombiesAtSpawnPoint[randomSpawnIndex] < maxZombiesPerSpawn)
            {
                zombiesAtSpawnPoint[randomSpawnIndex]++;
                SpawnZombieAtSpawnPoint(shuffledSpawnPoints[randomSpawnIndex]);
            }
            else
            {
                // Si ce point est plein, on en choisit un autre
                i--;
            }
        }
    }

    private void SpawnZombieAtSpawnPoint(Vector3 spawnPosition)
    {
        Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);
    }

    private IEnumerator StartNextWaveAfterDelay()
    {
        isWaitingForNextWave = true;
        yield return new WaitForSeconds(timeBetweenWaves);
        StartNewWave();
    }

    public void OnZombieKilled()
    {
        zombiesRemaining--;
    }
}