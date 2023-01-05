using System.Collections.Generic;
using System.Linq;
using CatBehaviour.Runtime;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CatBehaviour.Editor
{
    public partial class BehaviourTreeGraphView
    {
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
    }
}