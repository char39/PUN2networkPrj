using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// 입력과 움직임을 분리하여 관리함.
public class PlayerInput : MonoBehaviourPun
{
    private readonly string moveAxisName = "Vertical";
    private readonly string rotateAxisName = "Horizontal";
    private readonly string fireButtonName = "Fire1";
    private readonly string reloadButtonName = "Reload";

    public float move { get; private set; }
    public float rotate { get; private set; }
    public bool fire { get; private set; }
    public bool reload { get; private set; }

    void Update()
    {
        if (!photonView.IsMine) return;         // 본인(Local)의 입력만 받음. 다른 플레이어의 입력은 받지 않음.

        if (GameManager.instance != null && GameManager.instance.isGameover)    // 게임매니저가 있고, 게임오버일 때
        {
            move = 0f;  rotate = 0f;    fire = false;   reload = false;
            return;
        }
        move = Input.GetAxis(moveAxisName);
        rotate = Input.GetAxis(rotateAxisName);
        fire = Input.GetButton(fireButtonName);
        reload = Input.GetButtonDown(reloadButtonName);
    }
}
