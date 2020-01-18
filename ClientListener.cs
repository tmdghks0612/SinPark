using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class ClientListener : MonoBehaviour
{
    public static ClientListener instance;
    public static int dataBufferSize = 4096;
    protected String url = SpawnRequestForm.getUrl();
    protected int port = SpawnRequestForm.getPort();
    public int id;
    public TCP tcp;

    /*
    public class TCP
    {
        public TcpClient socket;

        private readonly int id;
        private NetworkStream stream;
        private byte[] receiveBuffer;

        
        //constructor to set the client id
        public TCP(int _id)
        {
            id = _id;
        }

        public void Connect(TcpClient _socket)
        {
            socket = _socket;
            socket.ReceiveBufferSize = dataBufferSize;
            socket.SendBufferSize = dataBufferSize;

            stream = socket.GetStream();
            receiveBuffer = new byte[dataBufferSize];

            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

            //send welcome packet

        }
        private void ReceiveCallback(IAsyncResult _result)
        {
            int byteLength = stream.EndRead(_result);
            if(byteLength <= 0)
            {
                //disconnect if no data to receive
                return;
            }

            //handle data
            byte[] _data = new byte[byteLength];
            Array.Copy(receiveBuffer, _data, byteLength);
            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
        }
        
    }
    */
    
    // Start is called before the first frame update
    private void Start()
    {
        
    }

    public void ConnectToServer()
    {
        tcp = new TCP();
        tcp.Connect();
    }

    public class TCP
    {
        public TcpClient socket;
        private NetworkStream stream;
        private byte[] receiveBuffer;

        public void Connect()
        {
            //initialize socket options on buffer size
            socket = new TcpClient
            {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize
            };

            receiveBuffer = new byte[dataBufferSize];
            socket.BeginConnect(instance.url, instance.port, ConnectCallback, socket);
        }

        private void ConnectCallback(IAsyncResult _result)
        {
            socket.EndConnect(_result);
            
            if(!socket.Connected)
            {
                //when socket is not connected
                return;
            }

            //stream for data input from server to client
            stream = socket.GetStream();

            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
        }

        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                int byteLength = stream.EndRead(_result);
                if (byteLength <= 0)
                {
                    //disconnect if no data to receive
                    return;
                }

                //handle data
                byte[] _data = new byte[byteLength];
                Array.Copy(receiveBuffer, _data, byteLength);

                //test message containing content on console
                Debug.Log(Convert.ToBase64String(receiveBuffer));
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }
            catch
            {
                Debug.Log("Error receiving TCP data");
                //disconnect
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
