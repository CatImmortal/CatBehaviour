using System;

namespace CatBehaviour.Runtime
{
    /// <summary>
    /// 节点信息的特性标签
    /// </summary>
    public class NodeInfoAttribute : Attribute
    {
        public string Name;
        public string Desc;
    }
}