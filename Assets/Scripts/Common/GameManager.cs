using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance_;
    public static GameManager instance
    {
        get
        {
            if (instance_ == null)
            {
                instance_ = FindObjectOfType<GameManager>();
            }
            return instance_;
        }
        set { instance_ = value; }
    }
    public bool isGameover = false;
    private int score = 0;

    void Awake()
    {
        if (instance_ == null)
            instance_ = this;
        else if (instance_ != this)
            Destroy(gameObject);
    }

    void Start()
    {
        FindObjectOfType<PlayerHP>().OnDeath += EndGame;    // Scene 전체의 PlayerHP 스크립트를 찾아서 OnDeath 이벤트에 EndGame 메서드를 등록
    }
    
    public void AddScore(int newScore)
    {
        if (isGameover) return;
        score += newScore;
        UIManager.instance.ScoreTextUpdate(score);
    }

    public void EndGame()
    {
        isGameover = true;
        UIManager.instance.SetActiveGameOverUI(isGameover);
    }
}
