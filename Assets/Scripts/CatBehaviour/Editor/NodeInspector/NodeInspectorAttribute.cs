using System;

namespace CatBehaviour.Editor
{
    /// <summary>
    /// 节点属性面板Inspector特性
    /// </summary>
    public class NodeInspectorAttribute : Attribute
    {
        /// <summary>
        /// 目标节点类型
        /// </summary>
        public Type NodeType;
    }
}