using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interface;
using Photon.Pun;

// 이 클래스는 IDamageable를 상속하므로 OnDamage() 메서드를 반드시 구현해야 함.
public class LivingEntity : MonoBehaviourPun, IDamageable
{
    public float startHP = 100f;                // 시작 hp
    public float HP { get; protected set; }     // 현재 hp.     상속받은 클래스에서만 set 가능.
    public bool Dead { get; protected set; }    // 사망 여부.    상속받은 클래스에서만 set 가능.
    public event Action OnDeath;                // 사망 시 발생하는 이벤트

    [PunRPC]                // 호스트 > 클라이언트 순서로 상태를 동기화하기 위함. Remote Procedure Call
    public void ApplyUpdateHP(float newHP, bool newDead)
    {
        HP = newHP;
        Dead = newDead;
    }

    protected virtual void OnEnable()
    {
        Dead = false;       // 사망 여부. false
        HP = startHP;       // 현재 hp는 시작 hp 할당
    }

    [PunRPC]
    public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (Dead)
            return;         // 죽었다면 데미지를 받지 않음
        if (PhotonNetwork.IsMasterClient)                                   // 마스터 클라이언트인 경우
        {
            HP -= damage;                                                               // hp 감소
            photonView.RPC("ApplyUpdateHP", RpcTarget.Others, HP, Dead);                // 다른 클라이언트에게도 적용
            photonView.RPC("OnDamage", RpcTarget.Others, damage, hitPoint, hitNormal);  // 다른 클라이언트에게도 적용
        }
        if (HP <= 0 && !Dead)
        {
            Die();
        }
    }

    [PunRPC]
    public virtual void RestoreHP(float newHP)
    {
        if (Dead)
            return;         // 죽었다면 체력 회복 불가
        if (PhotonNetwork.IsMasterClient)                                   // 마스터 클라이언트인 경우
        {
            HP += newHP;
            photonView.RPC("ApplyUpdateHP", RpcTarget.Others, HP, Dead);                // 다른 클라이언트에게도 적용
            photonView.RPC("RestoreHP", RpcTarget.Others, newHP);                       // 다른 클라이언트에게도 적용
        }
    }
    public virtual void Die()
    {
        if (OnDeath != null)
            OnDeath();              // if문 전체를 간소화 가능. -> OnDeath?.Invoke();
        Dead = true;
    }
}
