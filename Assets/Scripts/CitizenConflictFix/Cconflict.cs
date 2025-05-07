using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static Citizen;
using static UnityEditor.PlayerSettings;
using static UnityEngine.GraphicsBuffer;

public class Cconflict : MonoBehaviour
{
    public int moneyCount = 0;
    public float HP = 100;
    public enum Jobs { None, Archer, Builder }
    public Jobs currentJob = Jobs.None;

    [Header("Bools")]
    public bool equipment = false;
    public bool inPatrol = false,calculateDirectionPatrol = false,NPCMoney=false, isAttacking=false, isBuild=false;

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
    [Header("Archer and Builder Sprites")]
    public Sprite NPC;
    public Sprite none,archer,builder;
    [Space]
    public float patrolWalkDistance;
    public float patrolCoolDown;
    [Space]
    public GameObject arrowPrefab;
    Collider2D NPCTarget;
    Rigidbody2D rb;
    GameObject bowStock, hamStock;
    Coroutine patrolCoroutine, currentBehaviorCoroutine;
    SpriteRenderer sr;
    Vector2 patrolDirection;
    float targetXPatrol;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bowStock = GameObject.Find("BowStock");
        hamStock = GameObject.Find("HammerStock");
        sr= GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (currentJob == Jobs.None)
        {
            if (!CitizensManager.Instance.None.Contains(gameObject))
            {
                CitizensManager.Instance.None.Add(gameObject);
                sr.sprite = NPC;
            }
            CheckNearbyAndCollect();
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
                GoStockandGetEquipment(bowStock, citizenRunSpeed);
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
                GoStockandGetEquipment(hamStock, citizenRunSpeed);
            else
                if (currentBehaviorCoroutine == null)
                currentBehaviorCoroutine = StartCoroutine(BuilderBehaviour());
        }

        if (rb.linearVelocity.x > 0)
            sr.flipX = false;
        if(rb.linearVelocity.x < 0)
            sr.flipX = true;

