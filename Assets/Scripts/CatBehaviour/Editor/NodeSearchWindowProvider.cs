using System;
using System.Collections.Generic;
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

        private Texture2D emptyIcon;
        
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(BehaviourTreeWindow window,BehaviourTreeGraphView graphView)
        {
            this.window = window;
            this.graphView = graphView;

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
            foreach (Type type in TypeCache.GetTypesDerivedFrom<T>())
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
                
                entries.Add(new SearchTreeEntry(new GUIContent(BehaviourTreeNode.GetNodeName(type),emptyIcon)) { level = 2, userData = type });
            }
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            //取出类型信息
            Type type = (Type)searchTreeEntry.userData;
            
            //创建node
            BaseNode runtimeNode = Activator.CreateInstance(type) as BaseNode;
            BehaviourTreeNode node = new BehaviourTreeNode();
            node.Init(runtimeNode,window);
            
            //将节点创建在鼠标的位置里
            Vector2 windowMousePosition = graphView.ChangeCoordinatesTo(graphView.parent, context.screenMousePosition - window.position.position);
            Vector2 graphMousePosition = graphView.WorldToLocal(windowMousePosition);
            node.SetPosition(new Rect(graphMousePosition,node.GetPosition().size));
            
            graphView.AddElement(node);
            return true;
        }
    }
}