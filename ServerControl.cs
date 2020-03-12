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

    public static string serverMessage;
    
    private static int bufferSize = 1024;
    private static Byte[] buffer = new Byte[bufferSize];

    public static bool creatureReceiveSuccess;

    Coroutine sendCoroutine;
    Coroutine listenCoroutine;

    private bool cancelFlag = false;

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
            if (!cancelFlag)
            {
                socketConnection = new TcpClient(ServerControlForm.GetUrl(), ServerControlForm.GetPort());
                PublicLevel.SetServerStream(socketConnection.GetStream());

                HandShake();

                serverStream = PublicLevel.GetServerStream();

                // if success, send friendly creature list through stream
                sendCoroutine = StartCoroutine(SendCreatureList(0.1f));
                listenCoroutine = StartCoroutine(ListenForHostileCreatureList(1.0f));
            }
        }
        catch(SocketException socketException)
        {
            Debug.Log("SocketException " + socketException.ToString());
            StageButtonMultiplayer.NetworkErrorPanelactive();
        }


    }

    public IEnumerator SendCreatureList(float _waitTime)
    {
        yield return new WaitForSeconds(_waitTime);
        buffer = new byte[bufferSize];
        try
        {
            // create a socket for streaming data
            string serverMessage="";
            Vector2Int[] friendlyType = PublicLevel.GetFriendlyType();
            for (int i = 0; i < PublicLevel.usingCreatureNum; ++i)
            {
                serverMessage = serverMessage + friendlyType[i].x.ToString() + ',' + friendlyType[i].y.ToString() + " ";
            }
            serverMessage = serverMessage + friendlyType[PublicLevel.usingCreatureNum].x.ToString() + ',' + friendlyType[PublicLevel.usingCreatureNum].y.ToString();
            Encoding.ASCII.GetBytes(serverMessage).CopyTo(buffer, 0);
            PublicLevel.GetServerStream().Write(buffer, 0, bufferSize);
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
            Vector2Int[] _hostileType = new Vector2Int[PublicLevel.usingCreatureNum + 1];

            if (!cancelFlag)
            {
                serverStream.BeginRead(buffer, 0, bufferSize, new AsyncCallback(OnReceive), null);
            }
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
        serverMessage = Encoding.UTF8.GetString(buffer);
        string receivedMessage = Encoding.UTF8.GetString(buffer);
        
        // user interrupt to cancel callback
        if (cancelFlag)
        {
            return;
        }
        // if nothing was received
        else if (receivedMessage == null || receivedMessage.Length == 0)
        {
            StageButtonMultiplayer.NetworkErrorPanelactive();
            return;
        }
        receivedMessage = receivedMessage.TrimEnd(' ');

        // parse into pairs
        string[] parsedMessage = receivedMessage.Split(' ');
        string[] intPair;

        // user interrupt to cancel callback
        if (cancelFlag)
        {
            return;
        }
        // when number of creature in the list is not matched
        else if (parsedMessage.Length != PublicLevel.usingCreatureNum +1)
        {
            StageButtonMultiplayer.NetworkErrorPanelactive(); ;
            ClearBuffer(buffer);
            return;
        }

        if (cancelFlag)
        {
            return;
        }

        for (int i = 0; i < PublicLevel.usingCreatureNum +1; ++i)
        {
            intPair = parsedMessage[i].Split(',');
            PublicLevel.hostileCreatureList[i] = PublicLevel.friendlyPrefab[int.Parse(intPair[0]), int.Parse(intPair[1])];

            if (cancelFlag)
            {
                return;
            }
            // when a pair is not in right format
            else if (intPair.Length != 2)
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
                if (serverStream.Read(buffer, 0, bufferSize) != 0)
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
            buffer = new Byte[bufferSize];
            Encoding.ASCII.GetBytes(ServerControlForm.GetHandShakeString()).CopyTo(buffer, 0);
            PublicLevel.GetServerStream().Write(buffer, 0, bufferSize);
            ClearBuffer(buffer);
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
            buffer = new Byte[bufferSize];
            Encoding.ASCII.GetBytes(ServerControlForm.GetCloseHandShakeString()).CopyTo(buffer, 0);

            // send signal to server to close socket
            if(PublicLevel.GetServerStream() != null)
            {
                PublicLevel.GetServerStream().Write(buffer, 0, bufferSize);
            }
        }
        catch(Exception socketException)
        {
            Debug.Log("SocketException " + socketException.ToString());
        }

    }

    public void CloseSocket()
    {
        if (PublicLevel.GetServerStream() != null)
        {
            CloseHandShake();
        }
        try
        {
            // close socket from client
            if(socketConnection != null)
            {
                socketConnection.Close();
            }
        }
        catch (Exception socketException)
        {
            Debug.Log("SocketException " + socketException.ToString());
        }
        return;
    }

    public void EndConnection()
    {
        cancelFlag = true;
        if(sendCoroutine != null)
        {
            StopCoroutine(sendCoroutine);
        }
        if(listenCoroutine != null)
        {
            StopCoroutine(listenCoroutine);
        }
        
        CloseSocket();
        StageButtonMultiplayer.NetworkWaitPanelInactive();
    }

    public bool GetCancelFlag()
    {
        return cancelFlag;
    }

    public void SetCancelFlag(bool _cancelFlag)
    {
        cancelFlag = _cancelFlag;
    }
}