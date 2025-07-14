using EffortFramework;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TreeViewComponent;
using GameFramework.DataTable;
using TreeViewVariantComponent;
using System.Linq;
using System.Collections.Generic;

namespace HDYH
{
    public static partial class MessageConst
    {
        public const string BoilerDetailedShowLeftBottomPanel = "BoilerDetailedShowLeftBottomPanel";
    }
    public class BoilerDetailedPartForm : UGuiForm
    {
        [SerializeField, LabelText("返回")]
        private Button m_back;

        [SerializeField, LabelText("设备名称")]
        private TextMeshProUGUI m_partName;

        [SerializeField]
        private TreeViewVariant m_TreeViewVariant = null;

        [SerializeField]
        private BoilerDetailedLeftBottomPanel m_BoilerDetailedLeftBottomPanel = null;

        private List<DREquipment> drEquipment;
        private DREquipment m_CurrentEquipment;
        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            m_back.onClick.AddListener(OnClickBackBtn);
            MessageManager.Register<DREquipment,bool>(MessageConst.BoilerDetailedShowLeftBottomPanel, ShowLeftBottomPanel);
        }
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            string equipmentname = userData as string;
            if (string.IsNullOrEmpty(equipmentname))
                return;
            m_partName.text = equipmentname;
            m_BoilerDetailedLeftBottomPanel.Close();

            if (m_TreeViewVariant != null)
            {
                IDataTable<DREquipment> dtEquipment = GameEntry.DataTable.GetDataTable<DREquipment>();
                var drEquipmentlist = dtEquipment.GetDataRows((DREquipment x) => x.EquipmentPart == equipmentname);
                drEquipment = drEquipmentlist.ToList();
                m_TreeViewVariant.CreateItem(drEquipment);
            }
        }
        protected override void OnClose(bool isShutdown, object userData)
        {
            m_TreeViewVariant?.Clear();
            m_BoilerDetailedLeftBottomPanel?.Close();
            WebUtility.RequestHideWebUI((int)WebUIID.拆解设备详情弹窗, "");//Web端关闭设备详情UI弹窗
            base.OnClose(isShutdown, userData);
        }
        private void ShowLeftBottomPanel(DREquipment selectdr, bool state)
        {
            if (state)
            {
                if (m_CurrentEquipment == selectdr && m_BoilerDetailedLeftBottomPanel.IsShow) 
                    return;
                m_CurrentEquipment = selectdr;
                m_BoilerDetailedLeftBottomPanel.gameObject.SetActive(true);
                List<DREquipment> drequipment = new List<DREquipment>();
                if (selectdr.HasChild)//选中有子节点的一项。生成子节点
                    drequipment = drEquipment.FindAll(x => x.ParentNodeID == selectdr.Id);
                else
                    drequipment.Add(selectdr);//没有子节点的一项。生成自身
                m_BoilerDetailedLeftBottomPanel.ShowLeftBottomPanel(drequipment);
            }
            else
            {
                m_CurrentEquipment = null;
                m_BoilerDetailedLeftBottomPanel.Close();
            }
        }
        private void OnDestroy()
        {
            m_back.onClick.RemoveListener(OnClickBackBtn);
            MessageManager.Unregister<DREquipment,bool>(MessageConst.BoilerDetailedShowLeftBottomPanel, ShowLeftBottomPanel);
        }

        private void OnClickBackBtn()
        {
            MessageManager.SendMessage(MessageConst.ChangeModel, EntityType.BoilerApartEntity, EntityType.BoilerDetailedPartEntity, "BoilerDetailedPartForm", "");
        }
    }
    public static partial class MessageConst
    {
        public const string SelectViewVariantItem = "SelectViewVariantItem";
    }
}
