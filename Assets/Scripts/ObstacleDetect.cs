using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObstacleDetect : MonoBehaviour
{
    public GameObject[] obstacl;
    public GameObject antena;
    public Text textbox;
    // Start is called before the first frame update
    void Start()
    {
        
        obstacl = GameObject.FindGameObjectsWithTag("obstacle");
        
    }
    // Update is called once per frame
    void Update()
    {
        foreach (var obs in obstacl)
        {
            if (Vector3.Distance(antena.transform.position, obs.transform.position) < 200 )
            {
              //  Debug.Log("Distance less than 200");
                textbox.text = "vehicle approaching".ToString();
            }
            else
            {
                //Debug.Log("route is free");
                textbox.text = "route is free".ToString();
            }
        }
    }
}
