using System.Collections;
using UnityEngine;

public class characterConflict : MonoBehaviour
{
    public int moneyCount = 0;
    [Header("Bools")]
    public bool isRun;
    public bool checkLeft, checkRight, onBuildable = false, Cconflict = false;
    [Space]
    public float horseWalkSpeed;
    public float horseRunSpeed, canRunTime, buildingWaitothcoin;
    GameObject buildableObject;
    Rigidbody2D rb;
    SpriteRenderer sr;
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (checkLeft)
            {
                isRun = true;
                rb.linearVelocityX = -horseRunSpeed * Time.fixedDeltaTime;
            }
            else
            {

                rb.linearVelocityX = -horseWalkSpeed * Time.fixedDeltaTime;
            }
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            isRun = false;
            StartCoroutine(LRunCheck());
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            if (checkRight)
            {
                isRun = true;
                rb.linearVelocityX = horseRunSpeed * Time.fixedDeltaTime;
            }
            else
            {

                rb.linearVelocityX = horseWalkSpeed * Time.fixedDeltaTime;
            }
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            isRun = false;
            StartCoroutine(RRunCheck());
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (onBuildable && buildableObject.GetComponent<buildableObject>().centerDistanceCheck)
            {
                if (moneyCount > 0)
                {
                    GameObject coin = newCoinPool.Instance.GetCoin();
                    buildableObject bo = buildableObject.GetComponent<buildableObject>();
                    int currentSlot = bo.currentCoinCount;
                    coin.transform.position = transform.position;
                    bo.PlaceCoin(coin);
                    moneyCount--;
                    bo.CheckIfAllSlotsFilled(currentSlot);
                    //StartCoroutine(CoinDropOnBuildable(currentSlot));
                }
            }
            else
            {
                if (moneyCount > 0)
                {
                    GameObject coin = newCoinPool.Instance.GetCoin();
                    coin.transform.position = transform.position;
                    moneyCount--;
                }

            }
        }
        if (rb.linearVelocity.x > 0)
            sr.flipX = false;
        if (rb.linearVelocity.x < 0)
            sr.flipX = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("buildable"))
        {
            buildableObject = collision.gameObject;
            onBuildable = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("buildable"))
        {
            onBuildable = false;
            buildableObject bo = buildableObject.GetComponent<buildableObject>();
            bo.DropCoins(bo.currentCoinCount);
        }
    }
    IEnumerator LRunCheck()
    {
        checkLeft = true;
        yield return new WaitForSeconds(canRunTime);
        if (!isRun)
        {
            checkLeft = false;
        }


    }
    IEnumerator RRunCheck()
    {
        checkRight = true;
        yield return new WaitForSeconds(canRunTime);
        if (!isRun)
        {
            checkRight = false;
        }


    }
}


