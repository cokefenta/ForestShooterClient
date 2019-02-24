using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class CameraManager : BaseManager {
    private GameObject cameraGo;
    private Animator cameraAnim;
    private FollowTarget followTarget;
    private Vector3 originalPosition;
    private Vector3 originalRotation;


    public CameraManager(GameFacade facade):base(facade)
    {
        
    }

    public override void OnInit()
    {
        cameraGo = Camera.main.gameObject;
        cameraAnim = cameraGo.GetComponent<Animator>();
        followTarget = cameraGo.GetComponent<FollowTarget>();
    }

    //public override void Update()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        SwitchToFollowState(GameObject.Find("Hunter_BLUE").transform);
    //    }
    //    else if (Input.GetMouseButtonDown(1))
    //    {
    //        SwitchToAnimationState();
    //    }
    //}
    /// <summary>
    /// 转换到跟随状态
    /// </summary>
    /// <param name="targetTransform">目标位置</param>
    public void SwitchToFollowState(Transform targetTransform)
    {
        followTarget.target = targetTransform;
        cameraAnim.enabled = false;
        //为保证下一次切换回漫游状态时无缝连接，需要更新这两个参数
        originalPosition = cameraGo.transform.position;
        originalRotation = cameraGo.transform.eulerAngles;
        //先视角转过来之后，在启用followTarget，再由其的差值计算移动过去
        Quaternion targetQuaternion = Quaternion.LookRotation(followTarget.target.position - cameraGo.transform.position);
        cameraGo.transform.DORotateQuaternion(targetQuaternion, 1f).OnComplete(delegate ()
        {
            followTarget.enabled = true;
        });

    }
    /// <summary>
    /// 转换到漫游状态
    /// </summary>
    /// <param name="startPos">漫游的初始位置方向</param>
    public void SwitchToAnimationState()
    {
        followTarget.enabled = false;
        //回归到当时漫游的阶段
        cameraGo.transform.DOMove(originalPosition, 1f);
        cameraGo.transform.DORotate(originalRotation, 1f).OnComplete(delegate ()
        {
            cameraAnim.enabled = true;
        });
    }
}
