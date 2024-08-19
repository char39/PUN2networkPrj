using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interface;

namespace ItemPack
{
    public class Item : MonoBehaviour
    {
        public bool ammoPack = false;
        public bool healthPack = false;
        public bool coin = false;
        public AmmoPack a;
        public HealthPack h;
        public Coin c;

        void Start()
        {
            if (ammoPack)
                a = gameObject.AddComponent<AmmoPack>();
            else if (healthPack)
                h = gameObject.AddComponent<HealthPack>();
            else if (coin)
                c = gameObject.AddComponent<Coin>();
        }
    }

    public class AmmoPack : MonoBehaviour, IItem        // 탄알
    {
        private int ammo = 30;

        public void Use(GameObject target)
        {
            PlayerShooter shooter = target.GetComponent<PlayerShooter>();
            if (shooter != null && shooter.gun != null)
            {
                shooter.gun.ammoRemain += ammo;
            }
            Destroy(gameObject);
        }
    }

    public class HealthPack : MonoBehaviour, IItem      // 체력
    {
        private float health = 50f;

        public void Use(GameObject target)
        {
            LivingEntity entity = target.GetComponent<LivingEntity>();
            if (entity != null)
            {
                entity.RestoreHP(health);
            }
            Destroy(gameObject);
        }
    }

    public class Coin : MonoBehaviour, IItem            // 코인
    {
        private int score = 200;

        public void Use(GameObject target)
        {
            GameManager.instance.AddScore(score);
            Destroy(gameObject);
        }
    }
}