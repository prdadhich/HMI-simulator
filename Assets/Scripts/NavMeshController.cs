using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(LineRenderer))]

public class NavMeshController : MonoBehaviour
{
    private NavMeshAgent myAgent;
    [SerializeField]
    GameObject wayPointOne;
    private LineRenderer myLineRenderer;
  
    // Start is called before the first frame update
    void Start()
    {
        myAgent = GetComponent<NavMeshAgent>();
        myLineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Q)) 
        {
            myAgent.SetDestination(wayPointOne.transform.position);
            
        }
        if (myAgent.hasPath)
        {
            Debug.Log("Working");    
            DrawLine();
        }
    }
   void  DrawLine()
    {

        myLineRenderer.positionCount = myAgent.path.corners.Length;
        myLineRenderer.SetPosition(0, transform.position);

        if (myAgent.path.corners.Length < 2)
        {
            return;
        }
        for (int i = 1; i < myAgent.path.corners.Length; i++) 
        {
            Vector3 pointPosition = new Vector3(myAgent.path.corners[i].x, myAgent.path.corners[i].y, myAgent.path.corners[i].z);
            myLineRenderer.SetPosition(i, pointPosition);
            
        }
    }
}
