using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNightCycle : MonoBehaviour
{
    public Transform sun;       // G�ne� objesi
    public Transform moon;      // Ay objesi
    public Light2D globalLight; // Ortam �����
    public GameObject parallaxRoot; // T�m parallax layer'lar�n parent'� (Inspector'dan atay�n)

    [Header("Cycle Settings")]
    public float rotationSpeed = 1f;
    public Color sunriseColor = new Color(1f, 0.5f, 0.2f);
    public Color dayColor = new Color(1f, 1f, 1f);
    public Color sunsetColor = new Color(0.9f, 0.4f, 0.3f);
    public Color nightColor = new Color(0.1f, 0.1f, 0.3f);

    private SpriteRenderer[] allParallaxLayers; // T�m parallax SpriteRenderer'lar
    private GameObject cinemachineCam;
    public GameObject[] flight;
    public ForestGlobalLight[] fGlight;

    void Start()
    {
        cinemachineCam = GameObject.Find("CinemachineCamera");
        // Parallax root'un alt�ndaki T�M SpriteRenderer'lar� al (aktif/pasif fark etmez)
        if (parallaxRoot != null)
        {
            allParallaxLayers = parallaxRoot.GetComponentsInChildren<SpriteRenderer>(true);
        }
        else
        {
            Debug.LogError("ParallaxRoot is not assigned in Inspector!");
        }
        flight = GameObject.FindGameObjectsWithTag("forestGlight");
        fGlight = new ForestGlobalLight[flight.Length];
        for (int i = 0;i<flight.Length;i++)
        {
            fGlight[i] = flight[i].GetComponent<ForestGlobalLight>();
        }
    }

    void Update()
    {
        UpdateSunAndMoon();
        foreach(var Glight in fGlight)
        {
            if (!Glight.characterInForest)
            {
                UpdateLighting();
                break;
            }
                
        }
            
    }

    void UpdateSunAndMoon()
    {
        float angle = (GameManager.Instance.currentTime / 24f) * 360f;
        sun.position = RotateAround(new Vector3(cinemachineCam.transform.position.x, cinemachineCam.transform.position.y,0), Vector3.forward, angle, 10f);
        moon.position = RotateAround(new Vector3(cinemachineCam.transform.position.x, cinemachineCam.transform.position.y, 0), Vector3.forward, angle + 180f, 10f);
    }

    Vector3 RotateAround(Vector3 center, Vector3 axis, float angle, float radius)
    {
        Vector3 offset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0) * radius;
        return center + offset;
    }

    void UpdateLighting()
    {
        GameManager.DayPhase phase = GameManager.Instance.currentPhase;
        float t = Mathf.InverseLerp(5f, 8f, GameManager.Instance.currentTime); // Yumu�ak ge�i�

        Color targetColor = nightColor;
        float targetIntensity = 0.2f;

        switch (phase)
        {
            case GameManager.DayPhase.Sunrise:
                targetColor = Color.Lerp(nightColor, sunriseColor, t);
                targetIntensity = Mathf.Lerp(0.2f, 0.7f, t);
                break;
            case GameManager.DayPhase.Day:
                targetColor = dayColor;
                targetIntensity = 1f;
                break;
            case GameManager.DayPhase.Sunset:
                t = Mathf.InverseLerp(18f, 20f, GameManager.Instance.currentTime);
                targetColor = Color.Lerp(dayColor, sunsetColor, t);
                targetIntensity = Mathf.Lerp(1f, 0.4f, t);
                break;
            case GameManager.DayPhase.Night:
                targetColor = nightColor;
                targetIntensity = 0.2f;
                break;
        }

        // I��k ve renk ge�i�leri
        globalLight.intensity = Mathf.Lerp(globalLight.intensity, targetIntensity, Time.deltaTime * 2f);

        // T�M parallax layer'lara renk uygula
        if (allParallaxLayers != null)
        {
            foreach (SpriteRenderer layer in allParallaxLayers)
            {
                if (layer != null)
                {
                    // Alpha de�erini koru (�rne�in: bulutlar yar� saydam kalabilir)
                    Color newColor = new Color(targetColor.r, targetColor.g, targetColor.b, layer.color.a);
                    layer.color = Color.Lerp(layer.color, newColor, Time.deltaTime * 2f);
                }
            }
        }
    }
}
