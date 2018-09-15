using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System.Net.Sockets;
using System;
using System.Net;
using System.Threading;
using System.IO;

public class UIControl : MonoBehaviour 
{

    public Button _selectFileBtn;

    public Button _showFile;

    public Button _dowmloadBtn;

    public Image _tipPanel;

    private TipUIControl _tip;

    /// <summary>
    /// 下载路径文本
    /// </summary>
    private Text _fileText;

    private Socket _socket;

    private const int BUFFER_SIZE = 1024;

    public static byte[] readBuff = new byte[BUFFER_SIZE];

    private bool _isFirst = true;

    private List<string> addList;
    private List<string> deleteList;

    void Start()
    {
        _selectFileBtn.onClick.AddListener(SelectFileClickEvent);
        _tip = _tipPanel.GetComponent<TipUIControl>();
        _fileText = _showFile.transform.Find("Text").GetComponent<Text>();

        Connect();
    }

    void Update () 
    {
		
	}

    /// <summary>
    /// 连接服务器
    /// </summary>
    private void Connect() 
    {
        _socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        try
        {
            string ip = "127.0.0.1";
            int port = 2556;
            _socket.Connect(ip, port);
            _socket.BeginReceive(readBuff, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCb, null);
            Debug.LogError("开始连接服务器");
            if (_isFirst)
            {
                // 获取mod列表文件
                _isFirst = false;
                SendGetModFile();
            }
        }
        catch(Exception e)
        {
            // Todo 连接失败，提醒重连
            Debug.LogError("连接失败,是否重新连接？/n Error:" + e.Message);
            //_tip.SetTip("连接失败,是否重新连接？/n Error:" + e.Message, ConnectCallback);
        }

    }

