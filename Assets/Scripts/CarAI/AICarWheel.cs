using System.Collections;
using UnityEngine;

public class AICarWheel : MonoBehaviour {

    public WheelCollider targetWheel;  //allows to connect the wheel to the appropriate wheel collider

    private Vector3 wheelPosition = new Vector3();
    private Quaternion wheelRotation = new Quaternion();
	
	private void Update () {
        targetWheel.GetWorldPose(out wheelPosition, out wheelRotation); //get a variable
        transform.position = wheelPosition;    //copy the position
        transform.rotation = wheelRotation;    //copy the rotation

	}
}
