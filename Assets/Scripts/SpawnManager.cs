using UnityEngine;
using System.Collections;
using TMPro;

public class SpawnManager : MonoBehaviour
{

    public GameObject enemyPrefab;
    public float spawnInterval = 2f;
    public int maxEnemies = 10;


    private float minX = -5f;
    private float maxX = 5f;
    private float minZ = -5f;
    private float maxZ = 5f;
    private float spawnY = 0f;


    public TMP_Text enemyCounterText;

    private int currentEnemies = 0;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        StartCoroutine(SpawnEnemies());
        UpdateCounterUI();
    }

    void Update()
    {
        // Detección de clic del mouse
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    Destroy(hit.collider.gameObject);
                }
            }
        }
    }

    IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(1f);

        while (true)
        {
            if (currentEnemies < maxEnemies)
            {
                SpawnEnemy();
                UpdateCounterUI();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnEnemy()
    {
        float randomX = Random.Range(minX, maxX);
        float randomZ = Random.Range(minZ, maxZ);
        Vector3 spawnPosition = new Vector3(randomX, spawnY, randomZ);

        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        enemy.tag = "Enemy";

        // Asignar color aleatorio
        Renderer renderer = enemy.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = new Color(
                Random.value,
                Random.value,
                Random.value,
                1f
            );
        }

        currentEnemies++;

        EnemyDestructionNotifier notifier = enemy.AddComponent<EnemyDestructionNotifier>();
        notifier.OnDestroyed += () =>
        {
            currentEnemies--;
            UpdateCounterUI();
        };
    }

    void UpdateCounterUI()
    {
        if (enemyCounterText != null)
        {
            enemyCounterText.text = $"X: {currentEnemies}";
        }
    }

}

public class EnemyDestructionNotifier : MonoBehaviour
{
    public delegate void EnemyDestroyed();
    public event EnemyDestroyed OnDestroyed;

    void OnDestroy()
    {
        OnDestroyed?.Invoke();
    }
}