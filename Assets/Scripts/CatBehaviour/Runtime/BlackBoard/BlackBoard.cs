using System;
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
        public Rect Position;

#if UNITY_EDITOR
        /// <summary>
        /// 黑板参数类型 -> key列表
        /// </summary>
        private Dictionary<Type, List<string>> keyListDict = new Dictionary<Type, List<string>>();
#endif
        
        /// <summary>
        /// 获取参数
        /// </summary>
        public T GetParam<T>(string key) where T : BBParam
        {
            BBParam param = GetParam(key);
            if (param == null)
            {
                return default;
            }

            return ((T)param);
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
            if (param == null)
            {
                return;
            }
            
            param.Key = key;
            ParamDict[key] = param;

#if UNITY_EDITOR
            Type type = param.GetType();
            if (!keyListDict.TryGetValue(type,out var keyList))
            {
                keyList = new List<string> { "Null" };
                keyListDict.Add(type,keyList);
            }
            keyList.Add(key);
            keyList.Sort(KeyListSortFunc);
#endif
        }

        /// <summary>
        /// 移除参数
        /// </summary>
        public void RemoveParam(string key)
        {
#if UNITY_EDITOR
            if (ParamDict.TryGetValue(key, out var param))
            {
                Type type = param.GetType();
                if (keyListDict.TryGetValue(type,out var keyList))
                {
                   keyList.Remove(key);
                   keyList.Sort(KeyListSortFunc);
                }
            }
#endif
            ParamDict.Remove(key);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Key列表排序方法
        /// </summary>
        private int KeyListSortFunc(string x,string y)
        {
            //Null 固定排首位
            if (x == "Null")
            {
                return -1;
            }
            if (y == "Null")
            {
                return 1;
            }

            return x.CompareTo(y);
        }
        
        /// <summary>
        /// 根据黑板参数类型获取黑板key数组
        /// </summary>
        public string[] GetKeys(Type type)
        {
            if (!keyListDict.TryGetValue(type,out var keyList))
            {
                keyList = new List<string>() { "Null" };
            }
    
            return keyList.ToArray();
        }
#endif
        
    }
}