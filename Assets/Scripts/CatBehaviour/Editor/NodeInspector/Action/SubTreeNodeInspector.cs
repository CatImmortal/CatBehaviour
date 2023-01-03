using CatBehaviour.Runtime;
using UnityEditor;

namespace CatBehaviour.Editor
{
    /// <summary>
    /// 子树节点属性面板Inspector
    /// </summary>
    [NodeInspector(NodeType = typeof(SubTreeNode))]
    public class SubTreeNodeInspector : BaseNodeInspector
    {
        /// <inheritdoc />
        public override void OnGUI()
        {
            base.OnGUI();

            SubTreeNode subTreeNode = (SubTreeNode)Target;
            subTreeNode.IsInheritBlackBoard = EditorGUILayout.Toggle("是否继承黑板数据", subTreeNode.IsInheritBlackBoard);
            EditorGUILayout.Space();
        }
    }
}