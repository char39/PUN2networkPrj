using System.Collections;
using System.Collections.Generic;
using UnityEngine;          // 유니티 엔진 관련 라이브러리
using UnityEngine.UI;       // 유니티 UI 관련 라이브러리
using Photon.Pun;           // 유니티용 포톤 라이브러리
using Photon.Realtime;      // 포톤 서비스 관련 라이브러리

public class LobbyManager : MonoBehaviourPunCallbacks
{
    private string gameVersion = "1.0";     // 게임 버전
    public Text connectionInfoText;         // 네트워크 정보 표시
    public Button joinButton;               // 방 접속 버튼 방만들기 버튼

    void Start()
    {
        PhotonNetwork.GameVersion = gameVersion;    // 접속에 필요한 정보 설정
        PhotonNetwork.ConnectUsingSettings();       // 설정한 정보로 마스터 서버 접속 시도
        joinButton.interactable = false;
        connectionInfoText.text = "Connect to Master Server...";
    }

    public override void OnConnectedToMaster()                      // 마스터 서버 접속 성공시 실행
    {
        joinButton.interactable = true;
        connectionInfoText.text = "Online : Connected to Master Server";
    }

    public override void OnDisconnected(DisconnectCause cause)      // 마스터 서버 접속 실패시 실행
    {
        joinButton.interactable = false;
        connectionInfoText.text = "Offline : Disconnected to Master Server";
    }

    public void Connect()                                           // 방 접속 버튼을 누를 시
    {
        joinButton.interactable = false;            // 중복 접근을 막기 위하여
        if (PhotonNetwork.IsConnected)              // 마스터 서버에 연걸 중 일 때
        {
            connectionInfoText.text = "방을 찾고 있습니다...";
            PhotonNetwork.JoinRandomRoom();             // 랜덤 방에 접속 시도
        }
        else                                        // 연결이 끊겼다면
        {
            connectionInfoText.text = "Offline : Disconnected to Master Server";
            PhotonNetwork.ConnectUsingSettings();       // 서버에 재접속 시도
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)       // 빈 방이 없어서 랜덤 방 참가에 실패한 경우
    {
        connectionInfoText.text = "빈 방이 없습니다. 새로운 방을 생성합니다.";
        PhotonNetwork.CreateRoom(null, new RoomOptions() {MaxPlayers = 4});         // 최대 4명 수용 가능한 빈 방 생성. 아직 방 목록을 확인하는 기능은 없기에 이름을 할당하지 않음.

    }

    public override void OnJoinedRoom()                             // 방에 접속한 경우 자동 실행
    {
        connectionInfoText.text = "방 접속 성공";
        PhotonNetwork.LoadLevel("Main");
    }



}
