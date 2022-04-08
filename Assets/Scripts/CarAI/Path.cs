using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour    //the Gizmos of the scene shall have "3D icons" deactivated and alpha of the line color to white
{

    public Color lineColor;

    public List<Transform> nodes = new List<Transform>();  //create a private list of nodes called "nodes"

    void OnDrawGizmosSelected() {  //to visualize the line drawn by the nodes inside the scene
        Gizmos.color = lineColor;

        Transform[] pathTransforms = GetComponentsInChildren<Transform>();   // find the nodes in the child components of the Path
        nodes = new List<Transform>();                                       // to empty the list at the beginning of the simulation

        for(int i = 0; i < pathTransforms.Length; i++) { //lists the transforms in the child
            if (pathTransforms[i] != transform) {        //picks a transform, and if it is not our own transform...
                nodes.Add(pathTransforms[i]);            // ...it adds it to the list

            }
        }
        for (int i = 0; i < nodes.Count; i++)
        {
            Vector3 currentNode = nodes[i].position;
            Vector3 previousNode = Vector3.zero;

            if (i > 0) {
                previousNode = nodes[i - 1].position;
            }   else if(i == 0 && nodes.Count > 1) {
                previousNode = nodes[nodes.Count - 1].position;
            }

            Gizmos.DrawLine(previousNode, currentNode);  //draws the pathline on screen so it is visible
            Gizmos.DrawWireSphere(currentNode, 0.3f);  //highlights the nodes position with a white sphere


        }
    }
}
