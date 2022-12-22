using System;
using System.Reflection;
using CatBehaviour.Runtime;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace CatBehaviour.Editor
{
    /// <summary>
    /// 行为树节点图节点
    /// </summary>
    public class BehaviourTreeNode : Node
    {
        public BaseNode RuntimeNode;
        private Port inputPort;
        private Port outputPort;
        

        
        /// <summary>
        /// 获取节点在节点图的名字
        /// </summary>
        public static string GetNodeName(Type type)
        {
            NodeInfoAttribute nodeInfo = type.GetCustomAttribute<NodeInfoAttribute>();
            string name;
            if (nodeInfo != null)
            {
                name = nodeInfo.Name;
            }
            else
            {
                name = type.Name;
            }

            return name;
        }
        
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(BaseNode runtimeNode,BehaviourTreeWindow window)
        {
            RuntimeNode = runtimeNode;
            
            //注册点击事件
            RegisterCallback<MouseDownEvent>((evt =>
            {
                if (evt.button == 0)
                { 
                    window.OnNodeClick(this);
                }
            }));

            SetNameAndPos();
            SetVertical();
            AddPort();

        }

        /// <summary>
        /// 设置节点名和位置
        /// </summary>
        private void SetNameAndPos()
        {
            Type nodeType = RuntimeNode.GetType();
            title = GetNodeName(nodeType);
            SetPosition(new Rect(RuntimeNode.Position,GetPosition().size));
        }

        /// <summary>
        /// 将端口方向改成垂直的
        /// </summary>
        private void SetVertical()
        {
            var titleButtonContainer = contentContainer.Q<VisualElement>("title-button-container");
            titleButtonContainer.RemoveAt(0);  //删掉收起箭头 否则会有bug

            var titleContainer = contentContainer.Q<VisualElement>("title");
            var topContainer = this.Q("input");
            var bottomContainer = this.Q("output");
            
            var nodeBorder = contentContainer.Q<VisualElement>("node-border");
            nodeBorder.RemoveAt(0);
            nodeBorder.RemoveAt(0);
            
            nodeBorder.Add(topContainer);
            nodeBorder.Add(titleContainer);
            nodeBorder.Add(bottomContainer);
        }
        
        /// <summary>
        /// 根据节点类型添加端口
        /// </summary>
        private void AddPort()
        {
            if (!(RuntimeNode is RootNode))
            {
                inputPort = Port.Create<Edge>(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(Port));
                inputPort.portName = "父节点";
                inputPort.portColor = Color.cyan;
                //inputPort.style.flexDirection = FlexDirection.Column;
                inputContainer.Add(inputPort);
            }

            if (RuntimeNode is BaseActionNode)
            {
                return;
            }
            Port.Capacity outputCount;
            if (RuntimeNode is BaseCompositeNode)
            {
                outputCount = Port.Capacity.Multi;
                
            }else if (RuntimeNode is BaseDecoratorNode)
            {
                outputCount = Port.Capacity.Single;
            }
            else
            {
                throw new Exception($"行为树节点类型无效，不是3种基础节点类型的派生之一：{title}");
            }
            outputPort = Port.Create<Edge>(Orientation.Vertical, Direction.Output, outputCount, typeof(Port));
            outputPort.portName = "子节点";
            outputPort.portColor = Color.red;
            //outputPort.style.flexDirection = FlexDirection.ColumnReverse;
            outputContainer.Add(outputPort);

        }

        // /// <summary>
        // /// 绘制节点属性
        // /// </summary>
        // private void DrawProperty()
        // {
        //     RuntimeNode.CreateGUI(nodeBorder);
        //     IMGUIContainer imguiContainer = new IMGUIContainer(){};
        //     imguiContainer.onGUIHandler = RuntimeNode.OnGUI;
        //     nodeBorder.Add(imguiContainer);
        // }
        
        public override string ToString()
        {
            return RuntimeNode.ToString();
        }
    }
}