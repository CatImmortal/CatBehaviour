using System;

namespace CatBehaviour.Runtime
{
    /// <summary>
    /// 根节点
    /// </summary>
    [NodeInfo(Name = "根节点",Icon = "Icon/Root")]
    public class RootNode : BaseDecoratorNode
    {
        /// <summary>
        /// 运行结束回调
        /// </summary>
        public Action<bool> OnFinished;
        
        /// <inheritdoc />
        protected override void OnChildFinished(BaseNode child, bool success)
        {
            Finish(success);
            OnFinished?.Invoke(success);
        }
    }
}