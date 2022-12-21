using System;
using UnityEngine;
using UnityEngine.UIElements;

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
            None,
            
            /// <summary>
            /// 空闲
            /// </summary>
            Free,

            /// <summary>
            /// 运行中
            /// </summary>
            Running,
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
        /// 清空对父子节点的Id索引和引用
        /// </summary>
        public virtual void ClearIdAndReference()
        {
            ParentNodeId = 0;
            ParentNode = null;
        }
        
        /// <summary>
        /// 重建对父子节点的Id索引
        /// </summary>
        public virtual void RebuildId()
        {
            if (ParentNode != null)
            {
                ParentNodeId = ParentNode.Id;
            }
        }

        /// <summary>
        /// 重建对父子节点的引用
        /// </summary>
        public abstract void RebuildReference();

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
            
            OnCancel();
        }
        
        /// <summary>
        /// 结束运行节点
        /// </summary>
        protected virtual void Finish(bool success)
        {
            CurState = State.Free;
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
            
        }
        
#endif
    }
}