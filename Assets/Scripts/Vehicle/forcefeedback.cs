using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

[System.Serializable]
public class forcefeedback : MonoBehaviour
{
    //define the parameters of steering system
    private float rs = 16.1f;
    private float Js = 0.0035f;
    private float cs = 0.1f;
    private float Jw = 10f;
    private float cw = 300f;
    private float kt = 0f;

    private float lastposition, lastvelocity; //dynamic properties of vehicle
    public float angular_position, angular_velocity, angular_acceleration; //dynamic properties of steering wheel

    private float M_PSS; //feedback torque which is applied to Logitech Steering Wheel

    //define the tyre characteristics parameters
    [Tooltip("please only select one")] //type of tyre
    public bool road = true;
    public bool race = false;
    private float s, b, e; //non dimension factors which depend on the type of tyre
    [Tooltip("wheel radius (m)")]
    public float r = 0.3556f; //tyre radius
    [Tooltip("belt width (m)")]
    public float w = 0.205f; //belt width of tyre
    [Tooltip("tyre aspect ratio (tyre section height/tyre section width")]
    public float a = 0.6f; //tyre aspect ratio

    //define magic formula parameters: BCDE. Sh and Sv are horizontal shift and vertical shift, which can be neglected in this project.
    public float B, C, D, E, Sh, Sv;

    public float velocity; //integrated velocity of vehicle body
    public Rigidbody car; 
    public float wheel; //factor used in PID anti-oscillation function
    public static float wheelsta; //static factor serve other classes under the same namespace
    private float side_slip_L, side_slip_R, longi_slip_L, longi_slip_R; //slippages of tyre
    private float co_slip_L, co_slip_R, Fy_L, Fy_R, t_L, t_R, Mz_L, Mz_R; //tyre forces and aligning torque
    public WheelCollider leftWheel; 
    public WheelCollider rightWheel;
    public float cor_stiffness; //tyre cornering stiffness
    public PhysicMaterial ground; //friction element of ground
    private float miu_s; //static friction coefficient
    private float xdirec; //normalized 
    private float Fz; //vertical force applied to tyre from suspension arm
    public float wheelpre; //factor used in PID anti-oscillation function
    public static float Fx, Fy; //longitudinal and lateral forces of tyre
    private float time, angle, torque;

    private float error, error_pre, error_i_pre; //error between references and current values
    private float fac_p = 0.8f, fac_i = 0.01f, fac_d = 0f; //PID factors

    void Start()
    {
        //define b s e based on type of tyre
        if (road)
        {
            s = 0.15f;
            b = 0.015f;
            race = false;
        }
        if (race)
        {
            s = 0.1f;
            b = 0.01f;
            road = false;
        }
        e = Mathf.Pow(10, 6) * 27;

        //initialize the Logitech Steering Wheel and activate the function of torque from dual electromotor
        LogitechGSDK.LogiSteeringInitialize(false);
        print("LogiIsConnected is " + LogitechGSDK.LogiIsConnected(0));
        print("LogiHasForceFeedback is " + LogitechGSDK.LogiHasForceFeedback(0));
        print("functional force is " + LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_CONSTANT));

