using System.Collections;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject[] SpawnableItems;
    [SerializeField] private float spawnRate = 2f;
    [SerializeField] private Vector2 spawnArea = new Vector2(10f, 10f);
    [SerializeField] private float gameTime;
    [SerializeField] private TextMeshProUGUI gameTimer;
    [SerializeField] private PlayerCore player;
    [SerializeField] private LayerMask barrierLayer;
    [SerializeField] private int level;
    
    public CanvasGroup gameOver;
    public CanvasGroup gameEnd;
    public CanvasGroup startFade;
    public CanvasGroup GameUI;

    private float enemySpawnTimer = 2;
    private float itemSpawnTimer = 10;
    private bool isGameOver;
    private bool isPlayerSpawned;

    private void Awake()
    {
        SpawnPlayer();
        GameUI.alpha = 1;
        gameOver.alpha = 0;
        gameOver.interactable = false;
        gameEnd.alpha = 0;
        gameEnd.interactable = false;
        gameEnd.gameObject.SetActive(false);
        StartCoroutine(Fade(startFade, 1, 0, 1f));
        // Boss.IsAlive = false;
        // Boss.BossLevel = 0;
    }

    private void Start()
    {
        gameEnd.alpha = 0;
        gameEnd.interactable = false;
        gameEnd.gameObject.SetActive(false);
        Boss.IsAlive = false;
        isGameOver = false;
    }

    private void Update()
    {
        if (!isPlayerSpawned || player.IsDead) return;

        int minutes = Mathf.FloorToInt(gameTime / 60f);
        int seconds = Mathf.FloorToInt(gameTime % 60f);

        gameTimer.text = $"{minutes:00}:{seconds:00}";

        if (enemySpawnTimer >= spawnRate && !Boss.IsAlive)
        {
            if (SpawnEnemy())
            {
                enemySpawnTimer = 0;
            }
        }
        if (level == 1)
        {
            if (!Boss.IsAlive && gameTime >= 300 * (Boss.BossLevel + 1))
            {
                SpawnBoss();
                Debug.Log("Босс заспавнился, ошибка не тут");
            }
        }

        if (level == 2)
        {
            if (!Boss.IsAlive && gameTime >= 300)
            {
                SpawnBoss();
                SpawnBoss();
                SpawnBoss();
                SpawnBoss();
                level += 1;
            }
        }
        if (itemSpawnTimer >= 15)
        {
            SpawnItem();
            itemSpawnTimer = 0;
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            BackToMenu();
        }

        if (!Boss.IsAlive && !isGameOver)
        {
            gameTime += Time.deltaTime;
            enemySpawnTimer += Time.deltaTime;
            itemSpawnTimer += Time.deltaTime;
        }

        if (level == 1)
        {
            if (!Boss.IsAlive && gameTime >= 901 && !isGameOver)
            {
                EndGame();
            }
        }        
        if (level == 3) { 
            if (!Boss.IsAlive && gameTime >= 301 && !isGameOver && Boss.BossLevel >= 6)
            {
                EndGame();
            }        
        }
    }

    private void SpawnPlayer()
    {
        if (PlayerCore.Instance != null)
        {
            player = PlayerCore.Instance;
            isPlayerSpawned = true;
            return;
        }

        GameObject spawnedPlayer = Instantiate(
            playerPrefab,
            Vector2.zero,
            Quaternion.identity
        );

        player = spawnedPlayer.GetComponent<PlayerCore>();
        isPlayerSpawned = player != null;

        if (isPlayerSpawned)
            Debug.Log("Игрок создан.");
        else
            Debug.LogError("Чёт не так с игроком");
    }

    private bool SpawnEnemy()
    {
        Vector2 spawnPos = new Vector2(
                Random.Range(-spawnArea.x, spawnArea.x),
                Random.Range(-spawnArea.y, spawnArea.y));

        if (!Physics2D.OverlapCircle(spawnPos, 0.5f, barrierLayer))
        {
            Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)], spawnPos, Quaternion.identity);
            return true;
        }

        return false;
    }

    private void SpawnBoss()
    {
        // Вывод оповещения о боссе
        AnnouncementController announcement = gameObject.GetComponent<AnnouncementController>();
        announcement.Announce("Босс появился", Color.red);

        while (true)
        {
            Vector2 spawnPos = new Vector2(
            Random.Range(-spawnArea.x, spawnArea.x),
            Random.Range(-spawnArea.y, spawnArea.y));

            // Поиск норм места для спавна
            if (!Physics2D.OverlapCircle(spawnPos, 0.5f, barrierLayer))
            {
                Instantiate(bossPrefab, spawnPos, Quaternion.identity);
                return;
            }
        }
    }

    private void SpawnItem()
    {
        int itemIndex = Random.Range(0, SpawnableItems.Length);
        Vector2 spawnPos = new Vector2(
                Random.Range(-spawnArea.x, spawnArea.x),
                Random.Range(-spawnArea.y, spawnArea.y));

        if (!Physics2D.OverlapCircle(spawnPos, 0.5f, barrierLayer))
        {
            Instantiate(SpawnableItems[itemIndex], spawnPos, Quaternion.identity);
        }
    }

    public void NextLevel()
    {
        AnnouncementController announcement = gameObject.GetComponent<AnnouncementController>();
        announcement.DisplayInfo("Переход...", 80, Color.gray, 5);
        player.ResetPosition();
        SceneManager.LoadScene("Level2");
    }

    public void GameOver()
    {
        GameUI.alpha = 0;
        isGameOver = true;
        StartCoroutine(Fade(gameOver, 0, 1, 1));
        gameOver.interactable = true;
    }

    public void EndGame()
    {
        Debug.Log("Игра окончена");
        isGameOver = true;
        GameUI.alpha = 0;
        gameOver.gameObject.SetActive(false);
        AnnouncementController announcement = gameObject.GetComponent<AnnouncementController>();
        announcement.Announce("Финальный босс повержен!", Color.green);
        StartCoroutine(Fade(gameEnd, 0, 1, 1));
        gameEnd.gameObject.SetActive(true);
        gameEnd.interactable = true;
    }

    public void BackToMenu()
    {
        PlayerCore.Instance.ResetForNewGame();
        SceneManager.LoadScene("MainMenu");
    }

    public void StartGame()
    {
        gameOver.alpha = 0;
        gameOver.interactable = false;
        PlayerCore.Instance.ResetForNewGame();
        SceneManager.LoadScene("MainScene");
    }

    public IEnumerator Fade(CanvasGroup fade, float from, float to, float duration)
    {
        fade.alpha = from;
        
        float elapsed = 0f;
        while (elapsed < duration)
        {
            fade.alpha = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        fade.alpha = to;
        startFade.interactable = false;
    }

    public IEnumerator Test()
    { 
        yield return new WaitForSeconds(0.1f);
        Boss.IsAlive = false;
        Boss.BossLevel = 0;
        yield return null;
    }
}
