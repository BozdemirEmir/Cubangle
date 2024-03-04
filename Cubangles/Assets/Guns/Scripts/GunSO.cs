using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Useable/Gun")]
public class GunSO : ScriptableObject
{
    public int bulletDamage;
    public float attackSpeed;
    public GameObject gunPrefab;
    public bool isAreaGun;
    public AreaGunSO areaGunSO;

}
