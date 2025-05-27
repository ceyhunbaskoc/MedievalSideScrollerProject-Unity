using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private float length, startPos;
    public GameObject camera;
    public float parallaxEffect;

    void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;

        // Oyun baþladýðýnda arka planý hemen doðru konuma yerleþtir
        float distance = camera.transform.position.x;
        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);
    }

    void Update()
    {
        float temp = camera.transform.position.x * (1 - parallaxEffect);
        float distance = camera.transform.position.x * parallaxEffect;

        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);

        if (temp > startPos + length / 2)
        {
            startPos += length;
        }
        else if (temp < startPos - length / 2)
        {
            startPos -= length;
        }
    }
}
