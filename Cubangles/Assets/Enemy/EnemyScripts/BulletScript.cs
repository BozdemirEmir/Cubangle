using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public Rigidbody2D rb;
    [SerializeField] private float bulletSpeed = 13;

    Vector2 dir;

    void Start()
    {
        Destroy(gameObject,5);
    }

    void Update()
    {
        if (dir != null)
        {
            rb.velocity = dir.normalized * bulletSpeed;
        }
    }
    public void GoToDirection(Vector3 movePos)
    {
        dir = movePos - transform.position;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null && !collision.CompareTag("Enemy"))
        {
            damageable.Damage(1);
            Destroy(this.gameObject);
        }
    }
}
