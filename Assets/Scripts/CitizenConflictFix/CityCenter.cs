using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WaveManager;

public class CityCenter : buildableObject
{
    [System.Serializable]
    public class LevelProperties
    {
        public Sprite levelSprites;
        public float archerTowerLimit;
    }

    public List<LevelProperties> levels;
    public int level = 1;
    public float baseBuildRange = 10f;
    public float buildRangeMultiplier = 1.5f;
    
    public static CityCenter Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    protected override void Update()
    {
        if (coins.Count == coinSlots.Count)
        {
            UpgradeCityCenter();
        }
    }
    public void UpgradeCityCenter()
    {
        StartCoroutine(WaitForUpgrade());
    }
    public IEnumerator WaitForUpgrade()
    {
        yield return new WaitForSeconds(transitionTime + 0.15f);
        sr.sprite = levels[level].levelSprites;
        baseBuildRange += buildRangeMultiplier;
        level++;
        ResetCoinSlots();
    }
    void ResetCoinSlots()
    {
        currentCoinCount = 0;
        for (int i = 0; i < coins.Count; i++)
        {
            newCoinPool.Instance.DisableCoin(coins[i]);
        }
        coins.Clear();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (Vector2.Distance(transform.position, CityCenter.Instance.gameObject.transform.position) <= CityCenter.Instance.baseBuildRange)
            centerDistanceCheck = true;
        else
            centerDistanceCheck = false;
        if (collision.gameObject.CompareTag("character") && !isConstruction&& centerDistanceCheck)
        {
            foreach (GameObject slots in coinSlots)
            {
                slots.SetActive(true);
            }
        }
    }
    protected override void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("character"))
        {
            foreach (GameObject slots in coinSlots)
            {
                slots.SetActive(false);
            }
        }
    }

}
