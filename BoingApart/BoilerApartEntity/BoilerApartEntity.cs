using DoTweenAnimationUtility;
using EffortFramework;
using System.Collections.Generic;
using UnityEngine;

namespace HDYH
{
    // 锅炉拆解实体
    public sealed class BoilerApartEntity : BoilerEntityBase
    {
        public bool ApartPartState { get; set; } = false;

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            MessageManager.SendMessage(MessageConst.SetCameraOffsetPos, new Vector3(0, 0, 0));

            BoilerEntityDataBase bedb = userData as BoilerEntityDataBase;
            string data = bedb.m_data.userdata_Entity;

            if (string.IsNullOrEmpty(data))
                MessageManager.SendMessage(MessageConst.FocusToTargetWithDistance, new Vector3(0, 50, 0), new Vector3(10, 35, 0), 125f, 0.5f);
            else
                MessageManager.SendMessage(MessageConst.FocusToTargetWithDistance, new Vector3(0, 80, 0), new Vector3(3, 0, 0), 220f, 0.5f);

            MessageManager.SendMessage(MessageConst.SetMainSceneVolumeVignetteintensity, 0.0f);
        }

        // ------------------=== UI部分 ===------------------

        private List<MainDoTweenArtDirector> m_doTweenAnimationList = new List<MainDoTweenArtDirector>(); // 动画字典
        private Dictionary<string, BoilerApartPart> m_boilerApartPartDic = new Dictionary<string, BoilerApartPart>(); // 二级子对象状态字典
        private List<bool> m_apartStateList = new List<bool>();

        protected override void Awake()
        {
            base.Awake();
            FillList();// 填充列表
            MessageManager.Register(MessageConst.OnClickOverallButton, OnChangedOverallButton);
            MessageManager.Register<string>(MessageConst.OnClickButton, OnChangedButton);
            MessageManager.Register(MessageConst.OnClickBackButton, OnBackButton);
        }

        public override void OnDestroy()
        {
            MessageManager.Unregister(MessageConst.OnClickOverallButton, OnChangedOverallButton);
            MessageManager.Unregister<string>(MessageConst.OnClickButton, OnChangedButton);
            MessageManager.Unregister(MessageConst.OnClickBackButton, OnBackButton);
        }

        // ----------------------------------------------------------------------------------------

        private void FillList()
        {
            BoilerApartPart[] apartParts = GetComponentsInChildren<BoilerApartPart>();
            foreach (var part in apartParts)
            {
                m_boilerApartPartDic.Add(part.name, part);
            }

            MainDoTweenArtDirector[] animations = GetComponentsInChildren<MainDoTweenArtDirector>();
            foreach (var anim in animations)
            {
                m_doTweenAnimationList.Add(anim);
            }
        }
        
        // 总按钮
        private void OnChangedOverallButton()
        {
            // 改变子物件状态
            foreach (var data in m_boilerApartPartDic)
            {
                data.Value.ChangeSubState3(!ApartPartState);
            }
            ApartPartState = GetSubState().GetValueOrDefault(ApartPartState);

            // 根据状态调整镜头位置
            ChangeCamera(m_boilerApartPartDic);

            // 根据状态播放动画
            foreach (var anim in m_doTweenAnimationList)
            {
                DoTweenArtType artType = ApartPartState ? DoTweenArtType.Play : DoTweenArtType.BackPlay;
                anim.PlayAnimation(artType);
            }

            // 根据状态设置UI状态
            MessageManager.SendMessage(MessageConst.ChangeUIState, m_boilerApartPartDic, ApartPartState);
        }

