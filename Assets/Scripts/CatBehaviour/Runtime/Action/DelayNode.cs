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
        public float DelayTime;

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
            if (timer >= DelayTime)
            {
                UpdateManager.RemoveUpdateTimer(OnUpdate);
                Finish(true);
            }
        }

#if UNITY_EDITOR
        
        // /// <inheritdoc />
        // public override void CreateGUI(VisualElement contentContainer)
        // {
        //     Debug.Log(this);
        //     FloatField floatField = new FloatField("延时时间")
        //     {
        //         value = DelayTime
        //     };
        //     contentContainer.Add(floatField);
        //
        //     floatField.RegisterValueChangedCallback((evt =>
        //     {
        //         DelayTime = evt.newValue;
        //     }));
        // }

        /// <inheritdoc />
        public override void OnGUI()
        {
            DelayTime = EditorGUILayout.FloatField("延时时间", DelayTime);
        }
#endif
    }
}