using System;
using System.Reflection;
using UnityEngine;

namespace CatBehaviour.Runtime
{
    /// <summary>
    /// 重启行为树节点
    /// </summary>
    [NodeInfo(Name = "内置/重启行为树",Icon = "Icon/Root")]
    public class RestartNode : BaseActionNode
    {
        protected override void OnStart()
        {
            Owner.Restart();
        }

        protected override void OnCancel()
        {
 
        }
    }
}