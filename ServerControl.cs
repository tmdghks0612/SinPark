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
    // control instances
    public GameControlMultiplayer gameControlMultiplayer;

    // stream of server connection
    private NetworkStream serverStream;
    
    // thread to run socket connection and spawn requests
    private Thread tcpListenerThread;
    
    private int bufferSize = 1024;

    // Use this for initialization
    void Start()
    {
        serverStream = PublicLevel.GetServerStream();
        // Start TcpServer background thread
        tcpListenerThread = new Thread(new ThreadStart(ListenForIncommingRequests));
        tcpListenerThread.IsBackground = true;
        tcpListenerThread.Start();
    }

    public static bool OpenStream()
    {
        TcpClient _socketConnection;
        try
        {
            _socketConnection = new TcpClient(ServerControlForm.GetUrl(), ServerControlForm.GetPort());
            PublicLevel.SetServerStream(_socketConnection.GetStream());
        }
        catch(SocketException socketException)
        {
            Debug.Log("SocketException " + socketException.ToString());
            return false;
        }
        return true;
    }

    // Runs in background TcpServerThread; Handles incomming TcpClient requests
    private void ListenForIncommingRequests()
    {
        try
        {
            Byte[] buffer = new Byte[bufferSize];
            // create a socket for streaming data

            string serverMessage;

            ClearBuffer(buffer);
            while (true)
            {
                if (serverStream.Read(buffer, 0, buffer.Length) != 0)
                {
                    serverMessage = Encoding.UTF8.GetString(buffer);
                    Debug.Log("Server message is " + serverMessage);
                    // ReceiveSpawnRequest will call spawnControl.SummonCreature
                    gameControlMultiplayer.ReceiveSpawnRequest(serverMessage);

                    ClearBuffer(buffer);
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("SocketException " + socketException.ToString());
        }
    }

    // initialize given byte array to 0
    public void ClearBuffer(byte[] buffer)
    {
        for(int i=0; i < buffer.Length; ++i)
        {
            buffer[i] = 0;
        }
    }

    public NetworkStream GetServerStream()
    {
        return serverStream;
    }
}