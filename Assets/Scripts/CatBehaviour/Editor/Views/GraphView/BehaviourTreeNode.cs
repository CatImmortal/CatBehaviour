using System;
using System.Reflection;
using CatBehaviour.Runtime;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CatBehaviour.Editor
{
    /// <summary>
    /// 行为树节点图节点
    /// </summary>
    public class BehaviourTreeNode : Node
    {
        private static Color freeColor = new Color(81/255f, 81/255f, 81/255f, 255f/255);
        private static Color runningColor = new Color(38 / 255f, 130 / 255f, 205 / 255f, 255f / 255);
        private static Color successColor = new Color(36 / 255f, 178 / 255f, 50 / 255f, 255f / 255);
        private static Color failedColor = new Color(203 / 255f, 81 / 255f, 61 / 255f, 255f / 255);
        
        public BaseNode RuntimeNode;
        private Port inputPort;
        private Port outputPort;

        private EnumField stateField;
        
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

            var nodeInfo = RuntimeNode.GetType().GetCustomAttribute<NodeInfoAttribute>();
            if (nodeInfo != null && !string.IsNullOrEmpty(nodeInfo.Icon))
            {
                var icon = Resources.Load<Texture>(nodeInfo.Icon);
                if (icon != null)
                {
                    Image img = new Image();
                    img.image = icon;
                    var imgParent = this.Q<VisualElement>("title");
                    imgParent.Insert(0,img);
                }
            }
            
            if (window.IsDebugMode)
            {
                //调试模式下 增加节点状态显示
                AddStateField();
                RefreshNodeState();
            }
            
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

            var titleContainer = this.Q<VisualElement>("title");
            var topContainer = this.Q("input");
            var bottomContainer = this.Q("output");
            
            var nodeBorder = this.Q<VisualElement>("node-border");
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

        /// <summary>
        /// 添加状态显示UI
        /// </summary>
        private void AddStateField()
        {
            var nodeBorder = contentContainer.Q<VisualElement>("node-border");
            stateField = new EnumField(RuntimeNode.CurState);
            nodeBorder.Insert(2,stateField);

            IMGUIContainer imguiContainer = new IMGUIContainer();
            imguiContainer.onGUIHandler += RefreshNodeState;
            nodeBorder.Add(imguiContainer);
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

        /// <summary>
        /// 刷新节点状态显示
        /// </summary>
        private void RefreshNodeState()
        {
            stateField.value = RuntimeNode.CurState;
            
            var element = stateField.ElementAt(0);

            Color color = default;
            switch (RuntimeNode.CurState)
            {
                case BaseNode.State.Free:
                    color = freeColor;
                    break;
                case BaseNode.State.Running:
                    color = runningColor;
                    break;
                case BaseNode.State.Success:
                    color = successColor;
                    break;
                case BaseNode.State.Failed:
                    color = failedColor;
                    break;
            }
            
            element.style.backgroundColor = color;
        }
        
        public override string ToString()
        {
            return RuntimeNode.ToString();
        }
    }
}