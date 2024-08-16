using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public struct HashValue_E
{
    public const string DieTrigger = "Die";
}

public class EnemyHP : LivingEntity
{
    public LayerMask layerMask;             // 추적 대상의 레이어
    public LivingEntity targetEntity;       // 추적 대상
    private NavMeshAgent pathFinder;        // navMesh 경로
    public ParticleSystem hitEffect;
    public AudioClip hitClip;
    public AudioClip deathClip;
    private AudioSource enemyAudioSource;
    private Animator enemyAni;
    internal float damage = 20f;
    internal float timeBetAttack = 0.5f;
    private float lastAttackTime;
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

    }

    void Start()
    {
        InvokeRepeating("UpdatePath", 0.01f, 0.25f);
    }

    private void UpdatePath()
    {
        
    }

    public void Setup(float newHP, float newDamage, float newSpeed, Color skinColor)
    {

    }

    

}
