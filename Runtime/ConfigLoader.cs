using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace UTJ.ConfigUtil
{
    public class ConfigLoader
    {
        public static T LoadDataFromStreamingAssets<T>()
        {
            object obj;
            LoadDataFromStreamingAssets(out obj, typeof(T));
            return (T)obj;
        }

        public static bool LoadDataFromStreamingAssets(out object result,System.Type type) 
        {
            var attrs = type.GetCustomAttributes(typeof(ConfigUtilityAttribute), false);
            if( attrs == null || attrs.Length <= 0)
            {
                result = null;
                return false;
            }
            var configUtil = attrs[0] as ConfigUtilityAttribute;
            if (configUtil == null)
            {
                result = null;
                return false;
            }
            string file = configUtil.filename;
            var text = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, file));
            result = JsonUtility.FromJson(text,type);
            return true;
        }
    }
    
}