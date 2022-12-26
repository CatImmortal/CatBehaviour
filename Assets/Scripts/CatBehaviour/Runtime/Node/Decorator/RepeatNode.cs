using System;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace CatBehaviour.Runtime
{
    /// <summary>
    /// 重复节点，根据重复模式重复运行子节点，然后返回成功
    /// </summary>
    [NodeInfo(Name = "重复节点",Desc = "根据重复模式重复运行子节点，然后返回成功",Icon = "Icon/Repeat")]
    public class RepeatNode : BaseDecoratorNode
    {
        /// <inheritdoc />
        protected override FieldInfo[] FieldInfos =>
            typeof(RepeatNode).GetFields(BindingFlags.Public | BindingFlags.Instance);
        
        /// <summary>
        /// 重复模式
        /// </summary>
        public enum RepeatMode
        {
            /// <summary>
            /// 重复到指定次数后，返回成功
            /// </summary>
            RepeatTimes,
            
            /// <summary>
            /// 重复到子节点返回指定结果时，返回成功
            /// </summary>
            RepeatUntil,
        }

        /// <summary>
        /// 重复模式
        /// </summary>
        public RepeatMode Mode;

        /// <summary>
        /// 重复次数
        /// </summary>
        [BBParamInfo(Name = "重复次数")]
        public BBParamInt repeatCount = new BBParamInt();
        
        /// <summary>
        /// 等待子节点运行的结果
        /// </summary>
        [BBParamInfo(Name = "等待子节点运行的结果")]
        public BBParamBool UntilResult = new BBParamBool();
        
        /// <summary>
        /// 重复计数器
        /// </summary>
        private int repeatCounter;
        
        /// <inheritdoc />
        protected override void OnStart()
        {
            repeatCounter = 0;
            Child.Start();
        }

        /// <inheritdoc />
        protected override void OnCancel()
        {
            Child.Cancel();
        }

        /// <inheritdoc />
        protected override void OnChildFinished(BaseNode child, bool success)
        {
            switch (Mode)
            {
                case RepeatMode.RepeatTimes:
                    repeatCounter++;
                    if (repeatCounter >= repeatCount.Value)
                    {
                        Finish(true);
                    }
                    else
                    {
                        Child.Start();
                    }
                    break;
                
                case RepeatMode.RepeatUntil:
                    if (success == UntilResult.Value)
                    {
                        Finish(true);
                    }
                    else
                    {
                        Child.Start();
                    }
                    break;
            }
            
           
        }

#if UNITY_EDITOR
        public override void OnGUI()
        {
            base.OnGUI();
            EditorGUILayout.Space();
            
            Mode = (RepeatMode)EditorGUILayout.EnumPopup("重复模式",Mode );
            EditorGUILayout.LabelField("RepeatTimes：重复到指定次数后，返回成功");
            EditorGUILayout.LabelField("RepeatUntil：重复到子节点返回指定结果时，返回成功");
        }
#endif
       
    }
}