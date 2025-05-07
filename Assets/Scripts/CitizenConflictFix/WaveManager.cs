using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;
    [System.Serializable]
    public class Wave
    {
        public GameObject[] enemyTypes; // Farkl� canavar prefab'lar�
        public int enemyCount; // Toplam canavar say�s�
        public float spawnInterval = 1f; // Spawn aral���
    }

    [Header("Wave Settings")]
    public List<Wave> waves; // T�m dalgalar�n listesi
    public Transform[] spawnPoints; // Canavarlar�n ��kaca�� noktalar(sag ve sol belirli iki nokta)
    public int currentDay = 1;

    private int currentWaveIndex = 0;
    private bool isWaveActive = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    void Start()
    {
        currentWaveIndex = Mathf.Clamp(currentWaveIndex, 0, waves.Count - 1);
        GameManager.OnDayPhaseChanged += HandlePhaseChange;
    }
    private void OnDestroy()
    {
        GameManager.OnDayPhaseChanged -= HandlePhaseChange;
    }

    void HandlePhaseChange(GameManager.DayPhase phase)
    {
        switch (phase)
        {
            case GameManager.DayPhase.Sunrise:
                OnNightEnded();
                break;
            case GameManager.DayPhase.Day:

                break;
            case GameManager.DayPhase.Sunset:

                break;
            case GameManager.DayPhase.Night:
                StartNextWave();
                break;
        }
    }
    public void StartNextWave()
    {
        if (!isWaveActive)
        {
            StartCoroutine(SpawnWave(waves[currentWaveIndex]));
        }
    }

    IEnumerator SpawnWave(Wave wave)
    {
        isWaveActive = true;

        for (int i = 0; i < wave.enemyCount; i++)
        {
            // Rastgele canavar tipi se�
            GameObject enemyPrefab = wave.enemyTypes[Random.Range(0, wave.enemyTypes.Length)];

            // Rastgele spawn noktas� se�
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            print(enemyPrefab == newCoinPool.Instance.enemyPrefab1);
            // Canavar� olu�tur
            if (enemyPrefab == newCoinPool.Instance.enemyPrefab1)
            {
                GameObject enemy = newCoinPool.Instance.GetEnemy1();
                enemy.transform.position = spawnPoint.position;
            }

            // Spawn aral���
            yield return new WaitForSeconds(wave.spawnInterval);
        }

        isWaveActive = false;
    }
    public void OnNightEnded()
    {
        currentDay++;
        currentWaveIndex = Mathf.Min(currentWaveIndex + 1, waves.Count - 1); // Max dalga say�s�n� a�ma
    }
}
