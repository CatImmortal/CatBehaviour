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
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        protected sealed override void OnChildFinished(BaseNode child, bool success)
        {
            throw new System.NotImplementedException();
        }
    }
}