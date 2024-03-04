using UnityEngine;

[CreateAssetMenu(menuName = "NPC/Enemy")]
public class EnemySO : ScriptableObject
{
    [Header("Enemy Basic Properties")]
    public float speed;
    public GameObject prefab;

    [Header("Enemy Attack Properties")]
    public float rayRange;
    public float attackSpeed;
    public int damage;
    public float range;
    public bool isRanged;
    public GameObject bullet;

    [Header("Enemy Find Player")]
    public float findRange;

}
