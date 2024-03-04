using System.Collections;
using UnityEngine;

public class BombScript : MonoBehaviour
{

    private Rigidbody2D rb;
    private Vector2 dir;

    public float bombSpeed;
    public GameObject player;

    bool isExplode = false;

    [SerializeField] private ParticleSystem fxParticleSystem;
    void Start()
    {
        player = GameObject.Find("Triangle");
        dir = player.transform.position - transform.position;
        StartCoroutine(DestroyCharacter());
        rb = GetComponent<Rigidbody2D>();

    }

    void FixedUpdate()
    {
        GoToPlayer();
    }
   
    void GoToPlayer()
    {
        if (!isExplode)
        {
            rb.velocity = dir.normalized * bombSpeed;
        }
    }
    void Explode()
    {
        isExplode = true;
        rb.velocity = Vector2.zero;
        fxParticleSystem.Play();
        GetComponent<SpriteRenderer>().enabled = false;
        Destroy(gameObject, fxParticleSystem.main.duration * 2.5f);
        StopAllCoroutines();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();

        if( damageable != null && !collision.CompareTag("Enemy"))
        {
            if (collision.name == "Triangle")
            {
                damageable.Damage(1);
                damageable.Damage(1);
                damageable.Damage(1);
            }
            else
            {
                damageable.Damage(1);
            }
            Explode();

        }




    }
    IEnumerator DestroyCharacter()
    {
        yield return new WaitForSeconds(7);
        Destroy(gameObject);
        fxParticleSystem.Play();
    }
}
