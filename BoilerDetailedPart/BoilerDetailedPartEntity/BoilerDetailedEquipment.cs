using EffortFramework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HDYH 
{
    /// <summary>
    /// 锅炉细节
    /// </summary>
    public class BoilerDetailedEquipment : BoilerEquipmentBase
    {
        [System.Serializable]
        public class KeyValuePart<T1, T2>
        {
            public T1 key;
            public T2 value;
        }
        [SerializeField]
        [ColorUsageAttribute(true, true)]
        private Color m_endHDRColor;

        [SerializeField]
        [ColorUsageAttribute(true, true)]
        private Color m_startHDRColor;

        [SerializeField]
        private List<KeyValuePart<string, List<Renderer>>> m_Equipments = new List<KeyValuePart<string, List<Renderer>>>();
        [SerializeField]
        private List<GameObject> m_Headers = new List<GameObject>();//集箱集合
        [SerializeField]
        private Vector3 m_TargetPosition = Vector3.zero;
        [SerializeField]
        private Vector3 m_ViewRotation = Vector3.zero;
        [SerializeField]
        private float m_ViewDistance = 0.0f;

        private Vector3 m_StartPostion = Vector3.zero;
        private Dictionary<GameObject, Vector3> m_PositionCache = new Dictionary<GameObject, Vector3>();
        private List<BoxCollider> m_Boxcolliders = new List<BoxCollider>();
        public override void InitData()
        {
            m_StartPostion = transform.position;

            foreach (var item in m_Equipments)//记录原位置
            {
                foreach (var renderer in item.value)
                {
                    m_PositionCache.TryAdd(renderer.gameObject, renderer.transform.localPosition);
                }
            }
            m_Boxcolliders = GetComponents<BoxCollider>().ToList();
        }

        public (Vector3, float) GetViewPoint() => (m_ViewRotation, m_ViewDistance);
        public void Initialized()
        {
            transform.position = m_TargetPosition;
        }

        private void OnMouseUpAsButton()
        {
            // MessageManager.SendMessage(MessageConst.SetMainSceneVolumeVignetteintensity, 1.0f);
            // MessageManager.SendMessage(MessageConst.ChangeModel, EntityType.BoilerAttachmentEntity, EntityType.BoilerDetailedPartEntity, EquipmentName, EquipmentName);
        }
        /// <summary>
        /// 选中设备改变颜色
        /// </summary>
        /// <param name="parentname"></param>
        /// <param name="name"></param>
        public void SelectEquipment(string parentname, string name)
        {
            if (string.IsNullOrEmpty(parentname) && string.IsNullOrEmpty(name))//重置所有设备颜色
            {
                ResetAllEquipmentColor();
                return;
            }
            var equipment = m_Equipments.Find(x => x.key == name);
            if (equipment != null)//选中根节点或者与根节点同级的叶子节点设备
            {
                foreach (var item in equipment.value)
                {
                    SetMaterial(item, m_endHDRColor);
                }
            }
            else//选中叶子节点的设备
            {
                var result = m_Equipments.Find(x => x.key == parentname);
                if (result != null)
                {
                    var childequipment = result.value.Find(x => x.name == name);
                    SetMaterial(childequipment, m_endHDRColor);
                }
            }
        }

        private void SetMaterial(Renderer renderer, Color endcolor)
        {
            renderer.material.SetColor("_EmissionColor", endcolor);
        }
        /// <summary>
        /// 设置设备的开启状态
        /// </summary>
        /// <param name="parentname"></param>
        /// <param name="equipmentname"></param>
        /// <returns></returns>
        public (Vector3, bool) SetSplitInfoEquipmentActive(string parentname, string equipmentname)
        {
            if (string.IsNullOrEmpty(equipmentname) && string.IsNullOrEmpty(parentname)) //开启所有子设备
            {
                ResetAllPosition();
                transform.position = m_TargetPosition;
                SetAllEquipmentActive(true);
                m_Headers.ForEach((item) => item.SetActive(true));//开启集箱
                SetColliders(true);//开启碰撞体
                return (Vector3.zero, false);
            }
            else//只开启传入设备，关闭其他设备
            {
                SetEquipmentActive(parentname, equipmentname);
                m_Headers.ForEach((item) => item.SetActive(false));
                SetColliders(false);//开启碰撞体
                return (m_TargetPosition, true);
            }
        }

        /// <summary>
        /// 设置所有设备开启状态
        /// </summary>
        /// <param name="state"></param>
        private void SetAllEquipmentActive(bool state) 
        {
            foreach (var item in m_Equipments)
            {
                foreach (var render in item.value)
                {
                    if (render.gameObject.activeSelf != state) 
                        render.gameObject.SetActive(state);
                }
            }
        }
        /// <summary>
        /// 开启某一设备
        /// </summary>
        /// <param name="parentname"></param>
        /// <param name="equipmentname"></param>
        private void SetEquipmentActive(string parentname, string equipmentname)
        {
            var result = m_Equipments.Find(x => x.key == equipmentname);
            if (result != null)//无叶子节点的分部件设备
            {
                foreach (var item in m_Equipments)
                {
                    if (item.key == equipmentname)
                    {
                        foreach (var renderer in item.value)
                        {
                            renderer.gameObject.SetActive(true);
                            renderer.transform.position = m_TargetPosition;
                        }
                    }
                    else
                    {
                        foreach (var renderer in item.value)
                        {
                            renderer.gameObject.SetActive(false);
                        }
                    }
                }
            }
            else//普通详细部位
            {
                foreach (var item in m_Equipments)
                {
                    if (string.Equals(item.key, parentname))
                    {
                        foreach (var renderer in item.value)
                        {
                            if (string.Equals(renderer.name, equipmentname))
                            {
                                renderer.transform.position = m_TargetPosition;
                                renderer.gameObject.SetActive(true);
                            }
                            else
                            {
                                renderer.gameObject.SetActive(false);
                            }
                        }
                    }
                    else
                    {
                        foreach (var renderer in item.value)
                        {
                            renderer.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 重置所有设备颜色
        /// </summary>
        private void ResetAllEquipmentColor()
        {
            foreach (var item in m_Equipments)
            {
                foreach (var render in item.value)
                {
                    SetMaterial(render, m_startHDRColor);
                }
            }
        }
        /// <summary>
        /// 设置所有碰撞体状态
        /// </summary>
        /// <param name="state"></param>
        private void SetColliders(bool state) => m_Boxcolliders.ForEach((item) => item.enabled = state);
        /// <summary>
        /// 重置位置
        /// </summary>
        private void ResetAllPosition() 
        {
            foreach (var item in m_PositionCache)
            {
                item.Key.transform.localPosition = item.Value;
            }
        }
        public override void OnReset()
        {
            transform.position = m_StartPostion;
            ResetAllEquipmentColor();
            ResetAllPosition();
            SetAllEquipmentActive(true);
            SetColliders(true);
            m_Headers.ForEach((item) => item.SetActive(true));
        }
    }
}