using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

#if !DISABLE_UTJ_CONFIGUTIL || UNITY_EDITOR

namespace UTJ.ConfigUtil
{
    public class ConfigLoader
    {

        public static T LoadData<T>()
        {
            return LoadDataFromStreamingAssets<T>();
        }
        public static bool LoadData(out object result, System.Type type)
        {
            return LoadDataFromStreamingAssets(out result, type);
        }
        internal static T LoadDataFromStreamingAssets<T>()
        {
            object obj;
            LoadDataFromStreamingAssets(out obj, typeof(T));
            return (T)obj;
        }

        internal static bool LoadDataFromStreamingAssets(out object result,System.Type type) 
        {
            var configUtil = GetConfigAttribute(type);
            if (configUtil == null)
            {
#if DEBUG
                Debug.LogError("[ConfigUtil]Cannot found [ConfigUtil] on " + type.FullName);
#endif
                result = null;
                return false;
            }
            string file = GetConfigDataPath(configUtil);
#if UNITY_ANDROID && !UNITY_EDITOR
            UnityWebRequest unityWebRequest = UnityWebRequest.Get(file);
            unityWebRequest.SendWebRequest();
            while (!unityWebRequest.isDone)
            {
                System.Threading.Thread.Sleep(1);
            }
            if( unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
            {
#if DEBUG
                Debug.LogError("[ConfigUtil]Cannot found file " + file);
#endif
                result = null;
                return false;
            }
            string text = unityWebRequest.downloadHandler.text;
#else
            if (!File.Exists(file))
            {
#if DEBUG
                Debug.LogError("[ConfigUtil]Cannot found file " + file);
#endif
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
#endif
