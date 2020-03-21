using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace UTJ.ConfigUtil
{
    public class BuildPreProcess : IPreprocessBuildWithReport
    {
        public int callbackOrder { get { return 0; } }
        public void OnPreprocessBuild(BuildReport report)
        {
            Execute();
        }

        private static void Execute()
        {
            var typeInfoList = Utility.GetTypeList();
            foreach (var typeInfo in typeInfoList)
            {
                var interfaces = typeInfo.type.GetInterfaces();
                if (Contains(interfaces, typeof(IConfigUpdateOnBuild)))
                {
                    UpdateInfo(typeInfo);
                }
            }
        }

        private static void UpdateInfo(Utility.TypeAndAttr typeAndAttr)
        {
            object obj = null;
            ConfigLoader.LoadData(out obj,typeAndAttr.type);
            if( obj == null)
            {
                obj = System.Activator.CreateInstance(typeAndAttr.type);
            }

            IConfigUpdateOnBuild config = obj as IConfigUpdateOnBuild;
            config.OnPreprocessBuild();
            Utility.SaveDataToStreamingAssets(config);
        }

        private static bool Contains(System.Type[] srcTypes,System.Type type)
        {
            if (srcTypes == null) { return false; }
            foreach( var src in srcTypes)
            {
                if( src == type) { return true; }
            }
            return false;
        }
    }
}