using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AICarEngine : MonoBehaviour
{                                                 //This AI scripts are losely based on the following tutorial series https://www.youtube.com/watch?v=o1XOUkYUDZU&t=675s
                                                  // the Road Tiles shall be tagged as "terrain". OBJECTS with terrain tag will be ignored by the sensors
    public Vector3 centerOfMass;

    [Header("Veicle settings")]                   //the title of the section on the component
    public float maxSteerAngle = 35f;             //highest steer possible by the car
    public WheelCollider WheelFL;
    public WheelCollider WheelFR;
    public WheelCollider WheelBL;
    public WheelCollider WheelBR;
    private float maxMotorTorque = 2000f;         //torque when accellerating (until max speed is reached)  - max for truck is 3150f (N*m)
    private float maxBrakeTorque = 10000f;        //counter torque when braking  - about -100f
    private float emergencyBrakeTorque = 20000f;  //counter torque when braking  - about -100f
    private float maxCruiseTorque = 200f;         //max torque when a obstacle in the front is detected - about 200f

    // Compute current speed
    private Rigidbody AI_Rigidbody;
    private float ai_Topspeed = 105;
    public float aiCurrentSpeed { get { return AI_Rigidbody.velocity.magnitude * 3.6f; } }
    public float aiMaxspeed { get { return ai_Topspeed; } }     // maxspeed define the maximum speed; it is a constant
    
    //public float currentSpeed;
    private const float maxspeed = 100f;    // maxspeed define the maximum speed; it is a constant
    private float maxSpeed = maxspeed;      // "maxSpeed" floats with the constant "maxspeed"
    public bool SpeedingVehicle = false;   //make a bool switch to check the braking variant
   
    //public float maxSpeed=100f;
    [Header("AI Behaviour")]               //the title of the section on the component
    //public bool cruiseSpeed = false;     //make a bool switch to check the braking variant
    //public bool cruiseControl = false;   //make a bool switch to check the braking variant
    //public bool isBraking = false;       //make a bool switch to check the braking variant
    [Header("Braking effect")]             //the title of the section on the component
    public Texture2D textureNormal;        //texture for the car
    public Texture2D textureBraking;       //alternative texture for when the car i braking
    //public Renderer brakingRenderer;     //the 3d mesh to apply the alternative texture
    public Renderer brakeLRenderer;
    public Renderer brakeRRenderer;

    public static int emergencyEventCount = 0;
    public static int enough_emergencyEvents = 2;    //number of emergency brake events to trigger the next event


    //AI SENSOR SECTION:
    [Header("Sensors")]                                //the title of the section on the component
    public float sensorLengthFront = 60f;              //the range of the sensor  - about 20f
    public float sensorLengthCruise = 50f;             //the range of the sensor  - about 50f
    public float sensorLengthCruiseSpeed = 150f;       //the range of the sensor  - about 100f
    public float sensorLengthSide = 6f;                //the range of the sensor  - about 6f
    public float sensorLengthBack = 30f;               //the range of the sensor  - about f
    public float frontSideSensorPos = 1f;
    //public float frontSensorAngle = 30;              //the agle of the front side sensor
    public float tileSensorLength = 5f;                //the range of the sensor
    public float tileSensorAngle = 90;                 //the angle of the overtake sensor
    public Vector3 backSensorPos = new Vector3(0f, 0.5f, -3f);
    public float eventSensorAngle = 180;               //the agle of the front side sensor
    public int brakeDisableTime = 10;                  // Seconds after which the AI will return to the non-braking state

    //public static string AIStates.currentState;

    // List containing all the nodes of the trajectory to be followed
    public List<Transform> nodes;
    private int currentNode = 0;
    private bool avoiding = false;  //the behaviour that avoids obstacles on the sides by steering on the opposite direction
    private Vector3 sensorStartPos = new Vector3(0, 0, 0);                 //is the starting position of the sensors. it must be related to the car, not the enviroment
    public Vector3 sensorOffset = new Vector3(0, 0, 3);
    private Vector3 sensorBackPos = new Vector3(0, 0, 0);      //is the starting position of the back sensors. it must be related to the car, not the enviroment
    public Vector3 sensorBackOffset = new Vector3(0, 0, -3);   //is the offset to place the back sensors in the correct position
    private float tracerOffset = 0.9f;
    private bool isCoroutineExecuting = false;
    private bool running;
    private bool BackwardSensorIsActive = true;

    private TileManager tileManager = null;
    // Starting point of the last added tile
    private float lastZ = -100.0f;
    public string writePathToFollow = "Path2";

    // Reference to the Logger class
    private Logger Logger = null;
    // Reference to the EventManager class
    private EventManager EventManager = null;

    public enum AIStates { Braking, Drive } // ,Limit};

    public AIStates currentState, nextState;

    private float m_Downforce = 100f; //apply this force on the colliders to increese the aederence to the road when steering at high speed

    private bool isStill = false;
    private bool detectStillActive = false;
    private bool destroyStillActive = false;
    private bool isAlive = true;
    private bool readyForEmergency = false;

    private GameObject ThisCar;
    private BoxCollider stillCarBox = null;
    //private Transform stillCarBoxT;

    private void Start()
    {

            foreach (Transform child in transform)
            {
                if (child.name == "StillCarBox")
                    //Debug.Log("Assigned!");
                    stillCarBox = child.GetComponent<BoxCollider>();
            }
            //Debug.Log("my name is " + this.name + ":" + stillCarBox);
                    
            AI_Rigidbody = this.GetComponent<Rigidbody>();
            tileManager = GameObject.FindWithTag("TileManager").GetComponent<TileManager>();
            // Addtilenodes is called every 1.0 second
            InvokeRepeating("addTileNodes", 0, 1.0f);

            // Update the state every second
            InvokeRepeating("UpdateState", 0, 0.5f);

            // Get reference to another script just by using the owner gameobject's tag
            EventManager = GameObject.FindWithTag("EventManager").GetComponent<EventManager>();
            nextState = AIStates.Drive;
            Collider colidedObj = this.GetComponentInChildren<Collider>();
    }

    private void UpdateState()       // Every time you change the state, you update it with the next one and set the next to the same state
    {
        currentState = nextState;
        nextState = currentState;
        DriveManager();
    }

    private void addTileNodes()
    {
        float z = this.transform.position.z;
        if (z < lastZ)
        {
            return;
        }

        bool found = false;
        // [TODO] Refactor this algorithm as binary search
        foreach(float tileZ in tileManager.activeTileZ)
        {
            if (tileZ > z)
            {
                found = true;
                lastZ = tileZ;
                break;
            }
        }
        if (found)
        {
            GameObject tile = tileManager.zToTile[lastZ];
            // Change "Path1" to let the car pick the nodes from a different lane
            Path pathScript = (Path)tile.transform.Find(writePathToFollow).GetComponent(typeof(Path));
            nodes.AddRange(pathScript.nodes);
        }
    }


    IEnumerator DisableBraking() // Coroutine which will disable the braking flag after brakeDisableTime seconds
    {
        yield return new WaitForSeconds(2);
        if(currentState == AIStates.Braking)
            nextState = AIStates.Drive;
    }

    IEnumerator DetectStill() // Coroutine which will detect a still car that should drive
    {
        yield return new WaitForSeconds(5);
        detectStillActive = false;
        if (currentState == AIStates.Drive)
        {
            if (aiCurrentSpeed < (maxspeed / 10))  //if the car is still after 5 seconds
            {
                isStill = true;                    //mark it as still
            }
        }
    }

    IEnumerator DestroyStill() // Coroutine which will count 5 seconds before checking a still car speed and eventualy destroy it
    {
        yield return new WaitForSeconds(5);
        destroyStillActive = false;
        if (isStill && currentState == AIStates.Drive)
        {
            if (aiCurrentSpeed < (maxspeed / 10))
            {
                //isAlive = false;
                Destroy(this.gameObject);
                //Debug.Log("I'm about to die!");
                Destroy(this);
                //Debug.Log("I'm back motherfuckers! How? Who knows!");
            }
        }
    }


    private void FixedUpdate ()
    {
        Sensors();
        ApplySteer();
        CheckWaypointDistance();
        SpeedingCar();
        CheckEvent();
        GetComponent<Rigidbody>().AddForce(Physics.gravity, ForceMode.Acceleration);
        DestroyCar();
    }

    private void Sensors()
    {
        RaycastHit hit;                                          //Raycast is used for the sensor logic

        float avoidMultiplier = 0;                               //default avoidance value is zero (the car meets no obstacles)
        avoiding = false;                                        //default avoidance is set to false
        //cruiseSpeed = false;                                     //default cruisespeed is set to false
        //cruiseControl = false;                                   //default cruisecontrol is set to false

        sensorStartPos = gameObject.transform.position;          // assign sensor position to the center of the car
        sensorStartPos += transform.forward * sensorOffset.z;    // sensor offset
        sensorStartPos += transform.up * sensorOffset.y;

        sensorBackPos = gameObject.transform.position;
        sensorBackPos += transform.forward * backSensorPos.z;    // sensor offset
        sensorBackPos += transform.up * backSensorPos.y;


        //CRUISE SPEED SENSOR  to reduce the max speed  when another car is in front
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLengthCruiseSpeed))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);   //to check if the sensor is triggered by drawing a line
                maxSpeed = maxspeed / 3;   //the target maximum speed is reduced to 33%
            }
            else
            {
                maxSpeed = maxspeed;        //the target maximum speed is restored
            }
        }

        //CRUISE SENSOR to avoid further accelleration when another car is in front
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLengthCruise))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                if (!hit.collider.CompareTag("Terrain") && !hit.collider.CompareTag("AiCar")) //if the sensors hit a car that is still, make it brake instead of removing accelleration
                {
                    Debug.DrawLine(sensorStartPos, hit.point);   //to check if the sensor is triggered by drawing a line
                    nextState = AIStates.Braking;
                    StartCoroutine(DisableBraking());
                }
                Debug.DrawLine(sensorStartPos, hit.point);   //to check if the sensor is triggered by drawing a line
            }
        }

        //SENSOR for BRAKING
        for (int step = -1; step < 2; step++) //makes a matrix 3x1 of braking sensors to have a better detection
        {
            // Set the sensor position to make an array of raytracers
            Vector3 newStartPos = sensorStartPos + step * new Vector3(tracerOffset, 0, 0);
            if (Physics.Raycast(newStartPos, transform.forward, out hit, sensorLengthFront))   //set the sensor shape
            {
                if (!hit.collider.CompareTag("Terrain"))         // "!hit" ignores the object with Terrain tag
                {
                    Debug.DrawLine(newStartPos, hit.point);   //to check if the sensor is triggered by drawing a line
                    nextState = AIStates.Braking;
                    StartCoroutine(DisableBraking());
                }
                else
                {
                    //nextState = AIStates.Drive;
                }
            }
        }
        sensorStartPos += transform.right * frontSideSensorPos;

        /*
        //TILE DETECTION SENSOR 1
        if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(tileSensorAngle, transform.right) * transform.forward, out hit, tileSensorLength))
        {
            if (!hit.collider.CompareTag("3to2to4"))
            {
                readyForEmergency = true;
                Debug.DrawLine(sensorStartPos, hit.point);   //to check if the sensor is triggered by drawing a line
                Debug.Log("I'm on the spot to be dangerous!");
            }
        }*/

        if (avoiding)
        {
            WheelFL.steerAngle = maxSteerAngle * avoidMultiplier;  //the max steer angle for the car is multiplied to the avoidMultiplier (0 to 1 range)
            WheelFR.steerAngle = maxSteerAngle * avoidMultiplier;
            // //nextState = AIStates.Limit;
        }

        if (aiCurrentSpeed < maxSpeed)
        {
            //nextState = AIStates.Drive;
        }
    }

    private void ApplySteer()
    {
        if (currentNode >= nodes.Count)
            return;
        if (avoiding) return;

        try
        {
            if (this.gameObject.transform != null)
            {
                Vector3 relativeVector = this.gameObject.transform.InverseTransformPoint(nodes[currentNode].position); //make the steering equal to the inverse calculation of the movement required to go from the current position to the next node
                float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle; //the velocity of the steering is divided by its magnitude
                WheelFL.steerAngle = newSteer;
                WheelFR.steerAngle = newSteer;
            }
        }
        catch
        {
            Destroy(this.gameObject);
            Destroy(this);
            //Debug.Log("Horrible things happened!");

        }

        // //nextState = AIStates.Limit;
    }

    private void CheckWaypointDistance()
    {
        if (currentNode >= nodes.Count)
            return;
        if (nodes[currentNode] != null)
        {
            if (Vector3.Distance(transform.position, nodes[currentNode].position) < 15.5f)  //change target node to the next when reaching this distance
            {
                currentNode++;
            }
        }
    }

    private void DriveManager()
    {
        //Collider stillCarBox = GameObject.Find("StillCarBox").GetComponent<BoxCollider>();
        //Debug.Log("Hello world,  my name is " + this.name);
        switch (currentState)
        {
            case AIStates.Braking:  //stops any accelleration and decelerate by braking
                //The following things are always true inside Braking
                WheelFL.motorTorque = 0;
                WheelFR.motorTorque = 0;
                WheelFL.brakeTorque = maxBrakeTorque;
                WheelFR.brakeTorque = maxBrakeTorque;
                WheelBL.brakeTorque = maxBrakeTorque;
                WheelBR.brakeTorque = maxBrakeTorque;
                brakeLRenderer.material.mainTexture = textureBraking;
                brakeRRenderer.material.mainTexture = textureBraking;
                stillCarBox.transform.gameObject.SetActive(true);
                //Debug.Log("Braking!");
                break;
            case AIStates.Drive: //accellerate to max (untill max speed is reached)
                if (aiCurrentSpeed <  maxSpeed)  //aiMaxspeed)
                {
                    if (aiCurrentSpeed < 50f)                     //When the car is picking up speed the torque is applyed to 4 wheels to reduce the downtime of freshly spawned cars to reach cruise speed
                    {
                        WheelFL.motorTorque = maxMotorTorque;     
                        WheelFR.motorTorque = maxMotorTorque;
                        WheelBL.motorTorque = maxMotorTorque;     //the car is now 4WD
                        WheelBR.motorTorque = maxMotorTorque;
                        stillCarBox.transform.gameObject.SetActive(true);
                        //stillCarBox.enabled = true;
                    }
                    else
                    {
                        WheelFL.motorTorque = maxMotorTorque;
                        WheelFR.motorTorque = maxMotorTorque;
                        WheelBL.motorTorque = 0;                  //the car is now 2WD
                        WheelBR.motorTorque = 0;
                        stillCarBox.transform.gameObject.SetActive(false);
                        //stillCarBox.enabled = false;
                    }
                }
                else                                             //when the max cruise speed is reached
                {
                    WheelFL.motorTorque = 0;                     //no further accelleration on the front wheels
                    WheelFR.motorTorque = 0;
                    stillCarBox.transform.gameObject.SetActive(false);
                    //stillCarBox.enabled = false;
                }
                //The following things are always true inside Drive
                WheelFL.brakeTorque = 0;
                WheelFR.brakeTorque = 0;
                WheelBL.brakeTorque = 0; 
                WheelBR.brakeTorque = 0;
                brakeLRenderer.material.mainTexture = textureNormal;
                brakeRRenderer.material.mainTexture = textureNormal;
                //Debug.Log("Drive!");
                break;
        }
    }

    private void AddDownForce() //Adds a downforce to increese ground contact on 
    {
        WheelFL.attachedRigidbody.AddForce(-transform.up * m_Downforce * WheelFL.attachedRigidbody.velocity.magnitude);
        WheelFR.attachedRigidbody.AddForce(-transform.up * m_Downforce * WheelFR.attachedRigidbody.velocity.magnitude);
        WheelBL.attachedRigidbody.AddForce(-transform.up * m_Downforce * WheelBL.attachedRigidbody.velocity.magnitude);
        WheelBR.attachedRigidbody.AddForce(-transform.up * m_Downforce * WheelBR.attachedRigidbody.velocity.magnitude);
    }

    private void SpeedingCar()           //the car has now a super high max speed, thatmeans that it will push the vehicle to the physical limitation of the car (going around 130/140kmh enough to surpas a not speeding player)
    {
        if (SpeedingVehicle)
        {
            maxSpeed = maxspeed * 5/3 ;  //the target maximum speed is increesed
        }
        else
        {
            maxSpeed = maxspeed;         //the target maximum speed is restored
        }
    }

    private void CheckEvent() //[TODO] 
    {
        Vector3 sensorBackPos = transform.position;              //is the starting position of the back sensors. it must be related to the car, not the enviroment
        sensorBackPos += transform.forward * backSensorPos.z;    // sensor offset
        sensorBackPos += transform.up * backSensorPos.y;

        RaycastHit hit;                                          //Raycast is used for the sensor logic

        if (BackwardSensorIsActive && EventManager.state == EventManager.EventStates.Emergency || EventManager.state == EventManager.EventStates.Exit)
        {
            //Debug.Log("Back è attivo");
            //sensorBackPos = gameObject.transform.position + transform.forward * backSensorPos.z; // sensor offset to position it on the back of every aiCar
            if (Physics.Raycast(sensorBackPos, Quaternion.AngleAxis(eventSensorAngle, transform.up) * transform.forward, out hit, sensorLengthBack))
            {
                if (hit.collider.CompareTag("Player")) //(!hit.collider.CompareTag("Terrain") && !hit.collider.CompareTag("AiCar") && hit.collider.CompareTag("Player"))
                {
                    //Debug.Log("Back ha toccato il giocatore");
                    if (currentState == AIStates.Drive && aiCurrentSpeed > 70f)          //the car brakes until it reaches 33% of the max speed (about 40km/h from 100km/h)
                    {
                        //Debug.Log("Back ha fatto il suo dovere");
                        nextState = AIStates.Braking;
                        emergencyEventCount++;
                        BackwardSensorIsActive = false; //it removes the event braking, allowing this function to work only once
                    }

                    if (emergencyEventCount >= enough_emergencyEvents )
                        EventManager.nextEvent();

                    Logger = GameObject.FindWithTag("Logger").GetComponent<Logger>();

                    Logger.setEmergencyEventCount(emergencyEventCount);    //Log the overtake count

                    Debug.DrawLine(sensorBackPos, hit.point);   //to check if the sensor is triggered by drawing a line
                }
            }
        }
    }

    private void OnTriggerEnter(Collider collidedObj)
    {
        if (collidedObj.gameObject.tag == "AiCar") //when it read aicar tag
        {
            ///*
            //Debug.Log("AIUTO INCIDENTE! R.I.P AiCar :'( ");
            Destroy(this.gameObject);
            //Debug.Log("I'm about to die!");
            isAlive = false;
            Destroy(this);
            //Debug.Log("I'm back motherfuckers! How? Who knows!");
            //*/
        }
    }

    private void DestroyCar() //the function to destroy aiCars involved in accidents or just falling out of the highway
    {
        if (transform.position.y < -5 || transform.position.y > 5 )
        {
            // /*
            Destroy(this.gameObject);
            //Debug.Log("I'm about to die!");
            isAlive = false;
            Destroy(this);
            //Debug.Log("I'm back motherfuckers! How? Who knows!");
            //*/
        }
        
        if (!detectStillActive && currentState == AIStates.Drive && aiCurrentSpeed < (maxspeed / 10))
        {
            detectStillActive = true;
            StartCoroutine(DetectStill());
        }
        if (!destroyStillActive && isStill)
        {
            destroyStillActive = true;
            StartCoroutine(DestroyStill());
        }   //*/
    }
    
}



