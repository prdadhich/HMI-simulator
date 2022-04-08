using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 moveVector;        //every frame recalculate the move vector
    

    private float speed = 60.00f;      // controlls the speed of the object. value as meter per second
    private float verticalVelocity = 0.0f;  //the speed of gravity is player is not grounded
    private float gravity = 9.8f;           //you know this one!

	// Use this for initialization
	void Start () {
        controller = GetComponent<CharacterController> ();		
	}
	
	// Update is called once per frame
	void Update () {

        moveVector = Vector3.zero;   //reset the vector

        if (controller.isGrounded)
        {
            verticalVelocity = -0.5f;
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }


        // Go to Edit/ProjectSettings/Inputs and activate

        // X - left right
        moveVector.x = Input.GetAxisRaw("Horizontal") * speed;
        // Y - Up down
        moveVector.y = verticalVelocity;
        // Z - forward backward
        moveVector.z = Input.GetAxisRaw("Vertical") * speed;

        controller.Move (moveVector * Time.deltaTime);  //adjust the frame accelleration to the frame rate to achieve a normalized speed
		
	}
}
