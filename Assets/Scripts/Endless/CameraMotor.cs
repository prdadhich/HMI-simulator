using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotor : MonoBehaviour {

    private Transform lookAt;   //makes it look to the transform component of an object
    private Vector3 startOffset;   //create a starting offset to avoid compenetration between player and camera


	// Use this for initialization
	void Start () {
        lookAt = GameObject.FindGameObjectWithTag ("Player").transform;  //makes the lookAt value identical to the player position
        
        startOffset = transform.position - lookAt.position;    //calculate the distance offset from the starting position of the camera
    }
	
	// Update is called once per frame
	void Update () {
        transform.position = lookAt.position + startOffset;   //make a transformation identical to the lookAt value
        

	}
}
