using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Enemy_DetectPlayer : MonoBehaviour, IDamageable
{
    [SerializeField] private EnemySO enemySO;
    public int health { get; set; }
    public int realHealth;


    [SerializeField] private LayerMask attackableLayer;
    [SerializeField] private LayerMask playerLayer;
    GameObject player;


    [SerializeField] private Transform bulletSpawnPos;

    bool canAttack = true;
    bool canDetect = false;


    void Start()
    {
        health = realHealth;
        this.gameObject.SetActive(true);

    }

    void Update()
    {
        DetectPlayer();
    }
    void DetectPlayer()
    {
        if (canDetect)
        {
            
            if (player != null)
            {
                Vector2 moveDir = (player.transform.position - transform.position).normalized;
                Collider2D inAttackRangeObject = Physics2D.OverlapCircle(transform.position, enemySO.range, attackableLayer);


                if (inAttackRangeObject != player &&inAttackRangeObject != null && Vector2.Distance(this.transform.position, player.transform.position) > enemySO.range + 1.75f && !enemySO.isRanged)
                {

                    Vector2 breakableMoveDir = (inAttackRangeObject.transform.position - transform.position).normalized;
                    float angle = Mathf.Atan2(breakableMoveDir.y, breakableMoveDir.x) * Mathf.Rad2Deg - 90f;

                    if (Vector2.Distance(this.transform.position, gameObject.transform.position) > enemySO.range)
                    {
                        this.transform.position = Vector2.MoveTowards((Vector2)this.transform.position, (Vector2)inAttackRangeObject.transform.position, enemySO.speed * Time.deltaTime);
                        transform.rotation = Quaternion.Euler(Vector3.forward * angle);
                    }
                    else
                    {
                        Attack();
                    }


                }
                else if (Vector2.Distance(this.transform.position, player.transform.position) > enemySO.range)
                {
                    this.transform.position = Vector2.MoveTowards((Vector2)this.transform.position, (Vector2)player.transform.position, enemySO.speed * Time.deltaTime);
                    float angle = MathF.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg - 90f;
                    transform.rotation = Quaternion.Euler(Vector3.forward * angle);
                }
                else
                {
                    Attack();
                }
                if (Vector2.Distance(this.transform.position, player.transform.position) > enemySO.findRange)
                {
                    canDetect = false;
                }

            }
        }
        else
        {
            Collider2D inRangeObjects = Physics2D.OverlapCircle(transform.position, enemySO.findRange, playerLayer);
            if(inRangeObjects != null )
            {
                player = inRangeObjects.gameObject;
                canDetect = true;
            }

        }


    }

    private void Attack()
    {

        Collider2D hit = Physics2D.OverlapCircle(transform.position, enemySO.rayRange, attackableLayer);

        if(hit != null)
        {
            if (!enemySO.isRanged)
            {
                Vector2 breakableMoveDir = (hit.transform.position - transform.position).normalized;
                float angle = Mathf.Atan2(breakableMoveDir.y, breakableMoveDir.x) * Mathf.Rad2Deg - 90f;

                transform.rotation = Quaternion.Euler(Vector3.forward * angle);

                IDamageable damageable = hit.gameObject.GetComponent<IDamageable>();
                if (canAttack && damageable != null && !hit.gameObject.transform.CompareTag("Enemy"))
                {
                    Vector2 rayDir = (hit.transform.position - transform.position).normalized;
                    transform.DOMove(hit.transform.position, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
                    {
                        Ray ray = new Ray(transform.position, rayDir);

                        Debug.DrawRay(ray.origin, rayDir * enemySO.rayRange, Color.red, 5);

                        if (canAttack && Physics2D.Raycast(ray.origin, ray.direction, enemySO.rayRange, attackableLayer))
                        {
                            damageable.Damage(enemySO.damage);
                            StartCoroutine(AttackDelay());
                        }
                    }).SetLoops(2, LoopType.Yoyo);
                }
            }
            else
            {
                Vector2 playerDir = (player.transform.position - transform.position).normalized;
                float angle = Mathf.Atan2(playerDir.y, playerDir.x) * Mathf.Rad2Deg - 90f;

                transform.rotation = Quaternion.Euler(Vector3.forward * angle);

                if (canAttack)
                {

                    GameObject bullet = Instantiate(enemySO.bullet , bulletSpawnPos.position , Quaternion.identity);
                    bullet.GetComponent<BulletScript>().GoToDirection(player.transform.position);
                    StartCoroutine(AttackDelay());
                }
            }
            
        }
        
    }

   
    IEnumerator AttackDelay()
    {
        canAttack = false;
        yield return new WaitForSeconds(enemySO.attackSpeed);
        canAttack = true;
    }
    public void Damage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            this.gameObject.SetActive(false);
        }
    }
}
