using System.Collections;
using System.Collections.Generic;
using Aokoro.Entities.Player;
using UnityEngine;
using Cinemachine;
using NaughtyAttributes;

public class CinemachinePlayerBinder : MonoBehaviour
{
    [SerializeField]
    private bool overrideFollow;
    [SerializeField, ShowIf("overrideFollow")]
    private bool followSpecificBone;
    [SerializeField, ShowIf(EConditionOperator.And, "overrideFollow", "followSpecificBone")]
    private HumanBodyBones followBone;
    
    [Space]
    [SerializeField]
    private bool overrideLookAt;
    [SerializeField, ShowIf("overrideLookAt")]
    private bool lookAtSpecificBone;
    [SerializeField, ShowIf(EConditionOperator.And, "overrideLookAt", "lookAtSpecificBone")]
    private HumanBodyBones lookAtBone;


    private CinemachineVirtualCamera virtualCamera;

    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }
   

    public void SimpleBind()
    {
        if (!PlayerManager.LocalPlayer)
        {
            Debug.Log("OUT");
            return;
        }
#if UNITY_EDITOR
        if (!Application.isPlaying && virtualCamera == null)
            Awake();
#endif
        if (overrideLookAt && virtualCamera.LookAt == null)
            virtualCamera.LookAt = !lookAtSpecificBone ?
                PlayerManager.LocalPlayer.transform :
                PlayerManager.LocalPlayer.anim.GetBoneTransform(lookAtBone);

        if (overrideFollow && virtualCamera.Follow == null)
            virtualCamera.Follow = !followSpecificBone ?
                PlayerManager.LocalPlayer.transform :
                PlayerManager.LocalPlayer.anim.GetBoneTransform(lookAtBone);
    }
    public void TryBind(ICinemachineCamera to, ICinemachineCamera from)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying && virtualCamera == null)
            Awake();
#endif

        if (from.Equals(virtualCamera))
        {
            SimpleBind();
        }
    }

    private void OnEnable()
    {
        virtualCamera.m_Transitions.m_OnCameraLive.AddListener(TryBind);
    }
    private void OnDisable()
    {
        virtualCamera.m_Transitions.m_OnCameraLive.RemoveListener(TryBind);
    }


}
