using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public static class NetworkClient
{
    private static TcpClient client;
    private static StreamWriter sw;
    private static StreamReader sr;

    private static Mutex mutex;

    static NetworkClient()
    {
        mutex = new Mutex();
    }

    public static void Init(string hostname, int port)
    {
        mutex.WaitOne();
        client = new TcpClient();
        client.Connect(hostname, port);
        sw = new StreamWriter(client.GetStream());
        sr = new StreamReader(client.GetStream());
        mutex.ReleaseMutex();
    }

    public static void Disconnect()
    {
        mutex.WaitOne();
        sw.Close();
        sr.Close();
        client.Close();
        mutex.ReleaseMutex();
    }

    public static Task SendAsync<T, U>(T obj, Action<U> callback = null)
    {
        return Task.Factory.StartNew(() =>
        {
            string a = JsonConvert.SerializeObject(obj);
            mutex.WaitOne();
            sw.WriteLine(a);
            sw.Flush();
            if (callback != null)
            {
                string b = sr.ReadLine();
                mutex.ReleaseMutex();
                callback.Invoke(JsonConvert.DeserializeObject<U>(b));
            }
            else
            {
                mutex.ReleaseMutex();
            }
        });
    }

    public static Task<U> SendAsync<T, U>(T obj)
    {
        return Task<U>.Factory.StartNew(() =>
        {
            string a = JsonConvert.SerializeObject(obj);
            mutex.WaitOne();
            sw.WriteLine(a);
            sw.Flush();
            string b = sr.ReadLine();
            mutex.ReleaseMutex();
            return JsonConvert.DeserializeObject<U>(b);
        });
    }
}