        // 单个按钮
        private void OnChangedButton(string partName)
        {
            // 改变子物件状态
            if (m_boilerApartPartDic.ContainsKey(partName))
            {
                m_boilerApartPartDic[partName].ChangeSubState3(!m_boilerApartPartDic[partName].ApartPartState);
            }
            ApartPartState = GetSubState().GetValueOrDefault(ApartPartState);

            // 根据状态调整镜头位置
            ChangeCamera(m_boilerApartPartDic);

            // 根据状态播放动画
            foreach (var anim in m_doTweenAnimationList)
            {
                if (anim.name == partName)
                {
                    DoTweenArtType artType = m_boilerApartPartDic[partName].ApartPartState ? DoTweenArtType.Play : DoTweenArtType.BackPlay;
                    anim.PlayAnimation(artType);
                }
            }

            // 根据状态设置UI状态
            MessageManager.SendMessage(MessageConst.ChangeUIState, m_boilerApartPartDic, ApartPartState);
        }

        // 返回
        private void OnBackButton()
        {
            // 改变子物件状态
            foreach (var partName in m_boilerApartPartDic)
            {
                partName.Value.ChangeSubState3(false);
            }
            ApartPartState = GetSubState().GetValueOrDefault(ApartPartState);

            foreach (var anim in m_doTweenAnimationList)
            {
                anim.PlayAnimation(DoTweenArtType.Stop);
            }

            // 根据状态设置UI状态
            MessageManager.SendMessage(MessageConst.ChangeUIState, m_boilerApartPartDic, ApartPartState);
        }

        // ------------------=== 射线点击部分 ===------------------

        public float m_maxDistance = 10000f;    // 射线长度
        private Ray m_ray;                      // 创建射线
        RaycastHit m_raycastHit;    // 用于存储射线碰撞信息的变量

        void Update()
        {
            m_ray = FireRay();
            if (Input.GetMouseButtonDown(0))
            {
                // 执行射线检测
                if (Physics.Raycast(m_ray, out m_raycastHit, m_maxDistance))
                {
                    string hitName = m_raycastHit.transform.name;   // 三级对象名称
                    string hitParentName = m_raycastHit.transform.parent.name;  // 二级对象名称

                    // 改变子物件状态
                    if (m_boilerApartPartDic.ContainsKey(hitParentName))
                    {
                        m_boilerApartPartDic[hitParentName].ChangeSubState3(hitName);
                        ApartPartState = GetSubState().GetValueOrDefault(ApartPartState);
                    }

                    // 根据状态调整镜头位置
                    ChangeCamera(m_boilerApartPartDic);

                    // 根据状态播放动画
                    foreach (var anim in m_doTweenAnimationList)
                    {
                        if (anim.name == hitParentName)
                        {
                            DoTweenArtType artType = m_boilerApartPartDic[hitParentName].m_partStateDic[hitName] ? DoTweenArtType.Play : DoTweenArtType.BackPlay;
                            anim.PlayAnimation(artType, hitName);
                        }
                    }

                    // 根据状态设置UI状态
                    MessageManager.SendMessage(MessageConst.ChangeUIState, m_boilerApartPartDic, ApartPartState);
                }
            }
        }

        private Ray FireRay()
        {
            if (Camera.main == null)
                return new Ray();
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        // 根据状态调整镜头位置
        private void ChangeCamera(Dictionary<string, BoilerApartPart> partDic)
        {
            if (!Dic2ValueState(partDic))
                MessageManager.SendMessage(MessageConst.FocusToTargetWithDistance, new Vector3(0, 50, 0), new Vector3(10, 35, 0), 125f, 0.5f);
            else
                MessageManager.SendMessage(MessageConst.FocusToTargetWithDistance, new Vector3(0, 80, 0), new Vector3(3, 0, 0), 220f, 0.5f);
        }

        private bool? GetSubState()
        {
            m_apartStateList.Clear();
            foreach (var info in m_boilerApartPartDic.Values)
            {
                m_apartStateList.Add(info.ApartPartState);
            }

            bool allTrue = true;
            bool allFalse = true;
            foreach (var data in m_apartStateList)
            {
                if (data)
                    allFalse = false;
                else
                    allTrue = false;

                if (!allTrue && !allFalse)
                    return null;
            }
            return allTrue;
        }

        public bool Dic2ValueState(Dictionary<string, BoilerApartPart> partDic)
        {
            foreach (var data in partDic.Values)
            {
               if(data.Dic3ValueState())
                    return true;
            }
            return false;
        }

    }
}