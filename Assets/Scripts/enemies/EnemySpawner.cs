using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
/// <summary>
/// Contains the logic for spawning enemy waves at the start of the path
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private GameObject[] waveBossPrefabs;
    [SerializeField] private Button startButton;
    [SerializeField] private GameObject infoScreen;


    [Header("Attributes")]
    [SerializeField] private int baseEnemies = 8;
    [SerializeField] private float bossBaseEnemies = 0.5f;
    [SerializeField] private float enemiesPerSecond = 0.5f;
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private float difficultyScalingFactor = 0.75f;
    [SerializeField] private float bossDifficultyScalingFactor = 1.5f; // greater value means more enemies
    [SerializeField] private float enemiesPerSecondCap = 15f;

    [Header("Audio Elements")]
    [SerializeField] private AudioClip clickSound;
    private AudioSource audioSource;

    [Header("Events")]
    private static UnityEvent onEnemyDestroy = new UnityEvent();

    private int currentWave = 1;
    private float timeSinceLastSpawn;
    private int enemiesAlive;
    private int bossesAlive;
    private int enemiesLeftToSpawn;
    private int bossesLeftToSpawn;
    private float actualEnemiesPerSecond;
    private bool isSpawning = false;
    private bool checkForEnd = false;

    private void Awake()
    {
        onEnemyDestroy.AddListener(EnemyDestroyed);
        startButton.onClick.AddListener(() => StartGame());
    }

    public void Start()
    {
        InitAudio();
        // coroutine for time based waves 
        StartCoroutine(StartWave());
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Starts the game after the player has read the information pop-up
    /// </summary>
    public void StartGame()
    {
        PlayClickSound();
        Time.timeScale = 1f;
        infoScreen.SetActive(false);
        UIManager.Instance.SetHoveringState(false);
    }

    /// <summary>
    /// Initializes all audio components
    /// </summary>
    private void InitAudio()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.clip = clickSound;
    }

    private void Update()
    {
        if (isSpawning && !checkForEnd)
        {

            timeSinceLastSpawn += Time.deltaTime;

            if (timeSinceLastSpawn >= (1f / actualEnemiesPerSecond) && enemiesLeftToSpawn > 0)
            {
                SpawnEnemy();
                enemiesLeftToSpawn--;
                enemiesAlive++;
                timeSinceLastSpawn = 0f;
            }

            // start new wave if no enemies are left
            if (enemiesAlive == 0 && enemiesLeftToSpawn == 0 && !GameManager.Instance.IsFinished())
            {
                checkForEnd = true;
                EndWave();
            }

        }
    }

    /// <summary>
    ///     Destroys an enemy, i.e., decreases the number of enemies alive
    /// </summary>
    private void EnemyDestroyed()
    {
        enemiesAlive--;
    }

    /// <summary>
    ///     Spwans a random enemy at the beginning of the path
    /// </summary>
    private void SpawnEnemy()
    {
        int index = Random.Range(0, enemyPrefabs.Length);

        // spawn kitties only after wave 3
        if (index == enemyPrefabs.Length - 1 && currentWave < 4)
        {
            index = 0;
        }

        GameObject prefabToSpawn = enemyPrefabs[index];
        Instantiate(prefabToSpawn, LevelManager.Instance.GetStartPoint().position, Quaternion.identity );
        
    }

    /// <summary>
    ///     Spwans a random boss enemy at the beginning of the path
    /// </summary>
    private void SpawnBoss()
    {
        int index = Random.Range(0, waveBossPrefabs.Length);
        GameObject prefabToSpawn = waveBossPrefabs[index];
        Instantiate(prefabToSpawn, LevelManager.Instance.GetStartPoint().position, Quaternion.identity);       
    }

    /// <summary>
    /// Coroutine, die alle Bosse der Welle spawnt.
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnBosses()
    {
        while (bossesLeftToSpawn > 0)
        {
            SpawnBoss();
            bossesLeftToSpawn--;
                    
            yield return new WaitForSeconds(2f);
        }
    }

    /// <summary>
    ///     Handles phase between subsequent waves
    /// </summary>
    /// <returns>Iterator for coroutine</returns>
    private IEnumerator StartWave()
    {
        yield return new WaitForSeconds(timeBetweenWaves);
        isSpawning = true;
        enemiesLeftToSpawn = baseEnemies;
        actualEnemiesPerSecond = EnemiesPerSecond();
        if (currentWave % 2 == 0)
        {
            bossesLeftToSpawn = BossesPerWave();
        }
        else
        {
            bossesLeftToSpawn = 0; 
        }
    }

    /// <summary>
    ///     Ends a wave, thus starting the intermission coroutine.
    /// </summary>
    private async void EndWave()
    {
        isSpawning = false;
        timeSinceLastSpawn = 0f;
        Debug.Log(currentWave);
        if (currentWave % 5 == 0 && bossesLeftToSpawn > 0)
        {
            Debug.Log("spawned boss");
           
                StartCoroutine(SpawnBosses());
            
        }

        currentWave++;
        StartCoroutine(StartWave());
        bool isFinished = await QuestionManager.Instance.CheckForEnd();

        if (isFinished)
        {
            GameManager.Instance.LoadEndScreen();
        }
        else
        {
            isSpawning = false;
            timeSinceLastSpawn = 0f;
            
            StartCoroutine(StartWave());
            checkForEnd = false;
        }
    }

    /// <summary>
    ///     Calculates the number of enemies per wave depending on the difficulty factor 
    /// </summary>
    /// <returns>number of enemies per wave</returns>
    private int EnemiesPerWave()
    {
        return Mathf.RoundToInt(baseEnemies * Mathf.Pow(currentWave, difficultyScalingFactor));
    }

    /// <summary>
    ///     Calculates the number of bosses per wave depending on the difficulty factor 
    /// </summary>
    /// <returns>number of enemies per wave</returns>
    private int BossesPerWave()
    {
        return Mathf.Max(1, Mathf.RoundToInt(currentWave * bossBaseEnemies - 1));
    }

    /// <summary>
    ///     Calculates the the actual enemy per seconds rate depending on the difficulty factor
    /// </summary>
    /// <returns>actual enemy per second rate</returns>
    private float EnemiesPerSecond()
    {
        return Mathf.Clamp(enemiesPerSecond * Mathf.Pow(currentWave,
            difficultyScalingFactor), 0f, enemiesPerSecondCap);
    }

    /// <summary>
    ///     Gets the unity event that describes what happens if an enemy gets destroyed
    /// </summary>
    /// <returns>on enemy destroyed event </returns>
    public static UnityEvent GetOnEnemyDestroy()
    {
        return onEnemyDestroy;
    }

    /// <summary>
    /// This function plays the click sound
    /// </summary>
    private void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}
