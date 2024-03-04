using System.Collections;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class ItemOnTriggerEnter2D : MonoBehaviour
{
    [SerializeField] internal List<GunSO> gunsPrefab;
    [SerializeField] private float randomness;
    [SerializeField] private Transform itemSpawnPoint;

    bool canSpawnItem = true;
    private TakeGunButtonScript tgbs;
    internal GameObject lastSpawnedGun;

    void Start()
    {
        tgbs = GameObject.Find("CanvasManager").GetComponent<TakeGunButtonScript>();
    }

    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("TreasureChest") && canSpawnItem)
        {
            itemSpawnPoint = collision.transform;
            SelectItem(collision.gameObject);

        }
        else if (collision.CompareTag("HealthStone"))
        {
            AddHealth();
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("Gun"))
        {
            tgbs.takeGunButton.gameObject.SetActive(true);
            foreach (GunSO gun in gunsPrefab)
            {
                if(gun.gunPrefab.GetComponent<SpriteRenderer>().sprite == collision.gameObject.GetComponent<SpriteRenderer>().sprite)
                {
                    tgbs.takerGun = gun;
                    break;
                }
            }
        }
        
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Gun") && tgbs.takerGun == null)
        {
            tgbs.takeGunButton.gameObject.SetActive(true);
            foreach (GunSO gun in gunsPrefab)
            {
                if (gun.gunPrefab.GetComponent<SpriteRenderer>().sprite == collision.gameObject.GetComponent<SpriteRenderer>().sprite)
                {
                    tgbs.takerGun = gun;
                    break;
                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Gun"))
        {
            tgbs.takeGunButton.gameObject.SetActive(false);
        }
    }
    void SelectItem(GameObject collision)
    {
        canSpawnItem = false;

        if(randomness < 1) 
        {
            randomness = 1;
        }

        List<int> caseNumbs = new List<int>();
        List<int> winCaseNumbs = new List<int>();

        int randomWinNumb;

        for (int i = 0; i < Mathf.RoundToInt(gunsPrefab.Count * randomness); i++)
        {
            caseNumbs.Add(i);
        }

        int possibleWinnerNumb = caseNumbs.ElementAt(Random.Range(0, caseNumbs.Count));
        

        for (int i = 0; i < gunsPrefab.Count; i++)
        {
            randomWinNumb = Random.Range(0,caseNumbs.Count);
            winCaseNumbs.Add(caseNumbs[randomWinNumb]);
            caseNumbs.Remove(caseNumbs[randomWinNumb]);
        }


        if(winCaseNumbs.Contains(possibleWinnerNumb))
        {
            int winNumb = winCaseNumbs.IndexOf(possibleWinnerNumb);

            lastSpawnedGun = Instantiate(gunsPrefab[winNumb].gunPrefab, itemSpawnPoint.position, Quaternion.identity);

            lastSpawnedGun.transform.DOScale(0,0.5f).From();
            collision.transform.DOScale(0,0.5f).OnComplete(() =>
            {
                Destroy(collision.gameObject);
                canSpawnItem = true;

            });

        }
        else
        {

            collision.transform.DOScale(0, 0.5f).OnComplete(() =>
            {
                Destroy(collision.gameObject);
                canSpawnItem = true;
            });
        }

    }

    void AddHealth()
    {
        PlayerController pc = this.gameObject.GetComponent<PlayerController>();

        pc.Heal(true);
    }
}
