using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_2019_1_OR_NEWER || UNITY_2019_OR_NEWER
using UnityEngine.UIElements;
using UnityEditor.UIElements;
#else
using UnityEngine.Experimental.UIElements;
using UnityEditor.Experimental.UIElements;
#endif

namespace UTJ.ConfigUtil
{
    public class ConfigWindow : EditorWindow
    {

        private List<Utility.TypeAndAttr> typelist = new List<Utility.TypeAndAttr>();
        private Utility.TypeAndAttr currentTypeInfo;

        private object currentValue;

        private PopupField<Utility.TypeAndAttr> configTypePopup;
        private PopupField<PresetData> presetPopup;
        private List<PresetData> presetList = new List<PresetData>();
        private PresetData currentPreset;
        private bool isDirty = false;

#if !UNITY_2019_1_OR_NEWER && !UNITY_2019_OR_NEWER
        private VisualElement rootVisualElement
        {
            get
            {
                return this.GetRootVisualContainer();
            }
        }

        private float lastHeight = -1.0f;
        private void SetupScrollViewHeight()
        {
            if (lastHeight == this.position.height)
            {
                return;
            }

            var scroll = rootVisualElement.Q<ScrollView>("ItemValue");
            scroll.style.height = this.position.height - 180.0f;
            lastHeight = this.position.height;
        }
        private void Update()
        {
            SetupScrollViewHeight();
        }
#endif



        [MenuItem("Tools/Config")]
        public static void Create()
        {
            EditorWindow.GetWindow<ConfigWindow>();
        }

        private void OnEnable()
        {
            this.typelist = Utility.GetTypeList(true);

#if UNITY_2019_1_OR_NEWER || UNITY_2019_OR_NEWER
            string treeAssetPath = "Packages/com.utj.uniconfigutil/Editor/UI/UXML/ConfigWindow.uxml";
            var treeAssset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(treeAssetPath);
            treeAssset.CloneTree(this.rootVisualElement);
#else
            string treeAssetPath = "Packages/com.utj.uniconfigutil/Editor/UI/UXML2018/ConfigWindow.uxml";
            lastHeight = -1.0f;
            var treeAssset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(treeAssetPath);
            this.rootVisualElement.Add(treeAssset.CloneTree(null));
#endif


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
            configTypePopup = new PopupField<Utility.TypeAndAttr>(this.typelist, 0,
                TypeAndAttrToString, TypeAndAttrToString);

            currentTypeInfo = typelist[0];
#if UNITY_2019_1_OR_NEWER || UNITY_2019_OR_NEWER
            configTypePopup.RegisterValueChangedCallback((val) => {
                OnChangedSelectConfig(val);
            });
#else
            configTypePopup.OnValueChanged((val) => {
                OnChangedSelectConfig(val);
            });
#endif
            configType.Add(configTypePopup);
            this.ApplyToTypeInfo(currentTypeInfo);
        }

        private void OnChangedSelectConfig(ChangeEvent<Utility.TypeAndAttr> val) {

            if (currentTypeInfo == val.newValue) { return; }
            bool res = !isDirty;
            if (!res)
            {
                res = EditorUtility.DisplayDialog("Discard Change", "Discard any Chanages?", "ok", "cancel");
            }
            if (!res)
            {
                this.configTypePopup.value = val.previousValue;
            }
            else
            {
                this.ApplyToTypeInfo(val.newValue);
            }
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

#if UNITY_2019_1_OR_NEWER || UNITY_2019_OR_NEWER
                presetPopup.RegisterValueChangedCallback((val) =>
                {
                    this.currentPreset = val.newValue;
                });
#else
                presetPopup.OnValueChanged((val) =>
                {
                    this.currentPreset = val.newValue;
                });
#endif

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
