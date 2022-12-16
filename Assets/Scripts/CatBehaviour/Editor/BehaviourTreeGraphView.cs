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

        /// <summary>
        /// 行为树节点 -> 节点图节点
        /// </summary>
        private Dictionary<BaseNode, BehaviourTreeNode> nodeDict = new Dictionary<BaseNode, BehaviourTreeNode>();
        
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
        /// 节点图发生变化时的回调
        /// </summary>
        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            // if (bt == null)
            // {
            //     return graphViewChange;
            // }
            //
            // void RemoveNode(BaseNode node)
            // {
            //     //删除节点 解除此节点相关的父子关系
            //     node.ParenNode.RemoveChild(node);
            //     
            //     if (node is BaseDecoratorNode decoratorNode)
            //     {
            //         decoratorNode.RemoveChild(decoratorNode.Child);
            //     }else if (node is BaseCompositeNode compositeNode)
            //     {
            //         for (int i =   - 1; i >= 0; i--)
            //         {
            //             
            //         }
            //     }
            // }
            //
            // //节点或线被删除了
            // if (graphViewChange.elementsToRemove != null)
            // {
            //     for (int i = graphViewChange.elementsToRemove.Count - 1; i >= 0; i--)
            //     {
            //         var element = graphViewChange.elementsToRemove[i];
            //
            //         if (element is BehaviourTreeNode node)
            //         {
            //             if (node.RuntimeNode is RootNode)
            //             {
            //                 //根节点不允许删除
            //                 graphViewChange.elementsToRemove.RemoveAt(i);
            //             }
            //             else
            //             {
            //                 //删除节点
            //                 node.RuntimeNode.ParenNode.RemoveChild(node.RuntimeNode);
            //                 
            //             }
            //         }else if (element is Edge edge)
            //         {
            //             //解除父子节点关系
            //             node = (BehaviourTreeNode)edge.output.node;
            //             node.RuntimeNode.ParenNode.RemoveChild(node.RuntimeNode);
            //         }
            //         
            //     }
            // }
            //
            // //节点和节点连线了
            // if (graphViewChange.edgesToCreate != null)
            // {
            //     foreach (Edge edge in graphViewChange.edgesToCreate)
            //     {
            //         var parentNode = (BehaviourTreeNode)edge.output.node;
            //         var childNode = (BehaviourTreeNode)edge.input.node;
            //         parentNode.RuntimeNode.AddChild(childNode.RuntimeNode);
            //     }
            // }



            return graphViewChange;
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
            
            BuildGraphView();
        }

        /// <summary>
        /// 构建行为树节点图
        /// </summary>
        private void BuildGraphView()
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
    }
}