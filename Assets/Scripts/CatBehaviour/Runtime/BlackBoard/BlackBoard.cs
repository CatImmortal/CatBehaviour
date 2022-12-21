using System.Collections.Generic;
using UnityEngine;

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
        public Dictionary<string, BBParam> ParamDict = new Dictionary<string, BBParam>();

        /// <summary>
        /// 位置与大小
        /// </summary>
        public Rect Position = new Rect(10, 30, 350, 300);
        
        /// <summary>
        /// 获取参数
        /// </summary>
        public BBParam<T> GetParam<T>(string key)
        {
            var param = GetParam(key);
            if (param == null)
            {
                return null;
            }

            return (BBParam<T>)param;
        }

        /// <summary>
        /// 获取参数
        /// </summary>
        public BBParam GetParam(string key)
        {
            ParamDict.TryGetValue(key, out var param);
            return param;
        }
        
        /// <summary>
        /// 设置参数
        /// </summary>
        public void SetParam(string key, BBParam param)
        {
            param.Key = key;
            ParamDict[key] = param;
        }

        /// <summary>
        /// 移除参数
        /// </summary>
        public void RemoveParam(string key)
        {
            ParamDict.Remove(key);
        }
    }
}