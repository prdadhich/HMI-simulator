using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;


public class PlayerControl : MonoBehaviour
{   public float speed = 100f;
    public float rotSpeed = 100f;
    public CharacterController control;
    private float smoothVelocity = 10.0f;
    private float turnsmoothTime = 0.1f;
    private float initialVelocity = 0f;
    private float acc = 0f;
    public Text _displayspeed;
    public static float followpathvelocity;
    public static int speedkmph;
   
    
    // Start is called before the first frame update
  

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
       // Vector3 move = new Vector3(0f,0f,z).normalized;
        /*if (move.magnitude > 0.1f)
         {
             float targetAngle = Mathf.Atan2(move.x,move.z)*Mathf.Rad2Deg;
             float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref smoothVelocity, turnsmoothTime);
             transform.rotation = Quaternion.Euler(0f,angle,0f);
             control.Move(move * speed * Time.deltaTime);
         }*/
        float rotation = x * rotSpeed*Time.deltaTime;
        transform.Rotate(0f, rotation,0f);
        acc = z;
        if (z < 0)
            acc = z * 2;
        float velocity = initialVelocity + acc * Time.deltaTime ;
        initialVelocity = velocity;
        followpathvelocity = velocity; 
        transform.Translate(0, 0, velocity);
        speedkmph = Mathf.Abs( (int)(velocity * 36f)) ;   
        _displayspeed.text = speedkmph.ToString();

    }
}
