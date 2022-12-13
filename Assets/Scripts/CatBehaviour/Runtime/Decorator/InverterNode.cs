namespace CatBehaviour
{
    /// <summary>
    /// 反转节点（对子节点运行结果取反）
    /// </summary>
    public class InverterNode : BaseDecoratorNode
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
            Finish(!success);
        }
    }
}