using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFacade : MonoBehaviour {

    private static GameFacade _instance;

    private UIManager _uiManager;
    private AudioManager _audioManager;
    private PlayerManager _playerManager;
    private CameraManager _cameraManager;
    private RequestManager _requestManager;
    private ClientManager _clientManager;

    public static GameFacade Instance
    {
        get
        {
            return _instance;
        }

       
    }

    public UIManager UiManager
    {
        get
        {
            return _uiManager;
        }

      
    }

    public AudioManager AudioManager
    {
        get
        {
            return _audioManager;
        }

        
    }

    public PlayerManager PlayerManager
    {
        get
        {
            return _playerManager;
        }

       
    }

    public CameraManager CameraManager
    {
        get
        {
            return _cameraManager;
        }

        
    }

    public RequestManager RequestManager
    {
        get
        {
            return _requestManager;
        }

       
    }

    public ClientManager ClientManager
    {
        get
        {
            return _clientManager;
        }

      
    }

    void Awake()
    {
        _instance = this;    
    }
    void Start () {
        InitAllManager();

        //测试，取到MessagePanel，并执行其特有的ShowMessage
        //_uiManager.PushPanel(PanelType.Message);
        //(_uiManager.instantiatedPanelDic[PanelType.Message] as MessagePanel).ShowTipsMsg("hahaha");

        //开启StartPanel
         _uiManager.PushPanel(PanelType.Start);
    }
	
	// Update is called once per frame
	void Update () {
        //Manager非组件，Update不会自动被调用，需要组件化的GameFacde来调用
        _uiManager.Update();
        _audioManager.Update();
        _playerManager.Update();
        _cameraManager.Update();
        _requestManager.Update();
        _clientManager.Update();
    }
    /// <summary>
    /// 实例化初始化所有Manager
    /// </summary>
    private void InitAllManager()
    {
        //先构造，再调用他们的OnInit
        _uiManager = new UIManager(this);
        _audioManager = new AudioManager(this);
        _playerManager = new PlayerManager(this);
        _cameraManager = new CameraManager(this);
        _requestManager = new RequestManager(this);
        _clientManager = new ClientManager(this);
        //初始化
        _uiManager.OnInit();
        _audioManager.OnInit();
        _playerManager.OnInit();
        _cameraManager.OnInit();
        _requestManager.OnInit();
        StartCoroutine(InitClientManagerDelay());//延时执行防止因未开服务器产生MessagePanel和StartPanel时间间隔过小而导致栈内顺序异常的BUG
    }
    /// <summary>
    /// 销毁所有Manager
    /// </summary>
    private void DestroyAllManager()
    {
        _uiManager.OnDestroy();
        _audioManager.OnDestroy();
        _playerManager.OnDestroy();
        _cameraManager.OnDestroy();
        _requestManager.OnDestroy();
        _clientManager.OnDestroy();
    }

    private void OnDestroy()
    {
        DestroyAllManager();
    }

    private IEnumerator InitClientManagerDelay()
    {
        yield return new WaitForSeconds(2);
        _clientManager.OnInit();
    }
}
