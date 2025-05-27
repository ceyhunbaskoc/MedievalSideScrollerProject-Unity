using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class ArcherTower : MonoBehaviour
{
    private bool isAttacking=false,isConstruction;
    Coroutine currentBehaviorCoroutine;

    public GameObject arrowPrefab;
    public float attackRange;
    public float shootForce;
    public float damage;
    public float attackCoolDown;

    

    private void Start()
    {
        transform.position = new Vector2(transform.position.x, -0.6650324f);
    }
    private void Update()
    {
        if (currentBehaviorCoroutine == null)
            currentBehaviorCoroutine = StartCoroutine(ArcherTowerBehavior());
    }
    IEnumerator ArcherTowerBehavior()
    {
        while(!isConstruction)
        {
            GameObject nearestEnemy = FindNearest(attackRange,"enemy");
            if (!isAttacking)
            {
                if (nearestEnemy != null)
                {
                    isAttacking = true;
                    ShootArrow(nearestEnemy);
                    yield return new WaitForSeconds(attackCoolDown);
                    isAttacking = false;
                }
            }
            yield return null;
        }
    }
    private GameObject FindNearest(float range, string tag)  ///Ortak fonksiyon
    {
        // Etraftaki tav�anlar� bul
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, range);
        GameObject nearest = null;
        float nearestDistance = Mathf.Infinity;
        foreach (var hitCollider in hitColliders)
        {
            //Debug.Log($"Found object: {hitCollider.gameObject.name}, Tag: {hitCollider.tag}, Layer: {LayerMask.LayerToName(hitCollider.gameObject.layer)}");
            if (hitCollider.CompareTag(tag))
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = hitCollider.gameObject;
                }
            }
        }
        return nearest;
    }

    public void ShootArrow(GameObject rabbitTarget)
    {
        if (rabbitTarget == null) return;

        // Okun başlangıç pozisyonu (okçu pozisyonundan biraz yukarıda)
        Vector3 startPos = transform.position;
        GameObject newArrow = newCoinPool.Instance.GetArrow();
        newArrow.transform.position = startPos;
        Rigidbody2D rb = newArrow.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Tavşanın pozisyonu
            Vector3 targetPos = rabbitTarget.transform.position;

            // Yay şeklinde atış için gerekli parametreler
            float gravity = Mathf.Abs(Physics2D.gravity.y);
            float distance = Vector3.Distance(startPos, targetPos);

            // Havada kalma süresini hesapla (isteğe bağlı olarak ayarlanabilir)
            float flightTime = distance / shootForce * 1.5f;

            // Başlangıç hızını hesapla
            Vector2 initialVelocity = CalculateArcVelocity(startPos, targetPos, gravity, flightTime);

            // Okun dönüş açısını ayarla (uçuş yönüne doğru)
            float angle = Mathf.Atan2(initialVelocity.y, initialVelocity.x) * Mathf.Rad2Deg;
            newArrow.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            rb.linearVelocity = initialVelocity;
        }
        else
        {
            Debug.LogError("Ok prefabında Rigidbody2D component'i bulunamadı!");
        }
    }

    private Vector2 CalculateArcVelocity(Vector3 startPos, Vector3 targetPos, float gravity, float time)
    {
        // Yatay ve dikey hız bileşenlerini hesapla
        float horizontalVelocity = (targetPos.x - startPos.x) / time;
        float verticalVelocity = (targetPos.y - startPos.y) / time + 0.5f * gravity * time;

        return new Vector2(horizontalVelocity, verticalVelocity);
    }

}
