using System;
using CatBehaviour.Editor;

namespace CatBehaviour.Runtime
{
    /// <summary>
    /// 根节点
    /// </summary>
    [NodeInfo(Name = "根节点")]
    public class RootNode : BaseDecoratorNode
    {
        /// <summary>
        /// 运行结束回调
        /// </summary>
        public Action OnFinish;
        
        /// <inheritdoc />
        protected override void OnStart()
        {
            Child.Start();
        }

        /// <inheritdoc />
        protected override void OnCancel()
        {
            Child.Cancel();
        }

        /// <inheritdoc />
        protected override void OnChildFinished(BaseNode child, bool success)
        {
            Finish(true);
            OnFinish?.Invoke();
        }
    }
}