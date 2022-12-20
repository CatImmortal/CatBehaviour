using System;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CatBehaviour.Runtime
{
    [Serializable]
    public abstract class BBParam
    {
        /// <summary>
        /// 参数值obj
        /// </summary>
        public abstract object ValueObj { get; set; }

#if UNITY_EDITOR
        
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

    public class BBParamBool : BBParam<bool>
    {
#if UNITY_EDITOR
        public override void OnGUI()
        {
            Value = EditorGUILayout.Toggle(Value);
        }
#endif
    }
    
    public class BBParamInt : BBParam<int>
    {
#if UNITY_EDITOR
        public override void OnGUI()
        {
            Value = EditorGUILayout.IntField(Value);
        }
#endif
    }
    
    public class BBParamFloat : BBParam<float>
    {
#if UNITY_EDITOR
        public override void OnGUI()
        {
            Value = EditorGUILayout.FloatField(Value);
        }
#endif
    }
    
    public class BBParamString : BBParam<string>
    {
#if UNITY_EDITOR
        public override void OnGUI()
        {
            Value = EditorGUILayout.TextField(Value);
        }
#endif
    }
}