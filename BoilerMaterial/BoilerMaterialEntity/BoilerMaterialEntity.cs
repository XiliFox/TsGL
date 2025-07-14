using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using EffortFramework;
using System.Linq;

namespace HDYH
{
    /// <summary>
    /// 锅炉材质模块实体
    /// </summary>
    public sealed class BoilerMaterialEntity : BoilerEntityBase
    {
        private List<BoilerMaterialEquipment> m_BoilerPipelineIn;
        private List<BoilerMaterialEquipment> m_BoilerPipelineOut;
        private string m_StartShowMaterialName = "炉内管道材质";
        private BoilerMaterialEntityScriptableObject m_boilerMaterialEntityScriptableObject;
        protected override void OnInit(object userData)
        {
            // 如果 override OnInit 方法，则必须调用基类 OnInit 方法
            base.OnInit(userData);
            MessageManager.Register<string>(MessageConst.BoilerMaterialEntityShowMaterial, BoilerMaterialEntityShowMaterial);
            var m_BoilerEquipment = m_BoilerEquipmentBase.Select(x => x as BoilerMaterialEquipment).ToList();
            m_BoilerPipelineIn = m_BoilerEquipment.FindAll(x => x.EquipmentPipelineType == BoilerMaterialEquipment.PipelineType.In);
            m_BoilerPipelineOut = m_BoilerEquipment.FindAll(x => x.EquipmentPipelineType == BoilerMaterialEquipment.PipelineType.Out);
        }

        protected override void OnShow(object userData)
        {
            // 如果 override OnShow 方法，则必须调用基类 OnShow 方法
            base.OnShow(userData);
            m_boilerMaterialEntityScriptableObject = boilerEntityDataBase.m_data as BoilerMaterialEntityScriptableObject;
            if (m_boilerMaterialEntityScriptableObject != null)
            {
                MessageManager.SendMessage(MessageConst.SetCameraOffsetPos, m_boilerMaterialEntityScriptableObject.m_CameraStartOffset);
                BoilerMaterialEntityShowMaterial(m_StartShowMaterialName);
            }
        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            // 如果 override OnHide 方法，则必须调用基类 OnHide 方法
            base.OnHide(isShutdown, userData);
        }
        public override void OnDestroy()
        {
            MessageManager.Unregister<string>(MessageConst.BoilerMaterialEntityShowMaterial, BoilerMaterialEntityShowMaterial);
            base.OnDestroy();
        }
        /// <summary>
        /// 根据选择的材质名称，选择展示的材质颜色
        /// </summary>
        /// <param name="materialname"></param>
        private void BoilerMaterialEntityShowMaterial(string materialname) 
        {
            //点击UI"炉外管道材质"或"炉内管道材质"触发
            if (materialname.Equals("炉内管道材质")|| materialname.Equals("炉外管道材质"))
            {
                ShowTargetAllMaterial(materialname);
                return;
            }

            //点击单个设备触发
            List<Transform> transforms = new List<Transform>();
            (Color basecolor, Color hdrcolor) = m_boilerMaterialEntityScriptableObject.GetColor(materialname);

            m_BoilerPipelineIn.ForEach((item) 
                => item.ShowMaterialColor(materialname, basecolor, hdrcolor, m_boilerMaterialEntityScriptableObject.m_BoilerMaterialEntityDefaultColor.m_BaseColor, m_boilerMaterialEntityScriptableObject.m_BoilerMaterialEntityDefaultColor.m_HDRColor, transforms));
            m_BoilerPipelineOut.ForEach((item) 
                => item.ShowMaterialColor(materialname, basecolor, hdrcolor, m_boilerMaterialEntityScriptableObject.m_BoilerMaterialEntityDefaultColor.m_BaseColor, m_boilerMaterialEntityScriptableObject.m_BoilerMaterialEntityDefaultColor.m_HDRColor, transforms));
          
            var centerpoint = Calculate_CenterPoint(transforms);
            (float rotationY, float distance) = RandomUtility.GetRandomValue(new RandomRange(-45.0f, 45.0f), new RandomRange(60.0f, 120.0f));

            MessageManager.SendMessage(MessageConst.FocusToTargetWithDistance, centerpoint, new Vector3(0.0f, rotationY, 0.0f), distance, 0.5f);
        }
        /// <summary>
        /// 展示炉内设备或炉外管道的所有材质
        /// </summary>
        /// <param name="materialname"></param>
        private void ShowTargetAllMaterial(string materialname) 
        {
            List<BoilerMaterialEquipment> targetlist = materialname.Equals("炉内管道材质") ? m_BoilerPipelineIn : m_BoilerPipelineOut;
            List<BoilerMaterialEquipment> defaultlist = materialname.Equals("炉内管道材质") ? m_BoilerPipelineOut : m_BoilerPipelineIn;
            //根据实体配置，改变设备颜色
            foreach (var item in m_boilerMaterialEntityScriptableObject.m_BoilerMaterialEntityColor)
            {
                foreach (var equipment in targetlist)
                {
                    equipment.ShowMaterialColor(item.m_BoilerMaterialType, item.m_BaseColor, item.m_HDRColor);
                }
            }
            
            defaultlist.ForEach((equipment) => equipment.ShowMaterialColor(m_boilerMaterialEntityScriptableObject.m_BoilerMaterialEntityDefaultColor.m_BaseColor
                , m_boilerMaterialEntityScriptableObject.m_BoilerMaterialEntityDefaultColor.m_HDRColor));
            //改变镜头位置
            MessageManager.SendMessage(MessageConst.FocusToTargetWithDistance, m_boilerMaterialEntityScriptableObject.m_CameraStartPosition,
                 m_boilerMaterialEntityScriptableObject.m_CameraStartRotation, m_boilerMaterialEntityScriptableObject.m_CameraStartDistance, 0.5f);
        }
        /// <summary>
        /// 计算多个物体的中心点
        /// </summary>
        /// <param name="Points"></param>
        /// <returns></returns>
        public Vector3 Calculate_CenterPoint(List<Transform> Points)
        {
            int total = Points.Count;
            float lat = 0, lon = 0;

            foreach (Transform p in Points)
            {
                lat += p.transform.position.x;
                lon += p.transform.position.z;       
            }

            lat /= total;
            lon /= total;

            Vector3 centerPoint = new Vector3(lat, Points[0].position.y, lon);
            return centerPoint;
        }
    }
}
