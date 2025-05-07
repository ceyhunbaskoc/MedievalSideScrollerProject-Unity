using System;
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

    public static GameManager Instance;
    public static event Action<DayPhase> OnDayPhaseChanged;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    void Update()
    {
        UpdateTime();
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

