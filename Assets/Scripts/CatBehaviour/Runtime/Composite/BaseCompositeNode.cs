using System.Collections.Generic;

namespace CatBehaviour
{
    /// <summary>
    /// 复合节点基类
    /// </summary>
    public abstract class BaseCompositeNode : BaseNode
    {
        /// <summary>
        /// 子节点列表
        /// </summary>
        protected List<BaseNode> Children = new List<BaseNode>();

        /// <inheritdoc />
        public override void AddChild(BaseNode node)
        {
            node.ParenNode = this;
            Children.Add(node);
        }
    }
}