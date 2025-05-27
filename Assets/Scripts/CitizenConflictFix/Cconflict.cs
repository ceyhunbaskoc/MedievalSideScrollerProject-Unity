using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Citizen;
using static Unity.Cinemachine.CinemachineTargetGroup;
using static UnityEngine.GraphicsBuffer;

public class Cconflict : MonoBehaviour
{
    public int moneyCount = 0;
    public float HP = 100;
    public enum Jobs { None, Archer, Builder }
    public Jobs currentJob = Jobs.None;

    [Header("Bools")]
    public bool equipment = false;
    public bool inPatrol = false,calculateDirectionPatrol = false,NPCMoney=false, isAttacking=false, isBuild=false,goToBuild=false,nightBehavior=false;

    [Header("Speeds")]
    public float NPCspeed;
    public float citizenSpeed, citizenRunSpeed;

    [Header("NPC")]
    public float NPCdetectionRadius;
    [Header("Archer")]
    public float archerAttackRange;
    public float shootForce, attackCoolDown;
    [Header("Builder")]
    public float builderRange;

    [Space]
    public Vector2[] waitingPoints = new Vector2[] {
    new Vector2(0, -0.7937559f)
    // ... ihtiyaca göre artır
};
    public int waitingIndex; // Her NPC'ye Inspector'dan veya spawn sırasında atanacak

    //[Header("Archer and Builder Sprites")]
    //public Sprite NPC;
    //public Sprite none,archer,builder;
    [Space]
    public float patrolWalkDistance;
    public float patrolCoolDown;
    [Space]
    public GameObject arrowPrefab;
    Collider2D NPCTarget;
    Rigidbody2D rb;
    GameObject bowStock, hamStock;
    Coroutine patrolCoroutine, currentBehaviorCoroutine,GoStockCoroutine;
    SpriteRenderer sr;
    Vector2 patrolDirection;
    float targetXPatrol;
    Animator an;
    float threshold = 0.01f;  // Hızın sıfır kabul edileceği eşik değeri
    GameManager.DayPhase phase;
    private bool reachedNightTarget = false;
    private float nightTargetX;
    bool zeroReached = false;
    bool nightBehaviorStarted = false;
    float random;
    GameObject nightTargetWall = null;
    

    void Start()
    {
        waitingIndex = Random.Range(0, waitingPoints.Length);
        rb = GetComponent<Rigidbody2D>();
        bowStock = GameObject.Find("BowStock");
        hamStock = GameObject.Find("HammerStock");
        sr= GetComponent<SpriteRenderer>();
        an = GetComponent<Animator>();
    }

