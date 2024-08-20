using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class ItemSpawner : MonoBehaviourPun
{
    // 1. 탄약    2. hp회복
    public GameObject[] items;
    public Transform playerTr;              // 플레이어 근처에 생성되어야 하기 때문에 할당
    private float maxDist = 5.0f;           // 아이템 최대 스폰 반경
    private float timeBetSpawnMax = 3.0f;   // 최대 생성 간격
    private float timeBetSpawnMin = 2.0f;   // 최소 생성 간격
    private float timeBetSpawn;             // 생성 간격
    private float lastSpawnTime;            // 마지막 생성 시점

    void Start()
    {
        playerTr = GameObject.FindWithTag("Player").transform;
        timeBetSpawn = Random.Range(timeBetSpawnMin, timeBetSpawnMax);
        lastSpawnTime = 0f;
    }

    void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        SpawnCoolTime();
    }

    private void SpawnCoolTime()
    {
        if (Time.time >= lastSpawnTime + timeBetSpawn && playerTr != null)      // 현재 시간이 (생성 간격 + 마지막 생성 시점)보다 크거나 같고, Tr이 존재한다면
        {
            lastSpawnTime = Time.time;
            timeBetSpawn = Random.Range(timeBetSpawnMin, timeBetSpawnMax);
            Spawn();
        }
    }

    private void Spawn()
    {
        Vector3 spawnPos = GetRandomPointOnNavMesh(playerTr.position, maxDist);         // 플레이어 근처에 랜덤한 위치를 반환
        spawnPos += Vector3.up * 0.5f;                                                  // 높이값을 0.5로 설정
        GameObject selectedItem = items[Random.Range(0,items.Length)];                  // 아이템 배열 중 랜덤한 아이템을 선택
        //GameObject item = Instantiate(selectedItem, spawnPos, Quaternion.identity);     // 선택된 아이템을 생성되어야 할 위치에 생성
        //Destroy(item, 5.0f);                                                            // 5초 후 삭제
        GameObject item = PhotonNetwork.Instantiate(selectedItem.name, spawnPos, Quaternion.identity);
        StartCoroutine(DestroyAfter(item, 5.0f));
    }

    private IEnumerator DestroyAfter(GameObject target, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (target != null)
        {
            PhotonNetwork.Destroy(target);
        }
            
    }

    private Vector3 GetRandomPointOnNavMesh(Vector3 center, float distance)     // NavMesh 위에 랜덤한 위치를 반환하는 메서드.
    {
        Vector3 randomPos = Random.insideUnitSphere * distance + center;        // center를 중심으로 distance 범위 내의 랜덤한 위치를 반환. 구의 반지름은 1 * distance.
        NavMeshHit hit;                                                         // NavMesh 정보를 저장할 구조체
        NavMesh.SamplePosition(randomPos, out hit, distance, NavMesh.AllAreas); // randomPos 위치에 대한 NavMesh 정보를 hit에 저장.
        return hit.position;
    }


}