        if (HP <= 0)
            LoseJob();
    }
    private void CheckNearbyAndCollect()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, NPCdetectionRadius, LayerMask.GetMask("coin"));
        if (colliders.Length != 0)
        {
            NPCTarget = colliders[0];
        }
        else
        {
            return;
        }
        if (!NPCMoney)
        {
            Vector3 direction = (new Vector3(NPCTarget.transform.position.x, transform.position.y) - transform.position).normalized;
            rb.linearVelocity = direction * NPCspeed;

        }

        if (Mathf.Abs(transform.position.x - NPCTarget.transform.position.x) < 0.1f)
        {
            NPCMoney = true;
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

    IEnumerator NPCGetCoinWait(Jobs job)
    {
        yield return new WaitUntil(() => moneyCount == 1);
        currentJob = job;
        HP = 100;
        NPCMoney = false;
    }
    void GoStockandGetEquipment(GameObject target, float speed)
    {
        Vector3 direction = (new Vector3(target.transform.position.x, transform.position.y) - transform.position).normalized;
        rb.linearVelocity = direction * speed;
        sr.sprite = none;
        if (Mathf.Abs(transform.position.x - target.transform.position.x) < 0.1f)
        {
            equipment = true;
            if(target==bowStock)
                sr.sprite = archer;
            if (target == hamStock)
                sr.sprite = builder;
        }
    }

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
                isAttacking = false;
                if (!inPatrol)
                    patrolCoroutine = StartCoroutine(Patrol());

                yield return null; // Bir sonraki frame'e geç
                continue;         // Döngüyü yeniden başlat
            }

            // Target varsa saldır
            if (patrolCoroutine != null)
            {
                StopCoroutine(patrolCoroutine);
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
                    ShootArrow(target2);
                    yield return new WaitForSeconds(attackCoolDown);
                    isAttacking = false;
                }
                // Enemy yoksa rabbit kontrolü
                else if (target != null)
                {
                    isAttacking = true;
                    ShootArrow(target);
                    yield return new WaitForSeconds(attackCoolDown);
                    isAttacking = false;
                }
            }

            yield return null; // Her frame'de kontrol
        }
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

        // Okun başlangıç pozisyonu (okçu pozisyonundan biraz yukarıda)
        Vector3 startPos = transform.position;
        GameObject newArrow = newCoinPool.Instance.GetArrow();
        newArrow.transform.position = startPos;
        Rigidbody2D rb = newArrow.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Tavşanın pozisyonu
            Vector3 targetPos = rabbitTarget.transform.position;

            // Yay şeklinde atış için gerekli parametreler
            float gravity = Mathf.Abs(Physics2D.gravity.y);
            float distance = Vector3.Distance(startPos, targetPos);

            // Havada kalma süresini hesapla (isteğe bağlı olarak ayarlanabilir)
            float flightTime = distance / shootForce * 1.5f;

            // Başlangıç hızını hesapla
            Vector2 initialVelocity = CalculateArcVelocity(startPos, targetPos, gravity, flightTime);

            // Okun dönüş açısını ayarla (uçuş yönüne doğru)
            float angle = Mathf.Atan2(initialVelocity.y, initialVelocity.x) * Mathf.Rad2Deg;
            newArrow.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            rb.linearVelocity = initialVelocity;
        }
        else
        {
            Debug.LogError("Ok prefabında Rigidbody2D component'i bulunamadı!");
        }
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
            if (target == null)
            {
                isAttacking = false;
                if (!inPatrol)
                    patrolCoroutine = StartCoroutine(Patrol());
                isBuild = false;
                yield return null; // Bir sonraki frame'e geç
                continue;         // Döngüyü yeniden başlat
            }
            if (patrolCoroutine != null)
            {
                StopCoroutine(patrolCoroutine);
                inPatrol = false;
                calculateDirectionPatrol = false;
            }

            inPatrol = false;
            calculateDirectionPatrol = false;

            if (!isBuild)
            {
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
        Vector3 direction = (new Vector3(target.transform.position.x, transform.position.y) - transform.position).normalized;
        rb.linearVelocity = direction * speed;
        if (Mathf.Abs(transform.position.x - target.transform.position.x) < 0.1f)
        {
            isBuild = true;

        }
    }
    
    IEnumerator Patrol()     ////Ortak fonksiyon
    {
        if(!calculateDirectionPatrol)
        {
            int rightOrLeft = Random.Range(0, 2);
            targetXPatrol = (rightOrLeft == 0) ?
                transform.position.x + patrolWalkDistance :
                transform.position.x - patrolWalkDistance;

            Vector2 targetPosition = new Vector2(targetXPatrol, transform.position.y);
            patrolDirection = (targetPosition - (Vector2)transform.position).normalized;
        }
        calculateDirectionPatrol = true;
        rb.linearVelocity = new Vector2(patrolDirection.x * citizenSpeed, rb.linearVelocity.y);

        yield return new WaitUntil(() =>
            Mathf.Abs(transform.position.x - targetXPatrol) < 0.1f
        );
        inPatrol = true;
        yield return new WaitForSeconds(patrolCoolDown);

        inPatrol = false;
        calculateDirectionPatrol = false;
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
            if (currentJob != Jobs.None)
            {
                if (moneyCount <= 0) return;
                StartCoroutine(GiveMoneyToPlayer());
            }
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
        int coinsToGive = moneyCount;

        for (int i = 0; i < coinsToGive; i++)
        {
            GameObject coinToPlayer = newCoinPool.Instance.GetCoin();
            coinToPlayer.transform.position = transform.position;
            yield return new WaitForSeconds(0.15f);
        }
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
        currentJob = Jobs.None;
        if (CitizensManager.Instance.Builders.Contains(gameObject))
            CitizensManager.Instance.Builders.Remove(gameObject);
        if (CitizensManager.Instance.Archers.Contains(gameObject))
            CitizensManager.Instance.Archers.Remove(gameObject);
        equipment = false;
        inPatrol = false;
        isAttacking = false;
        isBuild = false;
        tag = "NPC";
    }
}
