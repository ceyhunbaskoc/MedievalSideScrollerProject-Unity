using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static WaveManager;

public class CityCenter : buildableObject
{
    [System.Serializable]
    public class LevelProperties
    {
        public Sprite levelSprites;
        public float archerTowerLimit;
        public float HPMultiplier;
    }

    public List<LevelProperties> levels;
    public int level = 1;
    public float baseBuildRange = 10f;
    public float buildRangeMultiplier = 1.5f;
    public float HP;
    public GameObject gameoverPanel;

    public float maxHP;
    public GameObject healthBarPrefab;
    private Slider healthBarSlider;
    private GameObject healthBarInstance;
    bool upgrade=false;

    public static CityCenter Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        base.Start();
        maxHP = HP;
    }
    protected override void Update()
    {
        if (HP < maxHP)
        {
            if (healthBarInstance == null)
            {
                healthBarInstance = Instantiate(healthBarPrefab, transform);
                healthBarInstance.transform.localPosition = new Vector3(0, 0.85f, 0);
            }
            else
            {
                healthBarSlider = healthBarInstance.GetComponentInChildren<Slider>();
                UpdateHealthBar();
            }
        }

        if (coins.Count == coinSlots.Count)
        {
            UpgradeCityCenter();
        }
        if (HP <= 0)
        {
            gameoverPanel.SetActive(true);
        }
    }
    void UpdateHealthBar()
    {
        if (healthBarSlider != null)
            healthBarSlider.value = HP / maxHP;
        else
            print("healtBarSlider == null");
    }
    public void UpgradeCityCenter()
    {
        if(!upgrade)
        StartCoroutine(WaitForUpgrade());
    }
    public IEnumerator WaitForUpgrade()
    {
        upgrade = true;
        yield return new WaitForSeconds(transitionTime + 0.15f);
        if (level < levels.Count)
            sr.sprite = levels[level].levelSprites;
        else
            Debug.LogWarning("CityCenter: Level index out of range!");
        baseBuildRange += buildRangeMultiplier;
        float hpMultiplier = levels[level].HPMultiplier;
        HP += hpMultiplier;
        maxHP = HP;
        level++;
        foreach (GameObject slots in coinSlots)
        {
            slots.SetActive(false);
        }
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
        upgrade = false;
    }

    public override void PlaceCoin(GameObject coin)
    {
        if (level < 2)
        {
            base.PlaceCoin(coin);
        }
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if(level<2)
        {
            if (Vector2.Distance(transform.position, CityCenter.Instance.gameObject.transform.position) <= CityCenter.Instance.baseBuildRange)
                centerDistanceCheck = true;
            else
                centerDistanceCheck = false;
            if (collision.gameObject.CompareTag("character") && !isConstruction && centerDistanceCheck)
            {
                foreach (GameObject slots in coinSlots)
                {
                    slots.SetActive(true);
                }
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
