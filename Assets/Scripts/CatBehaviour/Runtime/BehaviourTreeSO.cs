using UnityEngine;

namespace CatBehaviour.Runtime
{
    /// <summary>
    /// 行为树SO
    /// </summary>
    public class BehaviourTreeSO : ScriptableObject
    {
        /// <summary>
        /// 行为树序列化后的字符串数据
        /// </summary>
        public string StringData;
        
        /// <summary>
        /// 行为树序列化后的二进制数据
        /// </summary>
        public byte[] BinaryData;
    }
}