    void Update()
    {
        if (isAttacking)
            an.SetBool("isAttacking", true);
        else
            an.SetBool("isAttacking", false);
        if (currentJob == Jobs.None)
        {
            if (!CitizensManager.Instance.None.Contains(gameObject))
            {
                CitizensManager.Instance.None.Add(gameObject);
                an.SetBool("NPC", true);
            }
            if(!NPCMoney)
            CheckNearbyAndCollect();
            else
            {
                if (!zeroReached&& GameManager.Instance.hammerStock == 0 && GameManager.Instance.bowStock == 0)
                    GoToZero();
                CheckStocks();
            }

        }
        if (currentJob == Jobs.Archer)
        {
            
            if (CitizensManager.Instance.None.Contains(gameObject))
                CitizensManager.Instance.None.Remove(gameObject);
            if (!CitizensManager.Instance.Archers.Contains(gameObject))
            {
                CitizensManager.Instance.Archers.Add(gameObject);
                moneyCount = 0;
            }
            tag = "citizen";
            if (!equipment)
            {
                if (GameManager.Instance.bowStock != 0)
                {
                    an.SetBool("isRun", true);
                    GoStockandGetEquipment(bowStock, citizenRunSpeed);
                }
                else
                {
                    LoseJob();
                    NPCMoney = true;
                    moneyCount=1;
                }
            }
            else
            {
                if (currentBehaviorCoroutine == null)
                    currentBehaviorCoroutine = StartCoroutine(ArcherBehavior());

            }


        }
        if (currentJob == Jobs.Builder)
        {
            if (CitizensManager.Instance.None.Contains(gameObject))
                CitizensManager.Instance.None.Remove(gameObject);
            if (!CitizensManager.Instance.Builders.Contains(gameObject))
            {
                CitizensManager.Instance.Builders.Add(gameObject);
                moneyCount = 0;
            }
            tag = "citizen";
            if (!equipment)
            {

                if (GameManager.Instance.hammerStock != 0)
                {
                    an.SetBool("isRun", true);
                    GoStockandGetEquipment(hamStock, citizenRunSpeed);
                }
                else
                {
                    LoseJob();
                    NPCMoney = true;
                    moneyCount = 1;
                }
            }
            else
                if (currentBehaviorCoroutine == null)
                currentBehaviorCoroutine = StartCoroutine(BuilderBehaviour());
        }
            if (Mathf.Abs(rb.linearVelocity.x) > threshold)
            {
                sr.flipX = rb.linearVelocity.x < 0;
                if (!isBuild)
                    an.SetBool("isWalking", true);
            }
            else
            {
                if (!isBuild )
                    an.SetBool("isWalking", false);
            }
        phase = GameManager.Instance.currentPhase;
        switch(phase)
        {
            case GameManager.DayPhase.Sunrise:
                nightBehavior = false;
                nightBehaviorStarted= false;
                isBuild = false;
                inPatrol = false;
                isAttacking = false;
                if (currentBehaviorCoroutine != null)
                {
                    StopCoroutine(currentBehaviorCoroutine);
                    currentBehaviorCoroutine = null;
                }
                if (patrolCoroutine != null)
                {
                    StopCoroutine(patrolCoroutine);
                    patrolCoroutine = null;
                }

                break;
            case GameManager.DayPhase.Night:
                nightBehavior = true;
                if (!nightBehaviorStarted)
                {
                    random = Random.Range(2f, 5f);
                    int index=0;
                    if (currentJob == Jobs.Archer)
                    index = CitizensManager.Instance.Archers.IndexOf(gameObject);
                    if (currentJob == Jobs.Builder)
                        index = CitizensManager.Instance.Builders.IndexOf(gameObject);
                    if (index % 2 == 0)
                        nightTargetWall = GetLeftmostWall();
                    else
                        nightTargetWall = GetRightmostWall();
                    print(nightTargetWall.name);        
                }
                break;

        }
        if(phase == GameManager.DayPhase.Sunrise&&reachedNightTarget)
        {
            reachedNightTarget = false;
            nightBehaviorStarted = false;
        }

        if (HP <= 0)
            LoseJob();
    }
    void GoToZero()
    {
        Vector2 target = waitingPoints[waitingIndex];
        StartCoroutine(MoveToPoint(target));
    }
    IEnumerator MoveToPoint(Vector2 target)
    {
        an.SetBool("isRun", true);
        while (Vector2.Distance(transform.position, target) > 0.1f)
        {
            Vector2 dir = (target - (Vector2)transform.position).normalized;
            rb.linearVelocity = dir * NPCspeed;
            yield return null;
        }
        rb.linearVelocity = Vector2.zero;
        an.SetBool("isRun", false);
        // Burada idle bekle
        StartCoroutine(WaitForStock());
    }
    IEnumerator WaitForStock()
    {
        while (GameManager.Instance.hammerStock == 0 && GameManager.Instance.bowStock == 0)
            yield return null; // Stok gelene kadar bekle

        // Stok oluştu, mesleği almaya git
        CheckStocks();
    }
    void CheckStocks()
    {
        if (GameManager.Instance.hammerStock != 0 || GameManager.Instance.bowStock != 0)
        {
            if (GameManager.Instance.hammerStock > GameManager.Instance.bowStock)
            {
                StartCoroutine(NPCGetCoinWait(Jobs.Builder));
            }
            else if (GameManager.Instance.hammerStock < GameManager.Instance.bowStock)
            {
                StartCoroutine(NPCGetCoinWait(Jobs.Archer));
            }
            else if (GameManager.Instance.hammerStock == GameManager.Instance.bowStock)
            {
                Jobs job;
                job = Random.Range(0, 2) == 0 ? Jobs.Archer : Jobs.Builder;
                StartCoroutine(NPCGetCoinWait(job));
            }
        }
    }
    private void CheckNearbyAndCollect()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, NPCdetectionRadius, LayerMask.GetMask("coin"));
        if (!NPCMoney)
        {
            if (colliders.Length != 0)
            {
                NPCTarget = colliders[0];
            }
            else
            {
                return;
            }
        }

        if (!NPCMoney&& Mathf.Abs(transform.position.x - NPCTarget.transform.position.x) > 0.2f)
        {
            Vector3 direction = (new Vector3(NPCTarget.transform.position.x, transform.position.y) - transform.position).normalized;
            rb.linearVelocity = direction * NPCspeed;

        }

        if (Mathf.Abs(transform.position.x - NPCTarget.transform.position.x) < 0.2f&&NPCTarget!=null)
        {
            if (GameManager.Instance.hammerStock != 0 || GameManager.Instance.bowStock != 0)
            {
                if (GameManager.Instance.hammerStock > GameManager.Instance.bowStock)
                {
                    StartCoroutine(NPCGetCoinWait(Jobs.Builder));
                }
                else if (GameManager.Instance.hammerStock < GameManager.Instance.bowStock)
                {
                    StartCoroutine(NPCGetCoinWait(Jobs.Archer));
                }
                else if (GameManager.Instance.hammerStock == GameManager.Instance.bowStock)
                {
                    Jobs job;
                    job = Random.Range(0, 2) == 0 ? Jobs.Archer : Jobs.Builder;
                    StartCoroutine(NPCGetCoinWait(job));
                }
            }
            else
                print("tüm stoklar boş");
        }else if (NPCMoney)
        {
            if (GameManager.Instance.hammerStock != 0 || GameManager.Instance.bowStock != 0)
            {
                if (GameManager.Instance.hammerStock > GameManager.Instance.bowStock)
                {
                    StartCoroutine(NPCGetCoinWait(Jobs.Builder));
                }
                else if (GameManager.Instance.hammerStock < GameManager.Instance.bowStock)
                {
                    StartCoroutine(NPCGetCoinWait(Jobs.Archer));
                }
                else if (GameManager.Instance.hammerStock == GameManager.Instance.bowStock)
                {
                    Jobs job;
                    job = Random.Range(0, 2) == 0 ? Jobs.Archer : Jobs.Builder;
                    StartCoroutine(NPCGetCoinWait(job));
                }
            }else if(GameManager.Instance.hammerStock == 0 && GameManager.Instance.bowStock == 0&& Mathf.Abs(transform.position.x - 0) > 0.2f)
            {
                Vector3 direction = (new Vector3(0,transform.position.y,transform.position.z)-transform.position).normalized;
                rb.linearVelocity = direction * NPCspeed;
            }
        }
    }

    IEnumerator NPCGetCoinWait(Jobs job)
    {
        yield return new WaitUntil(() => NPCMoney);
        currentJob = job;
        HP = 100;
        NPCMoney = false;
    }
    void GoStockandGetEquipment(GameObject target, float speed)
    {
        Vector3 direction = (new Vector3(target.transform.position.x, transform.position.y) - transform.position).normalized;
        rb.linearVelocity = direction * speed;
        an.SetBool("NPC", true);
        if(target==bowStock&&GameManager.Instance.bowStock==0)
        {
            LoseJob();
            NPCMoney = true;
            moneyCount = 1;
            an.SetBool("isWalking", false);
            return;
        }
        if (target == hamStock && GameManager.Instance.hammerStock == 0)
        {
            LoseJob();
            NPCMoney = true;
            moneyCount = 1;
            an.SetBool("isWalking", false);
            return;
        }
        if (Mathf.Abs(transform.position.x - target.transform.position.x) < 0.1f)
        {
            an.SetBool("isRun", false);
            equipment = true;
            an.SetBool("NPC", false);
            if (target == bowStock)
            {
                GameManager.Instance.bowStock--;
                an.SetBool("Archer", true);
            }
            if (target == hamStock)
            {
                GameManager.Instance.hammerStock--;
                an.SetBool("Builder", true);
            }
        }
    }
    //IEnumerator GoStockandGetEquipment(GameObject target, float speed)
    //{
    //    // Stoğa doğru hareket
    //    while (Vector2.Distance(transform.position, target.transform.position) > 0.2f)
    //    {
    //        Vector3 direction = (new Vector3(target.transform.position.x, transform.position.y) - transform.position).normalized;
    //        rb.linearVelocity = direction * speed;
    //        an.SetBool("NPC", true);

    //        // Yolda stok biterse işi bırak
    //        if (target == bowStock && GameManager.Instance.bowStock == 0)
    //        {
    //            LoseJob();
    //            NPCMoney = true;
    //            moneyCount = 1;
    //            an.SetBool("isRun", false);
    //            an.SetBool("isWalking", false);
    //            rb.linearVelocity = Vector2.zero;
    //            yield break;
    //        }
    //        if (target == hamStock && GameManager.Instance.hammerStock == 0)
    //        {
    //            LoseJob();
    //            NPCMoney = true;
    //            moneyCount = 1;
    //            an.SetBool("isRun", false);
    //            an.SetBool("isWalking", false);
    //            rb.linearVelocity = Vector2.zero;
    //            yield break;
    //        }
    //        yield return null;
    //    }

    //    // Stoğa vardığında hareketi ve animasyonu durdur
    //    rb.linearVelocity = Vector2.zero;
    //    an.SetBool("isRun", false);
    //    an.SetBool("NPC", false);

    //    // Stoğa vardığında tekrar stok kontrolü
    //    if (target == bowStock)
    //    {
    //        if (GameManager.Instance.bowStock > 0)
    //        {
    //            equipment = true;
    //            GameManager.Instance.bowStock--;
    //            an.SetBool("Archer", true);
    //        }
    //        else
    //        {
    //            LoseJob();
    //            NPCMoney = true;
    //            moneyCount = 1;
    //            an.SetBool("isWalking", false);
    //        }
    //    }
    //    else if (target == hamStock)
    //    {
    //        if (GameManager.Instance.hammerStock > 0)
    //        {
    //            equipment = true;
    //            GameManager.Instance.hammerStock--;
    //            an.SetBool("Builder", true);
    //        }
    //        else
    //        {
    //            LoseJob();
    //            NPCMoney = true;
    //            moneyCount = 1;
    //            an.SetBool("isWalking", false);
    //        }
    //    }
    //}

    ////////////////// Archer Behaviour//////////////////////
    IEnumerator ArcherBehavior()
    {
        print("Archer Behavior basladi");
        while (currentJob == Jobs.Archer)
        {
            GameObject target = FindNearest(archerAttackRange, "Rabbit");
            GameObject target2 = FindNearest(archerAttackRange, "enemy");
            yield return null;
            // Target null ise patrol'a dön
            if (target == null&&target2==null)
            {
                //print("targetlar boş");
                isAttacking = false;
                if (phase == GameManager.DayPhase.Night)
                {                    
                    if (nightTargetWall != null)
                    {
                        print("farthestWall != null");
                        if (nightBehavior)
                        {
                            if (!nightBehaviorStarted)
                            {
                                float random = Random.Range(2f, 5f); // Sadece bir kez üret
                                if (nightTargetWall.transform.position.x < 0)
                                    StartNightBehavior(nightTargetWall, random);
                                else
                                    StartNightBehavior(nightTargetWall, -random);
                                nightBehaviorStarted = true;
                            }
                            NightBehavior();
                            if (patrolCoroutine != null)
                            {
                                inPatrol = false;
                                StopCoroutine(patrolCoroutine);
                                patrolCoroutine = null;
                                inPatrol = false;
                                calculateDirectionPatrol = false;
                            }
                        }
                    }
                }
                else if (!inPatrol && patrolCoroutine == null)
                {
                    patrolCoroutine = StartCoroutine(Patrol());
                    print("patrol başladı");
                }

                yield return null; // Bir sonraki frame'e geç
                continue;         // Döngüyü yeniden başlat
            }

            // Target varsa saldır
            if (patrolCoroutine != null)
            {
                StopCoroutine(patrolCoroutine);
                patrolCoroutine = null;
                inPatrol = false;
                calculateDirectionPatrol = false;
            }

            inPatrol = false;
            calculateDirectionPatrol = false;

            if (!isAttacking)
            {
                // Önce enemy kontrolü (yüksek öncelik)
                if (target2 != null)
                {
                    isAttacking = true;
                    if (target2.transform.position.x > transform.position.x)
                        sr.flipX = false;
                    if (target2.transform.position.x < transform.position.x)
                        sr.flipX = true;
                    ShootArrow(target2);
                    yield return new WaitForSeconds(attackCoolDown);
                    isAttacking = false;
                }
                // Enemy yoksa rabbit kontrolü
                else if (target != null)
                {
                    isAttacking = true;
                    if (target.transform.position.x > transform.position.x)
                    {
                        sr.flipX = false;
                    }
                    if (target.transform.position.x < transform.position.x)
                    {
                        sr.flipX = true;
                    }
                    ShootArrow(target);
                    yield return new WaitForSeconds(attackCoolDown);
                    isAttacking = false;
                }
            }

            yield return null; // Her frame'de kontrol
        }
    }

    public GameObject GetLeftmostWall()
    {
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        GameObject leftmost = null;
        float minX = float.MaxValue;
        foreach (var wall in walls)
        {
            if (wall.transform.position.x < minX)
            {
                minX = wall.transform.position.x;
                leftmost = wall;
            }
        }
        return leftmost;
    }

    public GameObject GetRightmostWall()
    {
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        GameObject rightmost = null;
        float maxX = float.MinValue;
        foreach (var wall in walls)
        {
            if (wall.transform.position.x > maxX)
            {
                maxX = wall.transform.position.x;
                rightmost = wall;
            }
        }
        return rightmost;
    }

    void StartNightBehavior(GameObject target, float random)
    {
        nightTargetX = target.transform.position.x + random;
        reachedNightTarget = false;
    }

    void NightBehavior()
    {
        if (reachedNightTarget||!nightBehavior) return;

        Vector3 direction = (new Vector3(nightTargetX, transform.position.y, transform.position.z) - transform.position).normalized;
        rb.linearVelocity = direction.normalized * citizenRunSpeed;
        an.SetBool("isRun", true);

        if (Mathf.Abs(transform.position.x - nightTargetX) < 0.34f)
        {
            rb.linearVelocity = Vector2.zero;
            reachedNightTarget = true;
            an.SetBool("isRun", false);
            an.SetBool("isWalking", false);
        }
    }
    public GameObject GetFarthestWall()
    {
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        GameObject farthestWall=null;
        float farthestDistance=0;
        foreach(GameObject wall in walls)
        {
            float distance = Vector2.Distance(wall.transform.position, Vector2.zero);
            if (distance > farthestDistance)
            {
                farthestDistance = distance;
                farthestWall = wall.gameObject;
            }
        }
        return farthestWall;
    }

    //private void Attack(GameObject target)
    //{
    //    if (!isAttacking)
    //    {
    //        isAttacking = true;

    //        // Sald�r� animasyonu veya efektleri burada tetiklenebilir
    //        ShootArrow(target);

    //        StartCoroutine(ResetAttack());

    //    }
    //}
    //IEnumerator ResetAttack()
    //{
    //    yield return new WaitForSeconds(attackCoolDown);
    //    isAttacking = false;
    //}

    public void ShootArrow(GameObject rabbitTarget)
    {
        if (rabbitTarget == null) return;

        Vector3 startPos = transform.position;
        if (newCoinPool.Instance == null)
        {
            Debug.LogError("newCoinPool.Instance null!");
            return;
        }
        GameObject newArrow = newCoinPool.Instance.GetArrow();
        print(newArrow.name);
        if (newArrow == null)
        {
            Debug.LogError("newArrow null! GetArrow() prefabı atanmadı veya pool boş.");
            return;
        }
        newArrow.transform.position = startPos;
        Rigidbody2D rb = newArrow.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector3 targetPos = rabbitTarget.transform.position;
            float gravity = Mathf.Abs(Physics2D.gravity.y);
            float distance = Vector3.Distance(startPos, targetPos);
            float flightTime = distance / shootForce * 1.5f;
            Vector2 initialVelocity = CalculateArcVelocity(startPos, targetPos, gravity, flightTime);
            float angle = Mathf.Atan2(initialVelocity.y, initialVelocity.x) * Mathf.Rad2Deg;
            newArrow.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            rb.linearVelocity = initialVelocity;
        }
        else
        {
            Debug.LogError("Ok prefabında Rigidbody2D component'i bulunamadı!");
        }
    }
    public void DamageTakenFalse()
    {
        an.SetBool("damageTaken",false);
    }

    private Vector2 CalculateArcVelocity(Vector3 startPos, Vector3 targetPos, float gravity, float time)
    {
        // Yatay ve dikey hız bileşenlerini hesapla
        float horizontalVelocity = (targetPos.x - startPos.x) / time;
        float verticalVelocity = (targetPos.y - startPos.y) / time + 0.5f * gravity * time;

        return new Vector2(horizontalVelocity, verticalVelocity);
    }

    ////////////////// Builder Behaviour////////////////////
    
    IEnumerator BuilderBehaviour()
    {
        while (currentJob == Jobs.Builder)
        {
            GameObject target = FindNearest(builderRange, "Construction");

            isAttacking = false;
            if (target == null)
            {
                //print("targetlar boş");
                an.SetBool("isBuild", false);
                an.SetBool("isRun", false);
                an.SetBool("isWalking", false);
                isAttacking = false;
                if (phase == GameManager.DayPhase.Night)
                {
                    if (nightTargetWall != null)
                    {
                        print("farthestWall != null");
                        if (nightBehavior)
                        {
                            if (!nightBehaviorStarted)
                            {
                                float random = Random.Range(2f, 5f); // Sadece bir kez üret
                                if (nightTargetWall.transform.position.x < 0)
                                    StartNightBehavior(nightTargetWall, random);
                                else
                                    StartNightBehavior(nightTargetWall, -random);
                                nightBehaviorStarted = true;
                            }
                            NightBehavior();
                            if (patrolCoroutine != null)
                            {
                                inPatrol = false;
                                StopCoroutine(patrolCoroutine);
                                patrolCoroutine = null;
                                inPatrol = false;
                                calculateDirectionPatrol = false;
                            }
                        }
                    }
                }
                else if (!inPatrol && patrolCoroutine == null)
                {
                    patrolCoroutine = StartCoroutine(Patrol());
                    print("patrol başladı");
                }
                isBuild = false;
                an.SetBool("isBuild", false);
                yield return null; // Bir sonraki frame'e geç
                continue;         // Döngüyü yeniden başlat
            }
            if (patrolCoroutine != null)
            {
                StopCoroutine(patrolCoroutine);
                patrolCoroutine = null;
                inPatrol = false;
                calculateDirectionPatrol = false;
            }

            inPatrol = false;
            calculateDirectionPatrol = false;

            if (!isBuild)
            {
                an.SetBool("isBuild", false);
                an.SetBool("isRun", true);
                GoToConstruction(target, citizenRunSpeed);
            }
            yield return null;
        }
    }
    private GameObject FindNearest(float range,string tag)  ///Ortak fonksiyon
    {
        // Etraftaki tav�anlar� bul
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, range);
        GameObject nearest = null;
        float nearestDistance = Mathf.Infinity;
        foreach (var hitCollider in hitColliders)
        {
            //Debug.Log($"Found object: {hitCollider.gameObject.name}, Tag: {hitCollider.tag}, Layer: {LayerMask.LayerToName(hitCollider.gameObject.layer)}");
            if (hitCollider.CompareTag(tag))
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = hitCollider.gameObject;
                }
            }
        }
        return nearest;
    }
    void GoToConstruction(GameObject target,float speed)
    {
        goToBuild = true;
        an.SetBool("isWalking", false);
        an.SetBool("isBuild", false);
        Vector3 direction = (new Vector3(target.transform.position.x, transform.position.y) - transform.position).normalized;
        rb.linearVelocity = direction * speed;
        if (Mathf.Abs(transform.position.x - target.transform.position.x) < 0.2f)
        {
            an.SetBool("isRun", false);
            an.SetBool("isBuild", true);
            isBuild = true;
            goToBuild = false;
        }
    }

    IEnumerator Patrol()
    {
        while (true)
        {
            if (!calculateDirectionPatrol)
            {
                int rightOrLeft = Random.Range(0, 2);
                targetXPatrol = (rightOrLeft == 0) ?
                    transform.position.x + patrolWalkDistance :
                    transform.position.x - patrolWalkDistance;

                Vector2 targetPosition = new Vector2(targetXPatrol, transform.position.y);
                patrolDirection = (targetPosition - (Vector2)transform.position).normalized;
                calculateDirectionPatrol = true;
            }

            an.SetBool("isRun", true);

            // Hedefe ulaşana kadar hareket
            while (Mathf.Abs(transform.position.x - targetXPatrol) > 0.1f)
            {
                rb.linearVelocity = new Vector2(patrolDirection.x * citizenSpeed, rb.linearVelocity.y);
                an.SetBool("isWalking", true);
                yield return null;
            }

            // Hedefe ulaştı
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            an.SetBool("isRun", false);

            inPatrol = true;
            yield return new WaitForSeconds(patrolCoolDown);
            inPatrol = false;
            calculateDirectionPatrol = false;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, archerAttackRange);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("character"))
        {
                if (moneyCount <= 0) return;
                StartCoroutine(GiveMoneyToPlayer());
        }
        if(collision.CompareTag("enemy"))
        {
            if (currentJob != Jobs.None)
            {
                if (HP <= 0) return;
                HP -= collision.gameObject.GetComponent<enemy>().damage;
            }
        }
    }
    IEnumerator GiveMoneyToPlayer()
    {
        int coinsToGive;
        if (currentJob == Jobs.None)
            coinsToGive = moneyCount - 1;
        else
            coinsToGive = moneyCount;

        for (int i = 0; i < coinsToGive; i++)
        {
            GameObject coinToPlayer = newCoinPool.Instance.GetCoin();
            coinToPlayer.transform.position = transform.position;
            yield return new WaitForSeconds(0.15f);
        }
        if (currentJob == Jobs.None)
            moneyCount = 1;
        else
            moneyCount = 0;
    }
    void LoseJob()
    {
        if (currentBehaviorCoroutine != null)
        {
            StopCoroutine(currentBehaviorCoroutine);
            currentBehaviorCoroutine = null;
        }
        if (patrolCoroutine != null)
        {
            StopCoroutine(patrolCoroutine);
            patrolCoroutine = null;
        }
        if(GoStockCoroutine != null)
        {
            StopCoroutine(GoStockCoroutine);
            GoStockCoroutine = null;
        }
        currentJob = Jobs.None;
        if (CitizensManager.Instance.Builders.Contains(gameObject))
            CitizensManager.Instance.Builders.Remove(gameObject);
        if (CitizensManager.Instance.Archers.Contains(gameObject))
            CitizensManager.Instance.Archers.Remove(gameObject);
        equipment = false;
        inPatrol = false;
        isAttacking = false;
        isBuild = false;
        an.SetBool("NPC", true);
        an.SetBool("Archer", false);
        an.SetBool("Builder", false);
        an.SetBool("isBuild", false);
        zeroReached = false;
        tag = "NPC";
    }
}
