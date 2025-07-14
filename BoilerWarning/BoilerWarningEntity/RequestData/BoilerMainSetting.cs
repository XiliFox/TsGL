using GameFramework.DataTable;
using System.Linq;
using UnityEngine;

namespace HDYH
{
    // 基础设定，运行后拉取预制体使用，不考虑运行时性能
    public class BoilerMainSetting : MonoBehaviour
    {
        [SerializeField]
        private Material m_NormalMaterial;

        private DREquipment[] drEquipment;
        private Transform[] transforms;

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
                    // 设置基础材质
                    equip.gameObject.GetComponent<Renderer>().material = m_NormalMaterial;

                    // 添加脚本BoilerMainEquipData
                    equip.gameObject.AddComponent<BoilerWarningFinallyEquipment>();
                }
            }

            // 添加Code的值到BoilerMainEquipData中
            foreach (var data in drEquipment)
            {
                if(data.Positioncode != "")
                {
                    if (data.ParentNodeID == -1)
                    {
                        var dataPart = this.transform.Find(data.RootEquipment).Find(data.EquipmentPart).Find(data.EquipmentName).GetComponent<BoilerWarningFinallyEquipment>();
                        dataPart.Code = data.Positioncode;
                    }
                    else
                    {
                        // 从数据表drEquipment中获取Id等于data.ParentNodeID的行
                        var parentDataName = drEquipment.FirstOrDefault(x => x.Id == data.ParentNodeID).EquipmentName;
                        if (parentDataName != null)
                        {
                            var dataPart = this.transform.Find(data.RootEquipment).Find(data.EquipmentPart).Find(parentDataName).Find(data.EquipmentName).GetComponent<BoilerWarningFinallyEquipment>();
                            dataPart.Code = data.Positioncode;
                        }
                    }
                }
            }
        }
    }
}