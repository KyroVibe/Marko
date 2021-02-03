using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;

public class ClientNetworkHandler
{
    private TcpClient client;
    private StreamReader sr;
    private StreamWriter sw;

    public ClientNetworkHandler(TcpClient client, Action<string> response)
    {
        
    }
}
