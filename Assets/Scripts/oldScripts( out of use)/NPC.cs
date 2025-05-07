using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
    public bool isRecruited = false,npcPatrol=true;
    public float NPCspeed, patrolWalkDistance,patrolCoolDown;
    Citizen citizenScript;
    NPC npcScript;
    CitizensManager manager;
    NavMeshAgent agent;
    Rigidbody2D rbNPC;
    private void Start()
    {
        citizenScript = GetComponent<Citizen>();
        npcScript = GetComponent<NPC>();
        manager = GameObject.Find("CitizensManager").GetComponent<CitizensManager>();
        rbNPC = GetComponent<Rigidbody2D>();
        if (!isRecruited)
        {
            StartCoroutine(Patrol());
        }
    }
    private void Update()
    {
        
    }
    IEnumerator Patrol()
    {
        if(npcPatrol)
        {
            int rightOrLeft = Random.Range(0, 2);
            print(rightOrLeft);
            float targetX = (rightOrLeft == 0) ?
            transform.position.x + patrolWalkDistance :
            transform.position.x - patrolWalkDistance;

            Vector2 targetPosition = new Vector2(targetX, transform.position.y);

            // Hareketi baþlat
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
            rbNPC.linearVelocity = new Vector2(direction.x * NPCspeed, rbNPC.linearVelocity.y);

            // Hedefe ulaþana kadar bekle
            yield return new WaitUntil(() =>
                Mathf.Abs(transform.position.x - targetX) < 0.1f
            );

            // Hareketi durdur
            rbNPC.linearVelocity = new Vector2(0, rbNPC.linearVelocity.y);

            // Bekleme süresi
            yield return new WaitForSeconds(patrolCoolDown);

            // Yeni patrol baþlat

            StartCoroutine(Patrol());
        }
        
        
    }
    public void Recruit()
    {
        if(!isRecruited)
        {
            isRecruited=true;
            print("NPC vatandaþ oldu!");
            citizenScript.enabled=true;
            manager.None.Add(gameObject);
            npcPatrol = false;
            npcScript.enabled=false;

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("coin"))
        {
            Recruit();
            collision.gameObject.SetActive(false);
            citizenScript.collectedMoney++;
        }
    }
}
