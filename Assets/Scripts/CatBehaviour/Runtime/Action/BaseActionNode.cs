using System;

namespace CatBehaviour.Runtime
{
    /// <summary>
    /// 动作节点基类
    /// </summary>
    public abstract class BaseActionNode : BaseNode
    {

        
        /// <inheritdoc />
        public sealed override void AddChild(BaseNode node)
        {
            
        }
        
        /// <inheritdoc />
        public sealed override void RemoveChild(BaseNode node)
        {
            
        }

        /// <inheritdoc />
        public sealed override void ForeachChild(Action<BaseNode> action)
        {
               
        }
        
        /// <inheritdoc />
        public sealed override void ClearIdAndReference()
        {
            base.ClearIdAndReference();
        }

        /// <inheritdoc />
        public sealed override void RebuildId()
        {
            base.RebuildId();
        }

        /// <inheritdoc />
        public sealed override void RebuildReference()
        {

        }

        /// <inheritdoc />
        protected sealed override void OnChildFinished(BaseNode child, bool success)
        {
            
        }
    }
}