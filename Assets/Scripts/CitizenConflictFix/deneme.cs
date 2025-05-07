using UnityEngine;

public class deneme : MonoBehaviour
{

    private void Update()
    {
        GameObject nearest = FindNearest(10, "Rabbit", LayerMask.GetMask("Rabbit"));
        print(nearest);
    }
    private GameObject FindNearest(float range, string tag, LayerMask rabbitLayer)  ///Ortak fonksiyon
    {
        // Etraftaki tav�anlar� bul
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, range, rabbitLayer);
        GameObject nearest = null;
        float nearestDistance = Mathf.Infinity;
        print("Lenght = " + hitColliders.Length);
        foreach (var hitCollider in hitColliders)
        {
            Debug.Log($"Found object: {hitCollider.gameObject.name}, Tag: {hitCollider.tag}, Layer: {LayerMask.LayerToName(hitCollider.gameObject.layer)}");
            if (hitCollider.CompareTag(tag)) // �n�aatlar�n tag'� "Construciton" olmal�
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = hitCollider.gameObject;
                }
            }
        }
        print("nearest = " + nearest);
        return nearest;
    }
}
