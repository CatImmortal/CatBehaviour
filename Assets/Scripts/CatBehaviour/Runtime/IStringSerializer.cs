namespace CatBehaviour.Runtime
{
    /// <summary>
    /// 基于字符串的序列化器接口
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