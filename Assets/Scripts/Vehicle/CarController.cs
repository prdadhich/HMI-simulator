using System;
using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    public class CarController : MonoBehaviour
    {
        [SerializeField] private WheelCollider[] m_WheelColliders = new WheelCollider[4];
        [SerializeField] private GameObject[] m_WheelMeshes = new GameObject[4];
        [SerializeField] public WheelEffects[] m_WheelEffects = new WheelEffects[4];
        [SerializeField] private Vector3 m_CentreOfMassOffset;
        [SerializeField] private float m_MaximumSteerAngle;
        [Range(0, 1)] [SerializeField] private float m_SteerHelper;     // 0 is raw physics , 1 the car will grip in the direction it is facing
        [Range(0, 1)] [SerializeField] private float m_TractionControl; // 0 is no traction control, 1 is full interference
        [SerializeField] private float m_FullTorqueOverAllWheels;
        [SerializeField] private float m_ReverseTorque;
        [SerializeField] private float m_MaxHandbrakeTorque;
        [SerializeField] private float m_Downforce = 100f;
        [SerializeField] private float m_Topspeed = 145;
        [SerializeField] private static int NoOfGears = 5;
        [SerializeField] private float m_RevRangeBoundary = 1f;
        [SerializeField] private float m_SlipLimit;
        [SerializeField] private float m_BrakeTorque;


        //Reaction forces on the steering for side impacts. see steeringwheel script with logitech api
        public Collider leftSideCollider;
        public Collider rightSideCollider;
        public bool isLeftSideColliding = false;
        public bool isRightSideColliding = false;
        public bool ForceEff = false;

        private Quaternion[] m_WheelMeshLocalRotations;
        private Vector3 m_Prevpos, m_Pos;
        private float m_SteerAngle;
        private int m_GearNum;
        private float m_GearFactor;
        private float m_OldRotation;
        private float m_CurrentTorque;
        private Rigidbody m_Rigidbody;
        private const float k_ReversingThreshold = 0.01f;

        public bool Skidding { get; private set; }
        public float BrakeInput { get; private set; }
        public float CurrentSteerAngle { get { return m_SteerAngle; } }
        public float CurrentSpeed { get { return m_Rigidbody.velocity.magnitude * 3.6f; } }
        public float MaxSpeed { get { return m_Topspeed; } }
        public float Revs { get; private set; }
        public float AccelInput { get; private set; }
        //ADDED PUBLIC
        public string Ground { get; private set; }       /* Haptic and sound variables */
        public Vector3 hitPosition;

        //ADDED from original scripts for AAP
        Rigidbody rb;                              // rb is linked to rigidbody of the gameobject, to calculate current speed for the speedometer
        public bool manualGear = false;            //TO BE CHECKED TRUE IF THE MANUAL GEAR IS CONNECTED
        public bool reversegear = false;
        private SteeringWheel valueV;

        public float actualVelocity;
        private float motorTorque;

        private bool exitGame = false;

        private GameObject alertCanvas = null;

        // Use this for initialization
        private void Start()
        {
            m_WheelMeshLocalRotations = new Quaternion[4];
            for (int i = 0; i < 4; i++)
            {
                m_WheelMeshLocalRotations[i] = m_WheelMeshes[i].transform.localRotation;
            }
            m_WheelColliders[0].attachedRigidbody.centerOfMass = m_CentreOfMassOffset;

            m_MaxHandbrakeTorque = float.MaxValue;

            m_Rigidbody = GetComponent<Rigidbody>();

            m_CurrentTorque = m_FullTorqueOverAllWheels - (m_TractionControl * m_FullTorqueOverAllWheels);

            actualVelocity = m_Rigidbody.velocity.magnitude * 3.6f;

            alertCanvas = GameObject.Find("AlertCanvas");
            //alertCanvas.SetActive(false);
        }


        private void GearChanging()
        {
            float f = Mathf.Abs(CurrentSpeed / MaxSpeed);
            float upgearlimit = (1 / (float)NoOfGears) * (m_GearNum + 1);
            float downgearlimit = (1 / (float)NoOfGears) * m_GearNum;

            if (m_GearNum > 0 && f < downgearlimit)
            {
                m_GearNum--;
            }

            if (f > upgearlimit && (m_GearNum < (NoOfGears - 1)))
            {
                m_GearNum++;
            }
        }


        // simple function to add a curved bias towards 1 for a value in the 0-1 range
        private static float CurveFactor(float factor)
        {
            return 1 - (1 - factor) * (1 - factor);
        }


        // unclamped version of Lerp, to allow value to exceed the from-to range
        private static float ULerp(float from, float to, float value)
        {
            return (1.0f - value) * from + value * to;
        }


        private void CalculateGearFactor()
        {
            float f = (1 / (float)NoOfGears);
            // gear factor is a normalised representation of the current speed within the current gear's range of speeds.
            // We smooth towards the 'target' gear factor, so that revs don't instantly snap up or down when changing gear.
            var targetGearFactor = Mathf.InverseLerp(f * m_GearNum, f * (m_GearNum + 1), Mathf.Abs(CurrentSpeed / MaxSpeed));
            m_GearFactor = Mathf.Lerp(m_GearFactor, targetGearFactor, Time.deltaTime * 5f);
        }

        
        private void CalculateRevs()
        {
            // calculate engine revs (for display / sound)
            // (this is done in retrospect - revs are not used in force/power calculations)
            CalculateGearFactor();
            var gearNumFactor = m_GearNum / (float)NoOfGears;
            var revsRangeMin = ULerp(0f, m_RevRangeBoundary, CurveFactor(gearNumFactor));
            var revsRangeMax = ULerp(m_RevRangeBoundary, 1f, gearNumFactor);
            Revs = ULerp(revsRangeMin, revsRangeMax, m_GearFactor);
        }


        public void Move(float steering, float accel, float brakePedal)
        {
            for (int i = 0; i < 4; i++)
            {
                Quaternion quat;
                Vector3 position;
                m_WheelColliders[i].GetWorldPose(out position, out quat);
                m_WheelMeshes[i].transform.position = position;
                m_WheelMeshes[i].transform.rotation = quat;

                m_WheelColliders[i].motorTorque = motorTorque;  //motortorque is added to Debug it on the console. Hope it doesn't break anything
                //m_WheelColliders[i].brakeTorque = brakeTorque;  //braketorque is added to Debug it on the console. Hope it doesn't break anything
            }

            //clamp input values
            steering = Mathf.Clamp(steering, -1, 1);
            AccelInput = accel = Mathf.Clamp(accel, 0, 1);
            BrakeInput =  brakePedal;
            //BrakeInput = brakePedal = -1 * brakePedal;

            //Set the steer on the front wheels.
            //Assuming that wheels 0 and 1 are the front wheels.
            m_SteerAngle = steering * m_MaximumSteerAngle;
            m_WheelColliders[0].steerAngle = m_SteerAngle;
            m_WheelColliders[1].steerAngle = m_SteerAngle;

            SteerHelper();
            //ApplyDrive(accel, footbrake);  //ORIGINAL CAR CONTROLLER ACCELLERATOR
            ApplyDriveSim(accel, brakePedal); //ALT.ACCELLERATOR FOR THE SIMULATOR
            Hapticfeedback();
            CapSpeed();

            CalculateRevs();
            GearChanging();
            AddDownForce();
            CheckForWheelSpin();
            //TractionControl();
            //EndSimulation();
        }
        

        private void CapSpeed()
        {
            float speed = m_Rigidbody.velocity.magnitude;
                speed *= 3.6f;
            if (speed > m_Topspeed)
                m_Rigidbody.velocity = (m_Topspeed / 3.6f) * m_Rigidbody.velocity.normalized;
        }

        private void Awake()
        {
            valueV = GetComponent<SteeringWheel>(); //get the normalized accel value of the AAP pedal from steeringwheel script
        }


        private void ApplyDriveSim(float accel, float brakePedal)   //Converts the Accel from the gas pedal in torque
        {
            float thrustTorque;

            thrustTorque = accel *(m_FullTorqueOverAllWheels/4); // * (m_CurrentTorque / 4f);
            //float reverse = accel * m_ReverseTorque;
            float brake = brakePedal;

            //DEBUGGERS FOR QUICK TROUBLESHOOTING:
            //UnityEngine.Debug.Log("MOTOR Torque is:" + motorTorque);
            //UnityEngine.Debug.Log("CURRENT Torque is:" + m_CurrentTorque);
            //UnityEngine.Debug.Log("THRUST Torque is:" + thrustTorque);
            //UnityEngine.Debug.Log("BRAKE Torque is:" + brakeTorque);
            //UnityEngine.Debug.Log("brakePedal value for CAR is:" + brakePedal);
            //UnityEngine.Debug.Log("brake value is:" + brake);


            if (manualGear)  //the manual gear bool must be actie to have the reverse option. Use it only when the gear are connected to the Logitech steeringwheel
            {
                if (Input.GetKeyDown(KeyCode.Joystick1Button13) | Input.GetKeyDown(KeyCode.Joystick1Button15) | Input.GetKeyDown(KeyCode.Joystick1Button17)) //button 18 is the reverse gear on the manual gear controller
                {
                    reversegear = true;
                    print("Player Entered Reverse Gear");
                }
                if (Input.GetKeyDown(KeyCode.JoystickButton12) | Input.GetKeyDown(KeyCode.Joystick1Button14) | Input.GetKeyDown(KeyCode.Joystick1Button16))
                {
                    reversegear = false;
                    print("Player Entered Standard Gear");
                }
                if (reversegear) //is true:
                { 
                     for (int i = 0; i < 4; i++)
                     {
                        m_WheelColliders[i].motorTorque = -thrustTorque; //takes the acceleration and makes it negative as a reverse gear
                        m_WheelColliders[i].brakeTorque =m_BrakeTorque * brakePedal;
                     }
                     return;
                }
                if (!reversegear) //is false:
                {
                    for (int i = 0; i < 4; i++)
                    {
                        m_WheelColliders[i].motorTorque = thrustTorque;  //the accelleration accounts for the forward torque
                        m_WheelColliders[i].brakeTorque = m_BrakeTorque * brakePedal;
                    }
                    return;
                }
            }
        }


        private void SteerHelper()
        {
            for (int i = 0; i < 4; i++)
            {
                WheelHit wheelhit;
                m_WheelColliders[i].GetGroundHit(out wheelhit);
                if (wheelhit.normal == Vector3.zero)
                    return; // wheels arent on the ground so dont realign the rigidbody velocity
            }

            // this if is needed to avoid gimbal lock problems that will make the car suddenly shift direction
            if (Mathf.Abs(m_OldRotation - transform.eulerAngles.y) < 10f)
            {
                var turnadjust = (transform.eulerAngles.y - m_OldRotation) * m_SteerHelper;
                Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);
                m_Rigidbody.velocity = velRotation * m_Rigidbody.velocity;
            }
            m_OldRotation = transform.eulerAngles.y;
        }


        // this is used to add more grip in relation to speed
        private void AddDownForce()
        {
            m_WheelColliders[0].attachedRigidbody.AddForce(-transform.up * m_Downforce * m_WheelColliders[0].attachedRigidbody.velocity.magnitude);
        }


        private void Hapticfeedback()
        {
            int mask = 1 << 9;
            for (int i = 0; i < 4; i++)
            {
                WheelHit wheelhit;
                m_WheelColliders[i].GetGroundHit(out wheelhit);
                if (wheelhit.collider == null)
                    continue;
                Ground = wheelhit.collider.name;
                hitPosition = wheelhit.point;

            }
        }


        // checks if the wheels are spinning and is so does three things
        // 1) emits particles
        // 2) plays tiure skidding sounds
        // 3) leaves skidmarks on the ground
        // these effects are controlled through the WheelEffects class
        private void CheckForWheelSpin()
        {
            // loop through all wheels
            for (int i = 0; i < 4; i++)
            {
                WheelHit wheelHit;
                m_WheelColliders[i].GetGroundHit(out wheelHit);

                // is the tire slipping above the given threshhold
                if (Mathf.Abs(wheelHit.forwardSlip) >= m_SlipLimit || Mathf.Abs(wheelHit.sidewaysSlip) >= m_SlipLimit)
                {
                    m_WheelEffects[i].EmitTyreSmoke();

                    // avoiding all four tires screeching at the same time
                    // if they do it can lead to some strange audio artefacts
                    if (!AnySkidSoundPlaying())
                    {
                        m_WheelEffects[i].PlayAudio();
                    }
                    continue;
                }
                // if it wasnt slipping stop all the audio
                if (m_WheelEffects[i].PlayingAudio)
                {
                    m_WheelEffects[i].StopAudio();
                }
                // end the trail generation
                m_WheelEffects[i].EndSkidTrail();
            }
        }
        /*
        // crude traction control that reduces the power to wheel if the car is wheel spinning too much
        private void TractionControl()
        {
            WheelHit wheelHit;
            {
                    // loop through all wheels
                    for (int i = 0; i < 4; i++)
                    {
                        m_WheelColliders[i].GetGroundHit(out wheelHit);
                        AdjustTorque(wheelHit.forwardSlip);
                    }
            }

            for (int i = 0; i < 4; i++)
            {
                m_WheelColliders[i].GetGroundHit(out wheelHit);

                AdjustTorque(wheelHit.forwardSlip);
            }
        } //*/


        private void AdjustTorque(float forwardSlip)
        {
            if (forwardSlip >= m_SlipLimit && m_CurrentTorque >= 0)
            {
                m_CurrentTorque -= 10 * m_TractionControl;
            }
            else
            {
                m_CurrentTorque += 10 * m_TractionControl;
                if (m_CurrentTorque > m_FullTorqueOverAllWheels)
                {
                    m_CurrentTorque = m_FullTorqueOverAllWheels;
                }
            }
        }

        private bool AnySkidSoundPlaying()
        {
            for (int i = 0; i < 4; i++)
            {
                if (m_WheelEffects[i].PlayingAudio)
                {
                    return true;
                }
            }
            return false;
        }

        private void EndSimulation() //the function end the simulaton when the player falls out of the road
        {
            if (transform.position.y < -7) //when the player position is 7 meter below the highway level
            {
                //Application.Quit();
                //Debug.Log("The Player is suicidal");
                exitGame = true;
                alertCanvas.SetActive(true);
            }
            //KNOWN STEERING WHEEL JOYSTICK BUTTONS: X=2, Y=3, A=0, B=1, RSB=8, LSB=9, "MENU"=6, "TAB"=7 .
            if (Input.GetKeyDown("joystick button 2"))
            {
                //Application.Quit();
                //Debug.Log("X button is pressed");
                exitGame = true;
                alertCanvas.SetActive(true);
            }
            if (exitGame && Input.GetKeyDown("joystick button 1"))
            {
                Application.Quit();
                //Debug.Log("B button is pressed");
            }
        }

        void Update()   //Update for the speedometer needle
        {
            AnalogueSpeedConverter.ShowSpeed(m_Rigidbody.velocity.magnitude * 3.6f, 0, 145);  //set the last 2 value as min max speed of the vehicle
            GlobalVariables.speed = m_Rigidbody.velocity.magnitude * 3.6f;
            GlobalVariables.playerZ = this.GetComponent<Transform>().position.z;

            //If you need to discover a specific INPUT from a joystic button reactivate this function. It uses a lot of performance so deactivate after you are done :)
            /*
            for (int i = 0; i < 20; i++)
            {
                if (Input.GetKeyDown("joystick 1 button " + i))
                {
                    print("joystick 1 button " + i);
                }
            }//*/
        }
    }
}
