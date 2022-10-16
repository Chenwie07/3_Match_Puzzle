using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    public Level _level;
    public GameOver _gameOver; 

    public TextMeshProUGUI remainingText;
    public TextMeshProUGUI remainingSubtext;
    public TextMeshProUGUI targetText;
    public TextMeshProUGUI targetSubtext;
    public TextMeshProUGUI scoreText;

    public GameObject[] starsPanels;

    private int starIdx = 0;

    private void Start()
    {
        for (int i = 0; i < starsPanels.Length; i++)
        {
            if (i == starIdx)
            {
                // we use .enabled if they were purely Images. 
                starsPanels[i].SetActive(true);
            }
            else
            {
                starsPanels[i].SetActive(false);
            }
        }
    }
    public void SetScore(int score)
    {
        scoreText.text = score.ToString();
        int visibleStar = 0;

        if (score >= _level.score1Star && score < _level.score2Star)
        {
            visibleStar = 1;
        }
        else if (score >= _level.score2Star && score < _level.score3Star)
        {
            visibleStar = 2;
        }
        else if (score >= _level.score3Star)
            visibleStar = 3;
        for (int i = 0; i < starsPanels.Length; i++)
        {
            if (i == visibleStar)
                starsPanels[i].SetActive(true);
            else
                starsPanels[i].SetActive(false);
        }
        starIdx = visibleStar;
    }
    public void SetTarget(int target) => targetText.SetText(target.ToString());

    public void SetRemaining(int remaining) => remainingText.SetText(remaining.ToString());
    public void SetRemaining(string remaining) => remainingText.SetText(remaining); 

    public void SetlevelType(Level.LevelType type)
    {
        if (type == Level.LevelType.MOVES)
        {
            remainingSubtext.SetText("moves remaining");
            targetSubtext.SetText("target score"); 
        }else if (type == Level.LevelType.OBSTACLE)
        {
            remainingSubtext.SetText("moves remaining");
            targetSubtext.SetText("boxes remaining"); 
        }else if (type == Level.LevelType.TIMER)
        {
            remainingSubtext.SetText("time remaining");
            targetSubtext.SetText("target score"); 
        }
    }

    public void OnGameWin(int score)
    {
        _gameOver.ShowWin(score, starIdx); 
    }
    public void OnGameLose()
    {
        _gameOver.ShowLose(); 
    }
}
