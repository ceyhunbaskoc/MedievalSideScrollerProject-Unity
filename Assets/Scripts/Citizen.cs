using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Citizen : MonoBehaviour
{
    public enum Job { None,Archer,Builder}
    public Job currentJob = Job.None;
    public int collectedMoney = 0;
    public float CitizenSpeed,builderGoConstSpeed,attackRange,attackCoolDown,patrolWalkDistance,patrolCoolDown,detectConstrucitonRange,constructionCoolDown;
    public float shootForce = 20f; // Okun f�rlatma h�z�
    public float shootAngle = 45f; // Okun f�rlatma a��s� (derece cinsinden)
    public GameObject arrowPrefab;
    CitizensManager manager;
    coinPool pool;
    Rigidbody2D rbCitizen;
    public bool isAttacking = false,isGoConstruction = false,patrolStarted=false,isGoTakeJob;
    private GameObject constructionTarget, builderTarget;
    Vector2 TakeJobTarget;
    Job whatJob;
    private Coroutine currentBehaviorCoroutine;
    private Coroutine patrolCoroutine;
    Collider2D citizenCollider;
    private void Start()
    {
        manager = GameObject.Find("CitizensManager").GetComponent<CitizensManager>();
        pool = GameObject.FindWithTag("coinPool").GetComponent<coinPool>();
        rbCitizen = GetComponent<Rigidbody2D>();
        citizenCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if(gameObject.CompareTag("NPC"))
        {
            tag = "citizen";
        }
        if (currentJob == Job.None)
        {
            if (!isGoTakeJob)
            {
            }

            if (Mathf.Abs(TakeJobTarget.x - transform.position.x) < 0.1f && isGoTakeJob)
            {
                print("0.1f mesafe ve isgotake job aktif.");
                rbCitizen.linearVelocity = new Vector2(0, rbCitizen.linearVelocity.y);
                AssignJob(whatJob);
                isGoTakeJob = false;
            }
        }

        if (isGoConstruction)
        {
            CheckIfReachedTarget();
        }
    }
    

    public void GoBow()
    {
        isGoTakeJob = true;
        Vector2 destinationBow = GameObject.Find("BowStock").transform.position;
        TakeJobTarget = destinationBow;
        whatJob = Job.Archer;
        Vector2 direction = destinationBow - (Vector2)transform.position;
        direction.y = 0;
        direction = direction.normalized;

        rbCitizen.linearVelocity = new Vector2(direction.x * CitizenSpeed, rbCitizen.linearVelocity.y);
    }
    public void GoHammer()
    {
        isGoTakeJob = true;
        Vector2 destinationHammer = GameObject.Find("HammerStock").transform.position;
        TakeJobTarget = destinationHammer;
        whatJob = Job.Builder;
        Vector2 direction = destinationHammer - (Vector2)transform.position;
        direction.y = 0;
        direction = direction.normalized;

        rbCitizen.linearVelocity = new Vector2(direction.x * CitizenSpeed, rbCitizen.linearVelocity.y);
    }

    public void AssignJob(Job newJob)
    {
        // Var olan behavior i durdur
        if (currentBehaviorCoroutine != null)
        {
            StopCoroutine(currentBehaviorCoroutine);
        }
        if (patrolCoroutine != null)
        {
            StopCoroutine(patrolCoroutine);
            patrolStarted = false;
        }

        currentJob = newJob;
        Debug.Log("vatanda�a meslek atand� = " + newJob);

        switch (currentJob)
        {
            case Job.Archer:
                currentBehaviorCoroutine = StartCoroutine(ArcherBehavior());
                break;
            case Job.Builder:
                currentBehaviorCoroutine = StartCoroutine(BuilderBehavior());
                break;
        }
    }
    IEnumerator ArcherBehavior()
    {
        while (currentJob == Job.Archer)
        {
            GameObject target = FindNearestRabbit();
            if (target != null)
            {
                print("target boş değil");
                Attack(target);
                yield return new WaitForSeconds(attackCoolDown);
            }
            else if (!patrolStarted)
            {
                print("target boş , patrol başlıyor.");
                patrolStarted = true;
                patrolCoroutine = StartCoroutine(Patrol());
                yield return patrolCoroutine; // Patrol'un bitmesini bekle
            }
            else
            {
                yield return null;
            }
        }
    }

    IEnumerator BuilderBehavior()
    {
        while (currentJob == Job.Builder)
        {
            GameObject target = FindNearestConstruction();
            builderTarget = target;
            if (target != null)
            {
                GoToConstruction(target);
                yield return new WaitForSeconds(constructionCoolDown);
            }
            else if (!patrolStarted)
            {
                patrolStarted = true;
                patrolCoroutine = StartCoroutine(Patrol());
                yield return patrolCoroutine; // Patrol'�n bitmesini bekle
            }
            else
            {
                yield return null; // �nemli: Yapacak bir �ey yoksa yield et
            }
        }
    }

    IEnumerator Patrol()
    {
        int rightOrLeft = Random.Range(0, 2);
        float targetX = (rightOrLeft == 0) ?
            transform.position.x + patrolWalkDistance :
            transform.position.x - patrolWalkDistance;

        Vector2 targetPosition = new Vector2(targetX, transform.position.y);
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        rbCitizen.linearVelocity = new Vector2(direction.x * CitizenSpeed, rbCitizen.linearVelocity.y);

        yield return new WaitUntil(() =>
            Mathf.Abs(transform.position.x - targetX) < 0.1f
        );

        rbCitizen.linearVelocity = new Vector2(0, rbCitizen.linearVelocity.y);
        yield return new WaitForSeconds(patrolCoolDown);

        patrolStarted = false;
    }
    
    ////////////////////////// Okcular icin//////////////////////////////////////// 
    private GameObject FindNearestRabbit()
    {
        // Etraftaki tavsanlari bul
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, attackRange, LayerMask.GetMask("Rabbit"));
        GameObject nearestRabbit = null;
        float nearestDistance = Mathf.Infinity;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Rabbit")) // Tav�anlar�n tag'� "Rabbit" olmal�
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestRabbit = hitCollider.gameObject;
                }
            }
        }

        return nearestRabbit;
    }

    private void Attack(GameObject target)
    {
        if (!isAttacking)
        {
            isAttacking = true;
            Debug.Log("Ok�u tav�ana sald�r�yor: " + target.name);

            // Sald�r� animasyonu veya efektleri burada tetiklenebilir
            ShootArrow(target);

            StartCoroutine(ResetAttack());

        }
    }
    IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(attackCoolDown);
        isAttacking = false;
    }

    public void ShootArrow(GameObject rabbitTarget)
    {
        if (rabbitTarget == null) return;

        // Okun başlangıç pozisyonu (okçu pozisyonundan biraz yukarıda)
        Vector3 startPos = transform.position;
        GameObject arrow = Instantiate(arrowPrefab, startPos, Quaternion.identity);

        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
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
            arrow.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("character"))
        {
            if (currentJob != Job.None)
            {
                GiveMoneyToPlayer();
            }
        }
    }
    ////////////////////////////////////////////////////////////////////////////////

    ///////////////////////////Insaatcilar icin/////////////////////////////////////

    private GameObject FindNearestConstruction()
    {
        // Etraftaki tav�anlar� bul
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, attackRange, LayerMask.GetMask("Construction"));
        GameObject nearestConstruction = null;
        float nearestDistance = Mathf.Infinity;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Construciton")) // �n�aatlar�n tag'� "Construciton" olmal�
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestConstruction = hitCollider.gameObject;
                }
            }
        }
        return constructionTarget;
    }

    private void GoToConstruction(GameObject target)
    {
        if (!isGoConstruction)
        {
            isGoConstruction = true;
            Debug.Log("�n�aat�� in�aata gidiyor: " + target.name);

            //�n�aata git
            Vector2 direction = target.transform.position-transform.position;
            direction.y = 0;
            Vector2 velocity = direction.normalized * builderGoConstSpeed;
            rbCitizen.linearVelocity = new Vector2(velocity.x, rbCitizen.linearVelocity.y);


        }
    }
    private void CheckIfReachedTarget()
    {
        // Hedefe ula��ld� m�?
        if (Mathf.Abs(builderTarget.transform.position.x - transform.position.x) < 0.1f)
        {
            rbCitizen.linearVelocity = new Vector2(0, rbCitizen.linearVelocity.y);
            StartConstruction();
        }
    }


    private void StartConstruction()
    {
        // �n�aat i�lemleri burada yap�l�r
        
        Debug.Log("�n�aat tamamland�!");
    }
    public void GiveMoneyToPlayer()
    {
            if (collectedMoney <= 0) return;
            //Debug.Log("Vatanda� oyuncuya " + collectedMoney + " para verdi.");

            StartCoroutine(WaitOtherCoin());
        

    }
    IEnumerator WaitOtherCoin()
    {
        int coinsToGive = collectedMoney;

        Collider2D citizenCollider = GetComponent<Collider2D>();
        citizenCollider.enabled = false;

        for (int i = 0; i < coinsToGive; i++)
        {
            GameObject coinToPlayer = pool.GetCoin();
            //print("oyuncuya para atildi "+i);
            coinToPlayer.transform.position = transform.position;
            yield return new WaitForSeconds(0.15f);
        }
        StartCoroutine(OpenCollider());
        collectedMoney = 0;
    }
    IEnumerator OpenCollider()
    {
        yield return new WaitForSeconds(1.15f);
        citizenCollider.enabled = true;
    }
}
