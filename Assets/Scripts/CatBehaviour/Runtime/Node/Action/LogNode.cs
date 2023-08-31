using System;
using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CatBehaviour.Runtime
{
    /// <summary>
    /// 日志节点
    /// </summary>
    [NodeInfo(Name = "内置/日志",Icon = "Icon/Log")]
    public class LogNode : BaseActionNode
    {
        private static FieldInfo[] fieldInfos = typeof(LogNode).GetFields(BindingFlags.Public | BindingFlags.Instance);

        /// <inheritdoc />
        public override FieldInfo[] FieldInfos => fieldInfos;
        
        /// <summary>
        /// 日志级别
        /// </summary>
        public enum LogLevel
        {
            Info,
            Warning,
            Error
        }

        /// <summary>
        /// 日志级别
        /// </summary>
        public LogLevel Level;
        
        /// <summary>
        /// 日志内容
        /// </summary>
        [BBParamInfo(Name = "日志信息")]
        public BBParamString Log;
        
        /// <inheritdoc />
        protected override void OnStart()
        {
            switch (Level)
            {
                case LogLevel.Info:
                    Debug.Log(Log);
                    break;
                
                case LogLevel.Warning:
                    Debug.LogWarning(Log);
                    break;
                
                case LogLevel.Error:
                    Debug.LogError(Log);
                    break;
            }
            
            Finish(true);
        }

        /// <inheritdoc />
        protected override void OnCancel()
        {

        }
    }
}