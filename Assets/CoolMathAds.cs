using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CoolMathAds : MonoBehaviour
{


    public static CoolMathAds instance;
    InputMain controls;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null && instance != this) {
            Destroy(this);
        }
      

    }


    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void PauseGame()
    {
        Debug.Log("PauseGame function called");
        Time.timeScale = 0f;
        //ADD LOGIC TO MUTE SOUND HERE
        FMODUnity.RuntimeManager.MuteAllEvents(true);
        


    }


    public void ResumeGame()
    {
        Debug.Log("ResumeGame function called");
        Time.timeScale = 1.0f;
        //ADD LOGIC TO UNMUTE SOUND HERE
        FMODUnity.RuntimeManager.MuteAllEvents(false);


    }

    public void InitiateAds()
    {
        Application.ExternalCall("triggerAdBreak");
    }

   
}
