using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using static Cconflict;

public class enemy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float HP=100,damage, speed,attackJumpSpeed,oppositeSpeed, detectionRange,attackCooldown,cameraNoiseTime;
    public bool reachedZero = false,attackFinished=true,isAttack=false;
    public NoiseSettings yourNoiseProfile;
    GameObject target;
    Rigidbody2D rb;
    Vector2 direction;
    Vector2 targetDirection;
    float threshold = 0.02f;  // Hızın sıfır kabul edileceği eşik değeri
    SpriteRenderer sr;
    Animator an;
    private CinemachineBasicMultiChannelPerlin cinemachineCam;
    // Update is called once per frame

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        an= GetComponent<Animator>();
        sr= GetComponent<SpriteRenderer>();
        cinemachineCam = GameObject
            .Find("CinemachineCamera")
            .GetComponent<CinemachineBasicMultiChannelPerlin>();
    }
    void Update()
    {

        if (Mathf.Abs(rb.linearVelocity.x) > threshold)
        {   
            sr.flipX = rb.linearVelocity.x < 0;
            if(!isAttack)
            an.SetBool("isRun", true);
        }
        else
        {
            an.SetBool("isRun", false);
        }
        target = FindNearestTarget(detectionRange, "Building","citizen","character","archerTower","Wall","CityCenter");

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
                if(!isAttack)
                rb.linearVelocity = direction * speed;
            }
            if (Mathf.Abs(transform.position.x - 0) < 0.2f)
            {
                reachedZero = true;
            }
        }
        if (HP <= 0)
            an.SetBool("dead", true);
    }
    public void DisableEnemy()
    {
        an.SetBool("dead", false);
        newCoinPool.Instance.DisableEnemy1(gameObject);
    }

    private GameObject FindNearestTarget(float range, string tag, string tag2, string tag3,string tag4,string tag5,string tag6)  ///herhangi bir yapi, karakter veya vatandas
    {
        // Etraftaki tav�anlar� bul
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, range);
        GameObject nearest = null;
        float nearestDistance = Mathf.Infinity;
        foreach (var hitCollider in hitColliders)
        {
            //Debug.Log($"Found object: {hitCollider.gameObject.name}, Tag: {hitCollider.tag}, Layer: {LayerMask.LayerToName(hitCollider.gameObject.layer)}");
            if (hitCollider.CompareTag(tag)|| hitCollider.CompareTag(tag2)|| hitCollider.CompareTag(tag3)|| hitCollider.CompareTag(tag4)|| hitCollider.CompareTag(tag5) || hitCollider.CompareTag(tag6))
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
        an.SetBool("isAttacking", true);
        //print("attackJump");
        yield return new WaitForSeconds(0.5f);
        an.SetBool("isAttacking", false);
        an.SetBool("isRun", false);
        an.SetBool("idle", true);
        isAttack = true;
        yield return new WaitForSeconds(attackCooldown);
        isAttack = false;
        an.SetBool("idle", false);
        an.SetBool("isRun", true);
        attackFinished = true;
    }
    IEnumerator CameraNoiseAttack()
    {
        cinemachineCam.NoiseProfile = yourNoiseProfile;   //noise aç
        yield return new WaitForSeconds(cameraNoiseTime);
        cinemachineCam.NoiseProfile = null;    //noise kapat
    }
    public void DamageTakenFalse()
    {
        an.SetBool("damageTaken", false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(target != null&& collision.gameObject == target)
        {
            if (collision.CompareTag("citizen"))
            {
                collision.gameObject.GetComponent<Cconflict>().HP -= damage;
                collision.gameObject.GetComponent<Animator>().SetBool("damageTaken", true);
                if (target!=null&&!isAttack)
                    rb.linearVelocity = -targetDirection * oppositeSpeed;
            }
            
            if (collision.CompareTag("Building"))
            {
                collision.gameObject.GetComponent<Building>().HP -= damage;
                if (target != null && !isAttack)
                    rb.linearVelocity = -targetDirection * oppositeSpeed;
            }
            //if (collision.CompareTag("buildable"))
            //{
            //    if (collision.gameObject.GetComponent<Building>() != null)
            //        collision.gameObject.GetComponent<Building>().HP -= damage;
            //    if (target != null && !isAttack)
            //        rb.linearVelocity = -targetDirection * oppositeSpeed;
            //}
            if (collision.CompareTag("character"))
            {
                collision.gameObject.GetComponent<characterConflict>().moneyCount--;
                StartCoroutine(CameraNoiseAttack());
                if (target != null && !isAttack)
                    rb.linearVelocity = -targetDirection * oppositeSpeed;
            }
            if(collision.CompareTag("CityCenter"))
            {
                CityCenter cityCenter = collision.gameObject.GetComponent<CityCenter>();
                cityCenter.HP -= damage;
                if (target != null && !isAttack)
                    rb.linearVelocity = -targetDirection * oppositeSpeed;
            }
            if (collision.CompareTag("archerTower"))
            {
                Building aTower = collision.gameObject.GetComponent<Building>();
                aTower.HP -= damage;
                if (target != null && !isAttack)
                {
                    print(target.name);
                    rb.linearVelocity = -targetDirection * oppositeSpeed;
                }
            }
            if (collision.CompareTag("Wall"))
            {
                Building wall = collision.gameObject.GetComponent<Building>();
                wall.HP -= damage;
                if (target != null && !isAttack)
                    rb.linearVelocity = -targetDirection * oppositeSpeed;
            }
        }
    }
}
