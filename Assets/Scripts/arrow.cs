using UnityEngine;

public class arrow : MonoBehaviour
{
    public float damage;
    Rigidbody2D rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            rb.rotation = angle; // Rigidbody rotasyonunu direkt ayarla
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("ground"))
        {
            newCoinPool.Instance.DisableArrow(gameObject);
        }
        if(collision.gameObject.CompareTag("enemy"))
        {
            if (collision.gameObject.name.Contains(newCoinPool.Instance.enemyPrefab1.name))
            {
                enemy enemyScript1 = collision.gameObject.GetComponent<enemy>();
                collision.gameObject.GetComponent<Animator>().SetBool("damageTaken", true);
                enemyScript1.HP -= damage;
            }
        }
    }
}
