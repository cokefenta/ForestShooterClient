using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseRequest : MonoBehaviour {

    //用来区分Controller
    protected RequestCode requestCode=RequestCode.None;
    //Controller相同时区分具体执行的函数
    protected ActionCode actionCode = ActionCode.None;

    //调试用
    public RequestCode Requestcode { get { return requestCode; } }
    public ActionCode Actioncode { get { return actionCode; } }
    public virtual	void Awake () {
        //初始化RequestCode类型	
        //加入到字典
        GameFacade.Instance.RequestManager.AddRequestToDictionary(actionCode, this);
	}

    /// <summary>
    /// 发送请求
    /// </summary>
    public virtual void SendRequest()
    {

    }

    /// <summary>
    /// 执行收该请求的响应
    /// </summary>
    public virtual void OnResponse(string data)
    {

    }

    public virtual void OnDestroy()
    {
        GameFacade.Instance.RequestManager.RemoveRequestFromDictionary(actionCode);
    }
}
