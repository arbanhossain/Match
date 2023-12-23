using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    int currentScore, bestScore;
    private bool highest = false;

    [SerializeField]
    private TextMesh CongratsText;
    [SerializeField]
    private TextMesh currentScoreText;
    [SerializeField]
    private TextMesh bestScoreText;
    
    // Start is called before the first frame update
    void Start()
    {
        // Check current score and best score from local memory
        currentScore = PlayerPrefs.GetInt("Current Score", 0);
        bestScore = PlayerPrefs.GetInt("Best Score", 0);
        
        if (currentScore == bestScore) {
            // this is done for first initialization of the key, when there is no previous best score
            PlayerPrefs.SetInt("Best Score", currentScore);
        }

        if (currentScore > bestScore) {
            highest = true;
            PlayerPrefs.SetInt("Best Score", currentScore);
            bestScore = currentScore;
        }

        // if current score is the new highest, show a "New Best Score" text
        if (highest) {
            CongratsText.gameObject.SetActive(true);
        }

        // convert integers to string and display them
        currentScoreText.text = System.Convert.ToString(currentScore);
        bestScoreText.text = System.Convert.ToString(bestScore);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
