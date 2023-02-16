using System;
using System.Reflection;

namespace CatBehaviour.Runtime
{
    /// <summary>
    /// 等待直到子节点运行结果为指定值的节点
    /// </summary>
    [NodeInfo(Name = "直到成功或失败",Desc = "等待直到子节点运行结果为指定值后，此节点运行结束")]
    public class UntilNode : BaseDecoratorNode
    {
        private static FieldInfo[] fieldInfos = typeof(UntilNode).GetFields(BindingFlags.Public | BindingFlags.Instance);

        /// <inheritdoc />
        public override FieldInfo[] FieldInfos => fieldInfos;

        /// <summary>
        /// 等待的子节点运行结果
        /// </summary>
        [BBParamInfo(Name = "等待的子节点运行结果")]
        public BBParamBool UntilResult;
        
        /// <summary>
        /// 此节点运行结果
        /// </summary>
        [BBParamInfo(Name = "此节点的运行结果")]
        public BBParamBool Result;

        private bool needStartChild;

        protected override void OnStart()
        {
            UpdateManager.AddUpdateTimer(OnUpdate);
            base.OnStart();
        }

        protected override void OnCancel()
        {
            base.OnCancel();
            UpdateManager.RemoveUpdateTimer(OnUpdate);
        }

        /// <inheritdoc />
        protected override void OnChildFinished(BaseNode child, bool success)
        {
            if (success == UntilResult.Value)
            {
                UpdateManager.RemoveUpdateTimer(OnUpdate);
                Finish(Result.Value);
            }
            else
            {
                //准备再次运行子节点
                needStartChild = true;
            }
        }

        private void OnUpdate(float dt)
        {
            if (needStartChild)
            {
                needStartChild = false;
                Child.Start();
            }
        }
    }
}