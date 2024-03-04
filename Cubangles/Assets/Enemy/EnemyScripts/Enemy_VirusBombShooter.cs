using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Enemy_VirusBombShooter : MonoBehaviour
{
    [SerializeField] private float circleRadius;
    [SerializeField] private LayerMask pLayer;

    [SerializeField] private GameObject virusBomb;
    [SerializeField] private GameObject bombSpawner;

    [SerializeField] private float shootSpeed;


    int bombCount = 0;



    bool canShoot = true;
    private void Start()
    {
        transform.localScale = new Vector3(0.6f , 0.6f, 1);
        bombCount = 0;
        this.gameObject.SetActive(true);
    }

    void Update()
    {
        DetectPlayer();
    }
    void DetectPlayer()
    {
        Collider2D isPlayerInRange = Physics2D.OverlapCircle(transform.position, circleRadius, pLayer);

        if (bombCount < 3)
        {
            if (isPlayerInRange != null && canShoot)
            {
                ShootBomb(virusBomb, isPlayerInRange.transform.position);
                bombCount++;
                StartCoroutine(ShootDelay());
            }
        }
        else
        {
            transform.DOScale(new Vector3(0,0,0), 0.75f).OnComplete(() =>
            {
                bombCount = 0;
                this.gameObject.SetActive(false);
            });
            
        }
        

       
    }
    void ShootBomb(GameObject bomb , Vector3 shootPos)
    {
        Instantiate(bomb, bombSpawner.transform.position, Quaternion.identity);
    }
    IEnumerator ShootDelay()
    {
        canShoot = false;
        yield return new WaitForSeconds(shootSpeed);
        canShoot = true;
    }

    

}
