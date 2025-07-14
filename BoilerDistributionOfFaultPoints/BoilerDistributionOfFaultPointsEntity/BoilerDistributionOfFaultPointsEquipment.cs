using System.Collections.Generic;
using System.Linq;

namespace HDYH 
{
    public class BoilerDistributionOfFaultPointsEquipment : BoilerEquipmentBase
    {
        private List<BoilerFaultPoints> m_DataCache = new List<BoilerFaultPoints>();
        private Dictionary<BoilerFaultPoints, PCode> datacache = new Dictionary<BoilerFaultPoints, PCode>();
        private int m_Max = 0;
        public int Max => m_Max;
        public override void InitData()
        {
            m_DataCache = GetComponentsInChildren<BoilerFaultPoints>().ToList();
            Hide();
        }

        public override void OnReset()
        {            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="max">最大值，当max大于0时，表示取总览的故障点最大值,max小于0时,表示使用当前设备的故障点最大值</param>
        /// <param name="pCodes"></param>
        public void UpdateItem(List<PCode> pCodes, int max = -1)
        {
            m_Max = 0;//当前设备的故障点最大值
            datacache.Clear();
            foreach (var item in m_DataCache)//获取当前设备最大值
            {
                var data = pCodes.Find(x => x.pCode == item.PositionCode);
                if (data != null)
                {
                    if (m_Max < data.num)
                        m_Max = data.num;
                    item.gameObject.SetActive(true);
                    datacache.TryAdd(item, data);
                }
                else
                    item.gameObject.SetActive(false);
            }
            int max1 = max > 0 ? max : m_Max;

            foreach (var item in datacache)//更新数据
            {
                //Debug.Log($"{EquipmentName} max value = {max1}-------pcode:{item.Value.pCode}");
                item.Key.UpdateValue(max1, item.Value);
            }
        }

        public override void Show(object userdata = null)
        {
            base.Show();
            foreach (var item in datacache)
                item.Key.gameObject.SetActive(true);
        }
        public override void Hide(object userdata = null)
        {
            base.Hide(userdata);
            foreach (var item in m_DataCache)
                item.gameObject.SetActive(false);
        }
    }
}