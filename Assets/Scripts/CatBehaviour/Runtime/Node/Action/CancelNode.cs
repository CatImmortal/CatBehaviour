using System;
using System.Reflection;
using UnityEngine;

namespace CatBehaviour.Runtime
{
    /// <summary>
    /// 取消行为树节点
    /// </summary>
    [NodeInfo(Name = "内置/取消行为树",Icon = "Icon/Failure")]
    public class CancelNode : BaseActionNode
    {
        protected override void OnStart()
        {
            Owner.Cancel();
            Finish(true);
        }

        protected override void OnCancel()
        {
 
        }
    }
}