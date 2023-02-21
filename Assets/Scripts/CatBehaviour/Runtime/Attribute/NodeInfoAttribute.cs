using System;

namespace CatBehaviour.Runtime
{
    /// <summary>
    /// 节点信息特性
    /// </summary>
    public class NodeInfoAttribute : Attribute
    {
        public string Name;
        public string Desc;
        public string Icon;
    }
}