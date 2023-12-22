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
    // Start is called before the first frame update
    void Start()
    {
        // duration = (float)(PlayerPrefs.GetInt("Timer Duration", 60));
        Fill = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        current += Time.deltaTime;
        Fill.fillAmount = current / duration;

        if (current > duration) {
            boardManager.TriggerGameOver();
        }
    }
}
