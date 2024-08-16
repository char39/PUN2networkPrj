using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interface;

// 이 클래스는 IDamageable를 상속하므로 OnDamage() 메서드를 반드시 구현해야 함.
public class LivingEntity : MonoBehaviour, IDamageable
{
    public float startHP = 100f;                // 시작 hp
    public float HP { get; protected set; }     // 현재 hp.     상속받은 클래스에서만 set 가능.
    public bool Dead { get; protected set; }    // 사망 여부.    상속받은 클래스에서만 set 가능.
    public event Action OnDeath;                // 사망 시 발생하는 이벤트

    protected virtual void OnEnable()
    {
        Dead = false;       // 사망 여부. false
        HP = startHP;       // 현재 hp는 시작 hp 할당
    }

    public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (Dead)
            return;         // 죽었다면 데미지 못받음
        HP -= damage;
        if (HP <= 0 && !Dead)
        {
            Die();
        }
    }

    public virtual void RestoreHP(float newHP)
    {
        if (Dead)
            return;         // 죽었다면 체력 회복 불가
        HP += newHP;
    }
    public virtual void Die()
    {
        if (OnDeath != null)
            OnDeath();              // if문 전체를 간소화 가능. -> OnDeath?.Invoke();
        Dead = true;
    }
}
