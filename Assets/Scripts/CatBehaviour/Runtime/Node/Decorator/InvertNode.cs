﻿namespace CatBehaviour.Runtime
{
    /// <summary>
    /// 反转节点（对子节点运行结果取反）
    /// </summary>
    [NodeInfo(Name = "反转节点",Desc = "对子节点运行结果取反",Icon = "Icon/Invert",Order = 3)]
    public class InvertNode : BaseDecoratorNode
    {
        /// <inheritdoc />
        protected override void OnChildFinished(BaseNode child, bool success)
        {
            Finish(!success);
        }
    }
}