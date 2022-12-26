using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

namespace CatBehaviour.Runtime
{
    /// <summary>
    /// 延时节点
    /// </summary>
    [NodeInfo(Name = "延时节点",Icon = "Icon/Delay")]
    public class DelayNode : BaseActionNode
    {
        /// <inheritdoc />
        protected override FieldInfo[] FieldInfos =>
            typeof(DelayNode).GetFields(BindingFlags.Public | BindingFlags.Instance);

        /// <summary>
        /// 延时的时间
        /// </summary>
        [BBParamInfo(Name = "延时时间")]
        public BBParamFloat DelayTime = new BBParamFloat();

        private float timer;
        
        /// <inheritdoc />
        protected override void OnStart()
        {
            timer = 0;
            UpdateManager.AddUpdateTimer(OnUpdate);
        }

        /// <inheritdoc />
        protected override void OnCancel()
        {
            UpdateManager.RemoveUpdateTimer(OnUpdate);
        }

        private void OnUpdate(float deltaTime)
        {
            timer += deltaTime;
            if (timer >= DelayTime.Value)
            {
                UpdateManager.RemoveUpdateTimer(OnUpdate);
                Finish(true);
            }
        }

#if UNITY_EDITOR
        public override void OnGUI()
        {
            base.OnGUI();
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"当前计时：{timer:f2}");
        }
#endif
        
    }
}