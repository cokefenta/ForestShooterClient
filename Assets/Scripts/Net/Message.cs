using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Message  {

   
    
        //这里面存放的是每一条完整的消息，【非半个，多个情况】
        private byte[] data = new byte[1024];//根据情况而定，要保证一条消息的最大不超过这个值
        private int startIdx = 0;//已经存储了多少，也是索引

        public byte[] Data
        {
            get
            {
                return data;
            }


        }

        public int StartIdx
        {
            get
            {
                return startIdx;
            }


        }

        public int RemainSize
        {
            get { return data.Length - startIdx; }
        }


        /// <summary>
        /// 解析消息-服务器用-解析来自客户端的请求
        /// </summary>
        /// <param name="newMsgAmount">新消息的数量</param>
        /// <param name="processDataCallback">回调函数执行完该方法后又执行者函数处理数据后的回调</param>
        /// 这样写的原因：保证单一性，这个Message类是单独用于处理消息的，不要引入其他的引用来破坏单一性
        public void ReadMessage(int newMsgAmount, Action<RequestCode, ActionCode, string> processDataCallback)
        {
            //更新索引
            startIdx += newMsgAmount;
            while (true)
            {
                if (startIdx <= 4) return;//下标不到4，说明连数据长度都没有读取完，直接退出
                int messageLength = BitConverter.ToInt32(data, 0);//该数据长度包含了,RequestCode占用字节数【固定长度4】,ActionCode占用字节数【固定长度4】，和数据部分【长度不固定】
                if ((startIdx - 4) >= messageLength)
                {
                    //解析RequestCode，从第4下标开始
                    RequestCode requestCode = (RequestCode)BitConverter.ToInt32(data, 4);
                    //解析ActionCode，从第8下标开始
                    ActionCode actionCode = (ActionCode)BitConverter.ToInt32(data, 8);
                    //解析数据部分
                    string dataStr = Encoding.UTF8.GetString(data, 12, messageLength - 8);//长度已经有8个用于【RequestCode和ActionCode-所以减8】
                    //通过利用委托进行处理，在调用ReadMessage的时候再具体指定委托的函数是谁【OnProcessDataCallback】-在Client中进行
                    //处理解析之后的消息
                    processDataCallback(requestCode, actionCode, dataStr);


                    //更新数组，把后面的数据提上来
                    Array.Copy(data, messageLength + 4, data, 0, startIdx - 4 - messageLength);
                    //更新下标，要拿回来，因为数组都放到前面去了
                    startIdx -= (messageLength + 4);
                }
                else
                {
                    return;
                }
            }

        }
    /// <summary>
    /// 解析消息-Unity客户端用-解析来自服务器的响应
    /// </summary>
    /// <param name="newMsgAmount"></param>
    /// <param name="processDataCallback"></param>
    public void ReadMessage(int newMsgAmount, Action<ActionCode, string> processDataCallback)
    {
        //更新索引
        startIdx += newMsgAmount;
        while (true)
        {
            if (startIdx <= 4) return;//下标不到4，说明连数据长度都没有读取完，直接退出
            int messageLength = BitConverter.ToInt32(data, 0);//该数据长度包含了,RequestCode占用字节数【固定长度4】,ActionCode占用字节数【固定长度4】，和数据部分【长度不固定】
            if ((startIdx - 4) >= messageLength)
            {
                //解析ActionCode，从第4下标开始
                ActionCode actionCode = (ActionCode)BitConverter.ToInt32(data, 4);
               
                //解析数据部分
                string dataStr = Encoding.UTF8.GetString(data, 8, messageLength -4);//长度已经有4个用于【ActionCode所以减4】
                                                                                      //通过利用委托进行处理，在调用ReadMessage的时候再具体指定委托的函数是谁【OnProcessDataCallback】-在ClientManager中进行
                                                                                      //处理解析之后的消息
                processDataCallback(actionCode, dataStr);


                //更新数组，把后面的数据提上来
                Array.Copy(data, messageLength + 4, data, 0, startIdx - 4 - messageLength);
                //更新下标，要拿回来，因为数组都放到前面去了
                startIdx -= (messageLength + 4);
            }
            else
            {
                return;
            }
        }

    }


    /// <summary>
    /// 包装数据-服务器用-发送响应给客户端
    /// </summary>
    /// <param name="requestCode"></param>
    /// <param name="data"></param>
    public static byte[] PackData(ActionCode actionCode, string data)
        {
            byte[] actionCodeBytes = BitConverter.GetBytes((int)actionCode);
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            int dataAmount = actionCodeBytes.Length + dataBytes.Length;
            byte[] dataAmountBytes = BitConverter.GetBytes(dataAmount);
            //进行组拼-3部分-注意顺序
            byte[] newBytes = dataAmountBytes.Concat(actionCodeBytes).ToArray();
            newBytes = newBytes.Concat(dataBytes).ToArray(); ;
            return newBytes;    
         }
    /// <summary>
    /// 包装数据-客户端用-发送请求到服务器
    /// </summary>
    /// <param name="requestCode"></param>
    /// <param name="actionCode"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static byte[] PackData(RequestCode requestCode, ActionCode actionCode, string data)
    {
        byte[] requestCodeBytes = BitConverter.GetBytes((int)requestCode);
        byte[] actionCodeBytes= BitConverter.GetBytes((int)actionCode);
        byte[] dataBytes = Encoding.UTF8.GetBytes(data);
        int dataAmount = requestCodeBytes.Length + dataBytes.Length+actionCodeBytes.Length;
        byte[] dataAmountBytes = BitConverter.GetBytes(dataAmount);
        //进行组拼-4部分-注意顺序
        byte[] newBytes = dataAmountBytes.Concat(requestCodeBytes).ToArray();
        newBytes = newBytes.Concat(actionCodeBytes).ToArray();
        newBytes = newBytes.Concat(dataBytes).ToArray();
        return newBytes;
    }
}
