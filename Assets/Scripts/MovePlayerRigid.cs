using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayerRigid : MonoBehaviour
{
    Rigidbody m_Rigidbody;
    public float m_Speed = 5f;

    // void Start()
    // {
    //     //Fetch the Rigidbody from the GameObject with this script attached
    //     m_Rigidbody = GetComponent<Rigidbody>();
    // }
    //
    // void FixedUpdate()
    // {
    //     //Store user input as a movement vector
    //     Vector3 m_Input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
    //
    //     //Apply the movement vector to the current position, which is
    //     //multiplied by deltaTime and speed for a smooth MovePosition
    //     m_Rigidbody.MovePosition(transform.position + m_Input * Time.deltaTime * m_Speed);

    // }

    public Rigidbody rb;
    public Transform car;
    public float speed = 17;


    Vector3 rotationRight = new Vector3(0, 30, 0);
    Vector3 rotationLeft = new Vector3(0, -30, 0);

    Vector3 forward = new Vector3(0, 0, 1);
    Vector3 backward = new Vector3(0, 0, -1);

    void FixedUpdate()
    {
        if (Input.GetKey("w"))
        {
            transform.Translate(forward * speed * Time.deltaTime);
        }
        if (Input.GetKey("s"))
        {
            transform.Translate(backward * speed * Time.deltaTime);
        }

        if (Input.GetKey("d"))
        {
            Quaternion deltaRotationRight = Quaternion.Euler(rotationRight * Time.deltaTime);
            rb.MoveRotation(rb.rotation * deltaRotationRight);
        }

        if (Input.GetKey("a"))
        {
            Quaternion deltaRotationLeft = Quaternion.Euler(rotationLeft * Time.deltaTime);
            rb.MoveRotation(rb.rotation * deltaRotationLeft);
        }
    }
}

