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
        private bool isDirty = false;
        


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


            rootVisualElement.Q<Button>("PresetBtn").clickable.clicked += PresetBtn;
            rootVisualElement.Q<Button>("SaveBtn").clickable.clicked += SaveBtn;
            rootVisualElement.Q<Button>("SaveAsPresetBtn").clickable.clicked += SaveAsPresetBtn;
        }


        private void SaveAsPresetBtn()
        {
            var field = this.rootVisualElement.Q<TextField>("SavePresetName");
            string name = field.value;
            
            Utility.SaveToPresetData(this.currentValue,name);
            this.UpdatePresetField();

            this.SelectPresetField(name);
        }

        private void SaveBtn()
        {
            Utility.SaveDataToStreamingAssets(this.currentValue);
            this.isDirty = false;
        }

        private void PresetBtn()
        {
            if(currentPreset == null)
            {
                return;
            }
            bool res = !isDirty;
            if (!res)
            {
                res = EditorUtility.DisplayDialog("Discard Change", "Discard any Chanages?", "ok", "cancel");
            }
            if(!res)
            {
                return;
            }
            string name = currentPreset.name;
            ApplyCurrentValue( this.currentPreset.data );
            SelectPresetField(name);
            this.isDirty = false;
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
                bool res = !isDirty;
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
            object obj = null;
            bool loadResult = ConfigLoader.LoadDataFromStreamingAssets(out obj, info.type);

            if (!loadResult)
            {
                obj = System.Activator.CreateInstance(info.type);
            }
            ApplyCurrentValue(obj);
        }
        private void ApplyCurrentValue(object val) {
            this.currentValue = val;
            ReflectionUIGenerator generator = new ReflectionUIGenerator(this.SetDataDirty);
            var scrollView = rootVisualElement.Q<ScrollView>("ItemValue");
            scrollView.Clear();
            generator.Generate(currentValue, scrollView,0);

            UpdatePresetField();
            this.isDirty = false;
        }

        private void SetDataDirty()
        {
            this.isDirty = true;
        }

        private void UpdatePresetField()
        {
            var presetParent = rootVisualElement.Q<VisualElement>("Preset");
            presetParent.Clear();
            PresetData.LoadPresets(this.currentTypeInfo.type,this.presetList);

            if (this.presetList.Count > 0) {
                presetPopup = new PopupField<PresetData>(this.presetList, 0, PresetDataToString, PresetDataToString);
                presetPopup.RegisterValueChangedCallback((val) =>
                {
                    this.currentPreset = val.newValue;
                });

                currentPreset = this.presetList[0];
                presetParent.Add(presetPopup);
            }
        }

        private void SelectPresetField(string name)
        {
            var presetParent = rootVisualElement.Q<VisualElement>("Preset");
            var presetField = presetParent.Q<PopupField<PresetData>>();
            if (presetField == null) { return; }
            for (int i = 0; i < this.presetList.Count; ++i) { 
                if ( presetList[i].name == name)
                {
                    presetField.index = i;
                    break;
                }
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
