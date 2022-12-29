﻿using System;
using System.Reflection;
using CatBehaviour.Runtime;
using UnityEditor;
using UnityEngine.UIElements;

namespace CatBehaviour.Editor
{
    /// <summary>
    /// 节点属性面板Inspector基类
    /// </summary>
    public class BaseNodeInspector
    {
        /// <summary>
        /// 要绘制的目标节点
        /// </summary>
        public BaseNode Target;
        
        /// <summary>
        /// 基于UIElements绘制节点属性面板
        /// </summary>
        public virtual void CreateGUI(VisualElement contentContainer)
        {

        }

        /// <summary>
        /// 基于IMGUI绘制节点属性面板
        /// </summary>
        public virtual void OnGUI()
        {
            var infoAttr = Target.GetType().GetCustomAttribute<NodeInfoAttribute>();
            if (!string.IsNullOrEmpty(infoAttr.Desc))
            {
                //节点描述信息
                EditorGUILayout.LabelField($"【{infoAttr.Desc}】");
            }
            
            EditorGUILayout.Space();
            
            Type type = typeof(BBParam);
            foreach (FieldInfo fieldInfo in Target.FieldInfos)
            {
                if (type.IsAssignableFrom(fieldInfo.FieldType))
                {
                    var nameAttr = fieldInfo.GetCustomAttribute<BBParamInfoAttribute>();
                    string name = null;
                    if (nameAttr != null)
                    {
                        name = nameAttr.Name;
                    }

                    if (!string.IsNullOrEmpty(name))
                    {
                        EditorGUILayout.LabelField($"{name}({BBParam.GetBBParamTypeName(fieldInfo.FieldType)})");
                    }

                    var bbParam = (BBParam)fieldInfo.GetValue(Target);
                    
                    bbParam.OnGUI(true,Target.Owner);
                    
                    EditorGUILayout.Space();
                }
            }
        }
    }
}