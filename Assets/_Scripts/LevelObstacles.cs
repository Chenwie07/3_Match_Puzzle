using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelObstacles : Level
{
    public int numMoves;
    public GameGrid.PieceType[] obstacleTypes;

    private int movesUsed = 0;
    private int numObstaclesLeft;

    private void Start()
    {
        type = LevelType.OBSTACLE;

        for (int i = 0; i < obstacleTypes.Length; i++)
        {
            numObstaclesLeft += _gameGrid.GetPiecesOfType(obstacleTypes[i]).Count; 
        }
        _levelHUD.SetlevelType(Type);
        _levelHUD.SetScore(currentScore);
        _levelHUD.SetTarget(numObstaclesLeft);
        _levelHUD.SetRemaining(numMoves); 
    }
    public override void OnMove()
    {
        movesUsed++;
        _levelHUD.SetRemaining(numMoves - movesUsed); 
        //Debug.Log("moves remaining: " + (numMoves - movesUsed)); 
        if (numMoves - movesUsed == 0 && numObstaclesLeft > 0)
        {
            GameLose(); 
        }
    }

    public override void OnPieceCleared(GamePiece piece)
    {
        base.OnPieceCleared(piece);
        for (int i = 0; i < obstacleTypes.Length; i++)
        {
            if (obstacleTypes[i] == piece.Type)
            {
                numObstaclesLeft--;
                _levelHUD.SetTarget(numObstaclesLeft); 
                if (numObstaclesLeft == 0)
                {
                    currentScore += 1000 * (numMoves - movesUsed);
                    _levelHUD.SetScore(currentScore); 
                    //Debug.Log("current score: " + currentScore);
                    GameWin(); 
                }
            }
        }
    }
}
