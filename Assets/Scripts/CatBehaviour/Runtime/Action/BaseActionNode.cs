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
        public sealed override void ClearChild()
        {
            
        }

        /// <inheritdoc />
        public sealed override void CollectChildToAllNodes(Action<BaseNode> action)
        {
               
        }

        /// <inheritdoc />
        public sealed override void RecordChildId()
        {
            
        }

        /// <inheritdoc />
        public sealed override void RebuildChildReference()
        {
            
        }

        /// <inheritdoc />
        protected sealed override void OnChildFinished(BaseNode child, bool success)
        {
            
        }
    }
}