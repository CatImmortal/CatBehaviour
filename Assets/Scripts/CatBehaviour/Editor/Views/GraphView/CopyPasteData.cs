using System;
using System.Collections.Generic;
using CatBehaviour.Runtime;

namespace CatBehaviour.Editor
{
    /// <summary>
    /// 复制粘贴辅助器
    /// </summary>
    [Serializable]
    public class CopyPasteData
    {
        /// <summary>
        /// 被复制的节点列表
        /// </summary>
        public List<JsonElement> CopiedNodes = new List<JsonElement>();
    }
}