namespace CatBehaviour.Runtime
{
    /// <summary>
    /// 基于字符串的序列化器接口
    /// 实现此接口的序列号器需要能处理多态对象，不需要能处理循环引用
    /// </summary>
    public interface IStringSerializer
    {
        /// <summary>
        /// 序列化
        /// </summary>
        string Serialize(BehaviourTree bt);
        
        /// <summary>
        /// 反序列化
        /// </summary>
        BehaviourTree Deserialize(string str);
    }
}