using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

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
            var configUtil = GetConfigAttribute(type);
            if (configUtil == null)
            {
                result = null;
                return false;
            }
            string file = GetConfigDataPath(configUtil);
#if UNITY_ANDROID && !UNITY_EDITOR
            UnityWebRequest unityWebRequest = new UnityWebRequest(file);
            unityWebRequest.SendWebRequest();
            while (!unityWebRequest.isDone)
            {
            }
            if( unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
            {
                result = null;
                return false;
            }
            string text = unityWebRequest.downloadHandler.text;
#else
            if (!File.Exists(file))
            {
                result = null;
                return false;
            }
            string text = File.ReadAllText(file);
#endif
            result = JsonUtility.FromJson(text,type);
            return true;
        }
        public static string GetConfigDataPath(ConfigUtilityAttribute attr)
        {
            string streamingPath = Application.streamingAssetsPath;
#if UNITY_EDITOR
            if( !Directory.Exists(streamingPath))
            {
                Directory.CreateDirectory(streamingPath);
            }
#endif
            System.Text.StringBuilder sb = new System.Text.StringBuilder(streamingPath.Length + attr.filename.Length + 7);
            sb.Append(streamingPath).Append('/').Append(attr.filename).Append(".json");
            return sb.ToString();
        }

        public static ConfigUtilityAttribute GetConfigAttribute(System.Type type)
        {
            var attrs = type.GetCustomAttributes(typeof(ConfigUtilityAttribute), false);
            if (attrs == null || attrs.Length <= 0) { return null; }
            var customAttr = attrs[0] as ConfigUtilityAttribute;
            return customAttr;
        }
    }
    
}