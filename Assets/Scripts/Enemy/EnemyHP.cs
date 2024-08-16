using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyHP : LivingEntity
{
    public LayerMask targetLayerMask;           // 추적 대상의 레이어
    public LivingEntity targetEntity;           // 추적 대상
    private NavMeshAgent pathFinder;            // navMesh 경로
    private SkinnedMeshRenderer enemyRenderer;  // 적 캐릭터의 스킨 메쉬 렌더러
    public ParticleSystem hitEffect;            // 피격 이펙트
    public AudioClip hitClip;                   // 피격 소리
    public AudioClip deathClip;                 // 죽는 소리
    private AudioSource enemyAudioSource;
    private Animator enemyAni;
    internal float damage;                      // 공격력
    internal float timeBetAttack = 0.5f;        // 공격 간격
    private float lastAttackTime = 0.0f;        // 마지막 공격 시점
    private bool hasTarget                  // 프로퍼티
    {
        get
        {
            if (targetEntity != null && !targetEntity.Dead)
                return true;
            else
                return false;
        }
    }

    void Awake()
    {
        pathFinder = GetComponent<NavMeshAgent>();
        enemyRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        enemyAudioSource = GetComponent<AudioSource>();
        enemyAni = GetComponent<Animator>();
    }

    void Start()
    {
        InvokeRepeating("UpdatePath", 0.01f, 0.25f);        // 0.01초 후에 0.25초 간격으로 UpdatePath() 메서드 호출
    }

    void Update()
    {
        enemyAni.SetBool(HashID_E.HasTarget, hasTarget);
    }

    private void UpdatePath()
    {
        if (!Dead)                                  // 적이 살아있을 때만 실행
        {
            if (hasTarget)                              // 추적 대상이 있을 때
            {
                pathFinder.isStopped = false;                                   // 경로 계산 시작
                pathFinder.SetDestination(targetEntity.transform.position);     // 목적지 설정
            }
            else                                        // 추적 대상이 없을 때
            {
                pathFinder.isStopped = true;                                    // 경로 계산 중지
                Collider[] colliders = Physics.OverlapSphere(transform.position, 20f, targetLayerMask);     // 주변에 있는 콜라이더 검색
                for (int i = 0; i < colliders.Length; i++)                                                  // 주변에 있는 콜라이더를 하나씩 검사
                {
                    LivingEntity livingEntity = colliders[i].GetComponent<LivingEntity>();              // 콜라이더로부터 LivingEntity 컴포넌트 가져오기
                    if (livingEntity != null && !livingEntity.Dead)                                     // LivingEntity 컴포넌트가 있고, 죽지 않았을 때
                    {
                        targetEntity = livingEntity;                                                        // 추적 대상을 해당 LivingEntity로 설정
                        break;                                                                              // for문 종료
                    }
                }
            }
        }
    }

    public void Setup(float newHP, float newDamage, float newSpeed, Color skinColor)
    {
        startHP = newHP;
        HP = newHP;
        damage = newDamage;
        pathFinder.speed = newSpeed;
        enemyRenderer.material.color = skinColor;
    }

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        base.OnDamage(damage, hitPoint, hitNormal);
        if (!Dead)
        {
            hitEffect.transform.position = hitPoint;
            hitEffect.transform.rotation = Quaternion.LookRotation(hitNormal);
            hitEffect.Play();
            enemyAudioSource.PlayOneShot(hitClip);
        }
    }

    public override void Die()
    {
        base.Die();
        Collider[] enemyColliders = GetComponents<Collider>();
        Rigidbody[] enemyRigidbodies = GetComponents<Rigidbody>();
        for (int i = 0; i < enemyColliders.Length; i++)
            enemyColliders[i].enabled = false;
        for (int i = 0; i < enemyRigidbodies.Length; i++)
            enemyRigidbodies[i].isKinematic = true;
        pathFinder.isStopped = true;
        pathFinder.enabled = false;
        enemyAudioSource.PlayOneShot(deathClip);
        enemyAni.SetTrigger(HashID_E.DieTrigger);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!Dead && Time.time >= lastAttackTime + timeBetAttack)
        {
            LivingEntity attackTarget = other.GetComponent<LivingEntity>();
            if (attackTarget != null && attackTarget == targetEntity)
            {
                lastAttackTime = Time.time;
                Vector3 hitPoint = other.ClosestPoint(transform.position);
                Vector3 hitNormal = transform.position - other.transform.position;
                attackTarget.OnDamage(damage, hitPoint, hitNormal);
            }
        }
    }


}
