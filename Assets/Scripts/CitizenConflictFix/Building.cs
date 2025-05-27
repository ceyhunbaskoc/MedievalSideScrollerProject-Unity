using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Building : buildableObject
{
    public float HP;
    public int level=1;
    public GameObject previousBuildable;

    public float maxHP;
    public GameObject healthBarPrefab;
    private Slider healthBarSlider;
    private GameObject healthBarInstance;

    public string mainTag;

    [System.Serializable]
    public class BuildingLevelProperties
    {
        public Sprite levelSprites;
        public float damage,rate,range,force,newHP,newArcAngle;
    }
    public List<BuildingLevelProperties> levels;
    protected override void Start()
    {
        base.Start();
        mainTag = tag;
        maxHP = gameObject.GetComponent<Building>().HP;
        GameObject coin = newCoinPool.Instance.GetCoin();
        coin.transform.position = transform.position;
    }

    // Update is called once per frame
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
                healthBarInstance.SetActive(true);
                healthBarSlider = healthBarInstance.GetComponentInChildren<Slider>();
                UpdateHealthBar();
            }
        }
        if (gameObject.CompareTag("archerTower"))
        {
            if (CityCenter.Instance.levels[CityCenter.Instance.level - 1].archerTowerLimit > level)
            {
                return;
            }
        }
        if (coins.Count == coinSlots.Count)
        {
            StartCoroutine(WaitForConstruction());
        }
        if (currentBuildersOnThis > 0)
        {
            if (!isConstruction) return;

            float buildAmount = progressPerSecond * currentBuildersOnThis * Time.deltaTime;
            currentProgress += buildAmount;

            currentProgress = Mathf.Clamp(currentProgress, 0f, 100f);

            if (currentProgress >= 100f)
            {
                isConstruction = false;
                cr.onBuildable = false;
                sr.sprite = levels[level].levelSprites;
                HP = levels[level].newHP;
                arcAngle = levels[level].newArcAngle;
                if (gameObject.GetComponent<ArcherTower>() != null)
                {
                    ArcherTower at = gameObject.GetComponent<ArcherTower>();
                    at.damage = levels[level].damage;
                    at.attackCoolDown = levels[level].rate;
                    at.attackRange = levels[level].range;
                    at.shootForce = levels[level].force;
                }
                if (mainTag == "Wall")
                    tag = "Wall";
                if (mainTag == "archerTower")
                    tag = "archerTower";
                ResetCoinSlots();
                GameObject rewardCoin = newCoinPool.Instance.GetCoin();
                rewardCoin.transform.position = transform.position;
                maxHP = HP;
                if (healthBarInstance != null)
                {
                    healthBarSlider.value = 1;
                    healthBarInstance.SetActive(false);

                }
                foreach (Cconflict builder in currentBuilders)
                {
                    builder.isBuild = false;
                }
                level++;
            }
        }
        if (HP <= 0)
        {
            Instantiate(previousBuildable, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
    public override void PlaceCoin(GameObject coin)
    {
        if(level<2)
        {
           base.PlaceCoin(coin);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(level<2)
        {
            base.OnTriggerEnter2D (collision);
        }
    }
    void UpdateHealthBar()
    {
        if (healthBarSlider != null)
            healthBarSlider.value = HP / maxHP;
        else
            print("healtBarSlider == null");
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
}
