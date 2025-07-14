using UnityEngine;

namespace HDYH
{
    /// <summary>
    /// 锅炉预警三维数据
    /// </summary>
    [System.Serializable]
    public class BoilerWarningRecord
    {
        public string code;
        public BoilerWarningData[] data;
        public string message;
        public bool success;
    }

    /// <summary>
    /// 锅炉预警三维数据对象
    /// </summary>
    [System.Serializable]
    public class BoilerWarningData
    {
        public string alarmLevel;//预警等级 (1-一般, 2-严重, 3-危险)
        public string damageType;//损伤类型编码 (1-磨损, 2-蠕变, 3-疲劳, 4-腐蚀, 5-氧化)
        public string positionCode;//部位编码
        public string thickness;//壁厚
        public string value;//损伤值

        // ----------------------------------------------------------------------------------------

        // 设置材质
        public static void SetValue(Material material, Color baseColor, Color HDRColor)
        {
            material.SetColor("_BaseColor", baseColor);
            material.SetColor("_EmissionColor", HDRColor);
        }

        // 获取损伤类型
        public static string GetDamageType(string value)
        {
            int damageTypeIntValue;
            if (int.TryParse(value, out damageTypeIntValue))
            {
                DamageType result = (DamageType)damageTypeIntValue;
                switch (result)
                {
                    case DamageType.WearAndTear:
                        return "磨损";
                    case DamageType.Creep:
                        return "蠕变";
                    case DamageType.Tired:
                        return "疲劳";
                    case DamageType.Corrosion:
                        return "腐蚀";
                    case DamageType.Oxidize:
                        return "氧化";
                    default:
                        return "";
                }
            }
            else
                return "";
        }
    }

    public enum DamageType
    {
        WearAndTear = 1,//磨损
        Creep,//蠕变
        Tired,//疲劳
        Corrosion,//腐蚀
        Oxidize,//氧化
    }
}