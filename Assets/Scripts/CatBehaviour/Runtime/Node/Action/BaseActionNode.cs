using System;
using System.Collections.Generic;

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
        public sealed override void ClearChild()
        {
           
        }

        /// <inheritdoc />
        public sealed override void ForeachChild(Action<BaseNode> action)
        {
               
        }

        /// <inheritdoc />
        public sealed override void ClearId()
        {
            base.ClearId();
        }

        /// <inheritdoc />
        public sealed override void RebuildId()
        {
            base.RebuildId();
        }

        /// <inheritdoc />
        public sealed override void ClearNodeReference()
        {
            base.ClearNodeReference();
        }

        /// <inheritdoc />
        public sealed override void RebuildNodeReference(List<BaseNode> allNodes)
        {

        }

        /// <inheritdoc />
        protected sealed override void OnChildFinished(BaseNode child, bool success)
        {
            
        }
    }
}