using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanel : MonoBehaviour {
    //该类为所有面板类的基类，虽不直接挂载到面板上，但他的子类要挂在面板上
    //所以脚本作为组件使用，那么要继承MonoBehavior

   /// <summary>
   /// 界面显示，且可交互
   /// </summary>
    public virtual void OnEnter()
    {

    }
    /// <summary>
    /// 界面暂停，不可交互，但是处于显示状态
    /// </summary>
    public virtual void OnPause()
    {

    }

    /// <summary>
    /// 界面恢复，从显示不可交互状态变为显示并且可以交互
    /// </summary>
    public virtual void OnResume()
    {

    }

    /// <summary>
    /// 界面移除，不显示，不可交互。取消激活【禁用】。非销毁，点面板X号时调用
    /// </summary>
    public virtual void OnExit()
    {

    }
}
