using System.Collections;
using UnityEngine;

public class newCoin : MonoBehaviour
{
    public float allowCollectTime, collectTransitionTime;
    public bool newBorn = true, canCollect = false, conflictCharacter, conflictCitizen,conflictNPC;
    characterConflict cr;
    GameObject currentCitizen;
    Cconflict citizen;
    void Start()
    {
        cr = FindObjectOfType<characterConflict>();
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
        if (canCollect)
        {
            if (conflictCharacter && !conflictCitizen)
            {
                StartCoroutine(CollectTransitionToObject(cr.gameObject));
            }
            else if (conflictCitizen && !conflictCharacter)
            {
                StartCoroutine(CollectTransitionToObject(currentCitizen));
            }
            else if (conflictCitizen && conflictCitizen)
            {
                StartCoroutine(CollectTransitionToObject(cr.gameObject));
            }
            else if(conflictNPC&&conflictCharacter)
                StartCoroutine(CollectTransitionToObject(currentCitizen));
            else if(conflictNPC&&!conflictCharacter)
                StartCoroutine(CollectTransitionToObject(currentCitizen));
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
        if (collision.gameObject.CompareTag("citizen"))
        {
            conflictCitizen = true;
            currentCitizen = collision.gameObject;
        }
        if (collision.gameObject.CompareTag("NPC"))
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
        float elapsedTime = 0;
        Vector2 startingPos = transform.position;
        while (elapsedTime < collectTransitionTime)
        {
            transform.position = Vector2.Lerp(startingPos, o.transform.position, (elapsedTime / collectTransitionTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = o.transform.position;
        if(o==cr.gameObject) { cr.moneyCount++; }
        if (o == currentCitizen) { citizen.moneyCount++; }
        newCoinPool.Instance.DisableCoin(gameObject);
    }
}
