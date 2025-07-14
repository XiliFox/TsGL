namespace HDYH
{
    /// <summary>
    /// Ԥ����ϵ�ֲ�ҳ������
    /// </summary>

    [System.Serializable]
    public class BoilerDistributionOfFaultPointsRecord
    {
        public int code;
        public DistributionOfFaultPointsData data;
        public string msg;
        public bool success;
    }

    [System.Serializable]
    public class DistributionOfFaultPointsData
    {
        public OneLevel[] oneLevel;
        public PCode[] pCode;
    }

    [System.Serializable]
    public class OneLevel
    {
        public string onelevel;
        public int num;
    }

    [System.Serializable]
    public class PCode
    {
        public string pCode;
        public int num;
        public string value;
    }
}