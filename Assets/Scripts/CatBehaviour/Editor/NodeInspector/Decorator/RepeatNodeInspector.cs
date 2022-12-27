using CatBehaviour.Runtime;
using UnityEditor;

namespace CatBehaviour.Editor
{
    /// <summary>
    /// 重复节点属性面板Inspector
    /// </summary>
    [NodeInspector(NodeType = typeof(RepeatNode))]
    public class RepeatNodeInspector : BaseNodeInspector
    {
        /// <inheritdoc />
        public override void OnGUI()
        {
            base.OnGUI();
            EditorGUILayout.Space();

            RepeatNode repeatNode = (RepeatNode)Target;
            
            repeatNode.Mode = (RepeatNode.RepeatMode)EditorGUILayout.EnumPopup("重复模式",repeatNode.Mode );
            EditorGUILayout.LabelField("RepeatTimes：重复到指定次数后，返回成功");
            EditorGUILayout.LabelField("RepeatUntil：重复到子节点返回指定结果时，返回成功");
        }
    }
}