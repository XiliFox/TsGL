using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TreeViewComponent;
using EffortFramework;

namespace HDYH
{
    public class BoilerDetailedPartTreeView : TreeView
    {
        private string m_tableName;
        private bool m_IsCanControl;

        protected override void Start()
        {
            base.Start();
            MessageManager.Register<bool>(MessageConst.BoilerDetailedPartTreeViewControl, SetControl);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            MessageManager.Unregister<bool>(MessageConst.BoilerDetailedPartTreeViewControl, SetControl);
        }
        public void OnShow(string tableName) => m_tableName = tableName;

        public void SetControl(bool state) => m_IsCanControl = state;

        public void OnItemEnterEvent(NodeBase go)
        {
            if (m_TreeViewComponents.ContainsKey(m_tableName))
            {
                foreach (var item in m_TreeViewComponents[m_tableName])
                {
                    if (go == item)//进入状态
                    {
                        item.m_gameObjectEntry.SetBackGroundState(TreeViewSelectState.Enter);
                        MessageManager.SendMessage(MessageConst.SetEquipmentInfoActive, "","");//还原所有设备开启状态
                        MessageManager.SendMessage(MessageConst.SelectEquipmentInfo, go.ParentName, go.NodeName);//选中部位变色
                    }
                    else
                        item.m_gameObjectEntry.SetBackGroundState(TreeViewSelectState.None);
                }
            }
        }

        public void OnItemExitEvent(NodeBase go)
        {
            if (m_TreeViewComponents.ContainsKey(m_tableName))
            {
                foreach (var item in m_TreeViewComponents[m_tableName])
                {
                    if (go == item)//退出状态
                    {
                        item.m_gameObjectEntry.SetBackGroundState(TreeViewSelectState.Exit);
                        MessageManager.SendMessage(MessageConst.SelectEquipmentInfo, "", "");//还原所有设备颜色
                    }
                    else
                        item.m_gameObjectEntry.SetBackGroundState(TreeViewSelectState.None);
                }
            }
        }

        public void OnItemPointEvent(NodeBase go)
        {
            //if (!m_IsCanControl)
            //    return;

            if (m_TreeViewComponents.ContainsKey(m_tableName))
            {
                foreach (var item in m_TreeViewComponents[m_tableName])
                {
                    if (go == item)//选中状态
                    {
                        item.m_gameObjectEntry.SetBackGroundState(TreeViewSelectState.Point);
                        MessageManager.SendMessage(MessageConst.SelectEquipmentInfo, "", "");//还原所有设备颜色
                        MessageManager.SendMessage(MessageConst.SetEquipmentInfoActive, go.ParentName, go.NodeName);//只显示选中设备
                        WebUtility.RequestOpenWebUI((int)WebUIID.拆解设备详情弹窗, $"{HomeViewForm.BoilerUnit}&{go.nodedata.Positioncode}");//Web端开启结焦状态弹窗
                        //Debug.Log("Web端开启结焦状态弹窗");
                    }
                    else
                        item.m_gameObjectEntry.SetBackGroundState(TreeViewSelectState.None);
                }
            }
        }

        public override void ClearNodes(string tablename = "")
        {
            base.ClearNodes(tablename);
            m_tableName = "";
        }
    }
    public static partial class MessageConst
    {
        public const string BoilerDetailedPartTreeViewControl = "BoilerDetailedPartTreeViewControl";
    }
}