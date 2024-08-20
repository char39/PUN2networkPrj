using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// IK : Inverse Kinematics

public class PlayerShooter : MonoBehaviourPun
{
    private PlayerInput playerInput;    // 플레이어 입력
    private Animator playerAni;         // 플레이어 애니메이터
    public Gun gun;                     // 총
    public Transform gunPivot;          // 총의 위치
    public Transform leftHandMount;     // 왼손 위치
    public Transform rightHandMount;    // 오른손 위치

    void OnEnable()                     // void OnEnable()
    {
        gun.gameObject.SetActive(true);             // 총 활성화
    }

    void Start()                        // void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerAni = GetComponent<Animator>();
    }

    void OnDisable()                    // void OnDisable()
    {
        gun.gameObject.SetActive(false);            // 총 비활성화
    }

    void Update()                       // void Update()
    {
        if (!photonView.IsMine) return;             // 본인(Local)의 입력만 받음. 다른 플레이어의 입력은 받지 않음.
        if (playerInput.fire)                           // 발사 입력
            gun.Fire();                                     // 총 발사
        else if (playerInput.reload && gun.Reload())    // 재장전 입력
            playerAni.SetTrigger("ReloadTrigger");          // 재장전 애니메이션
        UpdateUI();                                     // UI 갱신
    }

    private void UpdateUI()                 // UI 갱신
    {
        if (gun != null && UIManager.instance != null)
        {
            UIManager.instance.AmmoTextUpdate(gun.magAmmo, gun.ammoRemain);
        }
    }


    private void OnAnimatorIK(int layerIndex)
    {
        gunPivot.position = playerAni.GetIKHintPosition(AvatarIKHint.RightElbow);   // 오른쪽 팔꿈치 위치

        playerAni.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);                 // 왼손 위치 가중치
        playerAni.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);                 // 왼손 회전 가중치
        playerAni.SetIKPosition(AvatarIKGoal.LeftHand, leftHandMount.position);     // 왼손 위치
        playerAni.SetIKRotation(AvatarIKGoal.LeftHand, leftHandMount.rotation);     // 왼손 회전
        playerAni.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);                // 오른손 위치 가중치
        playerAni.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);                // 오른손 회전 가중치
        playerAni.SetIKPosition(AvatarIKGoal.RightHand, rightHandMount.position);   // 오른손 위치
        playerAni.SetIKRotation(AvatarIKGoal.RightHand, rightHandMount.rotation);   // 오른손 회전
    }
}
