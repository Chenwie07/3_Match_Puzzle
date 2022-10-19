using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinueGame : MonoBehaviour
{
    public GameObject _continuePanel;
    public TMPro.TextMeshProUGUI retryHint;
    private void Start()
    {
        _continuePanel.SetActive(false);
    }
    public void ShowContinuePanel()
    {
        print("should set panel active"); 
        _continuePanel.SetActive(true);
    }
    public void RetrySelected(string LevelType)
    {
        if (LevelType == "Moves")
        {
            // add moves 
            var test = FindObjectOfType<LevelMoves>().numMoves += 3;
            print(test); 
            retryHint.SetText("Spend 1 heart to buy +3 extra moves...");
            PlayerPrefs.SetInt("Tries Left", PlayerPrefs.GetInt("Tries Left") - 1);
        }
        else if (LevelType == "Timer")
        {
            // add Time. 
            print("Increase the time and subtract lives");
            retryHint.SetText("Spend 1 heart to buy 15 more seconds...");
            FindObjectOfType<LevelTimer>().timeInSeconds += 15;
            PlayerPrefs.SetInt("Tries Left", PlayerPrefs.GetInt("Tries Left") - 1);
            print(PlayerPrefs.GetInt("Tries Left")); 
        }
        else if (LevelType == "Obstacles")
        {
            // add moves still. 
            retryHint.SetText("Spend 1 heart to buy +3 extra moves...");
            FindObjectOfType<LevelObstacles>().numMoves += 3;
            PlayerPrefs.SetInt("Tries Left", PlayerPrefs.GetInt("Tries Left") - 1);
        }
    }
}
