using UnityEngine;
public interface IDamageable 
{
    [SerializeField] public int health { get; set; }
    public void Damage(int damage);
}
