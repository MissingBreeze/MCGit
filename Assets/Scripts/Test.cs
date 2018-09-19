using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{

	void Start () {
        transform.GetComponent<Button>().onClick.AddListener(ClickCallback);
	}

    int count = 100;

    private const int BUFFER_SIZE = 1024;

    public static byte[] readBuff = new byte[BUFFER_SIZE];
    Socket socket;

    private void ClickCallback()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            string ip = "127.0.0.1";
            int port = 2556;
            socket.Connect(ip, port);
            socket.BeginReceive(readBuff, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCb, null);
            Debug.LogError("开始连接服务器");
            SendGetModFile();
        }
        catch (Exception e)
        {
            Debug.LogError("连接失败,Error:" + e.Message);
        }
    }

    private void SendGetModFile()
    {

        Debug.LogError("发送接收mod列表文件");
        //Invoke("Test", 1);
        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2557);
        Thread thread = new Thread(ReceiveModTxtHandler);
        thread.Start(ipEndPoint);
        thread.IsBackground = true;
        try
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes("get");
            socket.Send(bytes);
        }
        catch (Exception e)
        {
            Debug.Log("发送失败：" + "get");
        }
    }

    private void ReceiveModTxtHandler(object obj)
    {
        Debug.LogError(">>>>>>>>>>>>>");
        IPEndPoint ipEndPoint = obj as IPEndPoint;
        TcpClient tcpClient = new TcpClient();
        try
        {
            tcpClient.Connect(ipEndPoint);
        }
        catch (Exception e)
        {
            Debug.LogError("接收mod列表文件错误。error:" + e.Message);
        }
        if (tcpClient.Connected)
        {
            string path = Directory.GetCurrentDirectory() + "/Text/" + count;
            if (!File.Exists(@path))
            {
                Directory.CreateDirectory(path);
            }
            NetworkStream stream = tcpClient.GetStream();
            if (stream != null)
            {
                FileStream fileStream = new FileStream(path + "/mod.txt", FileMode.Create, FileAccess.Write);
                int fileReadSize = 0;
                byte[] buffer = new byte[2048];
                while ((fileReadSize = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    fileStream.Write(buffer, 0, fileReadSize);
                }
                fileStream.Flush();
                fileStream.Close();
                stream.Flush();
                stream.Close();
                tcpClient.Close();
                Debug.LogError("接收mod列表文件成功");
            }
        }
        count = count - 1;
        if(count > 0)
        {
            SendGetModFile();
        }
    }

    private void ReceiveCb(IAsyncResult ar)
    {
        try
        {
            int count = socket.EndReceive(ar);
            string str = System.Text.Encoding.UTF8.GetString(readBuff, 0, count);
            socket.BeginReceive(readBuff, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCb, null);
        }
        catch (Exception e)
        {
            Debug.LogError("连接接收消息时断开, Error:" + e.Message);
        }
    }


}
