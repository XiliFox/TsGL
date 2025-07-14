using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HDYH
{
    public partial class BoilerDetailedPartNetwork : BoilerSystemNetworkBase
    {
        [System.Serializable]
        public class DetailedPartWebData 
        {
            public int code;
            public DetailedPartData[] data;
            public string message;
            public bool success;
        }

        [System.Serializable]
        public class DetailedPartData 
        {
            public string positionCode;//位置编码
            public string type;//损耗类型
            public string typeName;//损耗类型名称
            public string lossLength;//损耗长度
            public string originLength;//原长度
            public string sgtOverhaulTime;//建议检修时间
            public string sgtExchangeTime;//建议更换时间
            public string warnTime;//预警时间
            public string percent;
        }
        /// <summary>
        /// 发给后端的Json对象
        /// </summary>
        [System.Serializable]
        public class DetailedPartPostData 
        {
            public string[] positionCodes;//设备编码集合
            public string unit;//机组号
        }
    }
}