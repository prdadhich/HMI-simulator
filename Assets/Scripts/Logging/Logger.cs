using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using NetMQ;
using NetMQ.Sockets;

public class Logger : MonoBehaviour
{

    // FROM OLD SIMULATOR = getTimeStampWithoutMil(DateTime.Now),Time.time,headwayDistance,speed,steeringAngle,CarBody.position.x,CarBody.position.y,CarBody.position.z,initialPathPosition.x,initialPathPosition.y, initialPathPosition.z, finalPathPosition.x, finalPathPosition.y, finalPathPosition.z, accelerator, brake)
    /* --------------- Logger ----------------
    * Logging class to save relevant information into a csv saving the following information:
    * - Timestamp
    * - Speed
    * - Accelerator
    * - Brake
    * - Steer
    * - AAP Status // the called condition of the AAP Active Accellerator Pedal
    * - Overtake sensor
    * - PlayerSensor Stats (cruise is on etc.)
    * - DistanceFromStart on Z axis

    * - (Collisions)
    */

    // Variables to be logged
    private float currentSpeed = 0;
    private float acceleration = 0;
    private float brake = 0;
    private float steer = 0;
    public static string pedalmode = "none";             //better for the analysis 
    private int overtakeCount = 0;
    private int emergencyEventCount = 0;
    private int safetyalert = 0;
    public static string eventstate = "none";
    public static int adasState = 0; //normal

    


    //
    private StringBuilder logBuffer = new StringBuilder();

    void Start()
    {
        if (GlobalVariables.Trial_counter == 0)
        {
            AsyncIO.ForceDotNet.Force();
            GlobalVariables.pub = new PublisherSocket();
            GlobalVariables.pub.Bind("tcp://*:5515");
            InvokeRepeating("SendOnLineStatus", 1, 1);
        }

    }
    // Update is called once per frame
    public void StartLogger()

    {
        CancelInvoke("SendOnLineStatus");
        NetMQMessage msg = new NetMQMessage(2);
        msg.Append("Status");
        msg.Append("start");
        GlobalVariables.pub.SendMultipartMessage(msg);
        InvokeRepeating("SendStatus", 0, 0.1f);
    }
    public void StopLogger()
    {
        CancelInvoke("SendStatus");
        // for (int i = 0; i < 100; i++)
        // {
        NetMQMessage msg = new NetMQMessage(2);
        msg.Append("Status");
        msg.Append("stop");
        GlobalVariables.pub.SendMultipartMessage(msg);
        //    System.Threading.Thread.Sleep(10);
        //}
        InvokeRepeating("SendOnLineStatus", 1, 1);
    }


    void SendStatus()
    {

        NetMQMessage msg = new NetMQMessage(2);
        msg.Append("Status");
        //msg.Append(DateTime.Now.ToString("HH:mm:ss.fff"));
        msg.Append(Setlog());
        GlobalVariables.pub.SendMultipartMessage(msg);
    }

    void SendOnLineStatus()
    {
        NetMQMessage msg = new NetMQMessage(2);
        msg.Append("Status");
        msg.Append("UnityIsOnline");
        GlobalVariables.pub.SendMultipartMessage(msg);

    }
    void SendOffLineStatus()
    {
        NetMQMessage msg = new NetMQMessage(2);
        msg.Append("Status");
        msg.Append("UnityIsOffline");
        GlobalVariables.pub.SendMultipartMessage(msg);

    }
    private void OnDisable()
    {
        if (GlobalVariables.Trial_counter > 2)
        {
            CancelInvoke("SendOnLineStatus");
            CancelInvoke("SendStatus");
            SendOffLineStatus();
            NetMQConfig.Cleanup(false);
        }
    }

    private void OnApplicationQuit()
    {
        CancelInvoke("SendOnLineStatus");
        CancelInvoke("SendStatus");
        SendOffLineStatus();
        NetMQConfig.Cleanup(false);
    }


    public void setCurrentSpeed(float value)
    {
        currentSpeed = value;
    }

    public void setAccelerator(float value)
    {
        acceleration = value;
        //Debug.Log(acceleration);
    }

    public void setBrake(float value)
    {
        brake = value;
    }

    public void setSteer(float value)
    {
        steer = value;
    }

    public void setPedalmode(string value)
    {
        pedalmode = value;

    }

    public void setOvertakeCount(int value)
    {
        overtakeCount = value;
    }

    public void setEmergencyEventCount(int value)
    {
        emergencyEventCount = value;
    }

    public void setSafetyAlert(int value)
    {
        safetyalert = value;
    }

    public void setEventstate(string value)
    {
        eventstate = value;
    }


    private string fileName = GlobalVariables.LogFolder + GlobalVariables.Trial_counter + "_" + GlobalVariables.Label[GlobalVariables.Trial_counter] + ".csv";


    public void Flush()
    {
        log();
        File.AppendAllText(fileName, logBuffer.ToString());
        logBuffer.Length = 0;
    }


    // Use this for initialization
    void StartFile()
    {
        if (!System.IO.Directory.Exists(GlobalVariables.LogFolder))
            System.IO.Directory.CreateDirectory(GlobalVariables.LogFolder);
        File.WriteAllText(fileName, string.Format("{0},{1},{2},{3},{4},{5},\n"/*{6},{7},{8},{9},{10}\n"*/, "Timestamp", "Speed", "Accelerator", "Brake", "Steer", "Adas State"/*,"AAP Mode","Overtake Count", "Emergency Event Count", "Safety Alert", "Event State", "PlayerZ"*/));
        // Flush the buffer every 5 seconds
        InvokeRepeating("Flush", 0, 0.1f);  // save the buffer every 5 seconds
    }

    // Update is called once per frame
    // [TODO] To save performance on this write log to a buffer and flush to disk every N seconds
    void log()
    {

        logBuffer.Append(string.Format("{0},{1},{2},{3},{4},{5}\n"/*,{6},{7},{8},{9},{10}\n"*/, DateTime.Now.ToString("HH:mm:ss.fff"),
                                                                                         currentSpeed.ToString(),
                                                                                         acceleration.ToString(),
                                                                                         brake.ToString(),
                                                                                         steer.ToString(),
                                                                                         adasState.ToString()/*,
                                                                                         pedalmode,
                                                                                         overtakeCount.ToString(),
                                                                                         emergencyEventCount.ToString(),
                                                                                         safetyalert.ToString(),
                                                                                         eventstate.ToString(),
                                                                                         GlobalVariables.playerZ.ToString()*/));
    }
    string Setlog()
    {

        return string.Format("{0}\t{1}\t{2}\t{3}\t{4}"/*,{5}\n"/*,{6},{7},{8},{9},{10}\n", DateTime.Now.ToString("HH:mm:ss.fff")*/,
                                                                                         currentSpeed.ToString(),
                                                                                         acceleration.ToString(),
                                                                                         brake.ToString(),
                                                                                         steer.ToString(),
                                                                                         adasState.ToString());
    }




}
