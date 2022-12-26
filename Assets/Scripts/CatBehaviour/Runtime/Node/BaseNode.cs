using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


#if UNITY_EDITOR
using UnityEngine.UIElements;
using UnityEditor;
#endif

namespace CatBehaviour.Runtime
{
    /// <summary>
    /// 节点基类
    /// </summary>
    [Serializable]
    public abstract class BaseNode
    {
        /// <summary>
        /// 节点状态
        /// </summary>
        public enum State
        {
            /// <summary>
            /// 空闲
            /// </summary>
            Free,

            /// <summary>
            /// 运行中
            /// </summary>
            Running,
            
            /// <summary>
            /// 运行成功
            /// </summary>
            Success,
            
            /// <summary>
            /// 运行失败
            /// </summary>
            Failed,
        }

        /// <summary>
        /// 节点Id，从1开始
        /// </summary>
        public int Id;

        /// <summary>
        /// 节点位置
        /// </summary>
        public Vector2 Position;
        
        /// <summary>
        /// 当前节点状态
        /// </summary>
        [NonSerialized]
        public State CurState = State.Free;

        /// <summary>
        /// 持有此节点的行为树
        /// </summary>
        [NonSerialized]
        public BehaviourTree Owner;

        /// <summary>
        /// 黑板
        /// </summary>
        public BlackBoard BlackBoard => Owner.BlackBoard;

        /// <summary>
        /// 父节点ID
        /// </summary>
        public int ParentNodeId;

        /// <summary>
        /// 父节点
        /// </summary>
        [NonSerialized]
        public BaseNode ParentNode;

        /// <summary>
        /// 字段列表，重载此属性才会自动绘制节点用到的的黑板参数
        /// </summary>
        protected virtual FieldInfo[] FieldInfos => Array.Empty<FieldInfo>();
        
        /// <summary>
        /// 添加子节点
        /// </summary>
        public abstract void AddChild(BaseNode node);

        /// <summary>
        /// 删除子节点
        /// </summary>
        public abstract void RemoveChild(BaseNode node);

        /// <summary>
        /// 遍历子节点
        /// </summary>
        public abstract void ForeachChild(Action<BaseNode> action);

        /// <summary>
        /// 排序子节点
        /// </summary>
        public virtual void SortChild()
        {
            
        }



        /// <summary>
        /// 清空对自身Id和父子节点Id的记录
        /// </summary>
        public virtual void ClearId()
        {
            Id = 0;
            ParentNodeId = 0;
        }
        
        /// <summary>
        /// 重建对父子节点的Id记录
        /// </summary>
        public virtual void RebuildId()
        {
            if (ParentNode != null)
            {
                ParentNodeId = ParentNode.Id;
            }
        }

        /// <summary>
        /// 清空对父子节点的引用
        /// </summary>
        public virtual void ClearNodeReference()
        {
            ParentNode = null;
        }

        /// <summary>
        /// 重建对父子节点的引用
        /// </summary>
        public abstract void RebuildNodeReference(List<BaseNode> allNodes);
        
        /// <summary>
        /// 重建对黑板参数的引用
        /// </summary>
        public void RebuildBBParamReference()
        {
            var bbParamType = typeof(BBParam);
            foreach (var fieldInfo in FieldInfos)
            {
                if (bbParamType.IsAssignableFrom(fieldInfo.FieldType) )
                {
                    var bbParam = (BBParam)fieldInfo.GetValue(this);

                    if (string.IsNullOrEmpty(bbParam.Key))
                    {
                        //节点里的黑板参数key为空 跳过
                        continue;
                    }

                    BBParam newBBParam = BlackBoard.GetParam(bbParam.Key);
                    if (newBBParam == null)
                    {
                        Debug.LogError($"{GetType().Name}的黑板参数key:{bbParam.Key}不存于黑板中，请检查");
                        continue;
                    }
                    
                    //替换节点中对黑板参数的引用为黑板中的黑板参数
                    fieldInfo.SetValue(this,newBBParam);
                }
            }
        }
        
        /// <summary>
        /// 开始运行节点
        /// </summary>
        public void Start()
        {
            if (CurState == State.Running)
            {
                return;
            }
            
            CurState = State.Running;
            OnStart();
        }

        /// <summary>
        /// 取消运行节点
        /// </summary>
        public void Cancel()
        {
            if (CurState != State.Running)
            {
                return;
            }

            CurState = State.Free;
            OnCancel();
        }
        
        /// <summary>
        /// 结束运行节点
        /// </summary>
        protected virtual void Finish(bool success)
        {
            CurState = success ? State.Success : State.Failed;

            ParentNode?.ChildFinished(this, success);
        }

        /// <summary>
        /// 子节点运行结束
        /// </summary>
        protected void ChildFinished(BaseNode child,bool success)
        {
            OnChildFinished(child,success);   
        }

        /// <summary>
        /// 开始运行节点时调用
        /// </summary>
        protected abstract void OnStart();
        
        /// <summary>
        /// 取消运行节点时调用
        /// </summary>
        protected abstract void OnCancel();
        
        /// <summary>
        /// 子节点运行结束时
        /// </summary>
        protected abstract void OnChildFinished(BaseNode child,bool success);

        public override string ToString()
        {
            return GetType().Name;
        }

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
            var infoAttr = GetType().GetCustomAttribute<NodeInfoAttribute>();
            if (!string.IsNullOrEmpty(infoAttr.Desc))
            {
                //节点描述信息
                EditorGUILayout.LabelField($"【{infoAttr.Desc}】");
            }
            
            EditorGUILayout.Space();
            
            Type type = typeof(BBParam);
            foreach (FieldInfo fieldInfo in FieldInfos)
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

                    var bbParam = (BBParam)fieldInfo.GetValue(this);
                    
                    bbParam.OnGUI(true,Owner);
                    
                    EditorGUILayout.Space();
                }
            }
        }

#endif
    }
}