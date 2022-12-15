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
            var window = CreateWindow<BehaviourTreeWindow>("行为树节点图窗口");
    
            var graphView = new BehaviourTreeGraphView(window)
            {
                style = { flexGrow = 1 }
            };
                
            window.rootVisualElement.Add(graphView);
            window.ShowPopup();
                
            graphView.Init(bt);
                
            window.rootVisualElement.Add(new Button(graphView.Save) { text = "保存" });
        }
        
        public void CreateGUI()
        {

            VisualElement root = rootVisualElement;
            
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/CatBehaviour/Editor/BehaviourTreeWindow.uxml");
            VisualElement labelFromUXML = visualTree.Instantiate();
            root.Add(labelFromUXML);

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/CatBehaviour/Editor/BehaviourTreeWindow.uss");
            
            root.styleSheets.Add(styleSheet);
        }
    }
}

