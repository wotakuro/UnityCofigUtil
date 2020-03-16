﻿using System.Collections.Generic;

using System.Reflection;

using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace UTJ.ConfigUtil
{
    public class ReflectionUIGenerator
    {
        object target;
        int level;
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
                var uiField = new LongField(name);
                RegistEvent(uiField, fieldInfo);
                visualElement.Add(uiField);
            }
            else if (type == typeof(int))
            {
                var uiField = new IntegerField(name);
                RegistEvent(uiField, fieldInfo);
                visualElement.Add(uiField);
            }
            else if (type == typeof(float))
            {
                var uiField = new FloatField(name);
                RegistEvent(uiField, fieldInfo);
                visualElement.Add(uiField);
            }
            else if (type == typeof(string))
            {
                var uiField = new TextField(name);
                RegistEvent(uiField, fieldInfo);
                visualElement.Add(uiField);
            }
            else if (type == typeof(Vector2))
            {
                var uiField = new Vector2Field(name);
                RegistEvent(uiField, fieldInfo);
                visualElement.Add(uiField);
            }
            else if (type == typeof(Vector3))
            {
                var uiField = new Vector3Field(name);
                RegistEvent(uiField, fieldInfo);
                visualElement.Add(uiField);
            }
            else if (type == typeof(Vector4))
            {
                var uiField = new Vector4Field(name);
                RegistEvent(uiField, fieldInfo);
                visualElement.Add(uiField);
            }
            else if (type == typeof(Color))
            {
                var uiField = new ColorField(name);
                RegistEvent(uiField, fieldInfo);
                visualElement.Add(uiField);
            }
            else if (type.IsEnum)
            {
                var uiField = new EnumField(name,(System.Enum) fieldInfo.GetValue(target) );
                RegistEvent(uiField, fieldInfo);
                visualElement.Add(uiField);
            }
            else
            {
            }
        }
        private void RegistEvent<T>(INotifyValueChanged<T> notify,FieldInfo fieldInfo)
        {
            notify.RegisterValueChangedCallback((val) =>
           {
               fieldInfo.SetValue(this.target, val.newValue);
           });
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