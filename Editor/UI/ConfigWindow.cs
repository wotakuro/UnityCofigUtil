using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace UTJ.ConfigUtil
{
    public class ConfigWindow : EditorWindow
    {

        private List<Utility.TypeAndAttr> typelist = new List<Utility.TypeAndAttr>();
        private Utility.TypeAndAttr currentTypeInfo;

        private object currentValue;

        private PopupField<PresetData> presetPopup;
        private List<PresetData> presetList = new List<PresetData>();
        private PresetData currentPreset;
        


        [MenuItem("Tools/Config")]
        public static void Create()
        {
            EditorWindow.GetWindow<ConfigWindow>();
        }

        private void OnEnable()
        {
            this.typelist = Utility.GetTypeList();
            string treeAssetPath = "Packages/com.utj.uniconfigutil/Editor/UI/UXML/ConfigWindow.uxml";
            var treeAssset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(treeAssetPath);
            treeAssset.CloneTree(this.rootVisualElement);
            this.InitConfigType();


            rootVisualElement.Q<Button>("PresetBtn");
            rootVisualElement.Q<Button>("SaveBtn").clickable.clicked += ()=> {
            };
            rootVisualElement.Q<Button>("SaveAsPresetBtn");
        }
        

        private void InitConfigType()
        {
            if( this.typelist == null || this.typelist.Count <= 0) { return; }
            var configType = rootVisualElement.Q<VisualElement>("ConfigType");
            var popup = new PopupField<Utility.TypeAndAttr>(this.typelist, 0,
                TypeAndAttrToString, TypeAndAttrToString);

            currentTypeInfo = typelist[0];
            popup.RegisterValueChangedCallback((value) => {
                if(currentTypeInfo == value.newValue) { return; }
                bool res = false;
                if (!res)
                {
                    res = EditorUtility.DisplayDialog("Discard Change", "Discard any Chanages?", "ok", "cancel");
                }
                if (!res)
                {
                    popup.value = value.previousValue;
                }
                else
                {
                    this.ApplyToTypeInfo( value.newValue );
                }
            });
            configType.Add(popup);
            this.ApplyToTypeInfo(currentTypeInfo);
        }

        private void ApplyToTypeInfo(Utility.TypeAndAttr info)
        {
            this.currentTypeInfo = info;
            
            bool loadResult = ConfigLoader.LoadDataFromStreamingAssets(out currentValue,info.type);
            if (!loadResult)
            {
                this.currentValue = System.Activator.CreateInstance(info.type);
            }

            ReflectionUIGenerator generator = new ReflectionUIGenerator();

            var scrollView = rootVisualElement.Q<ScrollView>("ItemValue");
            scrollView.Clear();
            generator.Generate(currentValue, scrollView,0);

            UpdatePresetField();
        }

        private void UpdatePresetField()
        {
            var presetParent = rootVisualElement.Q<VisualElement>("Preset");
            presetParent.Clear();
            PresetData.LoadPresets(this.currentTypeInfo.type,this.presetList);

            if (this.presetList.Count > 0) {
                presetPopup = new PopupField<PresetData>(this.presetList, 0, PresetDataToString, PresetDataToString);
                currentPreset = this.presetList[0];
                presetParent.Add(presetPopup);
            }
        }
        

        private static string PresetDataToString(PresetData data)
        {
            return data.name;
        }

        private static string TypeAndAttrToString(Utility.TypeAndAttr data) {
            return data.attr.filename;
        }
    }


}
