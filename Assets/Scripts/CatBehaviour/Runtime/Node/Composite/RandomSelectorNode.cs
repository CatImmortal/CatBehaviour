using System;
using System.Reflection;

namespace CatBehaviour.Runtime
{
    /// <summary>
    /// 随机选择节点（依次执行随机排序后的子节点，直到有子节点运行成功，则此节点运行成功，否则失败）
    /// </summary>
    [NodeInfo(Name = "随机选择",Desc = "依次执行随机排序后的子节点，直到有子节点运行成功，则此节点运行成功，否则失败",Icon = "Icon/Selector")]
    public class RandomSelectorNode : SelectorNode
    {
        private static FieldInfo[] fieldInfos = typeof(RandomSelectorNode).GetFields(BindingFlags.Public | BindingFlags.Instance);

        /// <inheritdoc />
        public override FieldInfo[] FieldInfos => fieldInfos;
        
        private Random random;
        
        /// <summary>
        /// 随机数种子
        /// </summary>
        [BBParamInfo(Name = "随机数种子")]
        public BBParamInt Seed = new BBParamInt();
        
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