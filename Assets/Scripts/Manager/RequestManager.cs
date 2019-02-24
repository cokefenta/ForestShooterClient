using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestManager :BaseManager {

	public RequestManager(GameFacade gameFacade):base(gameFacade)
    {

    }
    //对于统一Controller的两个请求，用ActionCode来区分，该字典用于处理响应时使用，【解析出来的ActionCode要在这个字典里面找对应的Request对象，好去执行该Request的OnResponse】
    private Dictionary<ActionCode, BaseRequest> requestDic;

    public override void OnInit()
    {
        base.OnInit();
        requestDic = new Dictionary<ActionCode, BaseRequest>();
    }

    /// <summary>
    /// 请求所在的游戏物体初始化时将该请求加入RequestManager的字典中
    /// </summary>
    /// <param name="requestCode">RequestCode枚举</param>
    /// <param name="request">Request对象</param>
    public void AddRequestToDictionary(ActionCode actionCode, BaseRequest request)
    {
        requestDic.Add(actionCode, request);
    }

    /// <summary>
    /// 请求所在游戏物体销毁时将该请求从RequestManager字典中移除
    /// </summary>
    /// <param name="requestCode"></param>
    public void RemoveRequestFromDictionary(ActionCode actionCode)
    {
        requestDic.Remove(actionCode);
    }


    /// <summary>
    /// 处理来自服务器端的响应
    /// </summary>
    /// <param name="requestCode">请求类型枚举</param>
    /// <param name="data">数据</param>
    public void HandleRespone(ActionCode actionCode, string data)
    {
        //执行对应请求里面的OnResponse方法.[利用扩展类里面封装好的那个]
        BaseRequest request= requestDic.TryGet(actionCode);
        if (request != null)
        {
            //
            
            request.OnResponse(data);
            Debug.Log("执行了对应请求RequestCode:"+request.Requestcode+"ActionCode:"+request.Actioncode+"的OnResponse函数");
        }
        else {
            Debug.LogError("未找到ActionCode为" + actionCode + "对应的Request类");
        }
    }
}
