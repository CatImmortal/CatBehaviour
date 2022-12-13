using System.Collections.Generic;

namespace CatBehaviour
{
    /// <summary>
    /// 节点基类
    /// </summary>
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
        /// 当前节点状态
        /// </summary>
        public State CurState { get; protected set; } = State.Free;
        
        /// <summary>
        /// 父节点
        /// </summary>
        public BaseNode ParenNode { get; set; }
        
        /// <summary>
        /// 添加子节点
        /// </summary>
        public abstract void AddChild(BaseNode node);

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
            ParenNode?.ChildFinished(this, success);
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
        
        
    }
}