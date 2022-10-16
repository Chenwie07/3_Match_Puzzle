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

    public int score1Star; 
    public int score2Star; 
    public int score3Star;

    protected int currentScore; 

    public virtual void GameWin()
    {
        Debug.Log("You Win!"); 
        _gameGrid.GameOver();
    }
    public virtual void GameLose()
    {
        Debug.Log("You Lose!");
        _gameGrid.GameOver(); 
    }
    public virtual void OnMove()
    {
        Debug.Log("You Moved"); 
    }
    public virtual void OnPieceCleared(GamePiece piece)
    {
        currentScore += piece.score;
        Debug.Log("Score: " + currentScore); 
    }
}
