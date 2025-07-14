using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HDYH 
{
    public class BoilerMaterialEquipment : BoilerEquipmentBaseWithEditorTools
    {
        public enum PipelineType 
        {
            In,//锅内管道
            Out,//锅外管道
        }
        public PipelineType EquipmentPipelineType = PipelineType.In;
        /// <summary>
        /// 只改变符合条件的材质颜色，不符合条件的变为默认颜色
        /// </summary>
        /// <param name="materialname">材质名称</param>
        /// <param name="materialbasecolor">材质基础颜色</param>
        /// <param name="materialhdrcolor">材质hdr颜色</param>
        /// <param name="materialdefaultcolor">材质默认基础颜色</param>
        /// <param name="materialdefaulthdrcolor">材质默认hdr颜色</param>
        public void ShowMaterialColor(string materialname, Color materialbasecolor, Color materialhdrcolor, Color materialdefaultcolor, Color materialdefaulthdrcolor, List<Transform> meshRenderers)
        {
            foreach (var item in m_SerializableDictionary.serializableDictionaryObjects)
            {
                if (string.Equals(materialname, item.Key))//显示颜色
                {
                    SetColor(item.Value.Value, materialbasecolor, materialhdrcolor);
                    meshRenderers.Add(item.Value.Value.transform);
                }
                else//显示默认颜色
                    SetColor(item.Value.Value, materialdefaultcolor, materialdefaulthdrcolor);
            }
        }
        /// <summary>
        /// 改变材质颜色
        /// </summary>
        /// <param name="materialname">材质名称</param>
        /// <param name="materialbasecolor">材质基础颜色</param>
        /// <param name="materialhdrcolor">材质hdr颜色</param>
        public void ShowMaterialColor(string materialname, Color materialbasecolor, Color materialhdrcolor)
        {
            foreach (var item in m_SerializableDictionary.serializableDictionaryObjects)
            {
                if (string.Equals(materialname, item.Key))//显示颜色
                    SetColor(item.Value.Value, materialbasecolor, materialhdrcolor);//全部开启时，高亮颜色为配置的颜色
            }
        }

        /// <summary>
        /// 改变材质颜色
        /// </summary>
        /// <param name="materialname">材质名称</param>
        /// <param name="materialbasecolor">材质基础颜色</param>
        /// <param name="materialhdrcolor">材质hdr颜色</param>
        public void ShowMaterialColor(Color materialbasecolor, Color materialhdrcolor)
        {
            m_SerializableDictionary.serializableDictionaryObjects.ForEach((item) => SetColor(item.Value.Value, materialbasecolor, materialhdrcolor));//全部开启时，高亮颜色为配置的颜色
        }
        private void SetColor(Renderer renderer, Color basecolor, Color hdrcolor)
        {
            renderer.material.SetColor("_BaseColor", basecolor);
            renderer.material.SetColor("_EmissionColor", hdrcolor);
        }
    }
}