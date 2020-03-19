﻿using System.Collections.Generic;

using System.Reflection;

using UnityEngine;


#if UNITY_2019_1_OR_NEWER || UNITY_2019_OR_NEWER
using UnityEngine.UIElements;
using UnityEditor.UIElements;
#else
using UnityEngine.Experimental.UIElements;
using UnityEditor.Experimental.UIElements;
#endif

namespace UTJ.ConfigUtil
{
    public class ReflectionUIGenerator
    {
        private System.Action onDirty;
        private object target;
        private int level;

        public ReflectionUIGenerator(System.Action dirtyFunc)
        {
            this.onDirty = dirtyFunc;
        }

        public void Generate(object t,VisualElement visualElement,int l)
        {
            if( this.level >= 6) { return; }
            this.level = l;
            this.target = t;
            var fields = GetFields();

            foreach( var field in fields)
            {
                ExecuteFieldInfo(field, visualElement);
            }
        }

        private void ExecuteFieldInfo(FieldInfo fieldInfo,VisualElement visualElement)
        {
            var type = fieldInfo.FieldType;
            string name = fieldInfo.Name;

            if( type == typeof(long))
            {
                var uiField = CreateFieldWithName<LongField>(name, visualElement);
                RegistEvent(uiField, fieldInfo);
                SetValue(uiField, fieldInfo);
            }
            else if (type == typeof(int))
            {
                var uiField = CreateFieldWithName<IntegerField>(name, visualElement);
                RegistEvent(uiField, fieldInfo);
                SetValue(uiField, fieldInfo);
            }
            else if (type == typeof(float))
            {
                var uiField = CreateFieldWithName<FloatField>(name, visualElement);
                RegistEvent(uiField, fieldInfo);
                SetValue(uiField, fieldInfo);
            }
            else if (type == typeof(string))
            {
                var uiField = CreateFieldWithName<TextField>(name, visualElement);
                RegistEvent(uiField, fieldInfo);
                SetValue(uiField, fieldInfo);
            }
            else if (type == typeof(Vector2))
            {
                var uiField = CreateFieldWithName<Vector2Field>(name, visualElement);
                RegistEvent(uiField, fieldInfo);
                SetValue(uiField, fieldInfo);
            }
            else if (type == typeof(Vector3))
            {
                var uiField = CreateFieldWithName<Vector3Field>(name, visualElement);

                RegistEvent(uiField, fieldInfo);
                SetValue(uiField, fieldInfo);
            }
            else if (type == typeof(Vector4))
            {
                var uiField = CreateFieldWithName<Vector4Field>(name, visualElement);
                RegistEvent(uiField, fieldInfo);
                SetValue(uiField, fieldInfo);
            }
            else if (type == typeof(Color))
            {
                var uiField = CreateFieldWithName<ColorField>(name, visualElement);
                RegistEvent(uiField, fieldInfo);
                SetValue(uiField, fieldInfo);
            }
            else if (type.IsEnum)
            {
                var uiField = CreateFieldWithName<EnumField>(name, visualElement);
                uiField.Init((System.Enum)fieldInfo.GetValue(target) );
                RegistEvent(uiField, fieldInfo);
            }
            else if (type.IsArray)
            {
                Debug.LogError("not yet implements array " + type +" " +fieldInfo.Name);
            }
            else if (typeof(System.Collections.IList).IsAssignableFrom(type ) )
            {
                Debug.LogError("not yet implements List  " + type + " " + fieldInfo.Name);
            }
            else if (!type.IsValueType)
            {
                Foldout foldout = new Foldout();
                foldout.text = fieldInfo.Name;
                var generator = new ReflectionUIGenerator(this.onDirty);
                generator.Generate( fieldInfo.GetValue(this.target), foldout, this.level + 1);
                visualElement.Add(foldout);
            }
        }


#if UNITY_2019_1_OR_NEWER || UNITY_2019_OR_NEWER
#else
        private static T CreateFieldWithName<T>(string name,VisualElement parent)
            where T: BindableElement
        {
            BindableElement val = null;
            System.Type t = typeof(T);
            if (t == typeof(LongField))
            {
                val = new LongField(
#if UNITY_2019_1_OR_NEWER || UNITY_2019_OR_NEWER
                    name
#endif
                    );
            }
            else if (t == typeof(IntegerField))
            {
                val = new IntegerField(
#if UNITY_2019_1_OR_NEWER || UNITY_2019_OR_NEWER
                    name
#endif
                    );
            }
            else if (t == typeof(FloatField))
            {
                val = new FloatField(
#if UNITY_2019_1_OR_NEWER || UNITY_2019_OR_NEWER
                    name
#endif
                    );
            }
            else if (t == typeof(TextField))
            {
                val = new TextField(
#if UNITY_2019_1_OR_NEWER || UNITY_2019_OR_NEWER
                    name
#endif
                    );
            }
            else if (t == typeof(ColorField))
            {
                val = new ColorField(
#if UNITY_2019_1_OR_NEWER || UNITY_2019_OR_NEWER
                    name
#endif
                    );
            }
            else if (t == typeof(EnumField))
            {
                val = new EnumField(
#if UNITY_2019_1_OR_NEWER || UNITY_2019_OR_NEWER
                    name
#endif
                    );
            }
            else if (t == typeof(Vector2Field))
            {
                val = new Vector2Field(
#if UNITY_2019_1_OR_NEWER || UNITY_2019_OR_NEWER
                    name
#endif
                    );
            }
            else if (t == typeof(Vector3Field))
            {
                val = new Vector3Field(
#if UNITY_2019_1_OR_NEWER || UNITY_2019_OR_NEWER
                    name
#endif
                    );
            }
            else if (t == typeof(Vector4Field))
            {
                val = new Vector4Field(
#if UNITY_2019_1_OR_NEWER || UNITY_2019_OR_NEWER
                    name
#endif
                    );
            }

            return val as T;
        }
#endif


        private void SetValue<T>( BaseField<T> uiField,FieldInfo fieldInfo)
        {
            uiField.value = (T)fieldInfo.GetValue(this.target);
        }

        private void RegistEvent<T>(INotifyValueChanged<T> notify,FieldInfo fieldInfo)
        {
#if UNITY_2019_1_OR_NEWER || UNITY_2019_OR_NEWER
            notify.RegisterValueChangedCallback((val) =>
           {
               fieldInfo.SetValue(this.target, val.newValue);
               this.onDirty?.Invoke();
           });
#else
            notify.OnValueChanged((val) =>
            {
                fieldInfo.SetValue(this.target, val.newValue);
                this.onDirty?.Invoke();
            });
#endif
        }

        public List<FieldInfo> GetFields()
        {
            var t = this.target.GetType();
            var publicFields = t.GetFields(BindingFlags.Instance | BindingFlags.Public);
            var nonPublicFields = t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

            List<FieldInfo> fieldInfos = new List<FieldInfo>(publicFields.Length);
            foreach( var field in publicFields)
            {
                fieldInfos.Add(field);
            }
            foreach( var field in nonPublicFields)
            {
                if(!field.IsNotSerialized)
                {
                    fieldInfos.Add(field);
                }
            }
            return fieldInfos;
        }
        

    }
}