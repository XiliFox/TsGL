namespace HDYH
{
    [System.Serializable]
    public class HomeViewRecord
    {
        public bool success;
        public int code;
        public string message;
        public HomeViewData data;
    }

    [System.Serializable]
    public class HomeViewData
    {
        public HomeCoalQuality CoalQuality;//煤质参数
        public HomeWaterQuality WaterQuality; //水质参数
        public HomeBoilerAlarmNum BoilerAlarmNum;//锅炉报警
        public HomeCoalQualityExpected CoalQualityExpected; //没用上
        public HomeBoilerWarningNum BoilerWarningNum;
    }

    [System.Serializable]
    public class HomeCoalQuality //煤质参数
    {
        public string aad;           //灰分Aad
        public string combustibles;  //飞灰可燃物
        public string mad;           //水分Mad
        public string mt;            //全水分Mt
        public string sad;           //硫Sad
        public string vad;           //挥发分Vad
    }

    [System.Serializable]
    public class HomeWaterQuality //水质参数
    {
        public string chloride;         //氯离子
        public string hardness;         //硬度
        public string oxygen;           //溶解氧
        public string ph;               //ph
        public string sulfiteRadical;   //亚硫酸根
        public string turbidity;        //浊度
    }

    [System.Serializable]
    public class HomeBoilerAlarmNum //锅炉报警
    {
        public int overTemp; //超温报警
        public int leak; //泄露报警
        public int parameter; //参数报警
    }

    [System.Serializable]
    public class HomeCoalQualityExpected
    {
        public string aad;
        public string mad;
        public string mt;
        public string sad;
        public string vad;
    }

    [System.Serializable]
    public class HomeBoilerWarningNum
    {
        public int dangerous;  //危险
        public int generally;  //一般
        public int serious;    //严重
    }
}