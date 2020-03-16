using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                    var customAttr = GetConfigAttribute(type);
                    if(customAttr == null) { continue; }
                    list.Add(new TypeAndAttr(type, customAttr));
                }
            }
            return list;
        }
        internal static ConfigUtilityAttribute GetConfigAttribute(System.Type type)
        {
            var attrs = type.GetCustomAttributes(typeof(ConfigUtilityAttribute), false);
            if (attrs == null || attrs.Length <= 0) { return null; }
            var customAttr = attrs[0] as ConfigUtilityAttribute;
            return customAttr;
        }
    }
}