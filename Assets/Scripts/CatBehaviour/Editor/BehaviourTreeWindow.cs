using System;
using System.IO;
using System.Linq;
using CatBehaviour.Editor;
using CatBehaviour.Runtime;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace CatBehaviour.Editor
{
    /// <summary>
    /// 行为树茶壶功能卡
    /// </summary>
    public class BehaviourTreeWindow : EditorWindow
    {
        private string assetPath;
        private BehaviourTreeSO originBTSO;
        private BehaviourTree bt;
        
        private Label labelAssetPath;
        private SplitView splitView;
        private InspectorView inspector;
        private BehaviourTreeGraphView graphView;


        [MenuItem("CatBehaviour/打开行为树窗口")]
        public static void Open()
        {
            Open(null);
        }
        
        [OnOpenAsset(1)]
        public static bool OnOpenAssets(int id, int line)
        {
            if (EditorUtility.InstanceIDToObject(id) is BehaviourTreeSO btSO)
            {
                string assetPath = AssetDatabase.GetAssetPath(btSO);
                Open(assetPath);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 打开行为树窗口
        /// </summary>
        private static void Open(string assetPath)
        {
            BehaviourTreeWindow window = CreateWindow<BehaviourTreeWindow>("行为树窗口");
            window.Init(assetPath);
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            
            var visualTree = Resources.Load<VisualTreeAsset>("UXML/BehaviourTreeWindow");
            visualTree.CloneTree(root);
            
            var styleSheet = Resources.Load<StyleSheet>("USS/BehaviourTreeWindow");
            root.styleSheets.Add(styleSheet);

            labelAssetPath = rootVisualElement.Q<Label>("labelAssetPath");
            
            var btnSave = root.Q<Button>("btnSave");
            btnSave.clicked += Save;

            splitView = root.Q<SplitView>();
            inspector = root.Q<InspectorView>("inspector");
            graphView = rootVisualElement.Q<BehaviourTreeGraphView>("graphView");
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init(string assetPath)
        {
            this.assetPath = assetPath;
            
            if (!string.IsNullOrEmpty(assetPath))
            {
                labelAssetPath.text = $"文件名:{assetPath}";
                
                originBTSO = AssetDatabase.LoadAssetAtPath<BehaviourTreeSO>(assetPath);
                if (originBTSO != null)
                {
                    //深拷贝一份用于编辑
                    bt = originBTSO.CloneBehaviourTree();
                }
            }
            else
            {
               labelAssetPath.text = $"文件名:newFile";
            }

            //若反序列化得到的行为树为空则创建个默认的
            if (bt == null)
            {
                bt = new BehaviourTree();
                bt.RootNode = new RootNode
                {
                    Position = new Vector2(graphView.contentContainer.layout.position.x + 500, graphView.contentContainer.layout.position.y + 100),
                    Owner = bt
                };
                bt.AllNodes.Add(bt.RootNode);
            }

            //设置节点属性面板的宽度
            if (bt.InspectorWidth != 0)
            {
                splitView.fixedPaneInitialDimension = bt.InspectorWidth;
            }
            else
            {
                splitView.fixedPaneInitialDimension = 420;
            }

            graphView.Init(this, bt);
        }

        /// <summary>
        /// 节点被点击时的回调
        /// </summary>
        public void OnNodeClick(BehaviourTreeNode node)
        {
            inspector.DrawInspector(node);
        }
        
        /// <summary>
        /// 保存行为树
        /// </summary>
        private void Save()
        {
            if (originBTSO == null)
            {
                var assetPath = EditorUtility.SaveFilePanelInProject("选择保存位置", "BehaviourTree","asset", "aaa");
                if (string.IsNullOrEmpty(assetPath))
                {
                    Debug.Log("取消保存");
                    return;
                }

                this.assetPath = assetPath;
                originBTSO = CreateInstance<BehaviourTreeSO>();
                AssetDatabase.CreateAsset(originBTSO,assetPath);
            }
            else
            {
                if (!EditorUtility.DisplayDialog("提示","确认保存？","是","否"))
                {
                    return;
                }
            }
            
            //开始收集节点
            bt.AllNodes.Clear();
            
            //清空ID和父子关系
            foreach (Node element in graphView.nodes)
            {
                BehaviourTreeNode node = (BehaviourTreeNode) element;
                node.RuntimeNode.ClearId();
                node.RuntimeNode.ClearNodeReference();
                
                //记录位置
                node.RuntimeNode.Position = node.GetPosition().position;
                
                //添加到allNodes里 建立ID
                bt.AllNodes.Add(node.RuntimeNode);
                node.RuntimeNode.Id = bt.AllNodes.Count;
            }
            
            //根据节点图连线重建父子关系
            foreach (Node element in graphView.nodes)
            {
                BehaviourTreeNode node = (BehaviourTreeNode) element;
                if (node.inputContainer.childCount == 0)
                {
                    continue;
                }
                
                Port inputPort = (Port)node.inputContainer[0];
                Edge inputEdge = inputPort.connections.FirstOrDefault();
                if (inputEdge == null)
                {
                    continue;
                }
                
                //将自身添加的父节点的子节点里
                var parent = (BehaviourTreeNode)inputEdge.output.node;
                parent.RuntimeNode.AddChild(node.RuntimeNode);
            }

            //记录属性面板宽度
            bt.InspectorWidth = splitView.Q("left").layout.width;
            
            //记录节点图位置
            bt.Rect = new Rect(graphView.viewTransform.position, graphView.viewTransform.scale);
            
            //记录黑板位置
            bt.BlackBoard.Position = graphView.BlackboardView.GetPosition();

            //将编辑好的行为树赋值给原始SO
            originBTSO.BT = bt;

            //保存原始SO
            EditorUtility.SetDirty(originBTSO);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            labelAssetPath.text = $"文件名:{assetPath}";
            
            Debug.Log($"保存行为树成功，路径:{assetPath}");
        }


    }
}

