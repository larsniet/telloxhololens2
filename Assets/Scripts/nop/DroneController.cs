using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Net.Sockets;
using System.Net;
using UnityEngine;


public class DroneController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var client = new UdpClient();
        IPEndPoint ep = new IPEndPoint(IPAddress.Parse("192.168.10.1"), 8890); // endpoint where server is listening
        client.Connect(ep);

        // send data
        //Byte[] sendBytes = Encoding.ASCII.GetBytes("command");
        //client.Send(sendBytes, sendBytes.Length);

        // then receive data
        var receivedData = client.Receive(ref ep);

        Console.Write("receive data from " + ep.ToString());

        Console.Read();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
