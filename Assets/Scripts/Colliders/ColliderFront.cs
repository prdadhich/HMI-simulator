﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderFront : MonoBehaviour
{
    private bool ForceEff = false;
    //[TODO] me trying to made impact forcefeedback work :(   ME MONKEY *UGH *UGH *UGH *banana pealed sound*  ___   "dishonor on my family"
    //Colliders on impact trigger force reaction on the steering wheel. it should be worked out in the steeringWheel script following logitech api
    public void OnTriggerEnter(Collider other)
    {
        GlobalVariables.ForceEff = true;
        Debug.Log("Impact on the Front!");
    }
}