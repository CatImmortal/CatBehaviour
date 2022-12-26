using System;
using System.Collections.Generic;

namespace CatBehaviour.Runtime
{
    /// <summary>
    /// 装饰节点基类
    /// </summary>
    public abstract class BaseDecoratorNode : BaseNode
    {
        /// <summary>
        /// 子节点ID
        /// </summary>
        public int ChildId;

        /// <summary>
        /// 子节点
        /// </summary>
        [NonSerialized] 
        public BaseNode Child;
        
        /// <inheritdoc />
        public override void AddChild(BaseNode node)
        {
            if (node == null)
            {
                return;
            }
            
            node.ParentNode = this;
            Child = node;
        }

        /// <inheritdoc />
        public override void RemoveChild(BaseNode node)
        {
            if (node == null)
            {
                return;
            }

            ChildId = 0;
            node.ParentNode = null;
            Child = null;
        }

        /// <inheritdoc />
        public override void ForeachChild(Action<BaseNode> action)
        {
            action?.Invoke(Child);
        }

        public override void ClearId()
        {
            base.ClearId();
            ChildId = 0;
        }

        /// <inheritdoc />
        public override void RebuildId()
        {
            base.RebuildId();
            
            if (Child != null)
            {
                ChildId = Child.Id;
            }
        }

        /// <inheritdoc />
        public override void ClearNodeReference()
        {
            base.ClearNodeReference();
            Child = null;
        }

        /// <inheritdoc />
        public override void RebuildNodeReference(List<BaseNode> allNodes)
        {
            if (ChildId == 0)
            {
                return;
            }
            BaseNode child = allNodes[ChildId - 1];
            AddChild(child);
        }
    }
}