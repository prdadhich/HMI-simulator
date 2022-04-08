using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Vehicles.Car;
using System.Diagnostics;
using System.IO;
using Debug = System.Diagnostics.Debug;

[RequireComponent(typeof(CarController))]
public class SteeringWheel : MonoBehaviour
{
    private CarController m_Car; // the car controller we want to use
    private LogitechGSDK.LogiControllerPropertiesData properties;
    LogitechGSDK.LogiControllerPropertiesData actualProperties;
    private SoundEffect soundEffect;
    //private bool skipOneFixedupdate = false;

    //private BoschPedal boschpedal;

    // Reference to the Logger class
    private Logger Logger = null;

    public bool col;

    public bool springforce = false;
    public string ground;
    public float collission;
    public float sound;
    public float valueH;
    public float valueV;
    public float valueB;
    public int id = 0;
    public bool braking = false;
    public float accOffRT = 0f;
    public float brakeOnRT = 0f;
    public float keyaxis = 0f;
    public int count = 0; //brake once, count = 1, brake twice, count = 0;
    Stopwatch counter;
    public float currentAccPos;

    private bool stopAccTimer, stopBrakeTimer = false;
    public bool startCounter = false;

    private bool RTFlag;
    public float user_AccOffRT { get { return accOffRT; } }
    public float user_BrakeOnRT { get { return brakeOnRT; } }

    String filepathDP;
    String filepathRT;

    Rigidbody rb;        // declare the rigid body as "rb" in order to get the magnitude of velocity for the Speedometer

    private bool alreadyPlayed = false;
    private int ffbkTime = 0;
    public AltPanelTexture Speedometer;

    public bool BrakeFlag
    {
        get
        {
            return (stopAccTimer && stopBrakeTimer);
        }
    }
    private void Awake()
    {
        // get the car controller
        m_Car = GetComponent<CarController>();
        soundEffect = GetComponent<SoundEffect>();

        LogitechGSDK.LogiSteeringInitialize(true);

        //boschpedal = GetComponent<BoschPedal>();

    }

