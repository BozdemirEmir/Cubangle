using System.Collections;
using UnityEngine;
using DG.Tweening;

public class PoisonLake : MonoBehaviour
{
    PlayerController pc;
    bool canGiveDamage = true;
    int residueDamageCount = 0;
    void Start()
    {
        pc = GameObject.Find("Triangle").GetComponent<PlayerController>();
        transform.DOScale(0, pc.currentGun.areaGunSO.fx_AreaGunExplosion.main.duration).From();
    }

    void Update()
    {
        GiveDamage(pc.currentGun.areaGunSO);
    }
    void GiveDamage(AreaGunSO areaGunSO)
    {
        if (canGiveDamage)
        {
            ParticleSystem fx = Instantiate(areaGunSO.fx_AreaGunExplosion, transform.position, Quaternion.identity);
            fx.Play();

            Collider2D[] findEnemies = Physics2D.OverlapCircleAll(transform.position, areaGunSO.areaRadius);
            foreach (var enemies in findEnemies)
            { 
                IDamageable damageable = enemies.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.Damage(pc.currentGun.bulletDamage);
                }
            }
            StartCoroutine(DamageDelay(areaGunSO));

        }
    }
    IEnumerator DamageDelay(AreaGunSO areaGunSO)
    {
        canGiveDamage = false;
        residueDamageCount++;
        yield return new WaitForSeconds(areaGunSO.residueTimer);

        if (residueDamageCount <= areaGunSO.maxResidueDamageCount)
        {
            canGiveDamage = true;
        }
        else
        {
            transform.DOScale(0, areaGunSO.fx_AreaGunExplosion.main.duration).OnComplete(() =>
            {
                Destroy(this.gameObject);
            });
        }
    }
}
