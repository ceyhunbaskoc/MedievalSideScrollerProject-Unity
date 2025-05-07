using System.Collections;
using UnityEngine;
using static Cconflict;

public class enemy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float HP=100,damage, speed,attackJumpSpeed,oppositeSpeed, detectionRange,attackCooldown;
    public bool reachedZero = false,attackFinished=true,isAttack=false;
    GameObject target;
    Rigidbody2D rb;
    Vector2 direction;
    Vector2 targetDirection;
    // Update is called once per frame

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
    }
    void Update()
    {
        target = FindNearestTarget(detectionRange, "Building","citizen","character");

        if(target!=null)
        {
            //print("target = " + target);
            if (attackFinished)
                StartCoroutine(Attack(target));
        }
        else
        {
            if (!reachedZero)
            {
                direction = (new Vector2(0, transform.position.y) - (Vector2)transform.position).normalized;
                rb.linearVelocity = direction * speed;
            }
            if (Mathf.Abs(transform.position.x - 0) < 0.1f)
            {
                reachedZero = true;
            }
        }
        if (HP <= 0)
            newCoinPool.Instance.DisableEnemy1(gameObject);
    }

    private GameObject FindNearestTarget(float range, string tag, string tag2, string tag3)  ///herhangi bir yapi, karakter veya vatandas
    {
        // Etraftaki tav�anlar� bul
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, range);
        GameObject nearest = null;
        float nearestDistance = Mathf.Infinity;
        foreach (var hitCollider in hitColliders)
        {
            //Debug.Log($"Found object: {hitCollider.gameObject.name}, Tag: {hitCollider.tag}, Layer: {LayerMask.LayerToName(hitCollider.gameObject.layer)}");
            if (hitCollider.CompareTag(tag)|| hitCollider.CompareTag(tag2)|| hitCollider.CompareTag(tag3))
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

    IEnumerator Attack(GameObject target)
    {
        attackFinished = false;
        targetDirection = (new Vector2(target.transform.position.x,transform.position.y)-(Vector2)transform.position).normalized;
        rb.linearVelocity = targetDirection * attackJumpSpeed;
        //print("attackJump");
        yield return new WaitForSeconds(0.5f);
        isAttack = true;
        yield return new WaitForSeconds(attackCooldown);
        isAttack = false;
        attackFinished = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(target != null&& collision.gameObject == target)
        {
            if (collision.CompareTag("citizen"))
            {
                collision.gameObject.GetComponent<Cconflict>().HP -= damage;
                if(target!=null&&!isAttack)
                    rb.linearVelocity = -targetDirection * oppositeSpeed;
            }
            
            if (collision.CompareTag("Building"))
            {
                collision.gameObject.GetComponent<Building>().HP -= damage;
                if (target != null && !isAttack)
                    rb.linearVelocity = -targetDirection * oppositeSpeed;
            }
            if (collision.CompareTag("character"))
            {
                collision.gameObject.GetComponent<characterConflict>().moneyCount--;
                if (target != null && !isAttack)
                    rb.linearVelocity = -targetDirection * oppositeSpeed;
            }
        }
    }
}
