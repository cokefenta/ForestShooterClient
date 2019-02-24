using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {
    private Animator _animator;
    public GameObject arrowPrefab;
    private Transform _leftHandTrans;
    private int _toAttackID = Animator.StringToHash("toAttack");
	// Use this for initialization
	void Start () {
        _animator = this.GetComponent<Animator>();
       
        _leftHandTrans = this.transform.Find("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Neck/Bip001 L Clavicle/Bip001 L UpperArm/Bip001 L Forearm/Bip001 L Hand");
	}
	
	// Update is called once per frame
	void Update () {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded") && Input.GetKeyDown(KeyCode.Space))
        {
            //播放动作
            _animator.SetTrigger(_toAttackID);
            //TODO:发送同步动画请求
            GameFacade.Instance.PlayerManager.PlayerSyncGo.GetComponent<SyncAnimationRequest>().SendRequest();
            //延时实例化剑-朝向和人物相同
           Invoke("InstantiateArrow", 1);
        }
	}
    private void InstantiateArrow()
    {
        //播放音效
        GameFacade.Instance.AudioManager.PlaySound(AudioType.ArrowShoot);
        GameObject arrow=  Instantiate(arrowPrefab, _leftHandTrans.position, this.transform.localRotation);
        //设置箭的类型-本地箭
        arrow.GetComponent<Arrow>().arrowInstantType = ArrowInstantiateType.Local;
        //:发送同步箭请求
        GameFacade.Instance.PlayerManager.PlayerSyncGo.GetComponent<SyncArrowRequest>().SendRequest(this.GetComponent<RoleInfo>().roleType,arrow.transform.position,arrow.transform.eulerAngles);
    }
   
}
