using System;
using System.Reflection;
namespace CatBehaviour.Runtime
{
    /// <summary>
    /// 延时节点
    /// </summary>
    [NodeInfo(Name = "延时节点",Icon = "Icon/Delay",Order = 1)]
    public class DelayNode : BaseActionNode
    {
        /// <inheritdoc />
        public override FieldInfo[] FieldInfos =>
            typeof(DelayNode).GetFields(BindingFlags.Public | BindingFlags.Instance);

        /// <summary>
        /// 延时的时间
        /// </summary>
        [BBParamInfo(Name = "延时时间")]
        public BBParamFloat DelayTime = new BBParamFloat();

        [NonSerialized]
        public float Timer;
        
        /// <inheritdoc />
        protected override void OnStart()
        {
            Timer = 0;
            UpdateManager.AddUpdateTimer(OnUpdate);
        }

        /// <inheritdoc />
        protected override void OnCancel()
        {
            UpdateManager.RemoveUpdateTimer(OnUpdate);
        }

        private void OnUpdate(float deltaTime)
        {
            Timer += deltaTime;
            if (Timer >= DelayTime.Value)
            {
                UpdateManager.RemoveUpdateTimer(OnUpdate);
                Finish(true);
            }
        }

    }
}