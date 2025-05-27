using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class character : MonoBehaviour
{
    public float horseWalkSpeed, horseRunSpeed, canRunTime;
    public float /*transitionTime,*/ waitOtherCoinsTime;
    public bool isRun, checkLeft,checkRight, onBuildable = false, Cconflict=false;
    public int moneyCount = 0;
    GameObject buildableObject;
    Rigidbody2D rb;
    coinPool pool;
    void Start()
    {
        pool = FindFirstObjectByType<coinPool>();
        rb = GetComponent<Rigidbody2D>();
    }

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
        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            if(onBuildable)
            {
                if(moneyCount > 0)
                {
                    GameObject coin = pool.GetCoin();
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
                if(moneyCount>0)
                {
                    GameObject coin = pool.GetCoin();
                    coin coinBool = coin.GetComponent<coin>();
                    if(Cconflict)
                    {
                        coinBool.characterCoin = true;
                    }
                    coin.transform.position = transform.position;
                    coin.GetComponent<coin>().canRemove = false; // Ekstra güvenlik
                    moneyCount--;
                }
            
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("buildable"))
        { 
            buildableObject = collision.gameObject;
            onBuildable = true;
        }
        if(collision.gameObject.CompareTag("citizen"))
        {
            Cconflict = true;
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
        if (collision.gameObject.CompareTag("citizen"))
        {
            Cconflict = false;
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


