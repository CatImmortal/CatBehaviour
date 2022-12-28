using System;
using System.Collections.Generic;
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
        private DropdownField dropdownBTSO;
        private ObjectField ObjBTSO;
        private Button btnSave;
        private Button btnNew;
        
        private SplitView splitView;
        private InspectorView inspector;
        private BehaviourTreeGraphView graphView;

        private string assetPath;
        private BehaviourTreeSO originBTSO;
        private BehaviourTree bt;
        
        /// <summary>
        /// 是否为调试模式
        /// </summary>
        public bool IsDebugMode { get; private set; }

        [MenuItem("CatBehaviour/打开行为树窗口")]
        public static void Open()
        {
            OpenFromAssetPath(null);
        }
        
        [OnOpenAsset(1)]
        public static bool OnOpenAssets(int id, int line)
        {
            if (EditorUtility.InstanceIDToObject(id) is BehaviourTreeSO btSO)
            {
                string assetPath = AssetDatabase.GetAssetPath(btSO);
                OpenFromAssetPath(assetPath);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 通过行为树SO路径打开窗口
        /// </summary>
        private static void OpenFromAssetPath(string assetPath)
        {
            BehaviourTreeWindow window = CreateWindow<BehaviourTreeWindow>("行为树窗口");
            window.Init(false);
            window.RefreshFromAssetPath(assetPath);
        }

        /// <summary>
        /// 通过调试器打开窗口
        /// </summary>
        public static void OpenFromDebugger(BehaviourTree debugBT)
        {
            BehaviourTreeWindow window = CreateWindow<BehaviourTreeWindow>("行为树窗口");
            window.Init(true);
            window.RefreshFromDebugger(debugBT);
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            
            var visualTree = Resources.Load<VisualTreeAsset>("UXML/BehaviourTreeWindow");
            visualTree.CloneTree(root);
            
            var styleSheet = Resources.Load<StyleSheet>("USS/BehaviourTreeWindow");
            root.styleSheets.Add(styleSheet);
            
            dropdownBTSO = rootVisualElement.Q<DropdownField>("dropdownBTSO");
            dropdownBTSO.RegisterValueChangedCallback((evt =>
            {
                if (string.IsNullOrEmpty(evt.newValue))
                {
                    return;
                }
                
                string path = evt.newValue.Replace('\\', '/');
                Debug.Log(path);
                if (!IsDebugMode)
                {
                    RefreshFromAssetPath(path);
                }
                else
                {
                    var debugBT = BTDebugger.BTInstanceDict[path];
                    RefreshFromDebugger(debugBT);
                }
               
            }));
            
            ObjBTSO = rootVisualElement.Q<ObjectField>("ObjBTSO");
            
            btnSave = root.Q<Button>("btnSave");
            btnSave.clicked += Save;
            
            btnNew = root.Q<Button>("btnNew");
            btnNew.clicked += New;
            
            splitView = root.Q<SplitView>();
            inspector = root.Q<InspectorView>("inspector");
            graphView = rootVisualElement.Q<BehaviourTreeGraphView>("graphView");
            graphView.Init(this);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init(bool isDebugMode)
        {
            IsDebugMode = isDebugMode;
            RefreshDropDown(isDebugMode);

            if (isDebugMode)
            {
                //调试状态下 删掉SO引用 保存和新建按钮
                ObjBTSO.RemoveFromHierarchy();
            }
        }
        
        /// <summary>
        /// 刷新下拉菜单
        /// </summary>
        private void RefreshDropDown(bool isDebugMode)
        {
            List<string> paths;
            if (!isDebugMode)
            {
                paths = AssetDatabase.FindAssets($"t:{nameof(BehaviourTreeSO)}")
                    .Select(((s, i) => AssetDatabase.GUIDToAssetPath(s).Replace('/','\\'))).ToList();
            }
            else
            {
                paths = BTDebugger.BTInstanceDict.Keys.ToList();
            }
            dropdownBTSO.choices = paths.ToList();
        }
        
        /// <summary>
        /// 使用行为树SO路径刷新
        /// </summary>
        private void RefreshFromAssetPath(string assetPath)
        {
            this.assetPath = assetPath;
            BehaviourTree bt = null;
            if (!string.IsNullOrEmpty(assetPath))
            {

                originBTSO = AssetDatabase.LoadAssetAtPath<BehaviourTreeSO>(assetPath);
                if (originBTSO != null)
                {
                    //深拷贝一份用于编辑
                    bt = originBTSO.CloneBehaviourTree();
                }

                int index = dropdownBTSO.choices.IndexOf(assetPath.Replace('/','\\'));
                dropdownBTSO.index = index;
                ObjBTSO.value = originBTSO;
            }

            Refresh(bt);
        }

        /// <summary>
        /// 使用调试用行为树对象刷新
        /// </summary>
        private void RefreshFromDebugger(BehaviourTree debugBT)
        {
            int index = dropdownBTSO.choices.IndexOf(debugBT.DebugName.Replace('/','\\'));
            dropdownBTSO.index = index;
            
            Refresh(debugBT);
        }
        
        /// <summary>
        /// 刷新
        /// </summary>
        private void Refresh(BehaviourTree bt)
        {
            //若传入的行为树为空则创建个默认的
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
            this.bt = bt;

            //设置节点属性面板的宽度
            if (bt.InspectorWidth != 0)
            {
                splitView.fixedPaneInitialDimension = bt.InspectorWidth;
            }
            else
            {
                splitView.fixedPaneInitialDimension = 420;
            }
            
            graphView.Refresh(bt);
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

            RefreshDropDown(false);
            RefreshFromAssetPath(assetPath);
            
            
            Debug.Log($"保存行为树成功，路径:{assetPath}");
        }

        /// <summary>
        /// 新建行为树
        /// </summary>
        private void New()
        {
            assetPath = null;
            originBTSO = null;
            bt = null;
            dropdownBTSO.index = -1;
            ObjBTSO.value = null;
            RefreshFromAssetPath(null);
        }
    }
}

