using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMoves : Level
{
    public int numMoves;
    public int targetScore;

    private int movesUsed = 0;

    private void Start()
    {
        type = LevelType.MOVES;
        _levelHUD.SetlevelType(Type);
        _levelHUD.SetScore(currentScore);
        _levelHUD.SetTarget(targetScore);
        _levelHUD.SetRemaining(numMoves); 

        //Debug.Log($"Number of moves: {numMoves} Target score: {targetScore}"); 
    }

    public override void OnMove()
    {
        movesUsed++;

        _levelHUD.SetRemaining(numMoves - movesUsed); 
        //Debug.Log("Moves remaining: " + (numMoves - movesUsed)); 

        if (numMoves - movesUsed == 0)
        {
            if (currentScore >= targetScore)
            {
                GameWin(); 
            }else
            {
                GameLose(); 
            }
        }
    }
}
