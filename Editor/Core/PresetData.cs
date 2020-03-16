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

        public static void SavePreset<T>(string name, T data)
        {
            string json = JsonUtility.ToJson(data);

            string path = string.Format("{0}/{1}.json", GetDirPath<T>(), name);
            System.IO.File.WriteAllText(path, json);
        }

        private static string GetDirPath<T>()
        {
            return GetDirPath(typeof(T));
        }
        private static string GetDirPath(System.Type t) {
            var attr = Utility.GetConfigAttribute(t);
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
                string name = System.IO.Path.GetFileName(file);
                list.Add(new PresetData(name, data));
            }
        }
        private static object LoadData(string path,System.Type t)
        {
            var json = System.IO.File.ReadAllText(path);
            return JsonUtility.FromJson(path,t);
        }
    }
}