using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 所有Manager的基类-持有一些共有方法
/// </summary>
public class BaseManager {
    /// <summary>
    /// 初始化
    /// </summary>
    public virtual void OnInit() { }
    /// <summary>
    /// 销毁-切换场景或者关闭时调用
    /// </summary>
    public virtual void OnDestroy() { }
    /// <summary>
    /// 由于该类自己非组件，不能挂载到游戏物体上执行，这个Update也是每帧执行-只不过由GameFacade的Update委托执行
    /// </summary>
    public virtual void Update() { }
    protected GameFacade facade;

    public BaseManager(GameFacade facade) {
        this.facade = facade;
    }

 
}
