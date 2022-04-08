using System.Collections;
using System.Collections.Generic;
using UnityEngine; //Created followingthistutorial: https://www.youtube.com/watch?v=7EaEBerzkOg

public class AnalogueSpeedConverter : MonoBehaviour {  // this script is to be added to the needle of the speedometer in the car. please check the min and max angle value for the speedometer

    static float minAngle = 0;  //angle for 0 speed on the needle
    static float maxAngle = -177;   //angle for max speed on the needle ''' is -177 when the max speed is 145kmh  and -195 when max speed is 160kmh
    static AnalogueSpeedConverter thisSpeedo;



    // Use this for initialization
    void Start () {
        thisSpeedo = this;
		
	}
	
	// Update is called once per frame
	public static void ShowSpeed(float speed, float min, float max)                       //get speed from rigidbody in CarController script
    {
        float ang = Mathf.Lerp(minAngle, maxAngle, Mathf.InverseLerp(min, max, speed )); //the angular value is derived from a linear interpolation between the min and max angle of the needle and the current min and max speed of the player
        thisSpeedo.transform.localRotation = Quaternion.Euler(0, 0, ang);               //the angular value is applied on the needle rotation vector
	}
}
