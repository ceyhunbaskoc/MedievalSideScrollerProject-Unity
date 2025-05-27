using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ForestGlobalLight : MonoBehaviour
{
    public float forestGlobalIntensity,changeSpeed;
    public bool characterInForest;
    Light2D globalLight;
    private characterConflict cr;


    void Start()
    {
        cr = GameObject.FindFirstObjectByType<characterConflict>().GetComponent<characterConflict>();
        globalLight = GameObject.Find("GlobalLight").GetComponent<Light2D>();
    }
    private void Update()
    {
        if(characterInForest)
        {
            float currentIntensity = globalLight.intensity;
            float newIntensity = Mathf.Lerp(currentIntensity, forestGlobalIntensity, Time.deltaTime * changeSpeed);
            globalLight.intensity = newIntensity;
            cr.transform.Find("characterTorch").gameObject.SetActive(true);
            float characterCurrentIntensity = cr.transform.Find("characterTorch").gameObject.GetComponent<Light2D>().intensity;
            float characterNewIntensity = Mathf.Lerp(characterCurrentIntensity, 1.5f, Time.deltaTime * changeSpeed);
            cr.transform.Find("characterTorch").gameObject.GetComponent<Light2D>().intensity = characterNewIntensity;
        }
        else
        {
            float characterCurrentIntensity = cr.transform.Find("characterTorch").gameObject.GetComponent<Light2D>().intensity;
            float characterNewIntensity = Mathf.Lerp(characterCurrentIntensity, 0, Time.deltaTime * changeSpeed);
            cr.transform.Find("characterTorch").gameObject.GetComponent<Light2D>().intensity = characterNewIntensity;
            if (cr.transform.Find("characterTorch").gameObject.GetComponent<Light2D>().intensity <=0.1f)
                cr.transform.Find("characterTorch").gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("character"))
        {
            characterInForest = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("character"))
        {
            characterInForest = false;
        }
    }

}
