using CatBehaviour.Editor;
using CatBehaviour.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace CatBehaviour.Editor
{
    /// <summary>
    /// 行为树节点图窗口
    /// </summary>
    public class BehaviourTreeWindow : EditorWindow
    {
        private BehaviourTree bt;
        
        /// <summary>
        /// 打开窗口
        /// </summary>
        [MenuItem("CatBehaviour/打开行为树节点图窗口")]
        public static void Open()
        {
            Open(null);
        }
            
        /// <summary>
        /// 打开窗口
        /// </summary>
        private static void Open(BehaviourTree bt)
        {
            var window = GetWindow<BehaviourTreeWindow>("行为树节点图窗口");
            window.bt = bt;
        }
        
        
        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/CatBehaviour/Editor/BehaviourTreeWindow.uxml");
            VisualElement labelFromUXML = visualTree.Instantiate();
            root.Add(labelFromUXML);

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/CatBehaviour/Editor/BehaviourTreeWindow.uss");
            root.styleSheets.Add(styleSheet);
            
            var graphView = rootVisualElement.Q<BehaviourTreeGraphView>("treeview");
            graphView.Init(this,bt);
        }
    }
}

