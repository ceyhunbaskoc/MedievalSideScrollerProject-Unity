using System.Collections.Generic;
using UnityEngine;

public class newCoinPool : MonoBehaviour
{
    public int coinCount,arrowCount,enemyCount1,NPCcount,rabbitCount;
    public GameObject coinPrefab,arrowPrefab,enemyPrefab1,NPCPrefab,rabbitPrefab;
    public List<GameObject> coinList = new List<GameObject>();
    public List<GameObject> arrowList = new List<GameObject>();
    public List<GameObject> enemyList1 = new List<GameObject>();
    public List<GameObject> NPCList = new List<GameObject>();
    public List<GameObject> rabbitList = new List<GameObject>();
    public float gravity;

    public static newCoinPool Instance;

    Transform coinParent, arrowParent,enemy1Parent,NPCParent,rabbitParent;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        CreateParent(ref coinParent, "CoinPoolContainer");
        CreateParent(ref arrowParent, "ArrowPoolContainer");
        CreateParent(ref enemy1Parent, "Enemy1PoolContainer");
        CreateParent(ref NPCParent, "NPCPoolContainer");
        CreateParent(ref rabbitParent, "RabbitPoolContainer");


    }
    void CreateParent(ref Transform parent,string parentName)
    {
        if (parent == null)
        {
            GameObject container = new GameObject(parentName);
            parent = container.transform;
        }
    }
    
    private void Start()
    {
        start(coinCount, coinPrefab, coinList,coinParent);

        gravity = coinList[0].GetComponent<Rigidbody2D>().gravityScale;

        start(arrowCount,arrowPrefab, arrowList,arrowParent);

        start(enemyCount1 ,enemyPrefab1,enemyList1,enemy1Parent);

        start(NPCcount, NPCPrefab,NPCList,NPCParent);

        start(rabbitCount, rabbitPrefab, rabbitList,rabbitParent);
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
    public GameObject GetNPC()
    {
        GameObject NPC = get(NPCList, NPCPrefab);
        return NPC;
    }
    public GameObject GetRabbit()
    {
        GameObject rabbit = get(rabbitList, rabbitPrefab);
        return rabbit;
    }
    public void DisableCoin(GameObject coin)
    {
        coin.SetActive(false);
        newCoin cScript = coin.GetComponent<newCoin>();
        cScript.newBorn = true;
        cScript.canCollect = false;
        cScript.conflictCharacter = false;
        cScript.conflictCitizen = false;
        Animator an = coin.GetComponent<Animator>();
        an.SetBool("inSlot", false);
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
    public void DisableNPC(GameObject NPC)//out of use
    {
        NPC.SetActive(false);
        Cconflict NPCscript = NPC.GetComponent<Cconflict>();
        Animator an = NPCscript.GetComponent<Animator>();
        NPCscript.moneyCount = 0;
        NPCscript.HP = 100;
        NPCscript.isAttacking = false;
        NPCscript.isBuild = false;
        NPCscript.inPatrol = false;
        NPCscript.equipment = false;
        NPCscript.calculateDirectionPatrol = false;
        NPCscript.NPCMoney = false;
        NPCscript.isAttacking = false;
        NPCscript.isBuild = false;
        NPCscript.goToBuild = false;
        NPCscript.nightBehavior = false;
        NPCscript.currentJob = Cconflict.Jobs.None;
        an.SetBool("isWalking", false);
        an.SetBool("isRun", false);
        an.SetBool("isAttacking", false);
        an.SetBool("isBuild", false);
        an.SetBool("damageTaken", false);
        an.SetBool("NPC", false);
        an.SetBool("Archer", false);
        an.SetBool("Builder", false);

    }
    public void DisableRabbit(GameObject rabbit)
    {
        rabbit.SetActive(false);
        Rabbit rabbitScript = rabbit.GetComponent<Rabbit>();
        rabbitScript.calculateDirectionRabbit = false;
        rabbitScript.inPatrol = false;
    }
    void start(int count,GameObject prefab,List<GameObject> list,Transform container)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject poolObj = Instantiate(prefab,container);
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
