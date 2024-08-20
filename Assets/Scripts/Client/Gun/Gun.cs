using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interface;
using Photon.Pun;

public class Gun : MonoBehaviourPun, IPunObservable
{
    public enum State { Ready, Empty, Reloading }
    public State state { get; private set; }
    public Transform fireTr;
    public ParticleSystem muzzleFlashEffect;        // 총구 화염
    public ParticleSystem shellEjectEffect;         // 탄피 배출
    private LineRenderer lineRenderer;
    private AudioSource gunAudioPlayer;
    public AudioClip shotClip;
    public AudioClip reloadClip;
    public float damage = 25f;              // 총알 데미지
    private float fireDistance = 50f;       // 총알 사정거리
    public int ammoRemain = 100;            // 남은 탄약
    public int magCapacity = 25;            // 탄창 용량
    public int magAmmo;                     // 탄창에 있는 탄약 개수
    public float timeBetFire = 0.12f;       // 탄피 발사 간격
    public float reloadTime = 1.0f;         // 재장전 시간
    private float lastFireTime;             // 마지막으로 발사한 시점

    void Awake()
    {
        gunAudioPlayer = GetComponent<AudioSource>();
        lineRenderer = GetComponent<LineRenderer>();
        fireTr = transform.GetChild(3).transform;
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
    }

    void OnEnable()
    {
        magAmmo = magCapacity;
        state = State.Ready;
        lastFireTime = 0;
    }

    public void Fire()              // 발사 시도
    {
        if (state == State.Ready && Time.time >= lastFireTime + timeBetFire)
        {
            lastFireTime = Time.time;
            Shot();
        }
    }
    private void Shot()             // 발사 처리
    {
        RaycastHit hit;
        Vector3 hitPosition = Vector3.zero;
        if (Physics.Raycast(fireTr.position, fireTr.forward, out hit, fireDistance))
        {
            IDamageable target = hit.collider.GetComponent<IDamageable>();
            if (target != null)
                target.OnDamage(damage, hit.point, hit.normal);
            hitPosition = hit.point;
        }
        else
            hitPosition = fireTr.position + fireTr.forward * fireDistance;
        StartCoroutine(ShotEffect(hitPosition));
        photonView.RPC(nameof(ShotProcessOnServer), RpcTarget.MasterClient);
        magAmmo--;
        if (magAmmo <= 0)
            state = State.Empty;
    }

    [PunRPC]
    public void ShotProcessOnServer()
    {
        RaycastHit hit;
        Vector3 hitPosition = Vector3.zero;
        if (Physics.Raycast(fireTr.position, fireTr.forward, out hit, fireDistance))
        {
            IDamageable target = hit.collider.GetComponent<IDamageable>();
            if (target != null)
                target.OnDamage(damage, hit.point, hit.normal);
            hitPosition = hit.point;
        }
        else
            hitPosition = fireTr.position + fireTr.forward * fireDistance;
        photonView.RPC(nameof(ShotEffectProcessOnClient), RpcTarget.All, hitPosition);
    }

    [PunRPC]
    public void ShotEffectProcessOnClient(Vector3 hitPos)
    {
        StartCoroutine(ShotEffect(hitPos));
    }


    private IEnumerator ShotEffect(Vector3 hitPosition)     // 발사 이펙트 처리
    {
        lineRenderer.enabled = true;
        muzzleFlashEffect.Play();
        shellEjectEffect.Play();
        gunAudioPlayer.PlayOneShot(shotClip);
        lineRenderer.SetPosition(0, fireTr.position);
        lineRenderer.SetPosition(1, hitPosition);
        yield return new WaitForSeconds(0.03f);

        lineRenderer.enabled = false;
        muzzleFlashEffect.Stop();
        shellEjectEffect.Stop();
    }
    public bool Reload()            // 재장전 시도
    {
        if (state == State.Reloading || ammoRemain <= 0 || magAmmo >= magCapacity)      // 재장전 중이거나 남은 탄약이 없거나 탄창이 가득 찼을 때
            return false;                                                                   // 재장전 실패
        else
        {
            StartCoroutine(ReloadRoutine());
            return true;                                                                    // 재장전 성공
        }
    }
    private IEnumerator ReloadRoutine()                     // 재장전 처리
    {
        state = State.Reloading;
        gunAudioPlayer.PlayOneShot(reloadClip);
        yield return new WaitForSeconds(reloadTime);
        int ammoToFill = magCapacity - magAmmo;             // 채워야 할 탄약 개수                 ex) 25 - 15 = 10
        if (ammoRemain < ammoToFill)                        // 남은 탄약이 채워야 할 탄약보다 적을 때   ex) 5 < 10
            ammoToFill = ammoRemain;                            // 남은 탄약만큼만 채움             ex) 5
        magAmmo += ammoToFill;                              // 탄창 채움                         ex) 15 + 5 = 20
        ammoRemain -= ammoToFill;                           // 남은 탄약에서 채운 탄약만큼 뺌         ex) 5 - 5 = 0
        state = State.Ready;
    }

    [PunRPC]
    public void AddAmmo(int ammo)
    {
        ammoRemain += ammo;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)      // 동기화
    {
        if (stream.IsWriting)               // 송신
        {
            stream.SendNext(magAmmo);           // 탄창에 있는 탄약 개수
            stream.SendNext(ammoRemain);        // 남은 탄약 개수
            stream.SendNext(state);             // 총 상태
        }
        else if (stream.IsReading)          // 수신
        {
            magAmmo = (int)stream.ReceiveNext();        // 탄창에 있는 탄약 개수
            ammoRemain = (int)stream.ReceiveNext();     // 남은 탄약 개수
            state = (State)stream.ReceiveNext();        // 총 상태
        }
    }
}
