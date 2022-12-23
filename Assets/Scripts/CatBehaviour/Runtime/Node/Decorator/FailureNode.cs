﻿namespace CatBehaviour.Runtime
{
    /// <summary>
    /// 失败节点（无论子节点运行结果是什么，此节点都运行失败）
    /// </summary>
    [NodeInfo(Name = "失败节点",Desc = "无论子节点运行结果是什么，此节点都运行失败")]
    public class FailureNode : BaseDecoratorNode
    {
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
            Finish(false);
        }
    }
}