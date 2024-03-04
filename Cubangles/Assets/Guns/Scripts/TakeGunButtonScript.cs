using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TakeGunButtonScript : MonoBehaviour
{
    [SerializeField] internal Button takeGunButton;
    [SerializeField] private Button gunBox_First;
    private Image currentGunImage;
    internal GunSO takerGun;

    [SerializeField] private List<GunSO> guns;


    private void Start()
    {
        takerGun = null;
        currentGunImage = GameObject.Find("GunImage").GetComponent<Image>();
        ChangeButtonImage(gunBox_First.GetComponent<Image>());
        foreach (var gun in guns)
        {
            if (gun.gunPrefab.GetComponent<SpriteRenderer>().sprite == currentGunImage.sprite)
            {
                GameObject.Find("Triangle").GetComponent<PlayerController>().currentGun = gun;
                break;
            }
        }
    }
    public void SelectGunImage(Image image)
    {
        currentGunImage = image;
        foreach (var gun in guns)
        {
            if(gun.gunPrefab.GetComponent<SpriteRenderer>().sprite == currentGunImage.sprite)
            {
                GameObject.Find("Triangle").GetComponent<PlayerController>().currentGun = gun;
                break;
            }
        }
    }
    public void ChangeButtonImage(Image buttonImage)
    {
        buttonImage.color = Color.white;
    }
    public void ChangeOldButtonImage (Image oldButtonImage)
    {
        Color newColor = HexToColor("#ADADAD");
        oldButtonImage.color = newColor;
    }
    public void TakeGun(Button button)
    {
        if(currentGunImage != null)
        {
            GameObject.Find("Triangle").GetComponent<PlayerController>().currentGun = takerGun;
            Debug.Log(takerGun);
            currentGunImage.sprite = takerGun.gunPrefab.GetComponent<SpriteRenderer>().sprite;

            Color imageColor = currentGunImage.color;
            imageColor.a = 1;
            currentGunImage.color = imageColor;

            button.gameObject.SetActive(false);
            Destroy(GameObject.Find("Triangle").GetComponent<ItemOnTriggerEnter2D>().lastSpawnedGun);
            takerGun = null;
        }
    }


    Color HexToColor(string hex)
    {
        Color renk = new Color();

        if (hex.StartsWith("#"))
        {
            hex = hex.Substring(1);
        }

        if (hex.Length == 6)
        {
            renk.r = (float)System.Convert.ToInt32(hex.Substring(0, 2), 16) / 255;
            renk.g = (float)System.Convert.ToInt32(hex.Substring(2, 2), 16) / 255;
            renk.b = (float)System.Convert.ToInt32(hex.Substring(4, 2), 16) / 255;
            renk.a = 1f;
        }

        return renk;
    }
}
