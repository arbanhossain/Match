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
        currentScore = PlayerPrefs.GetInt("Current Score", 0);
        bestScore = PlayerPrefs.GetInt("Best Score", 0);
        if (currentScore == bestScore) {
            PlayerPrefs.SetInt("Best Score", currentScore); // this is done for first initialization of the key
        }

        if (currentScore > bestScore) {
            highest = true;
            PlayerPrefs.SetInt("Best Score", currentScore);
            bestScore = currentScore;
        }

        if (highest) {
            CongratsText.gameObject.SetActive(true);
        }
        currentScoreText.text = System.Convert.ToString(currentScore);
        bestScoreText.text = System.Convert.ToString(bestScore);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
