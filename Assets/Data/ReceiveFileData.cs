using System;
using System.Net;
/// <summary>
/// 接收文件实体类
/// </summary>
public class ReceiveFileData
{
    /// <summary>
    /// ip
    /// </summary>
    public string ip { get; set; }

    /// <summary>
    /// 端口号
    /// </summary>
    public int port { get; set; }

    /// <summary>
    /// 完整路径
    /// </summary>
    public string pathFull { get; set; }

    /// <summary>
    /// 相对路径
    /// </summary>
    public string path { get; set; }

    /// <summary>
    /// md5
    /// </summary>
    public string md5 { get; set; }

    /// <summary>
    /// 回调
    /// </summary>
    public Action callback { get; set; }
}

