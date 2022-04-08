using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisPrint : MonoBehaviour {

    public List<string> axisList = new List<string>();

    public List<string> buttonsList = new List<string>();

    // Use this for initialization
    void Start () {
        axisList.Add("Jump");
        axisList.Add("Brakepedal");
        axisList.Add("Horizontal");
        axisList.Add("Vertical");
        axisList.Add("Accelerator");
        axisList.Add("Axis1");
        axisList.Add("Axis2");
        axisList.Add("Axis3");
        axisList.Add("Axis4");
        axisList.Add("Axis5");
        axisList.Add("Axis6");
        axisList.Add("Axis7");
        axisList.Add("Axis8");
        axisList.Add("Axis9");
        axisList.Add("Axis10");
        axisList.Add("Axis11");
        axisList.Add("Axis12");
        axisList.Add("Axis13");
        axisList.Add("Axis14");
        axisList.Add("Axis15");
        axisList.Add("Axis16");
        axisList.Add("Axis17");
        axisList.Add("Axis18");
        axisList.Add("Axis19");
        axisList.Add("Axis20");
        axisList.Add("Axis21");
        axisList.Add("Axis22");
        axisList.Add("Axis23");
        axisList.Add("Axis24");
        axisList.Add("Axis25");
        axisList.Add("Axis26");
        axisList.Add("Axis27");
        axisList.Add("Axis28");
        axisList.Add("Axis29");
        axisList.Add("Axis30");
        axisList.Add("Axis31"); 
        axisList.Add("Axis32");
        axisList.Add("Axis33");
        axisList.Add("Axis");

        buttonsList.Add("joystick button 0");
        buttonsList.Add("joystick button 1");
        buttonsList.Add("joystick button 2");
        buttonsList.Add("joystick button 3");
        buttonsList.Add("joystick button 4");
        buttonsList.Add("joystick button 5");
        buttonsList.Add("joystick button 6");
        buttonsList.Add("joystick button 7");
        buttonsList.Add("joystick button 8");
        buttonsList.Add("joystick button 9");
        buttonsList.Add("joystick button 10");
        buttonsList.Add("joystick button 11");
        buttonsList.Add("joystick button 12");
        buttonsList.Add("joystick button 13");
        buttonsList.Add("joystick button 14");
    }
	
	// Update is called once per frame
	void Update () {

        foreach (string axis in axisList)
            //Debug.Log(axis+" value is: "+UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager.GetAxis(axis));
            ;
    }
}
