using DG.Tweening;
using UnityEngine;

public class PrefabBreaker : MonoBehaviour, IDamageable
{
    public int health { get; set; }
    public ItemSO itemSO;

    [Header("Shake Variables")]
    public float duration = 0.5f;
    public float strength = 0.2f;
    public int vibrato = 10;
    public float randomness = 1;

    [Header("Explode Variables")]
    public bool canExplode;
    public float explodeRadius;
    public int damage;
    public LayerMask layerMask;
    public ParticleSystem fxExplosion;


    private void Start()
    {
        health = itemSO.itemHealth;
    }
    private void Update()
    {
        
    }
    public void Damage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Explode();
        }
        else
        {
            transform.DOShakePosition(duration, strength, vibrato, randomness);
        }
    }
    void Explode()
    {
        if(canExplode)
        {
            Collider2D[] caughtExplosion = Physics2D.OverlapCircleAll(transform.position , explodeRadius , layerMask);
            foreach (var gameObjectCollider in caughtExplosion)
            {
                if (gameObjectCollider.CompareTag("Enemy"))
                {
                    Destroy(gameObjectCollider.gameObject);
                }
                else
                {
                    IDamageable damageable = gameObjectCollider.gameObject.GetComponent<IDamageable>();

                    if(damageable != null)
                    {
                        damageable.Damage(damage);
                    }
                }
            }
            fxExplosion.Play();
            transform.DOScale(0, fxExplosion.main.duration).OnComplete(() =>
            {
                Destroy(gameObject);
            });

        }
        else
        {
            transform.DOScale(0, 0.25f).OnComplete(() =>
            {
                Destroy(gameObject);
            });
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position , explodeRadius);
    }
}