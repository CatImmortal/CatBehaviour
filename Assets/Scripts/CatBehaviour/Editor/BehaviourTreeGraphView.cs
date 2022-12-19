using System.Collections.Generic;
using CatBehaviour.Runtime;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace CatBehaviour.Editor
{
    /// <summary>
    /// 行为树节点图
    /// </summary>
    public class BehaviourTreeGraphView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<BehaviourTreeGraphView, UxmlTraits> { }
        
        private BehaviourTree bt;
        private BehaviourTreeWindow window;
        public Blackboard Blackboard;
        
        public BehaviourTreeGraphView()
        {
            Insert(0, new GridBackground());  //格子背景
            
            //添加背景网格样式
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/CatBehaviour/Editor/BehaviourTreeWindow.uss");
            styleSheets.Add(styleSheet);

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);  //可缩放
            this.AddManipulator(new SelectionDragger());  //节点可拖动
            this.AddManipulator(new ContentDragger());  //节点图可移动
            this.AddManipulator(new RectangleSelector());  //可框选多个节点
            
            //graphViewChanged += OnGraphViewChanged;
            

        }


        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(BehaviourTreeWindow window, BehaviourTree bt)
        {
            this.window = window;
            this.bt = bt;
            
            //节点创建时的搜索窗口
            var searchWindowProvider = ScriptableObject.CreateInstance<NodeSearchWindowProvider>();
            searchWindowProvider.Init(window,this);
        
            nodeCreationRequest += context =>
            {
                //打开搜索窗口
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindowProvider);
            };
            
            CreateMiniMap();
            CreateBlackBoard();
            BuildGraphView();
        }

        /// <summary>
        /// 创建小地图
        /// </summary>
        private void CreateMiniMap()
        {
            // var miniMap = new MiniMap ();
            // var cords = contentViewContainer.WorldToLocal(new Vector2(window.position.width - 10, 30));
            // miniMap.SetPosition(new Rect(cords.x, cords.y, 200, 140));
            // Add(miniMap);
        }
        
        /// <summary>
        /// 创建黑板
        /// </summary>
        private void CreateBlackBoard()
        {
            // Blackboard = new Blackboard(this);
            // Blackboard.Add(new BlackboardSection(){title = "黑板变量"});
            // Blackboard.SetPosition(new Rect(10,30,200,300)); 
            // Add(Blackboard);
            //
            // Blackboard.addItemRequested = blackboard =>
            // {
            //
            // };
            //
            // Blackboard.editTextRequested = (blackboard, element, newValue) =>
            // {
            //
            // };

        }
        
        /// <summary>
        /// 构建行为树节点图
        /// </summary>
        private void BuildGraphView()
        {
            Dictionary<BaseNode, BehaviourTreeNode> nodeDict = new Dictionary<BaseNode, BehaviourTreeNode>();
            
            //创建节点
            CreateGraphNode(nodeDict);
            
            //根据父子关系连线
            BuildConnect(nodeDict);
        }
        
        /// <summary>
        /// 创建节点图节点
        /// </summary >
        private void CreateGraphNode(Dictionary<BaseNode, BehaviourTreeNode> nodeDict)
        {
            foreach (BaseNode node in bt.AllNodes)
            {
                BehaviourTreeNode graphNode = new BehaviourTreeNode();
                graphNode.Init(node,window);
                AddElement(graphNode);
                nodeDict.Add(node,graphNode);
            }
        }

                
        /// <summary>
        /// 构建节点连接
        /// </summary>
        private void BuildConnect(Dictionary<BaseNode, BehaviourTreeNode> nodeDict)
        {
            foreach (BaseNode node in bt.AllNodes)
            {
                BehaviourTreeNode graphNode = nodeDict[node];
                if (graphNode.outputContainer.childCount == 0)
                {
                    //没有子节点 跳过
                    continue;
                }
                
                Port selfOutput = (Port)graphNode.outputContainer[0];
                
                //与当前节点的子节点进行连线
                node.ForeachChild((child =>
                {
                    if (child == null)
                    {
                        return;
                    }
                
                    BehaviourTreeNode graphChildNode = nodeDict[child];
                
                    Port childInput = (Port)graphChildNode.inputContainer[0];
                    Edge edge = selfOutput.ConnectTo(childInput);
                    AddElement(edge);
                }));
            }
        }

        
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
        
            foreach (var port in ports.ToList())
            {
                if (startPort.node == port.node || startPort.direction == port.direction)
                {
                    //不能自己连自己
                    //只能input和output连接
                    continue;
                }

                compatiblePorts.Add(port);
            }
            return compatiblePorts;
        
        }
    }
}