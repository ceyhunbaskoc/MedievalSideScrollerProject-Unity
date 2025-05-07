using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Building : buildableObject
{
    public float HP;
    public int level=1;
    public GameObject previousBuildable;

    [System.Serializable]
    public class BuildingLevelProperties
    {
        public Sprite levelSprites;
        public float damage,rate,range,force,newHP;
    }
    public List<BuildingLevelProperties> levels;
    protected override void Start()
    {
        base.Start();
        GameObject coin = newCoinPool.Instance.GetCoin();
        coin.transform.position = transform.position;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (CityCenter.Instance.levels[CityCenter.Instance.level].archerTowerLimit>level)
        {
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
                    ArcherTower at = gameObject.GetComponent<ArcherTower>();
                    at.damage = levels[level].damage;
                    at.attackCoolDown = levels[level].rate;
                    at.attackRange = levels[level].range;
                    at.shootForce = levels[level].force;
                    tag = "buildable";
                    ResetCoinSlots();
                    GameObject rewardCoin = newCoinPool.Instance.GetCoin();
                    rewardCoin.transform.position = transform.position;
                    level++;
                }
            }
            if (HP <= 0)
            {
                Instantiate(previousBuildable, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
        
            
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