//OLD UNUSED SENSORS
/*
//FRONT-RIGHT SENSOR
sensorStartPos += transform.right * frontSideSensorPos;
if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLengthSide))
{
    if (!hit.collider.CompareTag("Terrain"))
    {
        Debug.DrawLine(sensorStartPos, hit.point);   //to check if the sensor is triggered by drawing a line
        avoiding = true;
        avoidMultiplier -= 0.5f;    //react to obstacles with a strong steer on the left
        cruiseSpeed = true;
    }
}
*/
/*
//FRONT-RIGHT ANGLE SENSOR
if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(frontSensorAngle, transform.up) * transform.forward, out hit, sensorLengthSide))
{
    if (!hit.collider.CompareTag("Terrain"))
    {
        Debug.DrawLine(sensorStartPos, hit.point);   //to check if the sensor is triggered by drawing a line
        avoiding = true;
        avoidMultiplier -= 0.5f;  //react to obstacles with a light steer on the left
    }
}
*/
/*
//FRONT-LEFT SENSOR
sensorStartPos -= transform.right * frontSideSensorPos * 2;
if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLengthSide))
{

    if (!hit.collider.CompareTag("Terrain"))         // "!hit" ignores the object with Terrain tag
    {
        Debug.DrawLine(sensorStartPos, hit.point);   //to check if the sensor is triggered by drawing a line
        avoiding = true;
        avoidMultiplier += 1f;     //react to obstacles with ...
        cruiseSpeed = true;
    }
}
*/
/*
//FRONT-LEFT ANGLE SENSOR
if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(-frontSensorAngle, transform.up) * transform.forward, out hit, sensorLengthSide))
{
    if (!hit.collider.CompareTag("Terrain"))
    {
        Debug.DrawLine(sensorStartPos, hit.point);   //to check if the sensor is triggered by drawing a line
        //avoiding = true;
        //avoidMultiplier += 0;     //react to obstacles with ...
    }
}
*/
