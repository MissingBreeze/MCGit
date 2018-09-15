
using System.Net;
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
    /// 接收文件
    /// </summary>
    /// <param name="receiveFileData">文件数据</param>
    private void ReceiveFile(ReceiveFileData receiveFileData)
    {

    }
}
