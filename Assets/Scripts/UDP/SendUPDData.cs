using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net;
using System.Net.Sockets;
using System.Text;

/// <summary>
///发送UDP字符串udpData_str
/// </summary>
public class SendUPDData : MonoBehaviour {

    public static SendUPDData instance;

    public string udpData_str;
    string _sSend = "";

    //[Tooltip("接受端口号")] public int m_ReceivePort = 29010;//接收的端口号 
    Socket udpserver = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
   // public string m_ip = "192.168.1.254";//定义一个IP地址

    public bool udp_Send(string da)
    {
        try
        {
            //设置服务IP，设置端口号
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(ValueSheet.ServerIP), ValueSheet.ServerPort);
            //发送数据
            byte[] data = new byte[1024];
            data = Encoding.ASCII.GetBytes(da);
            udpserver.SendTo(data, data.Length, SocketFlags.None, ipep);
            return true;
        }
        catch
        {
            return false;
        }
    }

    // Use this for initialization
    void Start()
    {
        initialization();
    }

    public void initialization() {
        if (instance == null)
        {
            instance = this;
        }
    }



}