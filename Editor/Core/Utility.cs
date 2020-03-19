using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UTJ.ConfigUtil
{
    public class Utility
    {
        internal class TypeAndAttr
        {
            public System.Type type;
            public ConfigUtilityAttribute attr;
            public TypeAndAttr(System.Type t, ConfigUtilityAttribute a)
            {
                this.type = t;
                this.attr = a;
            }
        }
        internal static List<TypeAndAttr> GetTypeList()
        {
            List<TypeAndAttr> list = new List<TypeAndAttr>();
            var domain = System.AppDomain.CurrentDomain;
            foreach (var asm in domain.GetAssemblies())
            {
                var types = asm.GetTypes();
                foreach (var type in types)
                {
                    var customAttr = ConfigLoader.GetConfigAttribute(type);
                    if(customAttr == null) { continue; }
                    list.Add(new TypeAndAttr(type, customAttr));
                }
            }
            return list;
        }

        public static void SaveDataToStreamingAssets(object obj)
        {
            System.Type type = obj.GetType();
            var customAttr = ConfigLoader.GetConfigAttribute(type);
            if( customAttr == null) { return; }
            string path = ConfigLoader.GetConfigDataPath(customAttr);
            if( System.IO.File.Exists(path))
            {
                bool res = EditorUtility.DisplayDialog("Overwrite?", "File is already exist.Overwrite?", "ok", "cancel");
                if(!res) { return; }
            }
            string json = JsonUtility.ToJson(obj);
            System.IO.File.WriteAllText(path, json);
        }

        public static void SaveToPresetData(object obj,string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                EditorUtility.DisplayDialog("No preset name","Please input preset name","ok");
                return;
            }
            if (PresetData.IsPresetExists(name, obj.GetType()))
            {
                bool res = EditorUtility.DisplayDialog("Overwrite?","Preset " + name + " is already exist.overwrite?","ok","cancel");
            }
            PresetData.SavePreset(name,obj);
        }

        public static bool ApplyPreset<T>(string preset)
        {
            if( !PresetData.IsPresetExists(preset,typeof(T)))
            {
                return false;
            }

            return true;
        }
    }
}