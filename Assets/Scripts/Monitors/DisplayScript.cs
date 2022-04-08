using UnityEngine;
using System.Collections;

public class DisplayScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//Debug.Log("displays connected: " + Display.displays.Length);
		// Display.displays[0] is the primary, default display and is always ON.
		// Check if additional displays are available and activate each.
        /*
		if (Display.displays.Length > 1)
			Display.displays[1].Activate();
		if (Display.displays.Length > 2)
			Display.displays[2].Activate();
        */
        for(int i=0; i< Display.displays.Length;i++)
        { Display.displays[i].Activate(); }


    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
