namespace CatBehaviour.Runtime
{
    /// <summary>
    /// 基于二进制的序列化器接口
    /// </summary>
    public interface IBinarySerializer
    {
        /// <summary>
        /// 序列化
        /// </summary>
        byte[] Serialize(BehaviourTree bt);
        
        /// <summary>
        /// 反序列化
        /// </summary>
        BehaviourTree Deserialize(byte[] bytes);
    }
}