using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerControlForm
{
    static private string serverUrl = "dev.jimjeon.me";
    static private int serverPort = 8080;
    
    public static string GetUrl()
    {
        return serverUrl;
    }

    public static int GetPort()
    {
        return serverPort;
    }
}
