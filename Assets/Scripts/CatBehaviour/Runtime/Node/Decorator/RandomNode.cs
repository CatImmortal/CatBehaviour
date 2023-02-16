using System;
using System.Reflection;

namespace CatBehaviour.Runtime
{
    /// <summary>
    /// 随机节点（会以指定的概率决定是否运行子节点，若概率命中，则返回子节点运行结果，否则返回失败）
    /// </summary>
    [NodeInfo(Name = "随机",Desc = "会以指定的概率决定是否运行子节点，若概率命中，则返回子节点运行结果，否则返回失败")]
    public class RandomNode : BaseDecoratorNode
    {
        private static FieldInfo[] fieldInfos = typeof(RandomNode).GetFields(BindingFlags.Public | BindingFlags.Instance);

        /// <inheritdoc />
        public override FieldInfo[] FieldInfos => fieldInfos;
        
        private Random random;
        
        /// <summary>
        /// 随机数种子
        /// </summary>
        [BBParamInfo(Name = "随机数种子")]
        public BBParamInt Seed;

        /// <summary>
        /// 概率
        /// </summary>
        [BBParamInfo(Name = "概率0-100")]
        public BBParamInt Probability;
        
        
        /// <inheritdoc />
        protected override void OnStart()
        {
            if (random == null)
            {
                if (Seed.Value != 0)
                {
                    random = new Random(Seed.Value);
                }
                else
                {
                    random = new Random();
                }
            }

            if (random.Next(0,100) <= Probability.Value)
            {
                base.OnStart();
            }
            else
            {
                Finish(false);
            }
        }
    }
}