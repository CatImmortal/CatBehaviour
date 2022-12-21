using System;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CatBehaviour.Runtime
{
    /// <summary>
    /// 黑板参数
    /// </summary>
    [Serializable]
    public abstract class BBParam
    {
        /// <summary>
        /// 参数key
        /// </summary>
        public string Key;
        
        /// <summary>
        /// 参数值obj
        /// </summary>
        public abstract object ValueObj { get; set; }

#if UNITY_EDITOR
        
        /// <summary>
        /// 基于UIElements绘制黑板值
        /// </summary>
        public virtual void CreateGUI(VisualElement contentContainer,bool isInspector)
        {

        }

        /// <summary>
        /// 基于IMGUI绘制黑板值
        /// </summary>
        public virtual void OnGUI(bool isInspector)
        {
            if (isInspector)
            {
                Key = EditorGUILayout.TextField("黑板Key",Key);
            }
        }

#endif
    }
    
    /// <summary>
    /// 黑板参数
    /// </summary>
    public abstract class BBParam<T> : BBParam
    {
        /// <summary>
        /// 参数值
        /// </summary>
        public T Value;

        /// <summary>
        /// 参数值obj
        /// </summary>
        public override object ValueObj
        {
            get => Value;
            set => Value = value == null ? Value = default : (T)value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        
    }
    
    [Serializable]
    public class BBParamBool : BBParam<bool>
    {
#if UNITY_EDITOR
        public override void OnGUI(bool isInspector)
        {
            base.OnGUI(isInspector);
            Value = EditorGUILayout.Toggle("Value",Value);
        }
#endif
    }
    
    [Serializable]
    public class BBParamInt : BBParam<int>
    {
#if UNITY_EDITOR
        public override void OnGUI(bool isInspector)
        {
            base.OnGUI(isInspector);
            Value = EditorGUILayout.IntField("Value",Value);
        }
#endif
    }
    
    [Serializable]
    public class BBParamFloat : BBParam<float>
    {
#if UNITY_EDITOR
        public override void OnGUI(bool isInspector)
        {
            base.OnGUI(isInspector);
            Value = EditorGUILayout.FloatField("Value",Value);
        }
#endif
    }
    
    [Serializable]
    public class BBParamString : BBParam<string>
    {
#if UNITY_EDITOR
        public override void OnGUI(bool isInspector)
        {
            base.OnGUI(isInspector);
            Value = EditorGUILayout.TextField("Value",Value);
        }
#endif
    }
}