using System.Reflection;

namespace CatBehaviour.Runtime
{
    /// <summary>
    /// 重复节点，根据重复模式重复运行子节点，然后返回成功
    /// </summary>
    [NodeInfo(Name = "重复",Desc = "根据重复模式重复运行子节点，然后返回成功",Icon = "Icon/Repeat")]
    public class RepeatNode : BaseDecoratorNode
    {
        private static FieldInfo[] fieldInfos = typeof(RepeatNode).GetFields(BindingFlags.Public | BindingFlags.Instance);

        /// <inheritdoc />
        public override FieldInfo[] FieldInfos => fieldInfos;
        
        /// <summary>
        /// 重复模式
        /// </summary>
        public enum RepeatMode
        {
            /// <summary>
            /// 重复到指定次数后，返回成功
            /// </summary>
            RepeatTimes,
            
            /// <summary>
            /// 重复到子节点返回指定结果时，返回成功
            /// </summary>
            RepeatUntil,
        }

        /// <summary>
        /// 重复模式
        /// </summary>
        public RepeatMode Mode;

        /// <summary>
        /// 重复次数
        /// </summary>
        [BBParamInfo(Name = "重复次数,-1表示无限次")]
        public BBParamInt repeatCount;
        
        /// <summary>
        /// 等待子节点运行的结果
        /// </summary>
        [BBParamInfo(Name = "等待子节点运行的结果")]
        public BBParamBool UntilResult;
        
        /// <summary>
        /// 重复计数器
        /// </summary>
        private int repeatCounter;
        
        /// <inheritdoc />
        protected override void OnStart()
        {
            repeatCounter = 0;
            Child.Start();
        }
        

        /// <inheritdoc />
        protected override void OnChildFinished(BaseNode child, bool success)
        {
            switch (Mode)
            {
                case RepeatMode.RepeatTimes:
                    repeatCounter++;
                    if (repeatCount.Value > 0 && repeatCounter >= repeatCount.Value)
                    {
                        Finish(true);
                    }
                    else
                    {
                        Child.Start();
                    }
                    break;
                
                case RepeatMode.RepeatUntil:
                    if (success == UntilResult.Value)
                    {
                        Finish(true);
                    }
                    else
                    {
                        Child.Start();
                    }
                    break;
            }
            
           
        }

    }
}