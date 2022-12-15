using System;

namespace CatBehaviour.Runtime
{
    /// <summary>
    /// 重复节点
    /// </summary>
    [NodeInfo(Name = "重复节点")]
    public class RepeaterNode : BaseDecoratorNode
    {
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
        /// 需要等待子节点运行的结果值
        /// </summary>
        public bool UntilResult;
        
        /// <summary>
        /// 重复次数
        /// </summary>
        public int repeatCount;
        
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
        protected override void OnCancel()
        {
            Child.Cancel();
        }

        /// <inheritdoc />
        protected override void OnChildFinished(BaseNode child, bool success)
        {
            switch (Mode)
            {
                case RepeatMode.RepeatTimes:
                    repeatCounter++;
                    if (repeatCounter >= repeatCount)
                    {
                        Finish(true);
                    }
                    else
                    {
                        Child.Start();
                    }
                    break;
                
                case RepeatMode.RepeatUntil:
                    if (success == UntilResult)
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