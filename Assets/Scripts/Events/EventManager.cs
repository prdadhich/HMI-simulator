using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour {

    // Event Manager class, handles the various game events. Modeled after a Mealy Machine FSA

    public enum EventStates { Initial, Overtake, Emergency, Exit, End };                        // States of the Event Manager FSA Machine, these could be added as desired
    private List<EventStates> stateSequence = new List<EventStates> {      // Current order of the events
                                                                      EventStates.Emergency,
                                                                      EventStates.Exit }; 
    public EventStates state { get; private set; }
    private static System.Random rand = new System.Random();
    
    // Reference to the Logger class
    private Logger Logger = null;

    public void nextEvent()  // Call this public method when you want to proceed to the next event in the order
    {
        state = stateSequence[0];
        stateSequence.RemoveAt(0);

        //Debug.Log("Next event has been called");
    }
    /*
    public void randomEvent() // Call this public method when you want to set a random event without modifying the events order
    {
        int index = rand.Next(stateSequence.Count);
        state = stateSequence[index];
        stateSequence.RemoveAt(index);
    }//*/
    /*
    public void finalEvent() // [TODO] Call this to terminate the simulator
    {
        state = EventStates.End;
    }//*/

	// Use this for initialization
	void Start ()
    {
        state = EventStates.Overtake; // Set the machine state to the initial state as overtake

        // Get reference to another script just by using the owner gameobject's tag
        Logger = GameObject.FindWithTag("Logger").GetComponent<Logger>();
    }

    // [TODO] Put here code to manually set event state with keyboard keys
    void Update ()
    {
    // Log the current event between 0-1
    Logger.setEventstate(state.ToString());
	}
}


