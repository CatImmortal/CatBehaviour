using System;
using System.Collections.Generic;
using System.Reflection;
using CatBehaviour.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CatBehaviour.Editor
{
    /// <summary>
    /// 黑板参数Drawer基类
    /// </summary>
    public abstract class BaseBBParamDrawer
    {
        /// <summary>
        /// 黑板参数类型 -> 黑板参数Drawer
        /// </summary>
        public static Dictionary<Type, BaseBBParamDrawer>
            BBParamDrawerDict = new Dictionary<Type, BaseBBParamDrawer>();
        
        /// <summary>
        /// 要绘制的目标黑板参数
        /// </summary>
        public BBParam Target;
        
        private int selectedKeyIndex = 0;

        static BaseBBParamDrawer()
        {
            BBParamDrawerDict.Clear();
            var types = TypeCache.GetTypesWithAttribute<BBParamDrawerAttribute>();
            foreach (Type type in types)
            {
                var attr = type.GetCustomAttribute<BBParamDrawerAttribute>();
                var bbParamDrawer = (BaseBBParamDrawer)Activator.CreateInstance(type);
                BBParamDrawerDict.Add(attr.BBParamType,bbParamDrawer);
            }
        }
        
        /// <summary>
        /// 基于UIElements绘制黑板值
        /// </summary>
        public virtual void CreateGUI(VisualElement contentContainer,bool isInspector,BehaviourTree bt)
        {

        }

        /// <summary>
        /// 基于UIElements绘制黑板值
        /// </summary>
        protected virtual void CreateGUI(VisualElement contentContainer)
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
                    var keys = bt.BlackBoard.GetKeys(Target.GetType());

                    //修正索引 防止因为删除了前面的key导致顺序变化 最终索引到了错误的key
                    for (int i = 0; i < keys.Length; i++)
                    {
                        if (Target.Key == keys[i] && selectedKeyIndex != i)
                        {
                            selectedKeyIndex = i;
                            break;
                        }
                    }
                    
                    int newIndex = EditorGUILayout.Popup("黑板Key" ,selectedKeyIndex, keys);
                    if (newIndex == 0 || newIndex >= keys.Length)
                    {
                        selectedKeyIndex = 0;
                        Target.Key = null;
                    }
                    else
                    {
                        if (selectedKeyIndex > 0 && Target.Key != keys[selectedKeyIndex])
                        {
                            //当前索引到的key和保存的key不一致 重置为null
                            //这时可能是之前的key被删了，或者被重命名了
                            selectedKeyIndex = 0;
                            Target.Key = null;
                        }
                        else
                        {
                            var newKey = keys[newIndex];
                            selectedKeyIndex = newIndex;
                            Target.Key = newKey;
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

        protected void RecordBBParam()
        {
            BehaviourTreeWindow window = (BehaviourTreeWindow)EditorWindow.focusedWindow;
            window.RecordObject($"Change BBParam {Target.Key}");
        }
    }

    public abstract class BaseBBParamDrawer<T> : BaseBBParamDrawer
    {

        protected override void OnGUI()
        {
            BBParam<T> param = (BBParam<T>) Target;
            var newValue = DrawValue(param.Value);
            if (IsDirty(param.Value,newValue))
            {
                RecordBBParam();
                param.Value = newValue;
            }
        }

        protected abstract T DrawValue(T value);
        protected abstract bool IsDirty(T value, T newValue);
    }
    
    [BBParamDrawer(BBParamType = typeof(BBParamBool))]
    public class BBParamBoolDrawer : BaseBBParamDrawer<bool>
    {
        protected override bool DrawValue(bool value)
        {
            var newValue = EditorGUILayout.Toggle("Value",value);
            return newValue;
        }

        protected override bool IsDirty(bool value, bool newValue)
        {
            return value != newValue;
        }
    }
    
    [BBParamDrawer(BBParamType = typeof(BBParamInt))]
    public class BBParamIntDrawer : BaseBBParamDrawer<int>
    {
        protected override int DrawValue(int value)
        {
            var newValue = EditorGUILayout.IntField("Value",value);
            return newValue;
        }

        protected override bool IsDirty(int value, int newValue)
        {
            return value != newValue;
        }
    }
    
    [BBParamDrawer(BBParamType = typeof(BBParamFloat))]
    public class BBParamFloatDrawer : BaseBBParamDrawer <float>
    {
        protected override float DrawValue(float value)
        {
            var newValue = EditorGUILayout.FloatField("Value",value);
            return newValue;
        }

        protected override bool IsDirty(float value, float newValue)
        {
            return Math.Abs(value - newValue) > float.Epsilon;
        }
    }
    
    [BBParamDrawer(BBParamType = typeof(BBParamString))]
    public class BBParamStringDrawer : BaseBBParamDrawer<string>
    {
        protected override string DrawValue(string value)
        {
            var newValue = EditorGUILayout.TextField("Value",value);
            return newValue;
        }

        protected override bool IsDirty(string value, string newValue)
        {
            return value != newValue;
        }
    }
    
    [BBParamDrawer(BBParamType = typeof(BBParamVector2))]
    public class BBParamVector2Drawer : BaseBBParamDrawer<Vector2>
    {
        protected override Vector2 DrawValue(Vector2 value)
        {
            var newValue = EditorGUILayout.Vector2Field("Value",value);
            return newValue;
        }

        protected override bool IsDirty(Vector2 value, Vector2 newValue)
        {
            return value != newValue;
        }
    }
    
    [BBParamDrawer(BBParamType = typeof(BBParamVector3))]
    public class BBParamVector3Drawer : BaseBBParamDrawer<Vector3>
    {

        protected override Vector3 DrawValue(Vector3 value)
        {
            var newValue = EditorGUILayout.Vector3Field("Value",value);
            return newValue;
        }

        protected override bool IsDirty(Vector3 value, Vector3 newValue)
        {
            return value != newValue;
        }
    }
    
    [BBParamDrawer(BBParamType = typeof(BBParamVector4))]
    public class BBParamVector4Drawer : BaseBBParamDrawer<Vector4>
    {
        protected override Vector4 DrawValue(Vector4 value)
        {
            var newValue = EditorGUILayout.Vector4Field("Value",value);
            return newValue;
        }

        protected override bool IsDirty(Vector4 value, Vector4 newValue)
        {
            return value != newValue;
        }
    }
}