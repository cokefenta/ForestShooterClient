using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// UI全局控制器，不做为组件存在，所以不继承MonoBehaviour
/// </summary>
public class UIManager:BaseManager {

   
    public Transform CanvasTransform
    {
        get
        {
            if (_canvasTransform == null)
            {
                _canvasTransform = GameObject.Find("Canvas").transform;
            }
            return _canvasTransform;
        }

        set
        {
            _canvasTransform = value;
        }
    }

    public override void OnInit()
    {
        //解析json信息
        ParseUIPanelJson();
        //
    }
    public UIManager(GameFacade gameFacade):base(gameFacade)
    {
        Debug.Log("UIManager构造函数执行了");
    }

    public Dictionary<PanelType, GameObject> uiPanelPrefabDic;//用于存储ui面板预制体的字典，通过面板类型枚举进行查找
    public Dictionary<PanelType, BasePanel> instantiatedPanelDic;//该字典用于保存已经实例化【显示过的面板】【包含处于激活和非激活状态的】
    private Transform _canvasTransform;//画布-其初始化赋值在Get Set方法中进行
    public Stack<BasePanel> panelStack;//页面容器栈，当前显示在屏幕上的【栈顶为可交互，其余不可交互】

    #region 出入栈操作
    /// <summary>
    /// 入栈
    /// </summary>
    /// <param name="panelType">面板类型</param>
    public BasePanel PushPanel(PanelType panelType)
    {
        //显示面板
        BasePanel panel = ShowPanel(panelType);
        //判断栈是否为null，为null需先构造
        if (panelStack == null)
        {
            panelStack = new Stack<BasePanel>();
        }
        //判断栈内是否有元素，若有应当将栈顶的面板 暂停 掉，调用其OnPause方法
        if (panelStack.Count > 0)
        {
            BasePanel topPanel = panelStack.Peek();//取出栈顶但不移除
            topPanel.OnPause();
        }

        //该面板打开，执行他的OnEnter方法【多态的思想显示出：虽然这里是用基类对象来接收的具体面板，基类里面的函数是空的，在该对象(实质是一个具体面板类)执行OnEnter就会】
        panel.OnEnter();
        //当前面板入栈
        panelStack.Push(panel);
        //返回当前面板
        return panel;
        
    }
    /// <summary>
    /// 出栈
    /// </summary>
    /// <param name="panelType">面板类型</param>
    public void PopPanel()
    {
        //判断栈是否为null，为null需先构造
        if (panelStack == null)
        {
            panelStack = new Stack<BasePanel>();
        }
        //栈内对象为空时，直接退出
        if (panelStack.Count <= 0)
        {
            return;
        }
        else {
            //不为空则取出并删除栈顶元素，执行他的OnExit方法
            BasePanel topPanel = panelStack.Pop();
            topPanel.OnExit();
            //再次判断栈是否为空
            if (panelStack.Count <= 0)
            {
                return;
            }
            else {
                //开启新栈顶的页面显示【可交互】
                BasePanel topPanel2 = panelStack.Peek();
                topPanel2.OnResume();
            }
        }
    }
    #endregion

    /// <summary>
    /// 内部类，与UIPanel.json对应，用于解析json后用该对象来接收
    /// </summary>
    [System.Serializable]
    class PanelTypeJson
    {
        public List<PanelDataModel> panelJsonDataList;//该字段名字和json中数组名字相同，才可以用JsonUtility自动解析
        public PanelTypeJson()
        {
            panelJsonDataList = new List<PanelDataModel>();
        }
    }

    /// <summary>
    /// 解析UIPanel.json,调用在构造函数中调用，在new UIManager()时会自动执行【因为放在了构造函数中】
    /// </summary>
    private void ParseUIPanelJson()
    {
        //string path = Application.dataPath + "/UIFramework/Res/UIPanel.json";
        //由于json文件不在Resources文件下，所以不能使用动态加载，用另一种方法
        // TextAsset ta = AssetDatabase.LoadAssetAtPath<TextAsset>(path) ;//可以找到Asset下的任何位置，但只用于PC平台

        //使用动态加载
        TextAsset ta = Resources.Load("UI/UIPanel") as TextAsset;
        //Debug.Log(ta.text);//测试是否读取到json文件
        //初始化字典
        uiPanelPrefabDic = new Dictionary<PanelType, GameObject>();
        //创建一个用于接收json解析数据的接收类
        PanelTypeJson panelTypeJsonObj = new PanelTypeJson();
        //利用JsonUtility进行解析
        panelTypeJsonObj = JsonUtility.FromJson<PanelTypeJson>(ta.text);
        //测试这个对象里面的列表是否已经有数据【解析进去没有】
        //foreach (PanelDataModel temp in panelTypeJsonObj.panelJsonDataList)
        //{
        //    Debug.Log("列表中" + temp.panelTypeStr + "   " + temp.path);
        //}
        //进行遍历，并对字典添加值【注意，字典是<枚举，Gameobject>,这个列表里面存的是<枚举，路径(String)>】
        foreach (PanelDataModel temp in panelTypeJsonObj.panelJsonDataList)
        {
            GameObject panelPrefab = Resources.Load(temp.path) as GameObject;//将路径对应的预制体加载出来，添加到字典中
            temp.panelType = (PanelType)Enum.Parse(typeof(PanelType), temp.panelTypeStr); //解析到的是字符串，再把他转换为枚举之后传入枚举
            uiPanelPrefabDic.Add(temp.panelType, panelPrefab);//该条索引加入字典
           
        }
    }
    /// <summary>
    /// 实例化显示面板
    /// </summary>
    /// <param name="panelType">面板类型枚举</param>
    /// <returns></returns>
    private BasePanel ShowPanel(PanelType panelType)
    {
        if (instantiatedPanelDic == null)
        {
            instantiatedPanelDic = new Dictionary<PanelType, BasePanel>();//若是第一次使用字典，那么字典就是空的需要初始化
        }
        //下面这两句可以单独封装，叫做字典类扩展
        //BasePanel currentPanel;//拿给下面out型参数用
        //instantiatedPanelDic.TryGetValue(panelType, out currentPanel);//尝试获取传入的面板枚举类型，看当前打开面板字典中有没有他

        //用扩展类的方法
        BasePanel currentPanel = instantiatedPanelDic.TryGet(panelType);
        if (currentPanel == null)
        {
            //为空说明，这个面板没有打开，就为其打开【实例化】
            GameObject panel = GameObject.Instantiate(uiPanelPrefabDic[panelType]);//由于该类未继承MonoBehavior，直接Instantiate不可行，需要GameObject.Instantiate
            //设置父对象为canvas保证显示
            panel.transform.SetParent(CanvasTransform,false);//TODO:可能布局会有问题,添加false后解决
            //加入到字典currentShowDic
            instantiatedPanelDic.Add(panelType, panel.GetComponent<BasePanel>());
            return panel.GetComponent<BasePanel>();
        }
        else
        {
            //说明这个面板是打开的状态，直接返回这个面板
            return currentPanel;

        }
    }


   
   
}
