using System;
using System.Collections;
using System.Collections.Generic;


namespace CatBehaviour.Runtime
{
    /// <summary>
    /// 行为树
    /// </summary>
    public class BehaviourTree
    {
        /// <summary>
        /// 字符串序列化器
        /// </summary>
        public static IStringSerializer StringSerializer { get; set; }

        /// <summary>
        /// 根节点ID
        /// </summary>
        public int RootNodeId { get; set; }

        /// <summary>
        /// 根节点
        /// </summary>
        [NonSerialized] 
        public RootNode RootNode;
        
        /// <summary>
        /// 所有节点的列表
        /// </summary>
        public List<BaseNode> AllNodes { get; set; } = new List<BaseNode>();

        /// <summary>
        /// 黑板
        /// </summary>
        public BlackBoard BlackBoard { get; set; } = new BlackBoard();
        
        /// <summary>
        /// 开始运行行为树
        /// </summary>
        public void Start()
        {
            RootNode.Start();
        }

        /// <summary>
        /// 取消运行行为树
        /// </summary>
        public void Cancel()
        {
            RootNode.Cancel();
        }

        /// <summary>
        /// 获取行为树节点
        /// </summary>
        public BaseNode GetNode(int nodeId)
        {
            if (nodeId == 0)
            {
                return null;
            }
            
            return AllNodes[nodeId - 1];
        }
        

        public static void OnUpdate(float deltaTime)
        {
            UpdateManager.OnUpdate(deltaTime);
        }
        
        /// <summary>
        /// 序列化为字符串
        /// </summary>
        public string Serialize()
        {
            PreProcessSerialize();
            string str = StringSerializer.Serialize(this);
            return str;
        }
        
        /// <summary>
        /// 从字符串反序列化
        /// </summary>
        public static BehaviourTree Deserialize(string str)
        {
            var bt = StringSerializer.Deserialize(str);
            bt.PostProcessDeserialize();
            return bt;
        }

        /// <summary>
        /// 序列化前的预处理
        /// </summary>
        private void PreProcessSerialize()
        {
            //排序子节点
            foreach (BaseNode node in AllNodes)
            {
                node.SortChild();
            }
            
            //建立父子节点间的ID索引
            RootNodeId = RootNode.Id;
            foreach (BaseNode node in AllNodes)
            {
                if (node.ParentNode != null)
                {
                    node.ParentNodeId = node.ParentNode.Id;
                }
                node.RecordChildId();
            }
        }

        /// <summary>
        /// 收集从根节点出发，能达到的所有节点
        /// </summary>
        private void CollectAllNodes()
        {
            AllNodes.Clear();
            CollectAllNodes(RootNode);
        }
        
        /// <summary>
        /// 收集从根节点出发，能达到的所有节点
        /// </summary>
        private void CollectAllNodes(BaseNode node)
        {
            if (node == null)
            {
                return;
            }

            AllNodes.Add(node);
            node.Id = AllNodes.Count;

            node.ForeachChild(CollectAllNodes);
        }

        /// <summary>
        /// 反序列化后处理
        /// </summary>
        private void PostProcessDeserialize()
        {
            //从Id恢复父子节点的引用
            RootNode = (RootNode)GetNode(RootNodeId);
            
            foreach (BaseNode node in AllNodes)
            {
                node.Owner = this;
                
                if (node.ParentNodeId > 0)
                {
                    node.ParentNode = GetNode(node.ParentNodeId);
                }
                
                node.RebuildChildReference();
            }
        }
        
       
    }

}
