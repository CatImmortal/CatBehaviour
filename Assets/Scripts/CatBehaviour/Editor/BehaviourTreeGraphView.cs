using System.Collections.Generic;
using CatBehaviour.Runtime;
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

        public BehaviourTreeGraphView()
        {
            Insert(0, new GridBackground());  //格子背景
            
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
         
            this.AddManipulator(new SelectionDragger());  //节点可拖动
            this.AddManipulator(new ContentDragger());  //节点图可移动
            this.AddManipulator(new RectangleSelector());  //可框选多个节点
            
            //节点创建时的搜索窗口
            var searchWindowProvider = ScriptableObject.CreateInstance<NodeSearchWindowProvider>();
            searchWindowProvider.Init( window,this);
        
            nodeCreationRequest += context =>
            {
                //打开搜索窗口
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindowProvider);
            };
        }
        
        /// <summary>
        /// 行为树节点 -> 节点图节点
        /// </summary>
        private Dictionary<BaseNode, BehaviourTreeNode> nodeDict = new Dictionary<BaseNode, BehaviourTreeNode>();


        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(BehaviourTreeWindow window, BehaviourTree bt)
        {
            this.window = window;
            
            if (bt == null)
            {
                bt = new BehaviourTree();
                RootNode rootNode = new RootNode();
                rootNode.Position = new Vector2(window.position.position.x + 100,window.position.position.y + 100);
                bt.RootNode = rootNode;
            }

            this.bt = bt;
            
            Build();
        }

        /// <summary>
        /// 构建行为树节点图
        /// </summary>
        private void Build()
        {
            CreateGraphNode(bt.RootNode);
        }

        /// <summary>
        /// 创建节点图节点
        /// </summary>
        private void CreateGraphNode(BaseNode node)
        {
            if (node == null)
            {
                return;
            }
            
            BehaviourTreeNode graphNode = new BehaviourTreeNode();
            graphNode.Init(node);
            nodeDict.Add(node,graphNode);

            AddElement(graphNode);
            
            if (node is BaseActionNode)
            {
                //动作节点 无子节点 直接返回
                return;
            }

            if (node is BaseCompositeNode compositeNode)
            {
                //复合节点
                foreach (BaseNode child in compositeNode.Children)
                {
                    CreateGraphNode(child);
                }
                return;
            }

            if (node is BaseDecoratorNode decoratorNode)
            {
                //装饰节点
                CreateGraphNode(decoratorNode.Child);
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

        /// <summary>
        /// 保存
        /// </summary>
        public void Save()
        {

        }
    }
}