using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
/// <summary>
/// 管理服务器的Socket连接
/// </summary>
public class ClientManager :BaseManager {
    /// <summary>
    /// IP地址
    /// </summary>
    private const string IP = "127.0.0.1";
    /// <summary>
    /// 端口号
    /// </summary>
    private const int port = 6688;
    private Socket clientSocket;
    private Message msg;
    

    public ClientManager(GameFacade gameFacade) : base(gameFacade)
    {

    }



    /// <summary>
    /// 重写onInit进行服务器的连接
    /// </summary>
    public override void OnInit()
    {
        msg = new Message();
        base.OnInit();
        try
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(new IPEndPoint(IPAddress.Parse(IP), port));
            StartClient();

        }
        catch (Exception e)
        {
            Debug.LogError("无法连接服务器，异常信息为" + e);
            //打开MessagePanel显示信息给用户
            MessagePanel msgPanel= GameFacade.Instance.UiManager.PushPanel(PanelType.Message) as MessagePanel;
            msgPanel.ShowTipsMsg("未能和服务器连接");
        }
    }

    private void StartClient()
    {
        clientSocket.BeginReceive(msg.Data, msg.StartIdx,msg.RemainSize,SocketFlags.None, ReceiveCallback,null);
    }


    private void ReceiveCallback(IAsyncResult ar)
    {
        try {
            if (clientSocket != null && clientSocket.Connected == true)
            {
                int count = clientSocket.EndReceive(ar);
                msg.ReadMessage(count, OnProcessDataCallBack);

                clientSocket.BeginReceive(msg.Data, msg.StartIdx, msg.RemainSize, SocketFlags.None, ReceiveCallback, null);
            }
            
        } catch (Exception e)
        {
            Debug.LogError("接收服务器的响应出现问题，异常信息为：" + e);
        }
        
    }

    /// <summary>
    /// 处理解析之后的来自服务器的响应
    /// </summary>
    /// <param name="actionCode"></param>
    /// <param name="data"></param>
    public void OnProcessDataCallBack(ActionCode actionCode,string data)
    {
        //:具体处理在RequestManager中HandleResponse处理来自服务器的响应
        //因为具体处理的代码是不属于这一块的
        GameFacade.Instance.RequestManager.HandleRespone(actionCode,data);
    }
   
    /// <summary>
    /// 向服务器发送请求
    /// </summary>
    /// <param name="requestCode"></param>
    /// <param name="actionCode"></param>
    /// <param name="data"></param>
    public void SendRequestToServer(RequestCode requestCode,ActionCode actionCode,string data)
    {
        //打包数据
        byte[] bytes= Message.PackData(requestCode, actionCode, data);
        //发送数据
        clientSocket.Send(bytes);

        Debug.Log("已经将请求RequestCode:" + requestCode + "   ActionCode:" + actionCode + "数据为" + data + "发送到服务器端");

    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        try
        {
            if (clientSocket != null)
            {
                clientSocket.Close();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("无法关闭与服务器的连接，异常信息:" + e);
        }
    }
}
