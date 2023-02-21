using System;
using System.Reflection;

namespace CatBehaviour.Runtime
{
    /// <summary>
    /// 随机序列节点（依次执行随机排序后的子节点，直到有子节点运行失败，则此节点运行失败，否则成功）
    /// </summary>
    [NodeInfo(Name = "随机序列",Desc = "依次执行随机排序后的子节点，直到有子节点运行失败，则此节点运行失败，否则成功",Icon = "Icon/Sequence")]
    public class RandomSequenceNode : SequenceNode
    {
        private static FieldInfo[] fieldInfos = typeof(RandomSequenceNode).GetFields(BindingFlags.Public | BindingFlags.Instance);

        /// <inheritdoc />
        public override FieldInfo[] FieldInfos => fieldInfos;
        
        private Random random;
        
        
        /// <summary>
        /// 随机数种子
        /// </summary>
        [BBParamInfo(Name = "随机数种子")]
        public BBParamInt Seed;
        
        
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
                
                //随机排序子节点
                int n = Children.Count;
                while (n > 1)
                {
                    int k =  random.Next(n--);
                    var temp = Children[n];
                    Children[n] = Children[k];
                    Children[k] = temp;
                }
            }
            
           
            
            base.OnStart();
        }
        
        
    }
}