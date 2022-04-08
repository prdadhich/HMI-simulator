using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetMQ;
using NetMQ.Sockets;
using System;

public class ServerMQ : MonoBehaviour
{

    PublisherSocket pub;

    void Start()
    {
        AsyncIO.ForceDotNet.Force();
        pub = new PublisherSocket();
        pub.Bind("tcp://*:5515");
        InvokeRepeating("SendOnLineStatus", 1, 1);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            CancelInvoke("SendOnLineStatus");
            NetMQMessage msg = new NetMQMessage(2);
            msg.Append("Status");
            msg.Append("start");
            pub.SendMultipartMessage(msg);
            InvokeRepeating("SendStatus", 0, 0.1f);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            NetMQMessage msg = new NetMQMessage(2);
            msg.Append("Status");
            msg.Append("stop");
            pub.SendMultipartMessage(msg);
            CancelInvoke("SendStatus");
            InvokeRepeating("SendOnLineStatus", 1, 1);
        }
    }

    void SendStatus()
    {
        
        NetMQMessage msg = new NetMQMessage(2);
        msg.Append("Status");
        msg.Append(DateTime.Now.ToString("HH:mm:ss.fff"));
        pub.SendMultipartMessage(msg);
    }

    void SendOnLineStatus()
    {
        NetMQMessage msg = new NetMQMessage(2);
        msg.Append("Status");
        msg.Append("UnityIsOnline");
        pub.SendMultipartMessage(msg);

    }
    void SendOffLineStatus()
    {
        NetMQMessage msg = new NetMQMessage(2);
        msg.Append("Status");
        msg.Append("UnityIsOffline");
        pub.SendMultipartMessage(msg);

    }
    private void OnDisable()
    {
        CancelInvoke("SendOnLineStatus");
        CancelInvoke("SendStatus");
        SendOffLineStatus();
        NetMQConfig.Cleanup(false);
    }


    private void OnApplicationQuit()
    {
       // NetMQConfig.Cleanup(false);
        Debug.Log("Exit");
    }
}