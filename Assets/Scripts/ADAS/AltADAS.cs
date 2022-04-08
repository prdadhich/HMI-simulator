using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class AltADAS : MonoBehaviour
{

    [Header("Alt. Texture")]            //the title of the section on the component
    public Texture2D textureNormal;        //texture for the car
    public Texture2D textureSpeed;      //alternative texture for when the speed limit is not respected
    public Texture2D textureDistance;   //alternative texture for when the safety distance is not respected
    public Texture2D textureDistanceSpeed;   //alternative texture for when the safety distance is not respected
    public Texture2D textureEmergency;  //alternative texture for when the safety distance is not respected
    public Renderer ADASlight1;

    //State machine for adas light
    public enum AdasStates {Normal, Overspeed, SafetyDistance, SafetyDistanceSpeed, Emergency}
    public AdasStates currentState, nextState;


    // Use this for initialization
    void Start()
    {
        // Update the state every second
        InvokeRepeating("UpdateState", 0, 0.1f);

        nextState = AdasStates.Normal;

        //ADASlight1.material.mainTexture = textureNormal;
    }

    private void UpdateState()       // Every time you change the state, you update it with the next one and set the next to the same state
    {
        currentState = nextState;
        nextState = currentState;
        AdasManager();
    }

    private void FixedUpdate()
    {
        ADASglow();
    }


    public void ADASglow()
    {
        if (GlobalVariables.safetyStillDistance)
        {
            if (GlobalVariables.overSpeed) //what to display when speeding and not respecting the distance from still cars
            {
                nextState = AdasStates.SafetyDistanceSpeed;
            }
            else //what to display when the car in front of you is still
            {
                nextState = AdasStates.Emergency;
            }
        }
        else
        {
            if (GlobalVariables.overSpeed)  //what to display when speeding
            {
                nextState = AdasStates.Overspeed;
            }
            else if (GlobalVariables.safetyDistance) //what to display when not respecting the distance from moving cars
            {
                nextState = AdasStates.SafetyDistance;
            }
            else //what to display when nothing happens
            {
                nextState = AdasStates.Normal;
            }
        }
    }
    //STATE MACHINE to determine the correct ADAS condition
    private void AdasManager()
    {
        switch (currentState)
        {
            case AdasStates.Normal:
                ADASlight1.material.mainTexture = textureNormal;
                //Debug.Log("ADAS entered state NORMAL");
                break;
            case AdasStates.Emergency:
                ADASlight1.material.mainTexture = textureEmergency;
                //Debug.Log("ADAS entered state EMERGENCY");
                break;
            case AdasStates.Overspeed:
                ADASlight1.material.mainTexture = textureSpeed;
                //Debug.Log("ADAS entered state OVERSPEED");
                break;
            case AdasStates.SafetyDistance:
                ADASlight1.material.mainTexture = textureDistance;
                //Debug.Log("ADAS entered state SAFETY");
                break;
            case AdasStates.SafetyDistanceSpeed:
                ADASlight1.material.mainTexture = textureDistanceSpeed;
                //Debug.Log("ADAS entered state SAFETY OVERSPEED");
                break;
        }
    }
}


