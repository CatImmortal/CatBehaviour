using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CatBehaviour.Runtime;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace CatBehaviour.Editor
{
    /// <summary>
    /// 搜索窗口
    /// </summary>
    public class NodeSearchWindowProvider: ScriptableObject, ISearchWindowProvider
    {
        private BehaviourTreeWindow window;
        private BehaviourTreeGraphView graphView;

        private PortView sourceInputPort;
        private PortView sourceOutputPort;
        
        private Texture2D emptyIcon;
        
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(BehaviourTreeWindow window,Edge edge = null)
        {
            this.window = window;
            graphView = window.GraphView;

            sourceInputPort = edge?.input as PortView;
            sourceOutputPort = edge?.output as PortView;

            emptyIcon = new Texture2D(1, 1);
            emptyIcon.SetPixel(0,0,new Color(0,0,0,0));
            emptyIcon.Apply();
        }
        
        /// <summary>
        /// 创建搜索树
        /// </summary>
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var entries = new List<SearchTreeEntry>();
            
            entries.Add(new SearchTreeGroupEntry(new GUIContent("行为树节点"),0));
            
            entries.Add(new SearchTreeGroupEntry(new GUIContent("复合节点"),1));
            AddNodeOptions<BaseCompositeNode>(entries);
            
            entries.Add(new SearchTreeGroupEntry(new GUIContent("装饰节点"),1));
            AddNodeOptions<BaseDecoratorNode>(entries);
            
            entries.Add(new SearchTreeGroupEntry(new GUIContent("动作节点"),1));
            AddNodeOptions<BaseActionNode>(entries);

           return entries;
        }

        /// <summary>
        /// 添加节点选项
        /// </summary>
        private void AddNodeOptions<T>(List<SearchTreeEntry> entries)
        {
            var types = TypeCache.GetTypesDerivedFrom<T>().ToList();
            
            //将节点按照order排序
            types.Sort((x, y) =>
            {
                var xNodeAttr = x.GetCustomAttribute<NodeInfoAttribute>();
                var yNodeAttr = y.GetCustomAttribute<NodeInfoAttribute>();

                return xNodeAttr.Order.CompareTo(yNodeAttr.Order);
            });
            
            foreach (Type type in types)
            {
                if (type.IsAbstract)
                {
                    continue;
                }

                if (type == typeof(RootNode))
                {
                    //跳过根节点
                    continue;
                }

                //加载节点icon
                Texture2D icon = null;
                var nodeAttr = type.GetCustomAttribute<NodeInfoAttribute>();
                if (nodeAttr == null)
                {
                    Debug.LogError($"节点{type.Name}未标记NodeInfo特性");
                    continue;
                }
                if (!string.IsNullOrEmpty(nodeAttr.Icon))
                {
                    icon = Resources.Load<Texture2D>(nodeAttr.Icon);
                    if (icon == null)
                    {
                        icon = emptyIcon;
                    }
                }
                else
                {
                   icon = emptyIcon;
                }
                
                var guiContent = new GUIContent(NodeView.GetNodeName(type), icon);
                entries.Add(new SearchTreeEntry(guiContent) { level = 2, userData = type });
            }
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            //取出类型信息
            Type type = (Type)searchTreeEntry.userData;
            
            //创建node
            BaseNode runtimeNode = (BaseNode)Activator.CreateInstance(type);
            runtimeNode.Owner = graphView.BT;
            NodeView nodeView = new NodeView();
            nodeView.Init(runtimeNode,window);
            
            //将节点创建在鼠标的位置里
            var point = context.screenMousePosition - window.position.position;  //鼠标相对于窗口的位置
            Vector2 graphMousePosition = graphView.contentViewContainer.WorldToLocal(point);  //鼠标在节点图下的位置
            nodeView.SetPosition(new Rect(graphMousePosition,nodeView.GetPosition().size));
            nodeView.RuntimeNode.Position = graphMousePosition;  //这里手动赋值一下初始坐标 否则后面同步数据到BT时，因为是同一帧会导致获取不到正确位置
            window.RecordObject($"Add {type} Node",(() =>
            {
                graphView.AddElement(nodeView);
            
                //如果是通过拖动线创建的节点 就连接起来
                var sourcePort = sourceInputPort ?? sourceOutputPort;
                if (sourcePort != null)
                {
                    //先断开已有的连线
                    foreach (Edge sourcePortEdge in sourcePort.connections.ToList())
                    {
                        sourcePortEdge.input.Disconnect(sourcePortEdge);
                        sourcePortEdge.output.Disconnect(sourcePortEdge);
                        graphView.RemoveElement(sourcePortEdge);
                    }

                    //连接发起连线的节点和创建出的节点
                    PortView targetPort;
                    if (sourcePort == sourceInputPort)
                    {
                        targetPort = (PortView)nodeView.outputContainer[0];
                    }
                    else
                    {
                        targetPort = (PortView)nodeView.inputContainer[0];
                    }
                    var edge = sourcePort.ConnectTo(targetPort);
                    graphView.AddElement(edge);
                }
            }));
            
           

            return true;
        }
    }
}