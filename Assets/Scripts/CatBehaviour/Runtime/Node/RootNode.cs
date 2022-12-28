﻿using System;

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

        protected override void OnStart()
        {
#if UNITY_EDITOR
            BTDebugger.Add(Owner);   
#endif
            base.OnStart();
        }

        /// <inheritdoc />
        protected override void OnCancel()
        {
            base.OnCancel();
            BTDebugger.Remove(Owner);   
        }

        /// <inheritdoc />
        protected override void OnChildFinished(BaseNode child, bool success)
        {
            Finish(success);
#if UNITY_EDITOR
            BTDebugger.Remove(Owner);
#endif
            OnFinished?.Invoke(success);
        }
    }
}