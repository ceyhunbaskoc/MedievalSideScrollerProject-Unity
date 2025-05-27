using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public enum DayPhase { Sunrise, Day, Sunset, Night }

    [Header("Day-Night Cycle")]
    public DayPhase currentPhase;
    public float timeSpeed = 1f;
    public float currentTime = 6f; // Varsayýlan: Sabah 6:00

    [Header("Stock Manager")]
    public int hammerStock;
    public int bowStock;

    [Header("NPC Spawn")]
    public GameObject npcPrefab;

    [Header("Rabbit Spawn")]
    public int rabbitCount; // Inspector’dan ayarlanabilir
    public float minX;
    public float maxX;
    public float spawnY; // Tavþanlarýn y pozisyonu
    public float minDistance = 2f; // Tavþanlar arasý minimum mesafe

    public List<GameObject> spawnedRabbits = new List<GameObject>();

    public static GameManager Instance;
    public static event Action<DayPhase> OnDayPhaseChanged;

    public bool tutorialOpen=true,dayShowed=false;
    public bool SpawnedThisMorning = false;
    private GameObject parallaxbground;
    characterConflict cr;
    public TextMeshProUGUI WhichDay;
    public TextMeshProUGUI coinCount;
    public float alphaFadeDuration;
    public GameObject characterNightTorch;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        parallaxbground = GameObject.Find("Parallax");
        parallaxbground.transform.position = new Vector2(GameObject.FindFirstObjectByType<characterConflict>().transform.position.x,parallaxbground.transform.position.y);
    }
    private void Start()
    {
        cr= FindFirstObjectByType<characterConflict>();
        tutorialOpen = true;
        StartCoroutine(WaitForNPCSpawnPoints());
    }
    IEnumerator WaitForNPCSpawnPoints()
    {
        yield return new WaitForSeconds(1f);
        CitizensManager.Instance.InitializeNPcSpawnPoints=true;
    }
    void Update()
    {
        coinCount.text=cr.moneyCount.ToString();
        if (!tutorialOpen)
            UpdateTime();
        
        if (GameManager.Instance.currentPhase == DayPhase.Sunrise && !SpawnedThisMorning)
        {
            CheckAndSpawnRabbits();
            SpawnNPCs();
            characterNightTorch.SetActive(false);
            dayShowed = false;
            SpawnedThisMorning = true;

        }
        if (GameManager.Instance.currentPhase != DayPhase.Sunrise)
        {
            SpawnedThisMorning = false;
        }
        if (!dayShowed&&!tutorialOpen)
        {
            NewDayShower();
        }
        if(currentPhase == DayPhase.Night)
        {
            characterNightTorch.SetActive(true);
        }


    }
    void NewDayShower()
    {
        if (WaveManager.Instance.currentDay == 1)
            WhichDay.text = "I";
        if (WaveManager.Instance.currentDay == 2)
            WhichDay.text = "II";
        if (WaveManager.Instance.currentDay == 3)
            WhichDay.text = "III";
        if (WaveManager.Instance.currentDay == 4)
            WhichDay.text = "IV";
        if (WaveManager.Instance.currentDay == 5)
            WhichDay.text = "V";
        if (WaveManager.Instance.currentDay == 6)
            WhichDay.text = "VI";
        if (WaveManager.Instance.currentDay == 7)
            WhichDay.text = "VII";
        if (WaveManager.Instance.currentDay == 8)
            WhichDay.text = "VIII";
        if (WaveManager.Instance.currentDay == 9)
            WhichDay.text = "IX";
        if (WaveManager.Instance.currentDay == 10)
            WhichDay.text = "X";
        if (WaveManager.Instance.currentDay == 11)
            WhichDay.text = "XI";
        if (WaveManager.Instance.currentDay == 12)
            WhichDay.text = "XII";
        if (WaveManager.Instance.currentDay == 13)
            WhichDay.text = "XIII";


        // Fade animasyonunu baþlat
        StartCoroutine(ShowDayRoutine());
    }
    IEnumerator FadeText(CanvasGroup parent, float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            parent.alpha = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        parent.alpha = to;
    }
    IEnumerator ShowDayRoutine()
    {
        // Fade-in
        yield return StartCoroutine(FadeText(WhichDay.transform.parent.GetComponent<CanvasGroup>(), 0f, 1f, alphaFadeDuration));
        // Bir süre ekranda kalsýn
        yield return new WaitForSeconds(2f);
        // Fade-out
        yield return StartCoroutine(FadeText(WhichDay.transform.parent.GetComponent<CanvasGroup>(), 1f, 0f, alphaFadeDuration));
        dayShowed = true;
    }


    public void CheckAndSpawnRabbits()
    {
        // 1. Sahnedeki aktif tavþanlarý temizle (ölmüþ veya silinmiþ olanlarý çýkar)
        spawnedRabbits.RemoveAll(r => r == null);

        int currentRabbitCount = spawnedRabbits.Count;
        int rabbitsToSpawn = rabbitCount - currentRabbitCount;

        if (rabbitsToSpawn <= 0)
        {
            // Yeterli veya fazla tavþan var, yeni spawn gerek yok
            return;
        }

        List<float> usedPositions = new List<float>();
        // Mevcut tavþanlarýn pozisyonlarýný da ekle (isteðe baðlý, çakýþmayý azaltmak için)
        foreach (var rabbit in spawnedRabbits)
            usedPositions.Add(rabbit.transform.position.x);

        int spawned = 0;
        int maxTries = 1000;

        while (spawned < rabbitsToSpawn && maxTries-- > 0)
        {
            float x = UnityEngine.Random.Range(CityCenter.Instance.baseBuildRange-100, CityCenter.Instance.baseBuildRange + 70);
            bool allowClose = UnityEngine.Random.value < 0.2f;
            bool tooClose = false;
            foreach (float usedX in usedPositions)
            {
                if (Mathf.Abs(x - usedX) < minDistance)
                {
                    tooClose = true;
                    break;
                }
            }
            if (tooClose && !allowClose)
                continue;

            Vector3 spawnPos = new Vector3(x, spawnY, 0f);
            GameObject rabbit = newCoinPool.Instance.GetRabbit();
            rabbit.transform.position = spawnPos;
            spawnedRabbits.Add(rabbit);
            usedPositions.Add(x);
            spawned++;
        }
    }
    void SpawnNPCs()
    {
        foreach (var spawnPoint in CitizensManager.Instance.NPCSpawnPoints)
        {
            bool npcExists = false;
            // Sadece baþlangýç pozisyonlarýný kontrol et
            foreach (var pos in CitizensManager.Instance.None)
            {
                if (Vector2.Distance(pos.transform.position, spawnPoint) < 0.1f)
                {
                    print("NPC exist true");
                    npcExists = true;
                    break;
                }
            }

            if (!npcExists)
            {
                GameObject npc = newCoinPool.Instance.GetNPC();
                npc.SetActive(true);
                npc.transform.position = spawnPoint;

                if (npc != null)
                {
                    npc.transform.position = spawnPoint;
                }
            }
        }
    }

    void UpdateTime()
    {
        currentTime += Time.deltaTime * timeSpeed;

        // 24 saatlik döngü
        if (currentTime >= 24f)
            currentTime -= 24f;

        CheckPhaseChange();
    }

    void CheckPhaseChange()
    {
        DayPhase newPhase = DetermineCurrentPhase(currentTime);

        if (newPhase != currentPhase)
        {
            currentPhase = newPhase;
            OnDayPhaseChanged?.Invoke(currentPhase);
        }
    }

    DayPhase DetermineCurrentPhase(float time)
    {
        if (time >= 5f && time < 8f) return DayPhase.Sunrise;
        if (time >= 8f && time < 18f) return DayPhase.Day;
        if (time >= 18f && time < 20f) return DayPhase.Sunset;
        return DayPhase.Night;
    }
}

