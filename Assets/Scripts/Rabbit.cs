using UnityEngine;

public class Rabbit : MonoBehaviour
{
    SpriteRenderer sr;
    Rigidbody2D rb;
    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if (rb.linearVelocity.x > 0)
            sr.flipX = false;
        if (rb.linearVelocity.x < 0)
            sr.flipX = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("arrow"))
        {
            GameObject dropCoin = newCoinPool.Instance.GetCoin();
            dropCoin.transform.position = transform.position;
            Destroy(gameObject);
        }
    }
}
