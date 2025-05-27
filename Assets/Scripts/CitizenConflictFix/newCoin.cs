using System.Collections;
using UnityEngine;

public class newCoin : MonoBehaviour
{
    public float allowCollectTime, collectTransitionTime;
    public bool newBorn = true, canCollect = false, conflictCharacter, conflictCitizen,conflictNPC, hasStartedCollect=false;
    characterConflict cr;
    GameObject currentCitizen;
    Cconflict citizen;
    void Start()
    {
        cr = FindFirstObjectByType<characterConflict>();
    }

    void Update()
    {
        if(currentCitizen != null)
        {
            citizen = currentCitizen.GetComponent<Cconflict>();
        }
        if (newBorn)
        {
            StartCoroutine(AllowCollect());
        }
        if (canCollect&&!hasStartedCollect)
        {
            if (conflictCharacter && !conflictCitizen)
            {
                hasStartedCollect = true;
                StartCoroutine(CollectTransitionToObject(cr.gameObject));
            }
            else if (conflictCitizen && !conflictCharacter)
            {
                hasStartedCollect = true;
                StartCoroutine(CollectTransitionToObject(currentCitizen));
            }
            else if (conflictCitizen && conflictCitizen)
            {
                hasStartedCollect = true;
                StartCoroutine(CollectTransitionToObject(cr.gameObject));
            }
            else if(conflictNPC&&conflictCharacter)
            {
                hasStartedCollect = true;
                StartCoroutine(CollectTransitionToObject(currentCitizen));
            }
            else if(conflictNPC&&!conflictCharacter)
            {
                hasStartedCollect = true;
                StartCoroutine(CollectTransitionToObject(currentCitizen));
            }
        }
    }
    IEnumerator AllowCollect()
    {
        newBorn = false;
        yield return new WaitForSeconds(allowCollectTime);
        canCollect = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("character"))
        {
            conflictCharacter = true;
        }
        if (collision.gameObject.CompareTag("citizen")&&collision.gameObject.GetComponent<Cconflict>().equipment)
        {
            conflictCitizen = true;
            currentCitizen = collision.gameObject;
        }
        if (collision.gameObject.CompareTag("NPC")&&!collision.gameObject.GetComponent<Cconflict>().NPCMoney)
        {
            conflictNPC = true;
            currentCitizen = collision.gameObject;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("character"))
        {
            conflictCharacter = false;
        }
        if (collision.gameObject.CompareTag("citizen"))
        {
            conflictCitizen = false;
            currentCitizen = null;
        }
        if (collision.gameObject.CompareTag("NPC"))
        {
            conflictNPC = false;
            currentCitizen = null;
        }
    }
    IEnumerator CollectTransitionToObject(GameObject o)
    {
        if (o == null)
        {
            hasStartedCollect = false;
            yield break; // Hedef yoksa çýk
        }

        float elapsedTime = 0;
        Vector2 startingPos = transform.position;
        while (elapsedTime < collectTransitionTime)
        {
            if (o == null)
            {
                hasStartedCollect = false;
                yield break;
            }// Hedef yoksa çýk
            transform.position = Vector2.Lerp(startingPos, o.transform.position, (elapsedTime / collectTransitionTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        if (o != null)
            transform.position = o.transform.position;

        if (cr != null && o == cr.gameObject)
        {
            cr.moneyCount++;
        }
        //print("currentCitizen != null = " + (currentCitizen != null));
        //print("currentCitizen != null = " + (currentCitizen != null));
        //print("o == currentCitizen = " + (o == currentCitizen));
        if (currentCitizen != null && citizen != null && o == currentCitizen)
        {
            citizen.moneyCount++;
            if(citizen.currentJob==Cconflict.Jobs.None)
            {
                citizen.NPCMoney = true;
                //print("NPC MONEY TRUE OLDU");
            }
        }
        hasStartedCollect = false;
        newCoinPool.Instance.DisableCoin(gameObject);
    }

}
