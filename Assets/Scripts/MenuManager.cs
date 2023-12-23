using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    // Check if sound is selected or not in settings and then apply that game-wide
    void Start()
    {
        bool soundState = (PlayerPrefs.GetInt("Sound", 1) == 1) ? true : false;
        if(soundState) AudioListener.volume = 1.0f;
        else AudioListener.volume = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
