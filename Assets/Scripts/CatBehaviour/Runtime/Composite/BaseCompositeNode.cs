using System;
using System.Collections.Generic;

namespace CatBehaviour.Runtime
{
    /// <summary>
    /// 复合节点基类
    /// </summary>
    public abstract class BaseCompositeNode : BaseNode
    {
        /// <summary>
        /// 子节点ID列表
        /// </summary>
        public List<int> ChildIdList { get; set; } = new List<int>();
        
        /// <summary>
        /// 子节点列表
        /// </summary>
        [NonSerialized] 
        public List<BaseNode> Children  = new List<BaseNode>();

        /// <inheritdoc />
        public override void AddChild(BaseNode node)
        {
            if (node == null)
            {
                return;
            }
            
            node.ParentNode = this;
            node.Owner = Owner;
            Children.Add(node);
        }

        /// <inheritdoc />
        public override void RemoveChild(BaseNode node)
        {
            if (node == null)
            {
                return;
            }
            
            node.ParentNode = null;
            Children.Remove(node);
        }

        /// <inheritdoc />
        public override void ForeachChild(Action<BaseNode> action)
        {
            foreach (var child in Children)
            {
                action?.Invoke(child);
            }
        }
        
        /// <inheritdoc />
        public override void ClearIdAndReference()
        {
            ChildIdList.Clear();
            foreach (var child in Children)
            {
                child.ParentNode = null;
            }
            Children.Clear();
        }
        
        /// <inheritdoc />
        public override void RebuildId()
        {
            base.RebuildId();
            
            ChildIdList.Clear();
            foreach (var child in Children)
            {
                ChildIdList.Add(child.Id);
            }
        }

        /// <inheritdoc />
        public override void RebuildReference()
        {
            foreach (int childId in ChildIdList)
            {
                BaseNode child = Owner.GetNode(childId);
                AddChild(child);
            }
        }

        /// <inheritdoc />
        public override void SortChild()
        {
            Children.Sort((a, b) =>
            {
                return a.Position.x.CompareTo(b.Position.x);
            });
        }
    }
}