#define B

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetMQ;
using NetMQ.Sockets;


public static class GlobalVariables
{
    public static float speed;
    public static float accelleration;
    public static float brake;              //braking level
    public static float steer;              //the steering value of the steeringwheel. positive value is a clockwise rotation; The pricise relation between this value and the angle should be investigated.
    public static string pedalmode = null;    //the current pedal mode sent with the can controler; when aapFeedback is off no mode will work
    public static int overtake;             //int value that count the number of overtakes made by the player; see PlayerSensors for the logic and the event trigger
    public static float playerZ;            //the distance on the Z axis between the player and the starting position that is more or less the total distance made in the simulation
    public static bool aapFeedback = true;  //set if the AAP feedback should be used. is controlled by a bool in BoschPedalControl that can deactivate the can controller


    //extra variables for speedometer and ADAS
    public static bool correctSpeed = false;
    public static bool overSpeed = false;
    public static bool safetyDistance = false;
    public static bool safetyStillDistance = false;

    //The condition of the collider on the player
    public static bool isLeftSideColliding = false;
    public static bool isRightSideColliding = false;
    public static bool ForceEff = false;

    public static int Trial_counter = 0;
   
    public enum TrialType { ADAPTATION, SOUND, HAPTIC };
    public static TrialType Trial_type = (TrialType)Trial_counter;
    public const float Trial_Time = 60; //dutation of the trial
#if A
    public static string[] Label = { "Adaptation_A", "Sound_A", "Haptic_A" };
    public static TrialType[] Trial_sequence = { TrialType.ADAPTATION, TrialType.SOUND, TrialType.HAPTIC };
#else
    public static string[] Label = { "Adaptation_B", "Haptic_B", "Sound_B" };
    public static TrialType[] Trial_sequence = { TrialType.ADAPTATION, TrialType.HAPTIC, TrialType.SOUND };
#endif
    public static bool isTestRunning = false;

    public static string LogFolder = "Logger/" + System.DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss/");

    public static PublisherSocket pub;


}
