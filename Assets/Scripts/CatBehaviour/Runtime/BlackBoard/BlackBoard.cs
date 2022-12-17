using System.Collections.Generic;

namespace CatBehaviour.Runtime
{
    /// <summary>
    /// 黑板
    /// </summary>
    public class BlackBoard
    {
        /// <summary>
        /// 参数字典
        /// </summary>
        private Dictionary<string, IBBParam> paramDict = new Dictionary<string, IBBParam>();

        /// <summary>
        /// 获取参数
        /// </summary>
        public BBParam<T> GetParam<T>(string key)
        {
            paramDict.TryGetValue(key, out var param);
            return (BBParam<T>)param;
        }

        /// <summary>
        /// 设置参数
        /// </summary>
        public void SetParam<T>(string key, BBParam<T> param)
        {
            paramDict[key] = param;
        }
        
        /// <summary>
        /// 设置参数
        /// </summary>
        public void SetParam<T>(string key, T param)
        {
            paramDict[key] = new BBParam<T>(){Param = param};
        }
    }
}