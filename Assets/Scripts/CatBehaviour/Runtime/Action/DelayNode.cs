using UnityEngine.UIElements;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

namespace CatBehaviour.Runtime
{
    /// <summary>
    /// 延时节点
    /// </summary>
    [NodeInfo(Name = "延时节点")]
    public class DelayNode : BaseActionNode
    {
        /// <summary>
        /// 延时的时间
        /// </summary>
        public BBParamFloat DelayTime = new BBParamFloat();

        private float timer;
        
        /// <inheritdoc />
        protected override void OnStart()
        {
            timer = 0;
            Debug.Log($"delayTime:{DelayTime}");
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
        
        /// <inheritdoc />
        public override void OnGUI()
        {
            EditorGUILayout.LabelField("延时时间");
            using (new EditorGUILayout.HorizontalScope())
            {
                DelayTime.OnGUI(true);
            }
        }
#endif
    }
}