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
using UnityEngine.SceneManagement;

public class ServerControl : MonoBehaviour
{
    // control instances
    public GameControlMultiplayer gameControlMultiplayer;
    public StageButtonMultiplayer stageButtonMultiplayer;

    // socket of server connection
    private TcpClient socketConnection;

    // stream of server connection
    private NetworkStream serverStream;
    
    // thread to run socket connection and spawn requests
    private Thread tcpListenerThread;
    
    private static int bufferSize = 1024;
    private static Byte[] buffer = new Byte[bufferSize];

    public static bool creatureReceiveSuccess;

    // 
    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    public void ListenToStream()
    {
        gameControlMultiplayer = GameObject.Find("GameControl").GetComponent<GameControlMultiplayer>();
        // Start TcpServer background thread
        if (serverStream != null)
        {
            tcpListenerThread = new Thread(new ThreadStart(ListenForIncommingRequests));
            tcpListenerThread.IsBackground = true;
            tcpListenerThread.Start();
        }
    }

    public IEnumerator OpenStream(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        try
        {
            socketConnection = new TcpClient(ServerControlForm.GetUrl(), ServerControlForm.GetPort());
            PublicLevel.SetServerStream(socketConnection.GetStream());

            HandShake();

            serverStream = PublicLevel.GetServerStream();
            // if success, send friendly creature list through stream
            StartCoroutine(SendCreatureList(3.0f));
            StartCoroutine(ListenForHostileCreatureList(3.0f));
        }
        catch(SocketException socketException)
        {
            Debug.Log("SocketException " + socketException.ToString());
            StageButtonMultiplayer.NetworkErrorPanelactive();
        }


    }

    public IEnumerator SendCreatureList(float _waitTime)
    {
        yield return _waitTime;
        buffer = new byte[bufferSize];
        try
        {
            // create a socket for streaming data
            string serverMessage="";
            Vector2Int[] friendlyType = PublicLevel.GetFriendlyType();
            for (int i = 0; i < PublicLevel.usingCreatureNum - 1; ++i)
            {
                serverMessage = serverMessage + friendlyType[i].x.ToString() + ',' + friendlyType[i].y.ToString() + " ";
            }
            serverMessage = serverMessage + friendlyType[PublicLevel.usingCreatureNum - 1].x.ToString() + ',' + friendlyType[PublicLevel.usingCreatureNum - 1].y.ToString();
            buffer = Encoding.ASCII.GetBytes(serverMessage);
            PublicLevel.GetServerStream().Write(buffer, 0, buffer.Length);
            stageButtonMultiplayer.SetCreatureSentFlag(true);

            ClearBuffer(buffer);
        }
        catch (SocketException socketException)
        {
            Debug.Log("SocketException " + socketException.ToString());
        }
    }

    public IEnumerator ListenForHostileCreatureList(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        buffer = new byte[bufferSize];
        try
        {
            Vector2Int[] _hostileType = new Vector2Int[PublicLevel.usingCreatureNum];
            
            serverStream.BeginRead(buffer, 0, bufferSize, OnReceive, null);
        }
        catch (Exception listSendException)
        {
            Debug.Log("Send error : " + listSendException);
            StageButtonMultiplayer.NetworkErrorPanelactive();
        }
        
    }

    // on receive callback method
    private void OnReceive(IAsyncResult result)
    {
        string receivedMessage = Encoding.UTF8.GetString(buffer);
        // if nothing was received
        if (receivedMessage == null || receivedMessage.Length == 0)
        {
            StageButtonMultiplayer.NetworkErrorPanelactive();
            return;
        }
        receivedMessage = receivedMessage.TrimEnd(' ');

        // parse into pairs
        string[] parsedMessage = receivedMessage.Split(' ');
        string[] intPair;

        // when number of creature in the list is not matched
        if (parsedMessage.Length != PublicLevel.usingCreatureNum)
        {
            StageButtonMultiplayer.NetworkErrorPanelactive(); ;
            ClearBuffer(buffer);
            return;
        }
        for (int i = 0; i < PublicLevel.usingCreatureNum; ++i)
        {
            intPair = parsedMessage[i].Split(',');
            PublicLevel.hostileCreatureList[i] = PublicLevel.hostilePrefab[int.Parse(intPair[0]), int.Parse(intPair[1])];

            // when a pair is not in right format
            if(intPair.Length != 2)
            {
                StageButtonMultiplayer.NetworkErrorPanelactive();
                ClearBuffer(buffer);

                return;
            }
        }

        stageButtonMultiplayer.SetCreatureReceivedFlag(true);
        StageButtonMultiplayer.NetworkWaitPanelInactive();
        
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

    // hand shake between server and client
    private static void HandShake()
    {
        try
        {
            buffer = Encoding.ASCII.GetBytes(ServerControlForm.GetHandShakeString());
            PublicLevel.GetServerStream().Write(buffer, 0, buffer.Length);
        }
        catch(Exception socketException)
        {
            Debug.Log("SocketException " + socketException.ToString());
        }
    }

    private static void CloseHandShake()
    {
        try
        {
            buffer = Encoding.ASCII.GetBytes(ServerControlForm.GetCloseHandShakeString());
            PublicLevel.GetServerStream().Write(buffer, 0, buffer.Length);
        }
        catch(Exception socketException)
        {
            Debug.Log("SocketException " + socketException.ToString());
        }

    }

    public void CloseSocket()
    {
        if(socketConnection != null)
        {
            if(PublicLevel.GetServerStream() != null)
            {
                CloseHandShake();
            }
            try
            {
                socketConnection.Close();
            }
            catch (Exception socketException)
            {
                Debug.Log("SocketException " + socketException.ToString());
            }
            socketConnection = null;
        }
        return;
    }
}