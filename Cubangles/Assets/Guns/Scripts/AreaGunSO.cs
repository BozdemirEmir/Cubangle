using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Useable/AreaGun")]
public class AreaGunSO : ScriptableObject
{
    public float areaRadius;
    public bool isResidue;
    public ParticleSystem fx_AreaGunExplosion;


    [Header("If isResidue")]
    public GameObject residue;
    public float residueTimer;
    public int maxResidueDamageCount;
   
}
