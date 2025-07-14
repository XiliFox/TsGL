using System.Collections.Generic;
using UnityEngine;
using EffortFramework;
using System.Linq;

namespace HDYH
{
    /// <summary>
    /// 故障点分布实体
    /// </summary>
    public sealed class BoilerDistributionOfFaultPointsEntity : BoilerEntityBase
    {
        private BoilerDistributionOfFaultPointsArtDirector m_BoilerDistributionOfFaultPointsArtDirector = null;
        private BoilerDistributionOfFaultPointsEntityScriptableObject m_BoilerDistributionOfFaultPointsEntityScriptableObject = null;
        //private Transform m_FireEffectGroup = null;
        private List<BoilerDistributionOfFaultPointsEquipment> m_DistributionOfFaultPointsEquipments = new List<BoilerDistributionOfFaultPointsEquipment>();
        private string m_CurrentEquipmentName = "总览";//当前选中的设备："水冷壁"、"过热器"、"再热器"、"省煤器"、"总览"
        protected override void OnInit(object userData)
        {
            // 如果 override OnInit 方法，则必须调用基类 OnInit 方法
            base.OnInit(userData);
            foreach (var item in m_BoilerEquipmentBase)
                m_DistributionOfFaultPointsEquipments.Add(item as BoilerDistributionOfFaultPointsEquipment);
            m_BoilerDistributionOfFaultPointsArtDirector = GetComponentInChildren<BoilerDistributionOfFaultPointsArtDirector>();
            MessageManager.Register<string>(MessageConst.更新故障点视角, UpdateEntityView);
            MessageManager.Register<List<PCode>>(MessageConst.故障点分布更新实体数据, UpdateEntityData);

            m_BoilerDistributionOfFaultPointsEntityScriptableObject = boilerEntityDataBase.m_data as BoilerDistributionOfFaultPointsEntityScriptableObject;
            //m_FireEffectGroup = transform.Find("FireEffectGroup");
        }

        protected override void OnShow(object userData)
        {
            // 如果 override OnShow 方法，则必须调用基类 OnShow 方法
            base.OnShow(userData);
            if (m_BoilerDistributionOfFaultPointsArtDirector != null)
                m_BoilerDistributionOfFaultPointsArtDirector.AnimationControl(DoTweenAnimationUtility.DoTweenArtType.Play);
        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            if (m_BoilerDistributionOfFaultPointsArtDirector != null)
                m_BoilerDistributionOfFaultPointsArtDirector.AnimationControl(DoTweenAnimationUtility.DoTweenArtType.Stop);
            // 如果 override OnHide 方法，则必须调用基类 OnHide 方法
            base.OnHide(isShutdown, userData);
        }
        public override void OnDestroy()
        {
            MessageManager.Unregister<string>(MessageConst.更新故障点视角, UpdateEntityView);
            MessageManager.Unregister<List<PCode>>(MessageConst.故障点分布更新实体数据, UpdateEntityData);
            base.OnDestroy();
        }
        private void UpdateEntityView(string equipment) 
        {
            m_CurrentEquipmentName = equipment;
            if (equipment.Equals("总览"))
            {
                //m_FireEffectGroup.gameObject.SetActive(true);
                MessageManager.SendMessage(MessageConst.FocusToTargetWithDistance, m_BoilerDistributionOfFaultPointsEntityScriptableObject.CameraStartPostion,
                        m_BoilerDistributionOfFaultPointsEntityScriptableObject.CameraStartRotation,
                        m_BoilerDistributionOfFaultPointsEntityScriptableObject.CameraStartDistance,
                        m_BoilerDistributionOfFaultPointsEntityScriptableObject.CameraMoveTime);
                return;
            }
            foreach (var item in m_DistributionOfFaultPointsEquipments)
            {
                if (item.EquipmentName.Equals(equipment))
                {
                    MessageManager.SendMessage(MessageConst.FocusToTargetWithDistance, item.transform.position,
                        new Vector3(0.0f, RandomUtility.GetRandomValue(m_BoilerDistributionOfFaultPointsEntityScriptableObject.RandomYRange), 0.0f),
                        RandomUtility.GetRandomValue(m_BoilerDistributionOfFaultPointsEntityScriptableObject.RandomDistanceRange),
                        m_BoilerDistributionOfFaultPointsEntityScriptableObject.CameraMoveTime);
                }
            }
            //m_FireEffectGroup.gameObject.SetActive(false);
        }
        /// <summary>
        /// 更新故障点数据
        /// </summary>
        /// <param name="pCodes"></param>
        private void UpdateEntityData(List<PCode> pCodes) 
        {
            if (m_CurrentEquipmentName.Equals("总览"))//总览的故障点最大值为所有设备故障点的最大值
            {
                int max = 0;
                if (pCodes != null && pCodes.Count > 0)
                    max = pCodes.Max(x => x.num);
                foreach (var item in m_DistributionOfFaultPointsEquipments)
                    item.UpdateItem(pCodes, max);
            }
            else
            {
                foreach (var item in m_DistributionOfFaultPointsEquipments)
                {
                    if (item.EquipmentName.Equals(m_CurrentEquipmentName))
                        item.UpdateItem(pCodes);
                    else
                        item.Hide();
                }
            }
        }
    }
}
