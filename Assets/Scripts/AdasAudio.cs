using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdasAudio : MonoBehaviour {

    public AudioClip overSpeed;
    public float Volume;
    AudioSource audio;
    public bool alreadyPlayed = false;

    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    void playAudio()
    {
        if (GlobalVariables.overSpeed == true) //what to display when speeding
        {
            if (!alreadyPlayed)
            {
                audio.PlayOneShot(overSpeed, Volume);
                alreadyPlayed = true;
            }
        }
        if (GlobalVariables.correctSpeed == true)
        {
            alreadyPlayed = false;
        }


    }

	void Update () {
		
	}
}
