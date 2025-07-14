using GameFramework.DataTable;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace HDYH
{
    public class BoilerDistributionOfFaultPointSetting : MonoBehaviour
    {
        [SerializeField]
        private Transform Content;

        private Transform[] transforms;
        private DREquipment[] drEquipment;

        private void Awake()
        {
            // 获取数据表
            IDataTable<DREquipment> dtEquipment = GameEntry.DataTable.GetDataTable<DREquipment>();
            drEquipment = dtEquipment.ToArray();

            // 获取所有子物体
            transforms = this.GetComponentsInChildren<Transform>();

            foreach (var equip in transforms)
            {
                if (equip.childCount == 0)
                {
                    // 设置名称
                    var pointName= string.Empty;
                    var code = string.Empty;

                    if (equip.parent.name != this.name)
                    {
                        var parentId = drEquipment.FirstOrDefault(x => x.EquipmentPart == this.name && x.EquipmentName == equip.parent.name).Id;
                        code = drEquipment.FirstOrDefault(x => x.EquipmentPart == this.name && x.ParentNodeID == parentId && x.EquipmentName == equip.name).Positioncode;
                        pointName = string.Format($"{this.name}-{equip.parent.name}-{equip.name}");
                    }

                    if (equip.parent.name == this.name)
                    {
                        code = drEquipment.FirstOrDefault(X => X.EquipmentPart == this.name && X.ParentNodeID == -1 && X.EquipmentName == equip.name).Positioncode;
                        pointName = string.Format($"{this.name}-{equip.name}");
                    }

                    // 在content下创建一个新物体，重置newEquip的位置和旋转
                    var newEquip = new GameObject(pointName);
                    newEquip.transform.SetParent(Content);
                    newEquip.transform.SetPositionAndRotation(equip.transform.position, equip.transform.rotation);

                    newEquip.AddComponent<SphereCollider>();
                    newEquip.AddComponent<BoilerFaultPoints>();

                    var newEquipPoint = newEquip.GetComponent<BoilerFaultPoints>();
                    newEquipPoint.PositionCode = code;
                    newEquipPoint.EquipmentName = equip.name;
                    newEquipPoint.m_MeshRenderer = equip.GetComponent<MeshRenderer>();
                }
            }
        }
    }
}
