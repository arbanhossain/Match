using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public Toggle soundToggle;
    public Toggle timerToggle;

    // Start is called before the first frame update
    void Start()
    {
        bool soundState = (PlayerPrefs.GetInt("Sound", 1) == 1) ? true : false;
        bool timerState = (PlayerPrefs.GetInt("Timer", 1) == 1) ? true : false;
        if(soundState) AudioListener.volume = 1.0f;
        else AudioListener.volume = 0f;
        soundToggle.SetIsOnWithoutNotify(soundState);
        timerToggle.SetIsOnWithoutNotify(timerState);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleSound() {
        Debug.Log("Changed Sound");
        PlayerPrefs.SetInt("Sound", 1 - PlayerPrefs.GetInt("Sound", 1));
        AudioListener.volume = (1.0f - AudioListener.volume);
        // soundToggle.isOn = !soundToggle.isOn;
    }
    public void ToggleTimer() {
        Debug.Log("Changed Timer");
        PlayerPrefs.SetInt("Timer", 1 - PlayerPrefs.GetInt("Timer", 1));
        // timerToggle.SetIsOnWithoutNotify( !timerToggle.isOn );
    }

}