    private void Start()
    {
        if (LogitechGSDK.LogiIsDeviceConnected(id, 0))
        {
            // Get the structure of force feedback data and set the current data to it
            actualProperties = new LogitechGSDK.LogiControllerPropertiesData();
            LogitechGSDK.LogiGetCurrentControllerProperties(id, ref actualProperties);
            //UnityEngine.Debug.Log("joystick is connected");
            //UnityEngine.Debug.Log(LogitechGSDK.LogiSteeringInitialize(false));
        }
        else
        {
            UnityEngine.Debug.Log("No joystick is connected");
        }

        // Get reference to another script just by using the owner gameobject's tag
        Logger = GameObject.FindWithTag("Logger").GetComponent<Logger>();

    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.JoystickButton11) == true)
            Application.Quit();
        // Get the steering wheel input
        float h = CrossPlatformInputManager.GetAxis("Horizontal");
        // Get the acceleration input
        //float accel = boschpedal.acceleration_normalized; //with bosch pedal
        float accel = CrossPlatformInputManager.GetAxis("Accelerator"); //with normal pedal
        // Get the brake pedal input
        float brakePedal = CrossPlatformInputManager.GetAxis("Brakepedal");
        //UnityEngine.Debug.Log("BrakePedal value is " + brakePedal);

        if (brakePedal > 0.9)
        {
            brakePedal = 0;
        }
        else if (brakePedal > 0)
        {
            brakePedal = 1 - brakePedal;
            //brakePedal = 0;
        }
        else
            brakePedal = 1;

        //UnityEngine.Debug.Log("brakePedaaaaaaaaaaaaaaaaaaaaaaal is: " + brakePedal);
        //UnityEngine.Debug.Log("accel is: " + accel);

        valueV = (accel + 1);
        valueH = h;
        valueB = brakePedal;
        //UnityEngine.Debug.Log("brakePedal value for STEER is:" + brakePedal);

        // Log the accel 
        Logger.setAccelerator((accel+1)/2);
        // Log the steering 
        Logger.setSteer(valueH);
        // Log the brake level
        Logger.setBrake(valueB);

        // Manual input override
        if (Input.GetKeyDown("w"))
            accel = 1.0f;
        if (Input.GetKeyDown("s"))
            brakePedal = 1.0f;
        if (Input.GetKeyDown("a"))
            h = -1.0f;
        if (Input.GetKeyDown("d"))
            h = 1.0f;

        // [TODO] Call this in an asynchronous way
        m_Car.Move(valueH, valueV, brakePedal);

        playWithForceFeedback();
    }
    ///*
    private void playWithForceFeedback()
    {
        if (LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsDeviceConnected(id, 0))
        {
            // Get the state of the Joystick
            LogitechGSDK.DIJOYSTATE2ENGINES rec;
            rec = LogitechGSDK.LogiGetStateCSharp(id);

            // Play the force feedback effect
            if (!LogitechGSDK.LogiIsPlaying(id, LogitechGSDK.LOGI_FORCE_SPRING))
            {
                springforce = true;
                LogitechGSDK.LogiPlaySpringForce(id, 0, 40, 40);
                LogitechGSDK.LogiPlayDamperForce(id, 10);
                //LogitechGSDK.LogiPlaySurfaceEffect(id, LogitechGSDK.LOGI_PERIODICTYPE_SINE, 15, 30);
            }



            ground = m_Car.Ground;
            //collission = m_Car.ForceEff;

            // Play the haptic effect when the car drive out of the road
            if (m_Car.Ground == "Terrain")
            {
                if (!LogitechGSDK.LogiIsPlaying(id, LogitechGSDK.LOGI_FORCE_DIRT_ROAD))
                {
                    LogitechGSDK.LogiPlayDirtRoadEffect(id, 30);
                }
            }
            else
            {
                LogitechGSDK.LogiStopDirtRoadEffect(id);
            }
            /*
            // Play the haptic effect when the car collide with the bumpy road
            if (m_Car.isPavementColliding)
            {
                if (!LogitechGSDK.LogiIsPlaying(id, LogitechGSDK.LOGI_FORCE_BUMPY_ROAD))
                {
                    LogitechGSDK.LogiPlayBumpyRoadEffect(id, 30);
                }
            }
            else
            {
                LogitechGSDK.LogiStopBumpyRoadEffect(id);
            }
            */
            /*
            if (m_Car.isLeftSideColliding)
                LogitechGSDK.LogiPlaySideCollisionForce(id, -30);

            if (m_Car.isRightSideColliding)
                LogitechGSDK.LogiPlaySideCollisionForce(id, 30);

            if (m_Car.ForceEff)
                LogitechGSDK.LogiPlayFrontalCollisionForce(id, 50);

            */
            //if (collission > 0.0f) //default collision value is a float for some reason(?)
            //    LogitechGSDK.LogiPlayFrontalCollisionForce(id, 50);
            if (GlobalVariables.Trial_type == GlobalVariables.TrialType.HAPTIC) { 
                if (Speedometer.currentState != AltPanelTexture.PanelStates.Normal && !alreadyPlayed && ffbkTime < 15)
                {
                    // if (!LogitechGSDK.LogiIsPlaying(id, LogitechGSDK.LOGI_FORCE_BUMPY_ROAD))
                    // LogitechGSDK.LogiPlaySpringForce(id, 0, 35, 35);
                    //LogitechGSDK.LogiPlayDirtRoadEffect(id, 20);
                    LogitechGSDK.LogiPlaySurfaceEffect(id, LogitechGSDK.LOGI_PERIODICTYPE_SINE, 50, 20);
                    //LogitechGSDK.LogiStopSpringForce(id);
                    ffbkTime++;

                }
                else
                {
                    //LogitechGSDK.LogiStopDirtRoadEffect(id);
                    LogitechGSDK.LogiStopSurfaceEffect(LogitechGSDK.LOGI_PERIODICTYPE_SINE);
                    //LogitechGSDK.LogiPlaySpringForce(id, 0, 35, 35);
                    //LogitechGSDK.LogiPlayDirtRoadEffect(id, 20);
                    alreadyPlayed = true;
                }
            if (Speedometer.currentState == AltPanelTexture.PanelStates.Normal && alreadyPlayed)
            {
                alreadyPlayed = false;
                ffbkTime = 0;
            }
        }
        }
    }

    void checkReactionTime(bool trigger, float acc, float brake)
    {
        // Check if the stopwatch trigger is active or not
        if (trigger == true)
        {

            // If trigger active, instantiate the stopwatch
            counter = Stopwatch.StartNew();

            // Stopwatch mode active
            startCounter = true;

            // Log the last acceleration pedal position
            currentAccPos = acc;

            // Reset the trigger
            braking = !braking;

            RTFlag = !RTFlag;

            accOffRT = 0f;
            brakeOnRT = 0f;

        }

        // If trigger is active, counter start
        if (startCounter)
        {
            if ((currentAccPos - acc) > 0.02)
            { // if accelerator pedal first unpressed .. THERE MIGHT BE SITUATION WHERE CurretACCPos is lower than ACC
                if (!stopAccTimer)
                {
                    accOffRT = counter.ElapsedMilliseconds; // count time from pressed to unpressed
                    stopAccTimer = true; // the acc pedal time release has been recorded
                    counter.Stop();
                    counter.Reset();
                    counter.Start();
                }
            }
            if (brake > 0.02)
            {
                if (!stopBrakeTimer)
                { // if brake predal is pressed
                    brakeOnRT = counter.ElapsedMilliseconds; // count the the first time taken to press brake when lead vehicle breaking
                    stopBrakeTimer = true; // brake pedal time pressing has been recorded
                    counter.Stop();
                    counter.Reset();
                }
                //File.AppendAllText(@"RT.txt", string.Format("{0,-24}\t {1,-10}\t{2,-10}\t{3,-10} \n".Replace("\n", System.Environment.NewLine), getTimeStampWithoutMil(DateTime.Now), DateTime.Now.Millisecond, accOffRT, brakeOnRT));
            }

            // if pedals' timer has been acquired, reset the stopwatch
            if (stopBrakeTimer)
            {
                count = count + 1;
                stopAccTimer = stopBrakeTimer = false;
                if (count > 1)
                {
                    soundEffect.PlayStopInstruction(4.0f);
                }
                startCounter = false;
            }
        }
    }
    //*/

    /*
    private void playSoundEffect()
    {
        sound = m_Car.SoundEff;

        if (collission > 0.0f)
            soundEffect.PlayRearEndCollisionSound();

        if (m_Car.isPavementColliding)
        {
            soundEffect.PlayBumpyRoadSound();
            m_Car.isPavementColliding = !m_Car.isPavementColliding;
            //UnityEngine.Debug.Log("Front");
        }
        if (m_Car.isLeftSideColliding)
        {
            soundEffect.PlayBumpyRoadSound();
            m_Car.isLeftSideColliding = !m_Car.isLeftSideColliding;
            //UnityEngine.Debug.Log("left");
        }

        if (m_Car.isRightSideColliding)
        {
            soundEffect.PlayBumpyRoadSound();
            m_Car.isRightSideColliding = !m_Car.isRightSideColliding;
            //UnityEngine.Debug.Log("right");
        }
    }
    */

    /*
    string getTimeStampWithoutMil(DateTime v)
    {
        return v.ToString(@"MM/dd/yyyy hh:mm:ss");

    }
    */
    /*
    void OnApplicationQuit()
    {
        File.Move("DP.txt", filepathDP);
        File.Move("RT.txt", filepathRT);
        if (LogitechGSDK.LogiIsPlaying(id, LogitechGSDK.LOGI_FORCE_SPRING))
            UnityEngine.Debug.Log("Spring force feedback has been " + (LogitechGSDK.LogiStopSpringForce(id) ? "successfully " : "UNSUCCESFULLY ") + "deactivated");

        if (LogitechGSDK.LogiIsPlaying(id, LogitechGSDK.LOGI_FORCE_DIRT_ROAD))
            UnityEngine.Debug.Log("Dirty road effect has been " + (LogitechGSDK.LogiStopDirtRoadEffect(id) ? "successfully " : "UNSUCCESFULLY ") + "deactivated");
        if (LogitechGSDK.LogiIsPlaying(id, LogitechGSDK.LOGI_FORCE_BUMPY_ROAD))
            UnityEngine.Debug.Log("Bumpy road effect  has been " + (LogitechGSDK.LogiStopBumpyRoadEffect(id) ? "successfully " : "UNSUCCESFULLY ") + "deactivated");
        LogitechGSDK.LogiSteeringShutdown();
    }
    //*/

    void Update()   //Update for the speedometer
    {
        //GlobalVariables.accelleration = boschpedal.acceleration_normalized;
        GlobalVariables.brake = -0.5f + CrossPlatformInputManager.GetAxis("Brakepedal") / 2f;
        GlobalVariables.steer = CrossPlatformInputManager.GetAxis("Horizontal");
    }
    private void OnDisable()
    {
        LogitechGSDK.LogiSteeringShutdown();
    }


    void OnApplicationQuit()
    {
        LogitechGSDK.LogiSteeringShutdown();
    }
}