using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "PCG/Item Data")]
public class ItemSO : ScriptableObject
{
    [Header("Item properties")]
    public GameObject objectPrefab;
    public Vector2Int size;
    public PlacementType placementType;
    public bool addOffset;
    public bool canDestructible;

    [Header("Ýf Destructible")]
    public int itemHealth;
    public Sprite dropObjectSprite;

    
}

public enum PlacementType
{
    NearWall,
    OpenSpace
}