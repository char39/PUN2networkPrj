using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private static UIManager instance_;
    public static UIManager instance
    {
        get
        {
            if (instance_ == null)
            {
                instance_ = FindObjectOfType<UIManager>();
            }
            return instance_;
        }
        set { instance_ = value; }
    }
    public Transform hud_Canvas;
    private GameObject gameOverUI;
    private Text ammoText;
    private Text scoreText;
    private Text waveText;

    void Awake()
    {
        if (instance_ == null)
            instance_ = this;
        else if (instance_ != this)
            Destroy(gameObject);
        gameOverUI = hud_Canvas.GetChild(3).gameObject;
        ammoText = hud_Canvas.GetChild(0).GetChild(0).GetComponent<Text>();
        scoreText = hud_Canvas.GetChild(1).GetComponent<Text>();
        waveText = hud_Canvas.GetChild(2).GetComponent<Text>();
    }

    public void AmmoTextUpdate(int magAmmo, int ammoRemain)
    {
        ammoText.text = $"{magAmmo}/{ammoRemain}";                      // ammoText 텍스트 갱신
    }

    public void ScoreTextUpdate(int newScore)
    {
        scoreText.text = $"Score : {newScore}";                         // scoreText 텍스트 갱신
    }

    public void WaveTextUpdate(int wave, int count)
    {
        waveText.text = $"Wave : {wave}\nEnemy Left : {count}";              // waveText 텍스트 갱신
    }

    public void SetActiveGameOverUI(bool active)
    {
        gameOverUI.SetActive(active);                                   // 게임오버 UI 활성화
    }

    public void GameRestart()
    {
        GameManager.instance.isGameover = false;                        // 게임오버 상태 해제
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);     // 현재 씬 재시작
    }
}
