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
    [NodeInfo(Name = "日志节点",Icon = "Icon/Log")]
    public class LogNode : BaseActionNode
    {
        /// <inheritdoc />
        protected override FieldInfo[] FieldInfos =>
            typeof(LogNode).GetFields(BindingFlags.Public | BindingFlags.Instance);
        
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
        public BBParamString Log = new BBParamString();
        
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
        
#if UNITY_EDITOR
        
        /// <inheritdoc />
        public override void OnGUI()
        {
            Level = (LogLevel)EditorGUILayout.EnumPopup("日志级别", Level);
            EditorGUILayout.Space();
            
            base.OnGUI();
        }
#endif
    }
}