        car = GetComponent<Rigidbody>();
        car.centerOfMass += new Vector3(0, 0, 1.0f);
        miu_s = ground.staticFriction;
        Fz = car.mass * 9.81f / 4;
    }

    void FixedUpdate()
    {
        //compute the cornering stiffness iteratively
        float coa = Mathf.Pow((r + w * a), 2);
        float cob = Mathf.Sin(Mathf.Acos(1 - ((s * w * a) / (r + w * a))));
        float coc = Mathf.PI - Mathf.Sin(1 - (s * w * a) / (r + w * a));
        cor_stiffness = 2 * e * b * Mathf.Pow(w, 3) / (coa * cob * coc) * 0.0174533f;
        //1deg≈0.0174533rad

        velocity = car.velocity.magnitude;  //vehicle speed

        LogitechGSDK.DIJOYSTATE2ENGINES rec;  //logitech control
        rec = LogitechGSDK.LogiGetStateCSharp(0);
        xdirec = rec.lX / 32767f;

        //input parameters of magic formula iteratively
        C = 1.3f;
        D = miu_s * Fz;
        B = cor_stiffness / C / D;
        float cof = B * leftWheel.sidewaysFriction.extremumSlip;
        E = (cof - Mathf.Tan(Mathf.PI / 2 / C)) / (cof - Mathf.Atan(cof));

        //compute slippages of tyre iteratively
        WheelHit hitl;
        WheelHit hitr;
        leftWheel.GetGroundHit(out hitl); //slippage of leftwheel
        side_slip_L = hitl.sidewaysSlip;
        longi_slip_L = hitl.forwardSlip;
        rightWheel.GetGroundHit(out hitr); //slippage of rightwheel
        side_slip_R = hitr.sidewaysSlip;
        longi_slip_R = hitr.forwardSlip;

        //activate the feedback force
        if (LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0)&& Mathf.Abs(xdirec) > 0.05f)
        {
            //compute forces and torque of tyre iteratively based on Magic Formula
            co_slip_L = Sh + (Sgn(side_slip_L) * Mathf.Atan(Mathf.Sqrt(Mathf.Pow(Mathf.Tan(side_slip_L), 2) + Mathf.Pow(Mathf.Tan(longi_slip_L), 2))));
            co_slip_R = Sh + (Sgn(side_slip_R) * Mathf.Atan(Mathf.Sqrt(Mathf.Pow(Mathf.Tan(side_slip_R), 2) + Mathf.Pow(Mathf.Tan(longi_slip_R), 2))));
            Fy_L = Sv + (D * Mathf.Sin(C * Mathf.Atan(B * co_slip_L - E * (B * co_slip_L - Mathf.Atan(B * co_slip_L)))));
            Fy_R = Sv + (D * Mathf.Sin(C * Mathf.Atan(B * co_slip_R - E * (B * co_slip_R - Mathf.Atan(B * co_slip_R)))));
            t_L = D * Mathf.Cos(C * Mathf.Atan(B * co_slip_L - E * (B * co_slip_L - Mathf.Atan(B * co_slip_L)))) / 1000;
            t_R = D * Mathf.Cos(C * Mathf.Atan(B * co_slip_R - E * (B * co_slip_R - Mathf.Atan(B * co_slip_R)))) / 1000;
            Mz_L = -(Fy_L * t_L);
            Mz_R = -(Fy_R * t_R);
            Fx = Sv + (D * Mathf.Sin(C * Mathf.Atan(B * longi_slip_L - E * (B * longi_slip_L - Mathf.Atan(B * longi_slip_L)))));
            Fy = Sv + (D * Mathf.Sin(C * Mathf.Atan(B * side_slip_L - E * (B * side_slip_L - Mathf.Atan(B * side_slip_L))))); ;

            //dynamic properties of steering wheel
            angular_position = xdirec * 2.5f * Mathf.PI;
            angular_velocity = (angular_position - lastposition) / Time.deltaTime;
            angular_acceleration = (angular_velocity - lastvelocity) / Time.deltaTime;
            lastposition = angular_position;
            lastvelocity = angular_velocity;
            float wheel_velocity = angular_velocity / rs;
            float wheel_acceleration = angular_acceleration / rs;

            //torque balance equation of integrated steering system
            M_PSS = Js * angular_acceleration + cs * angular_velocity + kt * angular_position + (Jw * wheel_acceleration + cw * wheel_velocity + (Mz_L + Mz_R)) / rs;
            wheel = M_PSS; //feedback torque

            LogitechGSDK.LogiPlayDamperForce(0, 12); //permanent damping force on steering wheel

            //define the direction of force on steering wheel
            if (leftWheel.steerAngle > 0)
            {
                wheel = Mathf.Abs(wheel);
            }
            else if (leftWheel.steerAngle < 0)
            {
                wheel = -Mathf.Abs(wheel);
            }
            else if (leftWheel.steerAngle == 0)
            {
                wheel = 0;
            }

            wheel = wheel / 7; //sensitive factor: influence the scala of feedback torque, which can be set by user arbitrarily
            oscillation_prevent(); 
            LogitechGSDK.LogiStopSpringForce(0); //temporary spring force on steering wheel
            LogitechGSDK.LogiPlayConstantForce(0, (int)wheel); //apply feedback force on steering wheel
        }

        //remove the residual torque of steering wheel when initial stage or very low speed
        else if(LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0) && Mathf.Abs(xdirec) <= 0.05f)
        {
            oscillation_prevent();
            LogitechGSDK.LogiPlayDamperForce(0, 12);
            LogitechGSDK.LogiStopConstantForce(0);
            LogitechGSDK.LogiPlaySpringForce(0, 0, 30, 30);
        }

        //increase the steering resistance when high speed driving
        if (LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0)&&velocity*3.6f>80) 
        {
            oscillation_prevent();
            LogitechGSDK.LogiStopDamperForce(0);
            LogitechGSDK.LogiPlayDamperForce(0, (int)(1.1875f * velocity * 3.6f - 90));
        }
        if (velocity * 3.6 < 2)
        {
            LogitechGSDK.LogiStopSpringForce(0);
            LogitechGSDK.LogiPlaySpringForce(0, 1, 1, 1);
            LogitechGSDK.LogiStopDamperForce(0);
            LogitechGSDK.LogiStopConstantForce(0);
        }
        wheelpre = wheel; //keep the values of last framework in memory to calculate the error
        wheelsta = wheel;
        error_pre = error; //set error of current frame as the previous error of next frame
        error_i_pre = Time.deltaTime * error;
    }

        //Sgn function to determine the sign of parameters
        public int Sgn(float alpha)
    {
        if (alpha > 0)
        {
            return 1;
        }
        else if (alpha == 0)
        {
            return 0;
        }
        else
        {
            return -1;
        }
    }

    //algorithm used for sharply decreasing the oscillation problem of steering wheel during driving
    private void oscillation_prevent()
    {
        //simple PID controller
        error = Mathf.Abs(Mathf.Abs(wheel) - Mathf.Abs(wheelpre)); //compute error of current frame
        float error_p = error * fac_p;
        float error_i = fac_i * (Time.deltaTime * error + error_i_pre);
        float error_d = fac_d * ((error - error_pre) / Time.deltaTime);
        float error_t = error_p + error_i + error_d;
        if (error > 0)
        {
            if (wheel > wheelpre)
            {
                wheel = wheel - error_t;
            }
            else if (wheel < wheelpre)
            {
                wheel = wheel + error_t;
            }
        }

        //if the direction steering force of current frame is opposite to the previous one
        //this function will rapidly decrease the oscillation problem
        if (Sgn(wheel) != Sgn(wheelpre) && velocity * 3.6 < 5&& velocity * 3.6>2)
        {
            wheel = 0.001f * wheel;
            kt = 1f;
        }
        if (Sgn(wheel) != Sgn(wheelpre) && velocity * 3.6 >= 5)
        {
            wheel = 0.1f * wheel;
            kt = 1f;
        }
    }
}


