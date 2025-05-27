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
    Animator an;
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        an= GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, WaveManager.Instance.spawnPoints[1].position.x, WaveManager.Instance.spawnPoints[0].position.x),
            transform.position.y,
            transform.position.z
            );
        if (!GameManager.Instance.tutorialOpen)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                an.SetBool("isRun", true);
                if (checkLeft)
                {
                    isRun = true;
                    an.SetBool("isRun2", true);
                    rb.linearVelocityX = -horseRunSpeed * Time.fixedDeltaTime;
                }
                else
                {

                    rb.linearVelocityX = -horseWalkSpeed * Time.fixedDeltaTime;
                }
            }
            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                an.SetBool("isRun", false);
                an.SetBool("isRun2", false);
                isRun = false;
                StartCoroutine(LRunCheck());
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                an.SetBool("isRun", true);
                if (checkRight)
                {
                    isRun = true;
                    an.SetBool("isRun2", true);
                    rb.linearVelocityX = horseRunSpeed * Time.fixedDeltaTime;
                }
                else
                {

                    rb.linearVelocityX = horseWalkSpeed * Time.fixedDeltaTime;
                }
            }
            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                an.SetBool("isRun", false);
                an.SetBool("isRun2", false);
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
                        Animator an = coin.GetComponent<Animator>();
                        an.SetBool("inSlot", true);
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
        }
        if (rb.linearVelocity.x > 0)
            sr.flipX = false;
        if (rb.linearVelocity.x < 0)
            sr.flipX = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("buildable")|| collision.gameObject.CompareTag("archerTower")||
            collision.gameObject.CompareTag("Wall")||collision.gameObject.CompareTag("CityCenter") ||
            collision.gameObject.CompareTag("Stock"))
        {
            onBuildable = true;
            buildableObject = collision.gameObject;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("buildable") || collision.gameObject.CompareTag("archerTower") ||
            collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("CityCenter") ||
            collision.gameObject.CompareTag("Stock"))
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


