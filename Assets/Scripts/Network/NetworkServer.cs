using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public static class NetworkServer
{
    private static Mutex mutex;
    private static TcpListener listener;
    private static Task clientListener;
    private static List<ClientNetworkHandler> clients;

    static NetworkServer()
    {

    }

    public static void Init(int port)
    {
        listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
        clientListener = Task.Factory.StartNew(() =>
        {

        });
    }

    public static Task Halt()
    {
        return Task.Factory.StartNew(() =>
        {
            mutex.WaitOne();
            if (listener == null)
            {
                mutex.ReleaseMutex();
                return;
            }
            listener.Stop();
            mutex.ReleaseMutex();
        });
    }

}
