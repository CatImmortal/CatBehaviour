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
        
        private NodeView sourceNode;
        private bool isParentWithSourceNode;
        
        private Texture2D emptyIcon;
        
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(BehaviourTreeWindow window,Edge edge = null)
        {
            this.window = window;
            graphView = window.GraphView;

            if (edge != null)
            {
                if (edge.output != null)
                {
                    sourceNode = (NodeView)edge.output.node;
                    isParentWithSourceNode = true;
                }
                else
                {
                    sourceNode = (NodeView)edge.input.node;
                    isParentWithSourceNode = false; 
                }
            }


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
            
            //创建节点 
            var point = context.screenMousePosition - window.position.position;  //鼠标相对于窗口的位置
            Vector2 graphMousePosition = graphView.contentViewContainer.WorldToLocal(point);  //鼠标在节点图下的位置
            NodeView nodeView = NodeView.Create(type, window,graphMousePosition);
            
            //如果是通过拖动线创建的节点 就连接起来
            if (sourceNode != null)
            {
                if (isParentWithSourceNode)
                {
                    sourceNode.AddChild(nodeView);
                }
                else
                {
                    nodeView.AddChild(sourceNode);
                }
            }
            return true;
        }
    }
}