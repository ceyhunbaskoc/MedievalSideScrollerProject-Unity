using System.Collections;
using UnityEngine;

public class Rabbit : MonoBehaviour
{
    public float patrolWalkDistance,rabbitSpeed,patrolCoolDown;
    SpriteRenderer sr;
    Rigidbody2D rb;
    public bool calculateDirectionRabbit,inPatrol;
    float targetXPatrol;
    Vector2 patrolDirection;
    Animator an;
    float threshold = 0.02f;  // Hýzýn sýfýr kabul edileceði eþik deðeri
    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        an = GetComponent<Animator>();
    }
    private void Update()
    {
        if (!inPatrol)
            StartCoroutine(RabbitBehavior());

        if (Mathf.Abs(rb.linearVelocity.x) > threshold)
        {
            sr.flipX = rb.linearVelocity.x < 0;
            an.SetBool("isWalking", true);
        }
        else
        {
            an.SetBool("isWalking", false);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("arrow"))
        {
            GameObject dropCoin=null;
            if (!an.GetBool("dead"))
            {
                dropCoin = newCoinPool.Instance.GetCoin();
                dropCoin.transform.position = transform.position;
            }
            an.SetBool("dead", true);
        }
    }
    public void Death()
    {
        GameManager.Instance.spawnedRabbits.Remove(gameObject);
        an.SetBool("dead", false);
        an.SetBool("isWalking", false);
        newCoinPool.Instance.DisableRabbit(gameObject);

    }
    IEnumerator RabbitBehavior()
    {
        if (!calculateDirectionRabbit)
        {
            int rightOrLeft = Random.Range(0, 2);
            targetXPatrol = (rightOrLeft == 0) ?
                transform.position.x + patrolWalkDistance :
                transform.position.x - patrolWalkDistance;

            Vector2 targetPosition = new Vector2(targetXPatrol, transform.position.y);
            patrolDirection = (targetPosition - (Vector2)transform.position).normalized;
        }
        calculateDirectionRabbit = true;
        rb.linearVelocity = new Vector2(patrolDirection.x * rabbitSpeed, rb.linearVelocity.y);

        yield return new WaitUntil(() =>
            Mathf.Abs(transform.position.x - targetXPatrol) < 0.1f
        );
        inPatrol = true;
        yield return new WaitForSeconds(patrolCoolDown);

        inPatrol = false;
        calculateDirectionRabbit = false;
    }
}
