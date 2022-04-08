using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    public class PlayerSensors : MonoBehaviour
    {
        public Vector3 centerOfMass;

        [Header("Speed")]                           //the title of the section on the component
        public float currentSpeed;
        public float safetyMovingDistanceValue;     //the safe cruise distance when the car in front is moving
        public float safetyStillDistanceValue;      //the safe distance when the car in front is stil
        //public float maxSpeed=100f;

        [Header("Sensor Response")]                 //the title of the section on the component
        public bool cruiseSpeed = false;            //make a bool switch to check the braking variant
        public bool cruiseControl = false;          //make a bool switch to check the braking variant
        public bool correctspeed = false;           //make a bool switch to check the braking variant
        public bool overspeed = false;              //make a bool switch to check the braking variant
        public bool safetyDistance = false;         //make a bool switch to check the braking variant
        public bool safetyStillDistance = false;    //make a bool switch to check the braking variant
        public static int overtakeCount = 0;
        public bool overtakeStarted = false;        //Alternative way to make the overtake sensors work alternatevly
        public static int enough_overtakes = 10;    //number of overtake to trigger the next event

        public const float speedLimit = 130f;       // maxspeed define the speed limit; it is a constant
        public WheelCollider WheelHubFrontLeft;     // the WheelCollider used for the calculation; must be assigned

        [Header("Speedometer References")]
        public GameObject SpeedometerPanel = null;

        [Header("Sensors Settings")]                //the title of the section on the component
        public float sensorLengthFront = 25f;       //the range of the sensor  - about 20f
        public float sensorLengthCruise = 60f;      //the range of the sensor  - about 50f
        public float sensorLengthCruiseSpeed = 100f;//the range of the sensor  - about 100f
        public float sensorLengthSide = 6f;         //the range of the sensor  - about 6f
        public float sensorLengthUnit = 1f;         //the sensor unit to scale the safety distance lenght with the player speed
        public float frontSideSensorPos = 0.5f;
        public float frontSensorAngle = 30;         //the agle of the front side sensor
        public float overtakeLength = 5f;           //the range of the sensor
        public float overtakeAngle = 90;            //the angle of the overtake sensor
        private float tracerOffset = 0.6f;

        private List<Transform> nodes;                             //the list that keeps the nodes
        //private int currentNode = 0;
        //private bool avoiding = false;                           //the behaviour that avoids obstacles on the sides by steering on the opposite direction
                       
        private Vector3 sensorStartPos = new Vector3(0, 0, 0);     //is the starting position of the sensors. it must be related to the car, not the enviroment
        private Vector3 sensorBackPos = new Vector3(0, 0, 0);      //is the starting position of the back sensors. it must be related to the car, not the enviroment
        public Vector3 sensorOffset = new Vector3(0, 0, 3);        //is the offset to place the front sensors in the correct position outside the player's car
        public Vector3 sensorBackOffset = new Vector3(0, 0, -3);   //is the offset to place the back sensors in the correct position

        // Reference to the Logger class
        private Logger Logger = null;
        // Reference to the EventManager class
        private EventManager EventManager = null;

        private bool aMovingCarInFront = false;
        private bool aStillCarInFront = false;

        CarController data;

        private void Start()
        {
            Logger = GameObject.FindWithTag("Logger").GetComponent<Logger>();    // Get reference to another script just by using the owner gameobject's tag
            EventManager = GameObject.FindWithTag("EventManager").GetComponent<EventManager>();
            data = gameObject.GetComponent<CarController>();
        }

        private void FixedUpdate()
        {
            IsThereACar();
            Sensors();
            SpeedLimit();
            SendSensorsInput();
        }

        private void IsThereACar()
        {
            aMovingCarInFront = false;
            aStillCarInFront = false;
        }

        private void Sensors()
        {
            RaycastHit hit;                                          //Raycast is used for the sensor logic

            cruiseSpeed = false;                                     //default is set to false
            cruiseControl = false;                                   //default is set to false
            safetyDistance = false;                                  //default is set to false
            safetyStillDistance = false;                             //default is set to false

            //is the starting position of the front sensors. it must be related to the car, not the enviroment
            sensorStartPos = gameObject.transform.position;          // assign sensor position to the center of the car
            sensorStartPos += transform.forward * sensorOffset.z;    // sensor offset
            sensorStartPos += transform.up * sensorOffset.y;

            //is the starting position of the back sensors. it must be related to the car, not the enviroment
            Vector3 sensorBackPos = transform.position;
            sensorBackPos += transform.forward * sensorBackOffset.z; //backSensorPos.z; // sensor offset
            sensorBackPos += transform.up * sensorOffset.y;


            //SAFETY DISTANCE SENSOR FOR MOVING CARS (AiCar in movement)
            currentSpeed = GlobalVariables.speed;                               //Get the current speed from global variable script, which reads the speed from CarController
            safetyMovingDistanceValue = sensorLengthUnit * (currentSpeed / 3);  //An incorrect formula to approximate safe distance from moving cars
            Logger.setCurrentSpeed(currentSpeed);      // Log the speed
            //Debug.Log(safetyMovingDistanceValue);    //the correct formula following the italian regulations
            if (Physics.Raycast(sensorStartPos, transform.forward, out hit, safetyMovingDistanceValue)) //&& !safetyStillDistance)   //it only works if the still car sensor is not triggered
            {
                if (hit.collider.CompareTag("AiCar"))   //doesn't hit the enviroment but only moving cars
                {
                    Debug.DrawLine(sensorStartPos, hit.point);                  //to check if the sensor is triggered by drawing a line
                    aMovingCarInFront  = true;
                }
            }
            safetyStillDistanceValue = sensorLengthUnit * (Mathf.Pow(currentSpeed / 10, 2));    //the correct formula following the italian regulations
            if (Physics.Raycast(sensorStartPos, transform.forward, out hit, safetyStillDistanceValue)) //&& !safetyStillDistance)   //it only works if the still car sensor is not triggered
            {

                if (hit.collider.CompareTag("StillCar"))   //doesn't hit the enviroment but only moving cars
                {
                    Debug.DrawLine(sensorStartPos, hit.point);             //to check if the sensor is triggered by drawing a line
                    aStillCarInFront = true;
                }
            }
            //  OVERTAKE SENSORS work as a pair; 1 is in the fron of the car and detects the beginning of an overtake, 2 convilidate
            //OVERTAKE SENSOR 1
            if  (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(overtakeAngle, transform.up) * transform.forward, out hit, overtakeLength))
            {
                //Debug.DrawLine(sensorStartPos, hit.point); //draw the line in any case
                if  (!hit.collider.CompareTag("Terrain"))   //hit.collider.CompareTag("AiCar") || hit.collider.CompareTag("StillCar"))
                {
                    //Debug.DrawLine(sensorStartPos, hit.point); //draw the line in any case
                    if (!overtakeStarted)
                    {
                        //if (overtakeStatus == 0)
                        //if (overtakeStarted == false)
                        //{
                            //overtakeStatus = 1;
                        overtakeStarted = true;
                        //StartCoroutine(DisableOvertakeStarted());
                        //}

                        //Debug.DrawLine(sensorStartPos, hit.point);   //to check if the sensor is triggered by drawing a line
                    }
                }
            }
            //OVERTAKE SENSOR 2
            if (Physics.Raycast(sensorBackPos, Quaternion.AngleAxis(overtakeAngle, transform.up) * transform.forward, out hit, overtakeLength))
            {
                //Debug.DrawLine(sensorBackPos, hit.point);  // draw the line in any case
                if (!hit.collider.CompareTag("Terrain")) //hit.collider.CompareTag("AiCar") || hit.collider.CompareTag("StillCar"))
                {
                    //Debug.DrawLine(sensorBackPos, hit.point);  // draw the line in any case
                    if (overtakeStarted == true)
                    {
                        //if (overtakeStatus == 1)
                        //if (overtakeStarted == true)
                        //{
                        //overtakeStatus = 0;
                        overtakeCount++;
                        //Debug.Log("Number of overtake is ");
                        //}
                        //Debug.DrawLine(sensorBackPos, hit.point);   //to check if the sensor is triggered by drawing a line
                        if (overtakeCount == enough_overtakes)
                        {
                            EventManager.nextEvent();
                            //Debug.Log("No more Overtakes");
                            overtakeCount++;
                        }

                        Logger = GameObject.FindWithTag("Logger").GetComponent<Logger>();
                        //Log the overtake count
                        Logger.setOvertakeCount(overtakeCount);

                        overtakeStarted = false;
                    }
                }
            }
        }

        private void SpeedLimit()                    //check if the driver speed is inside the legal limits
        {
            if (currentSpeed > speedLimit)
            {
                overspeed = true;                    // to indicate the condition
                correctspeed = false;                // to indicate the condition
                GlobalVariables.overSpeed = true;
                GlobalVariables.correctSpeed = false;
            }
            else if(currentSpeed < speedLimit) 
            {
                overspeed = false;
                correctspeed = true;                 // to indicate the condition
                GlobalVariables.overSpeed = false;
                GlobalVariables.correctSpeed = true;
            }
        }

        private void SendSensorsInput()
        {
            if (aMovingCarInFront)
            {
                safetyDistance = true;
                GlobalVariables.safetyDistance = true;
            }
            else
            {
                safetyDistance = false;
                GlobalVariables.safetyDistance = false;
            }
            if (aStillCarInFront)
            {
                safetyStillDistance = true;
                GlobalVariables.safetyStillDistance = true;
            }
            else
            {
                safetyStillDistance = false;
                GlobalVariables.safetyStillDistance = false;
            }
        }
    }
}

