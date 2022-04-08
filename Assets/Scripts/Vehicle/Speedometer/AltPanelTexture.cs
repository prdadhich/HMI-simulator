using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof (AudioSource))]
public class AltPanelTexture : MonoBehaviour {

    [Header("Alt. Texture")]            //the title of the section on the component
    public Sprite textureNormal;        //texture for the car
    public Sprite textureSpeed;      //alternative texture for when the speed limit is not respected
    public Sprite textureDistance;   //alternative texture for when the safety distance is not respected
    public Sprite textureDistanceSpeed;   //alternative texture for when the safety distance is not respected
    public Sprite textureEmergency;  //alternative texture for when the safety distance is not respected

    public AudioClip  overspeedSound;
    public AudioClip emergencySound;
    AudioSource audioSource;
    //State machine for adas light
    public enum PanelStates { Normal, Overspeed, SafetyDistance, SafetyDistanceSpeed, Emergency }
    public PanelStates currentState, nextState;


    // Use this for initialization
    void Start()
    {
        // Update the state every second
        InvokeRepeating("UpdateState", 0, 0.1f);
        nextState = PanelStates.Normal;

        audioSource = GetComponent<AudioSource>();
    }

    private void UpdateState()       // Every time you change the state, you update it with the next one and set the next to the same state
    {
        
        if (GlobalVariables.Trial_type == GlobalVariables.TrialType.SOUND)
        {
            if (nextState == PanelStates.Overspeed && currentState != PanelStates.Overspeed)
                audioSource.PlayOneShot(overspeedSound, 0.7f);

            if (nextState == PanelStates.Emergency && currentState != PanelStates.Emergency)
                audioSource.PlayOneShot(emergencySound, 0.7f);

            if (nextState == PanelStates.SafetyDistance && currentState != PanelStates.SafetyDistance)
                audioSource.PlayOneShot(emergencySound, 0.7f);

            if (nextState == PanelStates.SafetyDistanceSpeed && currentState != PanelStates.SafetyDistanceSpeed)
            {
                audioSource.PlayOneShot(emergencySound, 0.7f);
                audioSource.PlayOneShot(overspeedSound, 0.7f);
            }


        }
        
        currentState = nextState;
        nextState = currentState;
        PedalManager();


    }

    private void FixedUpdate()
    {
        if(GlobalVariables.Trial_type!=GlobalVariables.TrialType.ADAPTATION && GlobalVariables.isTestRunning)
            ConditionChecker();
    }

    public void ConditionChecker()
    {
        if (GlobalVariables.safetyStillDistance) 
        {
            if (GlobalVariables.overSpeed) //what to display when speeding and not respecting the distance from still cars
            {
                nextState = PanelStates.SafetyDistanceSpeed;
            }
            else //what to display when the car in front of you is still
            {
                nextState = PanelStates.Emergency;
            }
        }
        else
        {
            if (GlobalVariables.overSpeed)  //what to display when speeding
            {
                nextState = PanelStates.Overspeed;
            }
            else if (GlobalVariables.safetyDistance) //what to display when not respecting the distance from moving cars
            {
                nextState = PanelStates.SafetyDistance; 
            }
            else //what to display when nothing happens
            {
                nextState = PanelStates.Normal;
            }
        }
    }

    //STATE MACHINE to determine the correct ADAS condition
    private void PedalManager()
    {
        Logger.adasState = (int)currentState;
        switch (currentState)
        {
            case PanelStates.Normal:
                this.GetComponent<Image>().sprite = textureNormal;
                GlobalVariables.pedalmode = "none";
                //Debug.Log("ADAS entered state NORMAL");
                break;
            case PanelStates.Emergency:
                this.GetComponent<Image>().sprite = textureEmergency;
                GlobalVariables.pedalmode = "vib";
                //if (GlobalVariables.Trial_type == GlobalVariables.TrialType.SOUND)
                  //  audioSource.PlayOneShot(emergencySound,0.7f);
                    //Debug.Log("ADAS entered state EMERGENCY");
                    break;
            case PanelStates.Overspeed:
                this.GetComponent<Image>().sprite = textureSpeed;
                GlobalVariables.pedalmode = "cfh";
                //if (GlobalVariables.Trial_type == GlobalVariables.TrialType.SOUND)
                //audioSource.PlayOneShot(overspeedSound, 0.7f);
                    //Debug.Log("ADAS entered state OVERSPEED");
                    break;
            case PanelStates.SafetyDistance:
                this.GetComponent<Image>().sprite = textureDistance;
                GlobalVariables.pedalmode = "cfl";
                //if (GlobalVariables.Trial_type == GlobalVariables.TrialType.SOUND)
                  //  audioSource.PlayOneShot(emergencySound, 0.7f);
                //Debug.Log("ADAS entered state SAFETY");
                break;
            case PanelStates.SafetyDistanceSpeed:
                this.GetComponent<Image>().sprite = textureDistanceSpeed;
                GlobalVariables.pedalmode = "vib";
               /* if (GlobalVariables.Trial_type == GlobalVariables.TrialType.SOUND)
                {
                    audioSource.PlayOneShot(overspeedSound, 0.7f);
                    audioSource.PlayOneShot(emergencySound, 0.7f);
                }
                */
                //Debug.Log("ADAS entered state SAFETY OVERSPEED");
                break;
        }
    }
}



/*
private void FixedUpdate()
{
    SpeedometerTexture();
}


public void SpeedometerTexture()
{
    if(GlobalVariables.overSpeed)
    {
        if (GlobalVariables.safetyStillDistance)
        {
            this.GetComponent<Image>().sprite = textureDistanceSpeed; 
        }
        else
        {
            this.GetComponent<Image>().sprite = textureSpeed; 
        }
    }
    else
    {
        if(GlobalVariables.safetyDistance && !GlobalVariables.safetyStillDistance)
        {
            this.GetComponent<Image>().sprite = textureDistance;
        }
        else
        {
            if (GlobalVariables.safetyStillDistance)
            {
                this.GetComponent<Image>().sprite = textureEmergency;
            }
        }

        this.GetComponent<Image>().sprite = textureNormal;
    }

}

// Use this for initialization
void Start () {
    this.GetComponent<Image>().sprite = textureNormal;
}

// Update is called once per frame
void Update () {

}
}

/*
public void setSpeedLimitWarning(bool active)
{
Debug.Log("Set Limit!");
this.GetComponent<Image>().sprite = active ? textureLimit : textureNormal; // Ternary assignment, assign the limit texture if limit is active, otherwise the normal texture
}

public void setSafetyAlert(bool active)
{
Debug.Log("Set Limit!");
this.GetComponent<Image>().sprite = distance ? (speedlimit ? d + s : d) : (speedlimit ? s : none) // Ternary assignment, assign the limit texture if limit is active, otherwise the normal texture
}
*/
