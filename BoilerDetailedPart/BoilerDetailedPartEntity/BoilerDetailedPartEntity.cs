using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using EffortFramework;

namespace HDYH
{
    /// <summary>
    /// 锅炉详情模块实体
    /// </summary>
    public sealed class BoilerDetailedPartEntity : BoilerEntityBase
    {
        private string m_EquipmentName;
        private BoilerDetailedPartArtDirector m_BoilerDetailedPartArtDirector;
        private BoilerDetailedEquipment m_BoilerDetailedEquipment;//当前操作的设备
        protected override void OnInit(object userData)
        {
            // 如果 override OnInit 方法，则必须调用基类 OnInit 方法
            base.OnInit(userData);
            MessageManager.Register<string, string>(MessageConst.SelectEquipmentInfo, SelectEquipmentInfo);
            MessageManager.Register<string, string>(MessageConst.SetEquipmentInfoActive, SetEquipmentInfoActive);
            m_BoilerDetailedPartArtDirector = GetComponentInChildren<BoilerDetailedPartArtDirector>();
        }
        private void SelectEquipmentInfo(string parentname, string equipmentname)
        {
            if (m_BoilerDetailedEquipment != null)
                m_BoilerDetailedEquipment.SelectEquipment(parentname, equipmentname);
        }

        private void SetEquipmentInfoActive(string parentname, string equipmentname)
        {
            if (m_BoilerDetailedEquipment != null)
            {
                //开启单独设备
                (Vector3 pos, bool state) = m_BoilerDetailedEquipment.SetSplitInfoEquipmentActive(parentname, equipmentname);

                if (state)//开启单独设备成功
                {
                    //改变相机位置。
                    (float randomX, float randomY, float randomDistance) = RandomUtility.GetRandomValue(new RandomRange(0.0f, 20.0f), new RandomRange(25.0f, 65.0f), new RandomRange(30.0f, 40.0f));
                    MessageManager.SendMessage(MessageConst.FocusToTargetWithDistance, pos, new Vector3(randomX, randomY, 0.0f), randomDistance, 0.5f);
                }
                else//全部开启
                {
                    BoilerDetailedPartEntityScriptableObject boilerDetailedPartEntityScriptableObject = boilerEntityDataBase.m_data as BoilerDetailedPartEntityScriptableObject;
                    if (boilerDetailedPartEntityScriptableObject == null || m_BoilerDetailedEquipment == null) 
                        return;

                    (Vector3, float) viewpoint = m_BoilerDetailedEquipment.GetViewPoint();
                    MessageManager.SendMessage(MessageConst.FocusToTargetWithDistance, boilerDetailedPartEntityScriptableObject.m_CameraStartPosition, viewpoint.Item1, viewpoint.Item2, 0.5f);
                }
            }
        }
        protected override void OnShow(object userData)
        {
            // 如果 override OnShow 方法，则必须调用基类 OnShow 方法
            base.OnShow(userData);
            BoilerEntityDataBase bedb = userData as BoilerEntityDataBase;
            if (bedb == null)
                return;
            m_EquipmentName = bedb.m_data.userdata_Entity;
            if (string.IsNullOrEmpty(m_EquipmentName))
                return;
            foreach (var item in m_BoilerEquipmentBase)
            {
                bool state = item.EquipmentName == m_EquipmentName;
                if (item.EquipmentName == m_EquipmentName)
                {
                    m_BoilerDetailedEquipment = item as BoilerDetailedEquipment;
                    m_BoilerDetailedEquipment.Initialized();
                }
                item.gameObject.SetActive(state);
            }

            BoilerDetailedPartEntityScriptableObject boilerDetailedPartEntityScriptableObject = boilerEntityDataBase.m_data as BoilerDetailedPartEntityScriptableObject;

            if (boilerDetailedPartEntityScriptableObject == null || m_BoilerDetailedEquipment == null)
                return;

            (Vector3, float) viewpoint = m_BoilerDetailedEquipment.GetViewPoint();
            MessageManager.SendMessage(MessageConst.FocusToTargetWithDistance, boilerDetailedPartEntityScriptableObject.m_CameraStartPosition,
                viewpoint.Item1, viewpoint.Item2, 0.5f);

            MessageManager.SendMessage(MessageConst.SetEnableRotateMouseOverEventSystemObject, true);
            MessageManager.SendMessage(MessageConst.SetCameraOffsetPos, Vector3.zero);
            if (m_BoilerDetailedPartArtDirector != null)
                m_BoilerDetailedPartArtDirector.AnimationControl(DoTweenAnimationUtility.DoTweenArtType.Play);
        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            if (m_BoilerDetailedEquipment != null)
                m_BoilerDetailedEquipment.OnReset();
            if (m_BoilerDetailedPartArtDirector != null)
                m_BoilerDetailedPartArtDirector.AnimationControl(DoTweenAnimationUtility.DoTweenArtType.Stop);
            MessageManager.SendMessage(MessageConst.SetEnableRotateMouseOverEventSystemObject, false);
            // 如果 override OnHide 方法，则必须调用基类 OnHide 方法
            base.OnHide(isShutdown, userData);
        }

        public override void OnDestroy()
        {
            MessageManager.Unregister<string, string>(MessageConst.SelectEquipmentInfo, SelectEquipmentInfo);
            MessageManager.Unregister<string, string>(MessageConst.SetEquipmentInfoActive, SetEquipmentInfoActive);
            base.OnDestroy();
        }
    }
    public static partial class MessageConst
    {
        public const string SelectEquipmentInfo = "SelectEquipmentInfo";
        public const string SetEquipmentInfoActive = "SetEquipmentInfoActive";
    }
}
