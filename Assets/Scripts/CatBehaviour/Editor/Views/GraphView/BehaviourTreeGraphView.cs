﻿using System;
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
            //TODO:用这三个回调实现节点的复制粘贴功能
            serializeGraphElements = SerializeGraphElementsCallback;
            canPasteSerializedData = CanPasteSerializedDataCallback;
            unserializeAndPaste = UnserializeAndPasteCallback;
            
            Insert(0, new GridBackground());  //格子背景
            
            //添加背景网格样式
            //var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/CatBehaviour/Editor/BehaviourTreeWindow.uss");
            var styleSheet = Resources.Load<StyleSheet>("USS/BehaviourTreeWindow");
            styleSheets.Add(styleSheet);

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);  //可缩放
            this.AddManipulator(new SelectionDragger());  //节点可拖动
            this.AddManipulator(new ContentDragger());  //节点图可移动
            this.AddManipulator(new RectangleSelector());  //可框选多个节点
            
            //graphViewChanged += OnGraphViewChanged;
        }

        /// <summary>
        /// 点击复制时，序列化节点的回调
        /// </summary>
        private string SerializeGraphElementsCallback(IEnumerable<GraphElement> elements)
        {
            CopyPasteData data = new CopyPasteData();
            List<BaseNode> allNodes = new List<BaseNode>();
            string result = null;
            foreach (GraphElement element in elements)
            {
                if (element is BehaviourTreeNode node)
                {
                    //清空ID和父子关系
                    node.RuntimeNode.ClearId();
                    node.RuntimeNode.ClearNodeReference();

                    //记录位置
                    node.RuntimeNode.Position = node.GetPosition().position;
                    
                    //添加到allNodes里 建立ID
                    allNodes.Add(node.RuntimeNode);
                    node.RuntimeNode.Id = allNodes.Count;
                }
            }
            
            //根据节点图的连线刷新父子关系
            foreach (GraphElement element in elements)
            {
                if (element is BehaviourTreeNode node)
                {
                    if (node.inputContainer.childCount == 0)
                    {
                        continue;
                    }
                
                    Port inputPort = (Port)node.inputContainer[0];
                    Edge inputEdge = inputPort.connections.FirstOrDefault();
                    if (inputEdge == null)
                    {
                        continue;
                    }
                
                    //向父节点添加自身为其子节点
                    var parent = (BehaviourTreeNode)inputEdge.output.node;
                    parent.RuntimeNode.AddChild(node.RuntimeNode);
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
            Debug.Log(result);
            return result;
        }
        
        /// <summary>
        /// 检测是否可点击粘贴的回调
        /// </summary>
        private bool CanPasteSerializedDataCallback(string data)
        {
            try
            {
                return false;

            } catch {
                return false;
            }
        }

        /// <summary>
        /// 点击粘贴时，反序列化节点的回调
        /// </summary>
        /// <param name="operationName"></param>
        /// <param name="data"></param>
        private void UnserializeAndPasteCallback(string operationName, string data)
        {
            
        }





        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(BehaviourTreeWindow window, BehaviourTree bt)
        {
            this.window = window;
            BT = bt;
            
            //节点创建时的搜索窗口
            var searchWindowProvider = ScriptableObject.CreateInstance<NodeSearchWindowProvider>();
            searchWindowProvider.Init(window,this);
        
            nodeCreationRequest += context =>
            {
                //打开搜索窗口
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindowProvider);
            };
            
            //还原位置与大小
            if (bt.Rect != default)
            {
                viewTransform.position = bt.Rect.position;
                viewTransform.scale = new Vector3(bt.Rect.size.x,bt.Rect.size.y,1);
            }

            CreateMiniMap();
            CreateBlackBoard();
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
        /// 创建黑板
        /// </summary>
        private void CreateBlackBoard()
        {
            BlackBoardView blackBoardView = new BlackBoardView();
            blackBoardView.Init(this);
            if (BT.BlackBoard.Position != default)
            {
                blackBoardView.SetPosition(BT.BlackBoard.Position);
            }
            
            Add(blackBoardView);

            BlackboardView = blackBoardView;
        }
        
        /// <summary>
        /// 构建行为树节点图
        /// </summary>
        private void BuildGraphView()
        {
            Dictionary<BaseNode, BehaviourTreeNode> nodeDict = new Dictionary<BaseNode, BehaviourTreeNode>();
            
            //创建节点
            CreateGraphNode(nodeDict);
            
            //根据父子关系连线
            BuildConnect(nodeDict);
        }
        
        /// <summary>
        /// 创建节点图节点
        /// </summary >
        private void CreateGraphNode(Dictionary<BaseNode, BehaviourTreeNode> nodeDict)
        {
            foreach (BaseNode node in BT.AllNodes)
            {
                BehaviourTreeNode graphNode = new BehaviourTreeNode();
                graphNode.Init(node,window);
                AddElement(graphNode);
                nodeDict.Add(node,graphNode);
            }
        }

                
        /// <summary>
        /// 构建节点连接
        /// </summary>
        private void BuildConnect(Dictionary<BaseNode, BehaviourTreeNode> nodeDict)
        {
            foreach (BaseNode node in BT.AllNodes)
            {
                BehaviourTreeNode graphNode = nodeDict[node];
                if (graphNode.outputContainer.childCount == 0)
                {
                    //没有子节点 跳过
                    continue;
                }
                
                Port selfOutput = (Port)graphNode.outputContainer[0];
                
                //与当前节点的子节点进行连线
                node.ForeachChild((child =>
                {
                    if (child == null)
                    {
                        return;
                    }
                
                    BehaviourTreeNode graphChildNode = nodeDict[child];
                
                    Port childInput = (Port)graphChildNode.inputContainer[0];
                    Edge edge = selfOutput.ConnectTo(childInput);
                    AddElement(edge);
                }));
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
        /// 添加黑板参数
        /// </summary>
        public void AddBlackBoardParam(string key,Type type,object value = null)
        {
            BBParam bbParam = (BBParam)Activator.CreateInstance(type);
            bbParam.ValueObj = value;
            
            BT.BlackBoard.SetParam(key,bbParam);

            OnBlackBoardChanged?.Invoke();
        }

        /// <summary>
        /// 移除黑板参数
        /// </summary>
        public void RemoveBlackBoardParam(string key)
        {
            BT.BlackBoard.RemoveParam(key);
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