    /// <summary>
    /// 发送接收mod列表文件消息
    /// </summary>
    private void SendGetModFile()
    {

        Debug.LogError("发送接收mod列表文件");
        //Invoke("Test", 1);
        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2557);
        Thread thread = new Thread(ReceiveModTxtHandler);
        thread.Start(ipEndPoint);
        thread.IsBackground = true;
        SendMsg("get");
    }

    private void Test()
    {
        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2557);
        Thread thread = new Thread(ReceiveModTxtHandler);
        thread.Start(ipEndPoint);
        thread.IsBackground = true;
    }


    /// <summary>
    /// 接收mod列表文件
    /// </summary>
    /// <param name="obj"></param>
    private void ReceiveModTxtHandler(object obj)
    {
        IPEndPoint ipEndPoint = obj as IPEndPoint;
        TcpClient tcpClient = new TcpClient();
        try
        {
            tcpClient.Connect(ipEndPoint);
        }catch(Exception e)
        {
            Debug.LogError("接收mod列表文件错误。error:" + e.Message);
        }
        if (tcpClient.Connected)
        {
            string path = Directory.GetCurrentDirectory();
            NetworkStream stream = tcpClient.GetStream();
            if(stream != null)
            {
                FileStream fileStream = new FileStream(path + "/mod.txt", FileMode.Create, FileAccess.Write);
                int fileReadSize = 0;
                byte[] buffer = new byte[2048];
                while((fileReadSize = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    fileStream.Write(buffer, 0, fileReadSize);
                }
                fileStream.Flush();
                fileStream.Close();
                stream.Flush();
                stream.Close();
                tcpClient.Close();
                Debug.LogError("接收mod列表文件成功");
                if (!string.IsNullOrEmpty(_fileText.text))
                {
                    ContrastMod();
                }
            }
        }
    }

    /// <summary>
    /// 连接失败，提示回调
    /// </summary>
    /// <param name="operate">按钮选择结果</param>
    private void ConnectCallback(bool operate)
    {
        if (operate)
        {
            Connect();
        }
        else
        {
            Application.Quit();
        }

    }

    private void ReceiveCb(IAsyncResult ar)
    {
        try
        {
            int count = _socket.EndReceive(ar);
            string str = System.Text.Encoding.UTF8.GetString(readBuff, 0, count);
            _socket.BeginReceive(readBuff, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCb, null);
        }
        catch (Exception e)
        {
            Debug.LogError("连接接收消息时断开,是否重新连接？/n Error:" + e.Message);
            //_tip.SetTip("连接接收消息时断开,是否重新连接？/n Error:" + e.Message, ConnectCallback);
        }
    }

    /// <summary>
    /// 选择文件夹
    /// </summary>
    public void SelectFileClickEvent() 
    {
        //System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();   //new一个方法
        //ofd.InitialDirectory = "file://" + UnityEngine.Application.dataPath;  //定义打开的默认文件夹位置//定义打开的默认文件夹位置
        //if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //{
        //    //显示打开文件的窗口
        //    Debug.Log(ofd.FileName);
        //}
        System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
        dlg.Description = "选择文件夹";
        dlg.RootFolder = System.Environment.SpecialFolder.MyComputer;
        dlg.ShowNewFolderButton = false;
        if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) 
        {
            _fileText.text = dlg.SelectedPath;
        }
        if (!string.IsNullOrEmpty(_fileText.text))
        {
            ContrastMod();
        }
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="msg"></param>
    private void SendMsg(string msg) 
    {
        try
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(msg);
            _socket.Send(bytes);
        }
        catch (Exception e) 
        {
            Debug.Log("发送失败："+ msg);
        }


    }

    /// <summary>
    /// 对比mod
    /// </summary>
    private void ContrastMod()
    {
        string path = Directory.GetCurrentDirectory() + @"\mod.txt";
        Debug.LogError(path);
        if (File.Exists(@path) && !string.IsNullOrEmpty(_fileText.text))
        {
            addList = new List<string>();
            deleteList = new List<string>();

            List<string> modList = new List<string>();
            foreach (string item in File.ReadAllLines(path))
            {
                modList.Add(item);
            }

            List<string> fileList = new List<string>();
            DirectoryInfo files = new DirectoryInfo(_fileText.text);
            foreach (var item in files.GetFiles())
            {
                fileList.Add(item.Name);
            }
            if(fileList.Count == 0)
            {
                addList = modList;
            }
            else
            {
                for (int i = 0; i < modList.Count; i++)
                {
                    if (!fileList.Contains(modList[i]))
                    {
                        addList.Add(modList[i]);
                    }
                }

                for (int i = 0; i < fileList.Count; i++)
                {
                    if (modList.Contains(fileList[i]))
                    {
                        deleteList.Add(fileList[i]);
                    }
                }
            }
            DeleteMod(deleteList);
        }
        else
        {
            Debug.LogError("mod目录文件不存在或者未选择下载路径");
        }
    }

    /// <summary>
    /// 删除mod
    /// </summary>
    /// <param name="modList">要删除的mod列表</param>
    private void DeleteMod(List<string> modList)
    {
        if(modList!=null && modList.Count > 0)
        {
            string path = Directory.GetCurrentDirectory();
            for (int i = 0; i < modList.Count; i++)
            {
                File.Delete(path + modList[i]);
            }
        }
        DoloadMod();
    }

    private int index;

    /// <summary>
    /// 下载mod
    /// </summary>
    private void DoloadMod()
    {
        if(addList!=null && addList.Count > 0)
        {
            index = 0;
            BeginDoload();
        }
    }


    private void BeginDoload()
    {
        if(index >= addList.Count)
        {
            return;
        }
        Debug.LogError(index + addList[index]);
        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2558);
        Thread thread = new Thread(ReceiveModHandler);
        thread.Start(ipEndPoint);
        thread.IsBackground = true;
        SendMsg("download " + addList[index]);
    }

    /// <summary>
    /// 接收mod文件
    /// </summary>
    /// <param name="obj">接收到的对象</param>
    private void ReceiveModHandler(object obj)
    {
        IPEndPoint ipEndPoint = obj as IPEndPoint;
        TcpClient tcpClient = new TcpClient();
        try
        {
            tcpClient.Connect(ipEndPoint);
        }
        catch(Exception e)
        {
            Debug.LogError("接收mod文件出错，错误信息：" + e.Message);
        }
        if (tcpClient.Connected)
        {
            string path = _fileText.text;
            NetworkStream stream = tcpClient.GetStream();
            if(stream != null)
            {
                FileStream fileStream = new FileStream(path + @"/" + addList[index], FileMode.Create, FileAccess.Write);
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
                Debug.LogError("接收mod文件成功,mod名:"+ addList[index]);
                index++;
                BeginDoload();
            }

        }

    }
}
