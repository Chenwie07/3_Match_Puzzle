using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class HUD : MonoBehaviour
{
    public Level _level;

    public TextMeshProUGUI remainingText;
    public TextMeshProUGUI remainingSubtext; 
    public TextMeshProUGUI targetText; 
    public TextMeshProUGUI targetSubtext; 
    public TextMeshProUGUI scoreText;

    public GameObject[] starsPanels;

    private int starIdx = 0;
    private bool isGameOver; 

}
