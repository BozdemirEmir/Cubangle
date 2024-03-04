using UnityEngine;
using DG.Tweening;

public class PlayerBulletScript : MonoBehaviour
{
    internal int damage;

    bool canGiveDamage = true;

    PlayerController pc;
    private void Start()
    {
        pc = GameObject.Find("Triangle").GetComponent<PlayerController>();
        Destroy(this.gameObject , 7);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();
        if(damageable != null && !collision.CompareTag("Player") && canGiveDamage && !collision.isTrigger) 
        {
            GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            Damage(damageable , pc.currentGun.isAreaGun, pc.currentGun.areaGunSO);
        }

    }

    void Damage(IDamageable damageable, bool isAreaGun , AreaGunSO areaGunSO = null)
    {
        
        if (isAreaGun && areaGunSO.isResidue)
        {
            Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y, 0);
            Instantiate(areaGunSO.residue, spawnPos, Quaternion.identity);
            Destroy(this.gameObject);
        }
        else if (isAreaGun && !areaGunSO.isResidue)
        {
            GiveAreaDamage(areaGunSO);
        }
        else
        {
            damageable.Damage(damage);
            this.canGiveDamage = false;
            transform.DOScale(0, 0.1f).OnComplete(() =>
            {
                Destroy(this.gameObject);
                this.canGiveDamage = true;
            });
        }
       

    }

    void GiveAreaDamage(AreaGunSO areaGunSO)
    {

        ParticleSystem fx = Instantiate(areaGunSO.fx_AreaGunExplosion, transform.position, Quaternion.identity);
        fx.Play();

        Collider2D[] findEnemies = Physics2D.OverlapCircleAll(transform.position, areaGunSO.areaRadius);
        foreach (var enemies in findEnemies)
        {
            IDamageable damageable = enemies.GetComponent<IDamageable>();
            if(damageable != null)
            {
                damageable.Damage(damage);
            }
        }
        this.canGiveDamage = false;
        
        transform.DOScale(0, areaGunSO.fx_AreaGunExplosion.main.duration).OnComplete(() =>
        {
            Destroy(this.gameObject);
            this.canGiveDamage = true;
        });
    }

    




}
