using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
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
    private List<EnemyHP> enemies = new();
    private int wave = 0;

    void Update()
    {
        if (GameManager.instance != null && GameManager.instance.isGameover) return;
        if (enemies.Count <= 0)
            SpawnWave();
        UIUpdate();
    }

    private void UIUpdate()
    {
        UIManager.instance.WaveTextUpdate(wave, enemies.Count);
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

        EnemyHP enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);     // enemyPrefab을 spawnPoint에 생성
        enemy.Setup(hp, damage, speed, skinColor);                                              // enemy의 hp, damage, speed, skinColor 설정
        enemies.Add(enemy);                                                                     // enemies에 enemy 추가

        enemy.OnDeath += () => enemies.Remove(enemy);                                           // enemy의 OnDeath 이벤트에 enemies에서 enemy 제거
        enemy.OnDeath += () => Destroy(enemy.gameObject, 10f);                                  // enemy의 OnDeath 이벤트에 10초 후 enemy 제거
        enemy.OnDeath += () => GameManager.instance.AddScore(100);                              // enemy의 OnDeath 이벤트에 100점 추가
    }
}
