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

        private int selectedKeyIndex = 0;
        
        /// <summary>
        /// 基于UIElements绘制黑板值
        /// </summary>
        public virtual void CreateGUI(VisualElement contentContainer,bool isInspector,BehaviourTree bt)
        {

        }

        /// <summary>
        /// 基于IMGUI绘制黑板值
        /// </summary>
        public virtual void OnGUI(bool isInspector,BehaviourTree bt)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (isInspector)
                {
                    var keys = bt.BlackBoard.GetKeys(GetType());

                    //修正索引 防止因为删除了前面的key导致顺序变化 最终索引到了错误的key
                    for (int i = 0; i < keys.Length; i++)
                    {
                        if (Key == keys[i] && selectedKeyIndex != i)
                        {
                            selectedKeyIndex = i;
                            break;
                        }
                    }
                    
                    int newIndex = EditorGUILayout.Popup("黑板Key" ,selectedKeyIndex, keys);
                    if (newIndex == 0 || newIndex >= keys.Length)
                    {
                        selectedKeyIndex = 0;
                        Key = null;
                    }
                    else
                    {
                        if (selectedKeyIndex > 0 && Key != keys[selectedKeyIndex])
                        {
                            //当前索引到的key和保存的key不一致 重置为null
                            //这时可能是之前的key被删了，或者被重命名了
                            selectedKeyIndex = 0;
                            Key = null;
                        }
                        else
                        {
                            var newKey = keys[newIndex];
                            selectedKeyIndex = newIndex;
                            Key = newKey;
                        }
                    }
                }
                OnGUI();
            }
        }

        /// <summary>
        /// 基于IMGUI绘制黑板值
        /// </summary>
        protected virtual void OnGUI()
        {
            
        }

#endif
    }
    
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
        protected override void OnGUI()
        {
            Value = EditorGUILayout.Toggle("Value",Value);
        }
#endif
    }
    
    [Serializable]
    public class BBParamInt : BBParam<int>
    {
#if UNITY_EDITOR
        protected override void OnGUI()
        {
            Value = EditorGUILayout.IntField("Value",Value);
        }
#endif
    }
    
    [Serializable]
    public class BBParamFloat : BBParam<float>
    {
#if UNITY_EDITOR
        protected override void OnGUI()
        {
            Value = EditorGUILayout.FloatField("Value",Value);
        }
#endif
    }
    
    [Serializable]
    public class BBParamString : BBParam<string>
    {
#if UNITY_EDITOR
        protected override void OnGUI()
        {
            Value = EditorGUILayout.TextField("Value",Value);
        }
#endif
    }
}