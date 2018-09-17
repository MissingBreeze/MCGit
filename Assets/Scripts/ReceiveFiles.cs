
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
/// <summary>
/// 接收文件通用类
/// </summary>
public sealed class ReceiveFiles
{
    private static volatile ReceiveFiles instance;

    /// <summary>
    /// 安全锁的对象
    /// </summary>
    private static readonly object obj = new object();

    /// <summary>
    /// 无参构造函数
    /// </summary>
    private ReceiveFiles()
    {
    }

    /// <summary>
    /// 获取单例类
    /// </summary>
    public static ReceiveFiles Instance
    {
        get
        {
            if(null == instance)
            {
                lock (obj)
                {
                    if (null == instance)
                    {
                        instance = new ReceiveFiles();
                    }
                }
            }
            return instance;
        }
    }

    /// <summary>
    /// 队列
    /// </summary>
    private Queue<ReceiveFileData> queue = new Queue<ReceiveFileData>();

    /// <summary>
    /// 是否在接收文件
    /// </summary>
    private bool isReceive = false;

    /// <summary>
    /// 将需要接受的文件数据加入队列中
    /// </summary>
    /// <param name="receiveFileData">文件数据</param>
    private void PushQueue(ReceiveFileData receiveFileData)
    {
        if(receiveFileData == null)
        {
            return;
        }
        queue.Enqueue(receiveFileData);
        if (queue.Count == 1)
        {

        }
    }

    /// <summary>
    /// 监听端口，开启线程接收文件
    /// </summary>
    private void BeginReceive()
    {
        if(queue.Count == 0)
        {
            return;
        }
        ReceiveFileData receiveFileData = queue.Peek();
        //if (string.IsNullOrEmpty(receiveFileData.ip))
        //{
        //    return;
        //}
        IPEndPoint ipEp = new IPEndPoint(IPAddress.Parse(receiveFileData.ip), receiveFileData.port);
        Thread thread = new Thread(ReceiveFile);
        thread.Start(ipEp);
        thread.IsBackground = true;
    }

    /// <summary>
    /// 接收文件
    /// </summary>
    /// <param name="obj">obj</param>
    private void ReceiveFile(object obj)
    {
        IPEndPoint ipEp = obj as IPEndPoint;
        ReceiveFileData receiveFileData = queue.Dequeue();
        TcpClient tcpClient = new TcpClient();
        try
        {
            tcpClient.Connect(ipEp);
            if (tcpClient.Connected)
            {
                // 没有传完整路径时，默认保存在当前项目下
                string path = receiveFileData.pathFull;
                if (string.IsNullOrEmpty(path))
                {
                    Debug.LogError(receiveFileData.path + "保存到项目目录下");
                    path = Directory.GetCurrentDirectory() + receiveFileData.path;
                }
                NetworkStream networkStream = tcpClient.GetStream();
                if(networkStream != null)
                {
                    FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);


                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(string.Format("下载{0}失败,报错信息：{1}", receiveFileData.path,e.Message));
        }
    }

}
