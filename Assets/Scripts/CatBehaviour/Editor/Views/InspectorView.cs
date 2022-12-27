using System;
using System.Collections.Generic;
using System.Reflection;
using CatBehaviour.Runtime;
using UnityEditor;
using UnityEngine.UIElements;

namespace CatBehaviour.Editor
{
    /// <summary>
    /// 节点检查器面板
    /// </summary>
    public class InspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { }

        /// <summary>
        /// 默认的节点属性面板Inspector
        /// </summary>
        private static BaseNodeInspector defaultNodeInspector = new BaseNodeInspector();

        /// <summary>
        /// 节点类型 ->节点属性面板Inspector
        /// </summary>
        private static Dictionary<Type, BaseNodeInspector>
            nodeInspectorDict = new Dictionary<Type, BaseNodeInspector>();

        public InspectorView()
        {
            nodeInspectorDict.Clear();
            var types = TypeCache.GetTypesWithAttribute<NodeInspectorAttribute>();
            foreach (Type type in types)
            {
                var attr = type.GetCustomAttribute<NodeInspectorAttribute>();
                var nodeInspector = (BaseNodeInspector)Activator.CreateInstance(type);
                nodeInspectorDict.Add(attr.NodeType,nodeInspector);
            }
        }
        
        /// <summary>
        /// 绘制节点检查器面板
        /// </summary>
        public void DrawInspector(BehaviourTreeNode node)
        {
            Clear();

            //获取节点对应的属性面板Inspector
            BaseNodeInspector nodeInspector = defaultNodeInspector;
            var nodeType = node.RuntimeNode.GetType();
            if (nodeInspectorDict.ContainsKey(nodeType))
            {
                nodeInspector = nodeInspectorDict[nodeType];
            }
            nodeInspector.Target = node.RuntimeNode;
            
            //绘制节点属性面板
            nodeInspector.CreateGUI(contentContainer);
            IMGUIContainer imguiContainer = new IMGUIContainer(){};
            imguiContainer.onGUIHandler = nodeInspector.OnGUI;
            Add(imguiContainer);
            
            
           
        }
    }
}