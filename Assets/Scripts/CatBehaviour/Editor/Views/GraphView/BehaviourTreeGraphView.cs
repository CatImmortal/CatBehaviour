using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
        
        public BehaviourTree BT;
        private BehaviourTreeWindow window;
        public BlackBoardView BlackboardView;

        /// <summary>
        /// 黑板变化事件
        /// </summary>
        public event Action OnBlackBoardChanged;
        
        public BehaviourTreeGraphView()
        {
            //监听节点与黑板移动 节点删除 线删除
            graphViewChanged = GraphViewChangedCallback;
            
            //这三个回调实现节点的复制粘贴功能
            serializeGraphElements = SerializeGraphElementsCallback;
            canPasteSerializedData = CanPasteSerializedDataCallback;
            unserializeAndPaste = UnserializeAndPasteCallback;
            
            Insert(0, new GridBackground());  //格子背景
            
            //添加背景网格样式
            var styleSheet = Resources.Load<StyleSheet>("USS/BehaviourTreeWindow");
            styleSheets.Add(styleSheet);

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);  //可缩放
            this.AddManipulator(new SelectionDragger());  //节点可拖动
            this.AddManipulator(new ContentDragger());  //节点图可移动
            this.AddManipulator(new RectangleSelector());  //可框选多个节点
        }

        private GraphViewChange GraphViewChangedCallback(GraphViewChange changes)
        {
            //移动
            if (changes.movedElements != null)
            {
                foreach (GraphElement element in changes.movedElements)
                {
                    if (element is NodeView nodeView)
                    {
                        nodeView.SetPosAndRecord(nodeView.GetPosition());
                    }
                    else if (element is BlackBoardView blackBoardView)
                    {
                        blackBoardView.SetPosAndRecord(blackBoardView.GetPosition());
                    }
                }
            }

            //删除
            if (changes.elementsToRemove != null)
            {
                foreach (GraphElement element in changes.elementsToRemove)
                {
                    if (element is NodeView nodeView)
                    {
                        //Debug.Log($"删除节点 {nodeView}");
                        nodeView.RemoveSelf();
                    }
                    else if (element is Edge edge)
                    {
                        //Debug.Log($"删除线 {edge.output.node} -> {edge.input.node}");
                        var parentNode = (NodeView) edge.output.node;
                        var childNode = (NodeView) edge.input.node;
                        parentNode.RemoveChild(childNode);
                    }
                }
            }
            

            return changes;
        }

        /// <summary>
        /// 点击复制时，序列化节点的回调
        /// </summary>
        private string SerializeGraphElementsCallback(IEnumerable<GraphElement> elements)
        {
            CopyPasteData data = new CopyPasteData();
            List<BaseNode> allNodes = new List<BaseNode>();
            string result = null;

            List<NodeView> graphNodes = new List<NodeView>();
            HashSet<NodeView> graphNodeSet = new HashSet<NodeView>();
            foreach (GraphElement element in elements)
            {
                if (element is NodeView graphNode)
                {
                    if (graphNode.RuntimeNode is RootNode)
                    {
                        //跳过根节点
                        continue;
                    }
                    graphNodes.Add(graphNode);
                    graphNodeSet.Add(graphNode);
                }
            }
           
                
            foreach (var graphNode in graphNodes)
            {
                //清空ID和父子关系
                graphNode.RuntimeNode.ClearId();
                graphNode.RuntimeNode.ClearNodeReference();

                //记录位置
                graphNode.RuntimeNode.Position = graphNode.GetPosition().position;
                graphNode.RuntimeNode.Position += new Vector2(100, 100);  //被复制的节点 位置要做一点偏移量
                    
                //添加到allNodes里 建立ID
                allNodes.Add(graphNode.RuntimeNode);
                graphNode.RuntimeNode.Id = allNodes.Count;
            }

            foreach (var graphNode in graphNodes)
            {
                if (graphNode.inputContainer.childCount == 0)
                {
                    continue;
                }
                
                Port inputPort = (Port)graphNode.inputContainer[0];
                Edge inputEdge = inputPort.connections.FirstOrDefault();
                if (inputEdge == null)
                {
                    continue;
                }
                
                //将自身添加的父节点的子节点里
                var parent = (NodeView)inputEdge.output.node;
                if (graphNodeSet.Contains(parent))
                {
                    //父节点要被包含在 被复制的节点集合 里，才添加
                    //否则视为此节点不具有父节点 防止把没选中的节点也复制了
                    parent.RuntimeNode.AddChild(graphNode.RuntimeNode);
                }
            }
            
            //记录父子节点ID
            foreach (BaseNode node in allNodes)
            {
                node.RebuildId();
                data.CopiedNodes.Add(JsonSerializer.Serialize(node));
            }
            
            ClearSelection();
            
            result = JsonUtility.ToJson(data, true);
            //Debug.Log(result);
            return result;
        }
        
        /// <summary>
        /// 检测是否可点击粘贴的回调
        /// </summary>
        private bool CanPasteSerializedDataCallback(string data)
        {
            try
            {
                return JsonUtility.FromJson(data, typeof(CopyPasteData)) != null;
            } catch {
                return false;
            }
        }

        /// <summary>
        /// 点击粘贴时，反序列化节点的回调
        /// </summary>
        private void UnserializeAndPasteCallback(string operationName, string data)
        {
            var copyPasteData = JsonUtility.FromJson<CopyPasteData>(data);

            List<BaseNode> allNodes = new List<BaseNode>();
            foreach (JsonElement jsonElement in copyPasteData.CopiedNodes)
            {
                BaseNode node = JsonSerializer.DeserializeNode(jsonElement);
                node.Owner = BT;
                allNodes.Add(node);
            }
            //恢复父子节点的引用
            foreach (BaseNode node in allNodes)
            {
                node.RebuildNodeReference(allNodes);
            }
            
            Dictionary<BaseNode, NodeView> nodeDict = new Dictionary<BaseNode, NodeView>();
            
            //创建节点
            CreateGraphNode(nodeDict,allNodes);
            
            //根据父子关系连线
            BuildConnect(nodeDict,allNodes);

            //选中粘贴后的节点
            foreach (var value in nodeDict.Values)
            {
                AddToSelection(value);
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(BehaviourTreeWindow window)
        {
            this.window = window;
            
            //节点创建时的搜索窗口
            var searchWindowProvider = ScriptableObject.CreateInstance<NodeSearchWindowProvider>();
            searchWindowProvider.Init(window);
        
            nodeCreationRequest += context =>
            {
                //打开搜索窗口
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindowProvider);
            };
            
            //黑板
            BlackboardView = new BlackBoardView();
            Add(BlackboardView);
            BlackboardView.Init(window);
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public void Refresh(BehaviourTree bt)
        {
            BT = bt;

            //还原位置与大小
            if (bt.Rect != default)
            {
                viewTransform.position = bt.Rect.position;
                viewTransform.scale = new Vector3(bt.Rect.size.x,bt.Rect.size.y,1);
            }
            
            BlackboardView.Refresh();
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
        /// 构建行为树节点图
        /// </summary>
        private void BuildGraphView()
        {
            Dictionary<BaseNode, NodeView> nodeDict = new Dictionary<BaseNode, NodeView>();

            //先删掉旧的节点和线
            foreach (Node node in nodes)
            {
                RemoveElement(node);
            }
            foreach (Edge edge in edges)
            {
                RemoveElement(edge);
            }
            
            //创建节点
            CreateGraphNode(nodeDict,BT.AllNodes);
            
            //根据父子关系连线
            BuildConnect(nodeDict,BT.AllNodes);
        }
        
        /// <summary>
        /// 创建节点图节点
        /// </summary >
        private void CreateGraphNode(Dictionary<BaseNode, NodeView> nodeDict,List<BaseNode> allNodes)
        {
            foreach (BaseNode node in allNodes)
            {
                NodeView nodeView = new NodeView();
                nodeView.Init(node,window);
                AddElement(nodeView);
                nodeDict.Add(node,nodeView);
            }
        }

        /// <summary>
        /// 构建节点连接
        /// </summary>
        private void BuildConnect(Dictionary<BaseNode, NodeView> nodeDict,List<BaseNode> allNodes)
        {
            foreach (BaseNode node in allNodes)
            {
                NodeView nodeView = nodeDict[node];
                if (nodeView.outputContainer.childCount == 0)
                {
                    //不需要子节点 跳过
                    continue;
                }
                
                Port selfOutput = (Port)nodeView.outputContainer[0];
                
                //与当前节点的子节点进行连线
                node.ForeachChild((child =>
                {
                    if (child == null)
                    {
                        return;
                    }
                
                    NodeView childNodeView = nodeDict[child];
                
                    Port childInput = (Port)childNodeView.inputContainer[0];
                    Edge edge = selfOutput.ConnectTo(childInput);
                    AddElement(edge);
                }));
            }
        }

        /// <summary>
        /// 获取可连线的节点列表
        /// </summary>
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
        /// 添加黑板参数
        /// </summary>
        public void AddBlackBoardParam(string key,Type type,object value = null)
        {
            window.RecordObject($"AddBlackBoard {key}");
            
            BBParam bbParam = (BBParam)Activator.CreateInstance(type);
            bbParam.ValueObj = value;
            window.ClonedBTSO.SetParam(key,bbParam);

            OnBlackBoardChanged?.Invoke();
        }

        /// <summary>
        /// 移除黑板参数
        /// </summary>
        public void RemoveBlackBoardParam(string key)
        {
            window.RecordObject($"RemoveBlackBoard {key}");
            
            window.ClonedBTSO.RemoveParam(key);
            OnBlackBoardChanged?.Invoke();
        }

        /// <summary>
        /// 重命名黑板参数
        /// </summary>
        public bool RenameBlackBoardParam(string oldKey, string newKey, BBParam param)
        {
            if (BT.BlackBoard.ParamDict.ContainsKey(newKey))
            {
                //Debug.Log($"重命名黑板key失败，已存在同名key:{newKey}");
                return false;
            }
            //Debug.Log($"重命名黑板key成功，{oldKey} -> {newKey}");
            BT.BlackBoard.RemoveParam(oldKey);
            BT.BlackBoard.SetParam(newKey,param);
            //OnBlackBoardChaged?.Invoke();

            return true;
        }
    }
}