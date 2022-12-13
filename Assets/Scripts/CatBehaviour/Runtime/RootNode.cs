namespace CatBehaviour
{
    /// <summary>
    /// 根节点
    /// </summary>
    public class RootNode : BaseDecoratorNode
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

        }
    }
}