using System.Collections.Generic;
using UnityEngine;

public class newCoinPool : MonoBehaviour
{
    public int coinCount,arrowCount,enemyCount1;
    public GameObject coinPrefab,arrowPrefab,enemyPrefab1;
    public List<GameObject> coinList = new List<GameObject>();
    public List<GameObject> arrowList = new List<GameObject>();
    public List<GameObject> enemyList1 = new List<GameObject>();
    public float gravity;

    public static newCoinPool Instance;
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    private void Start()
    {
        start(coinCount, coinPrefab, coinList);

        gravity = coinList[0].GetComponent<Rigidbody2D>().gravityScale;

        start(arrowCount,arrowPrefab, arrowList);

        start(enemyCount1 ,enemyPrefab1,enemyList1);
    }
    public GameObject GetCoin()
    {
        GameObject coin = get(coinList,coinPrefab);
        return coin;
    }
    public GameObject GetEnemy1()
    {
        GameObject enemy1 = get(enemyList1, enemyPrefab1);
        return enemy1;
    }
    public GameObject GetArrow()
    {
        GameObject arrow = get(arrowList,arrowPrefab);
        return arrow;
    }
    public void DisableCoin(GameObject coin)
    {
        coin.SetActive(false);
        newCoin cScript = coin.GetComponent<newCoin>();
        cScript.newBorn = true;
        cScript.canCollect = false;
        cScript.conflictCharacter = false;
        cScript.conflictCitizen = false;
        coin.GetComponent<Rigidbody2D>().gravityScale = gravity;

    }

    public void DisableEnemy1(GameObject enemy1)
    {
        enemy1.SetActive(false);
        enemy enemyScript = enemy1.GetComponent<enemy>();
        enemyScript.HP = 100;
        enemyScript.reachedZero = false;
        enemyScript.attackFinished = true;
        enemyScript.isAttack = false;
    }
    public void DisableArrow(GameObject arrow)
    {
        arrow.SetActive(false);
        Rigidbody2D rbArrow = arrow.GetComponent<Rigidbody2D>();
        rbArrow.linearVelocity = Vector2.zero;
    }
    void start(int count,GameObject prefab,List<GameObject> list)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject poolObj = Instantiate(prefab);
            poolObj.SetActive(false);
            list.Add(poolObj);
        }
    }

    GameObject get(List<GameObject> list,GameObject prefab)
    {
        foreach (GameObject poolObj in list)
        {
            if (poolObj != null)
            {
                if (!poolObj.activeInHierarchy)
                {
                    poolObj.SetActive(true);
                    return poolObj;
                }
            }
        }
        //Tüm objeler kullanýlýyorsa yeni obje oluþtur
        GameObject newPoolObj = Instantiate(prefab);
        newPoolObj.SetActive(true);
        list.Add(newPoolObj);
        return newPoolObj;
    }
}
