using System;
using UnityEngine;

namespace CatBehaviour
{
    /// <summary>
    /// 日志节点
    /// </summary>
    public class LogNode : BaseActionNode
    {
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
        public string Log;
        
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

        protected override void OnCancel()
        {

        }
    }
}