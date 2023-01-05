using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CatBehaviour.Runtime;

namespace CatBehaviour.Editor
{
    /// <summary>
    /// 注释块View
    /// </summary>
    public class CommentBlockView : Group
    {
        private BehaviourTreeWindow window;
        private CommentBlock commentBlock;

        public List<NodeView> NodeViews = new List<NodeView>();
        
        private ColorField colorField;
        
        public CommentBlockView()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("USS/CommentBlockView"));
        }


        public static CommentBlockView Create(BehaviourTreeWindow window,Vector2 pos)
        {
            window.RecordObject($"Create CommentBlock");
            
            CommentBlock commentBlock = new CommentBlock("注释块",pos);
            window.GraphView.BT.CommentBlocks.Add(commentBlock);
            
            CommentBlockView view = Create(commentBlock, window,null);
            
            return view;
        }
        
        public static CommentBlockView Create(CommentBlock commentBlock, BehaviourTreeWindow window,Dictionary<BaseNode, NodeView> nodeDict)
        {
            CommentBlockView view = new CommentBlockView();
            view.Init(window,commentBlock,nodeDict);
            window.GraphView.AddElement(view);
            window.GraphView.CommentBlockViews.Add(view);
            
            return view;
        }
        
        private static void BuildContextualMenu(ContextualMenuPopulateEvent evt) {}
        
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(BehaviourTreeWindow window,CommentBlock commentBlock,Dictionary<BaseNode, NodeView> nodeDict)
        {
            this.window = window;
            this.commentBlock = commentBlock;

            title = commentBlock.Comment;
            SetPosition(new Rect(commentBlock.Position,new Vector2(400, 200)));
            
            this.AddManipulator(new ContextualMenuManipulator(BuildContextualMenu));
            
            //注释
            headerContainer.Q<TextField>().RegisterCallback<ChangeEvent<string>>((evt =>
            {
                window.RecordObject($"Update Comment");
                commentBlock.Comment = evt.newValue;
            }));
            
            //颜色
            colorField = new ColorField{ value = commentBlock.Color, name = "headerColorPicker" };
            headerContainer.Add(colorField);
            colorField.RegisterValueChangedCallback(e =>
            {
                UpdateColor(e.newValue);
            });
            UpdateColor(commentBlock.Color);
            
            //初始节点
            if (commentBlock.Nodes != null)
            {
                foreach (BaseNode node in commentBlock.Nodes)
                {
                    NodeView nodeView = nodeDict[node];
                    NodeViews.Add(nodeView);
                    AddElement(nodeView);
                    
                }
            }
        }
        
        /// <summary>
        /// 更新颜色
        /// </summary>
        public void UpdateColor(Color newColor)
        {
            window.RecordObject($"Update Color");
            commentBlock.Color = newColor;
            style.backgroundColor = newColor;
        }
        
        /// <summary>
        /// 设置位置
        /// </summary>
        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            commentBlock.Position = newPos.position;
        }

        /// <summary>
        /// 添加节点到注释块内
        /// </summary>
        public void AddNodeView(NodeView nodeView)
        {
            commentBlock.Nodes.Add(nodeView.RuntimeNode);
            NodeViews.Add(nodeView);
            AddElement(nodeView);
        }
        
        /// <summary>
        /// 删除注释块
        /// </summary>
        public void RemoveSelf()
        {
            window.RecordObject($"Remove CommentBlock");
            window.GraphView.BT.CommentBlocks.Remove(commentBlock);
            window.GraphView.CommentBlockViews.Remove(this);
        }
        
        /// <summary>
        /// 添加元素
        /// </summary>
        protected override void OnElementsAdded(IEnumerable<GraphElement> elements)
        {
            window.RecordObject($"Add Node");
            
            base.OnElementsAdded(elements);
            foreach (GraphElement element in elements)
            {
                if (element is NodeView nodeView)
                {
                    if (commentBlock.Nodes.Contains(nodeView.RuntimeNode))
                    {
                        continue;
                    }
                    
                    commentBlock.Nodes.Add(nodeView.RuntimeNode);
                    NodeViews.Add(nodeView);
                }
            }
        }

        /// <summary>
        /// 删除元素
        /// </summary>
        protected override void OnElementsRemoved(IEnumerable<GraphElement> elements)
        {
            window.RecordObject($"Remove Node");
            
            base.OnElementsRemoved(elements);
            foreach (GraphElement element in elements)
            {
                if (element is NodeView nodeView)
                {
                    commentBlock.Nodes.Remove(nodeView.RuntimeNode);
                    NodeViews.Remove(nodeView);
                }
            }
        }
    }
}