using UnityEngine;

namespace Interface
{
    public interface IItem
    {
        void Use(GameObject target);
    }
    
    public interface IDamageable
    {
        public void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal);
    }
}