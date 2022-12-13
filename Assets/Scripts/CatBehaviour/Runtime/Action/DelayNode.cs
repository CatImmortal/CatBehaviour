namespace CatBehaviour
{
    /// <summary>
    /// 延时节点
    /// </summary>
    public class DelayNode : BaseActionNode
    {
        /// <summary>
        /// 延时的时间
        /// </summary>
        public float DelayTime;

        private float timer;
        
        /// <inheritdoc />
        protected override void OnStart()
        {
            timer = 0;
            UpdateManager.AddUpdateTimer(OnUpdate);
        }

        /// <inheritdoc />
        protected override void OnCancel()
        {
            UpdateManager.RemoveUpdateTimer(OnUpdate);
        }

        private void OnUpdate(float deltaTime)
        {
            timer += deltaTime;
            if (timer >= DelayTime)
            {
                UpdateManager.RemoveUpdateTimer(OnUpdate);
                Finish(true);
            }
        }
    }
}