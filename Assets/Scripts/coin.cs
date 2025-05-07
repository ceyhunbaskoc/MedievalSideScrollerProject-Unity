using System.Collections;
using UnityEngine;
using static System.TimeZoneInfo;

public class coin : MonoBehaviour
{
    public LayerMask groundLayer;
    public float colliderOpenTime, previousGravity;
    character cr;
    Rigidbody2D rb;
    public bool canRemove=false,characterCoin=false;
    public float sayac=0,collectTransitionTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cr=GameObject.FindWithTag("character").GetComponent<character>();
        previousGravity = rb.gravityScale;
    }
    private void Update()
    {
        if(gameObject.activeSelf)
        {
            if(sayac==0)
            {
                if (!cr.onBuildable)
                {
                    StartCoroutine(Wait());
                }

            }
        }
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, transform.localScale.y / 2,groundLayer);
        if(hit.collider !=null)
        {
            rb.gravityScale = 0;
            rb.linearVelocity = new Vector2(0, 0);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        RemoveCoinFunction(collision);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        RemoveCoinFunction(collision);
    }
    IEnumerator Wait()
    {
        canRemove = false;
        yield return new WaitForSeconds(colliderOpenTime);
        if (gameObject.activeSelf)
        {
            canRemove = true;
            sayac++;
        }

    }
    void RemoveCoinFunction(Collider2D collision)
    {
        if (canRemove)
        {
            print("can remove true");
            if (characterCoin)
            {
                if (collision.gameObject.CompareTag("citizen"))
                {
                    print("coin citizena deðdi");
                    Citizen citizen = collision.gameObject.GetComponent<Citizen>();
                    print("canRemove=true");
                    canRemove = false;
                    rb.gravityScale = 0.4f;
                    StartCoroutine(CollectTransitionToCitizen(collision, citizen));
                    return;
                }
            }
            else
            {
                if (collision.gameObject.CompareTag("character"))
                {
                    print("coin character e deðdi");
                    canRemove = false;
                    rb.gravityScale = 0.4f;
                    StartCoroutine(CollectTransitionToPlayer(collision));
                    return;
                }
            }
        }

    }
    IEnumerator CollectTransitionToPlayer(Collider2D collider)
    {
        float elapsedTime = 0;
        Vector2 startingPos = transform.position;
        while (elapsedTime < collectTransitionTime)
        {
            transform.position = Vector2.Lerp(startingPos, collider.transform.position, (elapsedTime / collectTransitionTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = collider.transform.position;
        cr.moneyCount++;
        canRemove = false;
        characterCoin = false;
        gameObject.SetActive(false);
    }
    IEnumerator CollectTransitionToCitizen(Collider2D collider,Citizen citizen)
    {
        print("collectTransitionTo Citizena girdi");
        float elapsedTime = 0;
        Vector2 startingPos = transform.position;
        while (elapsedTime < collectTransitionTime)
        {
            transform.position = Vector2.Lerp(startingPos, collider.transform.position, (elapsedTime / collectTransitionTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        print("yield return null sonrasý");
        transform.position = collider.transform.position;
        citizen.collectedMoney++;
        Debug.Log("vatandaþ para aldý");
        canRemove = false;
        characterCoin = false;
        gameObject.SetActive(false);
    }
}
