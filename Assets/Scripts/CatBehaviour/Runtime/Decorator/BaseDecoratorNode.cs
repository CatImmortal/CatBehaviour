namespace CatBehaviour.Runtime
{
    /// <summary>
    /// 装饰节点基类
    /// </summary>
    public abstract class BaseDecoratorNode : BaseNode
    {
        /// <summary>
        /// 子节点
        /// </summary>
        public BaseNode Child;
        
        /// <inheritdoc />
        public override void AddChild(BaseNode node)
        {
            node.ParenNode = this;
            Child = node;
        }
    }
}