using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public enum LevelType
    {
        TIMER, OBSTACLE, MOVES,
    }

    protected LevelType type;
    
    public LevelType Type
    {
        get { return type; }
    }
    public GameGrid _gameGrid;
    public HUD _levelHUD; 

    public int score1Star; 
    public int score2Star; 
    public int score3Star;

    protected int currentScore;

    private void Start()
    {
        _levelHUD.SetScore(currentScore); 
    }
    public virtual void GameWin()
    {
        _levelHUD.OnGameWin(currentScore); 
        _gameGrid.GameOver();
    }
    public virtual void GameLose()
    {
        _levelHUD.OnGameLose();
        _gameGrid.GameOver(); 
    }
    public virtual void OnMove()
    {
    }
    public virtual void OnPieceCleared(GamePiece piece)
    {
        currentScore += piece.score;
        _levelHUD.SetScore(currentScore); 
    }
}
