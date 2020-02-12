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
    
    private static int bufferSize = 1024;
    private static Byte[] buffer = new Byte[bufferSize];

    public static bool creatureReceiveSuccess;

    // Use this for initialization
    void Start()
    {
        serverStream = PublicLevel.GetServerStream();
        // Start TcpServer background thread
        tcpListenerThread = new Thread(new ThreadStart(ListenForIncommingRequests));
        tcpListenerThread.IsBackground = true;
        tcpListenerThread.Start();
    }

    public static IEnumerator OpenStream(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        TcpClient _socketConnection;
        try
        {
            _socketConnection = new TcpClient(ServerControlForm.GetUrl(), ServerControlForm.GetPort());
            PublicLevel.SetServerStream(_socketConnection.GetStream());
        }
        catch(SocketException socketException)
        {
            Debug.Log("SocketException " + socketException.ToString());
            StageButtonMultiplayer.NetworkErrorPanelactive();
        }
    }

    public static bool SendCreatureList()
    {
        buffer = new byte[bufferSize];
        try
        {
            // create a socket for streaming data

            string serverMessage="";
            Vector2Int[] friendlyType = PublicLevel.GetFriendlyType();
            for(int i = 0; i < PublicLevel.usingCreatureNum; ++i)
            {
                serverMessage = serverMessage + friendlyType[i].x.ToString() + ',' + friendlyType[i].y.ToString() + " ";
            }
            buffer = Encoding.ASCII.GetBytes(serverMessage);
            PublicLevel.GetServerStream().Write(buffer, 0, buffer.Length);
            ClearBuffer(buffer);
        }
        catch (SocketException socketException)
        {
            Debug.Log("SocketException " + socketException.ToString());
            return false;
        }
        return true;
    }

    public static IEnumerator ListenForHostileCreatureList(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        buffer = new byte[bufferSize];
        try
        {
            Vector2Int[] _hostileType = new Vector2Int[PublicLevel.usingCreatureNum];
            PublicLevel.GetServerStream().BeginRead(buffer, 0, bufferSize, OnReceive, null);
            
            creatureReceiveSuccess = true;
            // load after creaturelist receive is complete
            LoadingSceneManager.LoadScene("DefaultIngameCopy");
        }
        catch (Exception listSendException)
        {
            Debug.Log("ListSendException " + listSendException.ToString());
            StageButtonMultiplayer.NetworkErrorPanelactive();
        }
        StageButtonMultiplayer.SetCreatureFlag(true);
        StageButtonMultiplayer.NetworkWaitPanelInactive();
    }

    static void OnReceive(IAsyncResult result)
    {
        Debug.Log("hello1");
        string[] serverMessage = Encoding.UTF8.GetString(buffer).Split(' ');
        string[] intPair;
        Debug.Log("hello2");
        for (int i = 0; i < PublicLevel.usingCreatureNum; ++i)
        {
            intPair = serverMessage[i].Split(',');
            PublicLevel.hostileCreatureList[i] = PublicLevel.hostilePrefab[int.Parse(intPair[0]), int.Parse(intPair[1])];
            Debug.Log(intPair[0] + intPair[1]);
        }
        ClearBuffer(buffer);
        return;
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
    public static void ClearBuffer(byte[] buffer)
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