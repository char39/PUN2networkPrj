using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interface;

namespace ItemPack
{
    public class AmmoPack : MonoBehaviour, IItem       // 탄알
    {
        public int ammo = 30;

        public void Use(GameObject target)
        {
            Debug.Log("탄알이 증가함. " + ammo);
        }
    }

    public class HealthPack : MonoBehaviour, IItem     // 체력
    {
        public float health = 50f;

        public void Use(GameObject target)
        {
            Debug.Log("체력을 회복함. " + health);
        }
    }
}