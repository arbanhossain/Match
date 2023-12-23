using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float duration;
    private float current = 0f;
    private Image Fill;
    public BoardManager boardManager;

    void Start()
    {
        // duration = (float)(PlayerPrefs.GetInt("Timer Duration", 60));
        Fill = GetComponent<Image>();
    }

    void Update()
    {
        // update the timer's visual state

        current += Time.deltaTime;
        Fill.fillAmount = current / duration;

        // if timer runs out, end the round
        if (current > duration) {
            boardManager.TriggerGameOver();
        }
    }
}
