using System.Collections;
using System.Collections.Generic;
using Interface;
using UnityEngine;
using UnityEngine.UI;

public struct HashValue_P
{
    public const string DieTrigger = "Die";
}

public class PlayerHP : LivingEntity
{
    public Slider HPSlider;                 // 체력을 표시할 슬라이더
    public AudioClip hitClip;               // 피격 소리
    public AudioClip itemPickupClip;        // 아이템 줍는 소리
    public AudioClip deathClip;             // 죽는 소리
    private AudioSource playerAudioSource;  // 플레이어 AudioSource 컴포넌트
    private Animator playerAni;             // 플레이어 Animator 컴포넌트
    private PlayerMovement playerMovement;      // PlayerMovement.cs 참조
    private PlayerShooter playerShooter;        // PlayerShooter.cs 참조

    void Awake()
    {
        playerAudioSource = GetComponent<AudioSource>();
        playerAni = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();    // 같은 오브젝트에 스크립트가 있어서 가져오기 가능
        playerShooter = GetComponent<PlayerShooter>();      // 위와 같음
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        HPSlider.gameObject.SetActive(true);
        HPSlider.maxValue = startHP;
        HPSlider.value = HP;
        playerMovement.enabled = true;
        playerShooter.enabled = true;
    }

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (!Dead)
            playerAudioSource.PlayOneShot(hitClip, 1.0f);
        base.OnDamage(damage, hitPoint, hitNormal);
        HPSlider.value = HP;
    }

    public override void RestoreHP(float newHP)
    {
        base.RestoreHP(newHP);
        HPSlider.value = HP;
    }

    public override void Die()
    {
        base.Die();
        HPSlider.gameObject.SetActive(false);
        playerAudioSource.PlayOneShot(deathClip, 1.0f);
        playerAni.SetTrigger(HashValue_P.DieTrigger);
        playerMovement.enabled = false;
        playerShooter.enabled = false;
    }

    private void OnTriggerEnter(Collider other)     // 아이템과 충돌한 경우
    {
        if (!Dead)
        {
            other.TryGetComponent(out IItem item);
            if (item != null)
            {
                item.Use(gameObject);
                playerAudioSource.PlayOneShot(itemPickupClip, 1.0f);
            }
        }
    }

}
