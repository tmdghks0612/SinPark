using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine;

public static class ClientListenerForm
{
    private static string localIP;
    private static IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
    private static int port = 222;
    private static int myId = 0;

    public static string ip = "127.0.0.1";

    public static IPHostEntry GetHostEntry()
    {
        return host;
    }

    public static string GetIp()
    {
        return localIP;
    }

    public static int GetPort()
    {
        return port;
    }

}
