using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ArrowInstantiateType { Local,Remote }
public class Arrow : MonoBehaviour {
    public RoleType roleType;
    private Rigidbody _rgidBody;
    private float speed = 20;
    //命中特效
    private GameObject _effectPrefab;
    //箭的实例化类型-是本地的还是远端的
    public ArrowInstantiateType arrowInstantType = ArrowInstantiateType.Local;
    private void Awake()
    {
        _rgidBody = this.GetComponent<Rigidbody>();
    }
    // Use this for initialization
    void Start () {
        _rgidBody.velocity = this.transform.forward * speed;//设置一个正前方的速度
        //设置命中特效
        if (roleType == RoleType.Blue)
        {
            _effectPrefab = Resources.Load("Prefabs/DestroyEffect_Blue") as GameObject;
        }
        else if (roleType == RoleType.Red)
        {
            _effectPrefab = Resources.Load("Prefabs/DestroyEffect_Red") as GameObject;
        }
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    /// <summary>
    /// 生成特效
    /// </summary>
    private void CreatEffect()
    {
        Instantiate(_effectPrefab, this.transform.position, this.transform.rotation);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Env")
        {

            //碰到建筑物-地面，延时5s后销毁
            Destroy(this.gameObject, 5);
        }
        else if (collision.gameObject.tag == "Blue")
        {
            //播放音效
            GameFacade.Instance.AudioManager.PlaySound(AudioType.ShootPerson);
            //产生特效
            CreatEffect();
            //伤害判定【只在本地箭的情况处理且没有射中自己】
            if (arrowInstantType == ArrowInstantiateType.Local && !GameFacade.Instance.PlayerManager.CurrentControllRoleGo.tag.Equals("Blue"))
            {
                //发送扣血请求-扣对方【远端玩家】的血
                GameFacade.Instance.PlayerManager.PlayerSyncGo.GetComponent<TakeDamageRequest>().SendRequest(10);
            }
            //立即销毁
            Destroy(this.gameObject);

        }
        else if (collision.gameObject.tag == "Red") {
            //播放音效
            GameFacade.Instance.AudioManager.PlaySound(AudioType.ShootPerson);
            //产生特效
            CreatEffect();
            //伤害判定【只在本地箭的情况处理且没有射中自己】
            if (arrowInstantType == ArrowInstantiateType.Local && !GameFacade.Instance.PlayerManager.CurrentControllRoleGo.tag.Equals("Red"))
            {
                GameFacade.Instance.PlayerManager.PlayerSyncGo.GetComponent<TakeDamageRequest>().SendRequest(10);
            }
            //立即销毁
            Destroy(this.gameObject);
        }
    }
}