//UNUSED SERNSORS. IF NEEDED, RESTORE IN THE SENSOR() FUNCTION
/*
//FRONT SENSOR for emergency BRAKING
if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLengthFront))   //set the sensor shape
{
if (!hit.collider.CompareTag("Terrain"))         // "!hit" ignores the object with Terrain tag
{
    Debug.DrawLine(sensorStartPos, hit.point);   //to check if the sensor is triggered by drawing a line
    isBraking = true;
}
else
{

}
}
*/

/*
//FRONT-RIGHT SENSOR
sensorStartPos += transform.right * frontSideSensorPos;
if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLengthSide))
{
if (!hit.collider.CompareTag("Terrain"))
{
    Debug.DrawLine(sensorStartPos, hit.point);   //to check if the sensor is triggered by drawing a line
                                                 //avoiding = true;
                                                 //avoidMultiplier -= 0.8f;    //react to obstacles with a strong steer on the left
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
                                                     //avoiding = true;
                                                     //avoidMultiplier -= 0.5f;  //react to obstacles with a light steer on the left
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
                                                     //avoiding = true;
                                                     //avoidMultiplier += 0;     //react to obstacles with ...
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
/*
//FRONT-CRUISE SPEED SENSOR  to reduce the accelleration when an object is in front
if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLengthCruiseSpeed))
{
    if (!hit.collider.CompareTag("Terrain"))
    {
        Debug.DrawLine(sensorStartPos, hit.point);   //to check if the sensor is triggered by drawing a line
        cruiseSpeed = true;
    }
    else
    {
        cruiseSpeed = false;
    }
}

//FRONT-CRUISE SENSOR  to reduce the accelleration when an object is in front
if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLengthCruise))
{
    if (!hit.collider.CompareTag("Terrain"))
    {
        Debug.DrawLine(sensorStartPos, hit.point);   //to check if the sensor is triggered by drawing a line
        cruiseControl = true;
    }
    else
    {
        cruiseControl = false;
    }
}
*/
