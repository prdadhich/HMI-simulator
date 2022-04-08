using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    public static Transform goal;
    public static Transform nextgoal;
    public static Transform nextnextgoal;

    float accuracy = 2f;
    float rotSpeed = 5f;
    public GameObject wpManager;
    GameObject[] wps;
    GameObject currentNode;
    int currentWP = 0;
    Graph g;
    public static GameObject[] NavigationPoints;
    private int _speed;
    public static GameObject CNode;
   



    private void Start()
    {
        wps = wpManager.GetComponent<WPManager>().wayPoints;
        g = wpManager.GetComponent<WPManager>().graph;
        
        currentNode = wps[1];
        
    }

    private void LateUpdate()
    {

        _speed =  GetSpeed.SendSpeed;

        if (g.getPathLength() == 0 || currentWP == g.getPathLength())
            return;
        
        currentNode = g.getPathPoint(currentWP);
        
        if (Vector3.Distance(
            g.getPathPoint(currentWP).transform.position,
            transform.position) < accuracy)
        {
            currentWP++;
            
        }
        if (currentWP < g.getPathLength())
        {
            goal = g.getPathPoint(currentWP).transform;
            
            
            Vector3 lookAtGoal = new Vector3(goal.position.x, this.transform.position.y, goal.position.z);
            Vector3 direction = lookAtGoal - this.transform.position;
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
                Quaternion.LookRotation(direction),
                Time.deltaTime * rotSpeed);
            this.transform.Translate(0, 0, .5f);
           // if (currentWP < 5)
            //{
                nextgoal = g.getPathPoint(currentWP + 1).transform;
              //  nextnextgoal = g.getPathPoint(currentWP + 2).transform;
           // Debug.Log("next"+nextgoal.transform.position);
           // Debug.Log("nextnext"+nextnextgoal.transform.position);
            // }


        }


    }
    public void GoToDestination()
    {

        g.AStar(currentNode, wps[124]);
        currentWP = 0;
    }
    public void GoToRestaurant()
    {

        g.AStar(currentNode, wps[26]);
        currentWP = 0;
    }


}
