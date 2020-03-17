using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTJ.ConfigUtil
{
    public class PresetData
    {
        private const string UTIL_DIR = "ConfigUtil/Presets";

        public string name;
        public object data;
        public PresetData(string n, object d)
        {
            this.name = n;
            this.data = d;
        }

        public static void SavePreset(string name, object data)
        {
            System.Type type = data.GetType();
            string json = JsonUtility.ToJson(data);
            string path = string.Format("{0}/{1}.json", GetDirPath(type), name);
            System.IO.File.WriteAllText(path, json);
        }

        public static bool IsPresetExists(string name,System.Type type)
        {
            string path = string.Format("{0}/{1}.json", GetDirPath(type), name);
            return System.IO.File.Exists(path);
        }

        internal static string GetDirPath<T>()
        {
            return GetDirPath(typeof(T));
        }
        internal static string GetDirPath(System.Type t) {
            var attr = ConfigLoader.GetConfigAttribute(t);
            string path = string.Format("{0}/{1}", UTIL_DIR, attr.filename);
            if( !System.IO.Directory.Exists(path) ){
                System.IO.Directory.CreateDirectory(path);
            }
            return path;
        }

        public static void LoadPresets(System.Type t, List<PresetData>  list)
        {
            list.Clear();
            var path = GetDirPath(t);
            var files = System.IO.Directory.GetFiles(path, "*.json");

            foreach( var file in files)
            {
                var data = LoadData(file,t);
                string name = System.IO.Path.GetFileNameWithoutExtension(file);
                list.Add(new PresetData(name, data));
            }
        }
        private static object LoadData(string path,System.Type t)
        {
            if(!System.IO.File.Exists(path))
            {
                return null;
            }

            var json = System.IO.File.ReadAllText(path);
            return JsonUtility.FromJson(json,t);
        }
    }
}