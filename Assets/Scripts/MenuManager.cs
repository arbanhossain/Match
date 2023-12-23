using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    // Start is called before the first frame update
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
