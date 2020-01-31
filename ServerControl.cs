using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ServerControl : MonoBehaviour
{
    private SpawnControl spawnControl;
    private TcpClient socketConnection;
    private Thread tcpListenerThread;
    private TcpClient connectedTcpClient;
    
    private int bufferSize = 1024;

    // Use this for initialization
    void Start()
    {
        // Start TcpServer background thread 		
        tcpListenerThread = new Thread(new ThreadStart(ListenForIncommingRequests));
        tcpListenerThread.IsBackground = true;
        tcpListenerThread.Start();
    }

    // Runs in background TcpServerThread; Handles incomming TcpClient requests
    private void ListenForIncommingRequests()
    {
        try
        {
            Byte[] buffer = new Byte[bufferSize];
            socketConnection = new TcpClient(ServerControlForm.GetUrl() , ServerControlForm.GetPort());
            NetworkStream stream = socketConnection.GetStream();

            //server connection message
            string serverMessage = "server connection stream constructed";
            buffer = Encoding.ASCII.GetBytes(serverMessage);
            stream.Write(buffer, 0, buffer.Length);

            ClearBuffer(buffer);
            while (true)
            {
                if(stream.Read(buffer, 0, buffer.Length) != 0)
                {
                    Debug.Log(buffer.ToString());
                    
                    //process data saved in bytes
                    //call spawnControl.SummonCreature accordingly

                    ClearBuffer(buffer);
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("SocketException " + socketException.ToString());
        }
    }


    private void ClearBuffer(byte[] buffer)
    {
        for(int i=0; i < buffer.Length; ++i)
        {
            buffer[i] = 0;
        }
    }
}
