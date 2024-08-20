using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class EnemySpawner : MonoBehaviourPun, IPunObservable
{
    public EnemyHP enemyPrefab;
    public Transform[] spawnPoints;

    internal float damageMax = 10f;
    internal float damageMin = 5f;
    internal float healthMax = 200f;
    internal float healthMin = 100f;
    internal float speedMax = 2f;
    internal float speedMin = 0.8f;
    
    public Color strongEnemyColor = Color.red;
    public List<EnemyHP> enemies = new();
    private int enemyCount = 0;
    private int wave = 0;

    void Awake()
    {
        PhotonPeer.RegisterType(typeof(Color), 128, ColorSerialization.SerializeColor, ColorSerialization.DeserializeColor);
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (GameManager.instance != null && GameManager.instance.isGameover) return;
            if (enemies.Count <= 0)
                SpawnWave();
            UIUpdate();
        }
    }

    private void UIUpdate()
    {
        if (PhotonNetwork.IsMasterClient)                           // 마스터 클라이언트일 때
            UIManager.instance.WaveTextUpdate(wave, enemies.Count);     // wave, enemies.Count를 UIManager에 전달. enemies.Count는 enemies의 개수
        else                                                        // 마스터 클라이언트가 아닐 때
            UIManager.instance.WaveTextUpdate(wave, enemyCount);        // wave, enemyCount를 UIManager에 전달. enemyCount는 마스터 클라이언트의 enemies.Count
    }

    private void SpawnWave()
    {
        wave++;                                                     // wave 증가
        int spawnCount = Mathf.RoundToInt(wave * 1.5f);             // spawnCount를 wave에 따라 증가
        for (int i = 0; i < spawnCount; i++)                        // spawnCount만큼 반복
        {
            float enemyIntencity = Random.Range(0f, 1f);                // enemyIntencity를 0~1 사이의 랜덤 값으로 설정
            CreateEnemy(enemyIntencity);                                // enemyIntencity에 따라 enemy 생성
        }
    }

    private void CreateEnemy(float intencity)
    {
        float hp = Mathf.Lerp(healthMin, healthMax, intencity);                                 // hp를 intencity에 따라 비율을 조절
        float damage = Mathf.Lerp(damageMin, damageMax, intencity);                             // damage를 intencity에 따라 비율을 조절
        float speed = Mathf.Lerp(speedMin, speedMax, intencity);                                // speed를 intencity에 따라 비율을 조절
        Color skinColor = Color.Lerp(Color.white, strongEnemyColor, intencity);                 // skinColor를 intencity에 따라 비율을 조절
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];                // spawnPoint를 랜덤으로 선택

        GameObject enemyObj = PhotonNetwork.Instantiate(enemyPrefab.name, spawnPoint.position, spawnPoint.rotation);     // enemyPrefab을 spawnPoint에 생성
        EnemyHP enemy = enemyObj.GetComponent<EnemyHP>();                                                                // enemy를 EnemyHP로 형변환

        //enemy.Setup(hp, damage, speed, skinColor);                                            // enemy의 hp, damage, speed, skinColor 설정
        enemy.photonView.RPC("Setup", RpcTarget.All, hp, damage, speed, skinColor);                   // 모든 클라이언트에게 enemy의 hp, damage, speed, skinColor 설정

        enemies.Add(enemy);                                                                     // enemies에 enemy 추가
        enemy.OnDeath += () => enemies.Remove(enemy);                                           // enemy의 OnDeath 이벤트에 enemies에서 enemy 제거
        enemy.OnDeath += () => StartCoroutine(DestroyAfter(enemyObj, 3f));                      // enemy의 OnDeath 이벤트에 3초 후에 enemyObj 제거
        enemy.OnDeath += () => GameManager.instance.AddScore(100);                              // enemy의 OnDeath 이벤트에 100점 추가
    }

    private IEnumerator DestroyAfter(GameObject target, float delay)                // target을 delay 후에 제거
    {
        yield return new WaitForSeconds(delay);
        if (target != null)
            PhotonNetwork.Destroy(target);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)                       // 데이터를 보낼 때
        {
            stream.SendNext(enemies.Count);             // enemies의 개수를 전송.   1번째로 전송
            stream.SendNext(wave);                      // wave를 전송.            2번째로 전송
        }
        else if (stream.IsReading)                  // 데이터를 받을 때
        {
            enemyCount = (int)stream.ReceiveNext();     // enemyCount를 전송받은 데이터로 설정.     1번째로 전송된 데이터를 받음
            wave = (int)stream.ReceiveNext();           // wave를 전송받은 데이터로 설정.           2번째로 전송된 데이터를 받음
        }
    }
}