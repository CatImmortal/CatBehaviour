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
        
        public void Init(BaseNode runtimeNode)
        {
            RuntimeNode = runtimeNode;

            //设置节点名和位置
            Type nodeType = runtimeNode.GetType();
            title = GetNodeName(nodeType);
            SetPosition(new Rect(runtimeNode.Position,GetPosition().size));

            //将端口方向改成垂直的
            var titleButtonContainer = contentContainer.Q<VisualElement>("title-button-container");
            titleButtonContainer.RemoveAt(0);  //删掉收起箭头 否则会有bug
            
            var nodeBorder = contentContainer.Q<VisualElement>("node-border");
            
            var titleContainer = contentContainer.Q<VisualElement>("title");
            var topContainer = this.Q("input");
            var bottomContainer = this.Q("output");
            
            nodeBorder.RemoveAt(0);
            nodeBorder.RemoveAt(0);
            
            nodeBorder.Add(topContainer);
            nodeBorder.Add(titleContainer);
            nodeBorder.Add(bottomContainer);

            //根据节点类型处理端口
            if (!(runtimeNode is RootNode))
            {
                inputPort = Port.Create<Edge>(Orientation.Vertical, Direction.Input, Port.Capacity.Multi, typeof(Port));
                inputPort.portName = "父节点";
                inputPort.portColor = Color.cyan;
                //inputPort.style.flexDirection = FlexDirection.Column;
                inputContainer.Add(inputPort);
            }

            if (runtimeNode is BaseActionNode)
            {
                return;
            }
            Port.Capacity outputCount;
            if (runtimeNode is BaseCompositeNode)
            {
                outputCount = Port.Capacity.Multi;
                
            }else if (runtimeNode is BaseDecoratorNode)
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
            //outputPort.style.flexDirection = FlexDirection.Column;
            outputContainer.Add(outputPort);
        }
    }
}