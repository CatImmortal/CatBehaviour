using UnityEngine.UIElements;

namespace CatBehaviour.Editor
{
    /// <summary>
    /// 节点检查器面板
    /// </summary>
    public class InspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { }

        public InspectorView()
        {

        }
        
        /// <summary>
        /// 绘制节点检查器面板
        /// </summary>
        public void DrawInspector(BehaviourTreeNode node)
        {
            Clear();
            node.RuntimeNode.CreateGUI(contentContainer);
            
            IMGUIContainer imguiContainer = new IMGUIContainer(){};
            imguiContainer.onGUIHandler = node.RuntimeNode.OnGUI;
            Add(imguiContainer);
        }
    }
}