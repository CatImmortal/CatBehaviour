using System;

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
        public int ChildId { get; set; }

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
        public override void ClearChild()
        {
            RemoveChild(Child);
        }
        
        /// <inheritdoc />
        public override void ForeachChild(Action<BaseNode> action)
        {
            action?.Invoke(Child);
        }
        
        /// <inheritdoc />
        public override void RecordChildId()
        {
            if (Child != null)
            {
                ChildId = Child.Id;
            }
        }

        /// <inheritdoc />
        public override void RebuildChildReference()
        {
            Child = Owner.GetNode(ChildId);
        }
    }
}