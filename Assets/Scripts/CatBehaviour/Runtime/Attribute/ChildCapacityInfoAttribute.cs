using System;

namespace CatBehaviour.Runtime
{
    /// <summary>
    /// 子节点容量
    /// </summary>
    public enum ChildCapacity
    {
        /// <summary>
        /// 无
        /// </summary>
        None,
        
        /// <summary>
        /// 一个
        /// </summary>
        Single,
        
        /// <summary>
        /// 多个
        /// </summary>
        Multi,
    }
    
    [AttributeUsage(AttributeTargets.Class)]
    public class ChildCapacityInfoAttribute : Attribute
    {
        public ChildCapacity Capacity;
    }
}