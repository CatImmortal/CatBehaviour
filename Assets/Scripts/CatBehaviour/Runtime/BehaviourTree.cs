using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace CatBehaviour.Runtime
{
    /// <summary>
    /// 创建子树回调方法的原型
    /// </summary>
    public delegate void CreateSubTreeCallback(string subTreeName, BehaviourTree parent,
        Action<BehaviourTree> onSubTreeCreated);
    
    /// <summary>
    /// 行为树
    /// </summary>
    [Serializable]
    public class BehaviourTree
    {
        /// <summary>
        /// 字符串序列化器
        /// </summary>
        public static IStringSerializer StringSerializer { get; set; }
        
        /// <summary>
        /// 二进制序列化器
        /// </summary>
        public static IBinarySerializer BinarySerializer { get; set; }

        /// <summary>
        /// 创建子树的处理回调
        /// </summary>
        public static CreateSubTreeCallback OnCreateSubTreeCallback;
        
        /// <summary>
        /// 行为树运行结束回调
        /// </summary>
        public event Action<bool> OnFinished
        {
            add
            {
                if (RootNode == null)
                {
                    return;
                }

                RootNode.OnFinished += value;
            }

            remove
            {
                if (RootNode == null)
                {
                    return;
                }

                RootNode.OnFinished -= value;
            }
        }

        /// <summary>
        /// 行为树被取消回调
        /// </summary>
        public Action OnCanceled;
        
        /// <summary>
        /// 行为树调试名
        /// </summary>
        [NonSerialized]
        public string DebugName;
        
        /// <summary>
        /// 根节点ID
        /// </summary>
        public int RootNodeId;

        /// <summary>
        /// 根节点
        /// </summary>
        [NonSerialized] 
        public RootNode RootNode;
        
        /// <summary>
        /// 所有节点的列表
        /// </summary>
        [SerializeReference]
        public List<BaseNode> AllNodes = new List<BaseNode>();

        /// <summary>
        /// 黑板
        /// </summary>
        public BlackBoard BlackBoard = new BlackBoard();

        /// <summary>
        /// 位置与缩放
        /// </summary>
        public Rect Rect;
        
        /// <summary>
        /// 节点属性面板宽度
        /// </summary>
        public float InspectorWidth;
        
        /// <summary>
        /// 是否已初始化
        /// </summary>
        private bool isInit;
        
        private Action<BaseNode> resetAction;

        public static void OnUpdate(float deltaTime)
        {
            UpdateManager.OnUpdate(deltaTime);
        }

        public BehaviourTree()
        {
            resetAction = Reset;
        }
        
        /// <summary>
        /// 初始化行为树
        /// </summary>
        private void Init()
        {
            //重建节点对黑板参数的引用
            foreach (BaseNode node in AllNodes)
            {
                node.RebuildBBParamReference();
            }
            
        }

        /// <summary>
        /// 开始运行行为树
        /// </summary>
        public void Start(string debugName = null)
        {
            if (RootNode == null)
            {
                throw new Exception("行为树运行失败，根节点为空");
            }
            
            if (RootNode.CurState == BaseNode.State.Running)
            {
                return;
            }
            
            if (!isInit)
            {
                Init();
                isInit = true;
            }
            else
            {
                //第二次被启动 重置所有节点运行状态为Free
                Reset();
            }
            
#if UNITY_EDITOR
            DebugName = debugName;
            if (string.IsNullOrEmpty(DebugName))
            {
                DebugName = DateTime.Now.ToString();
            }
            DebugName = DebugName.Replace('/', '\\');
#endif
            
            RootNode.Start();
        }

        /// <summary>
        /// 取消运行行为树
        /// </summary>
        public void Cancel()
        {
            if (RootNode.CurState != BaseNode.State.Running)
            {
                return;
            }
            
            RootNode.Cancel();
            OnCanceled?.Invoke();
        }

        private void Reset()
        {
            Reset(RootNode);
        }

        
        private void Reset(BaseNode node)
        {
            node.CurState = BaseNode.State.Free;
            node.ForeachChild(resetAction);
        }

        /// <summary>
        /// 创建行为树节点
        /// </summary>
        public BaseNode CreateNode(Type nodeType)
        {
            BaseNode node = (BaseNode)Activator.CreateInstance(nodeType);
            node.Owner = this;
            AllNodes.Add(node);
            return node;
        }

        /// <summary>
        /// 删除行为树节点
        /// </summary>
        public void RemoveNode(BaseNode node)
        {
            node.Owner = null;
            AllNodes.Remove(node);
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


        /// <summary>
        /// 序列化前的预处理
        /// </summary>
        public void PreProcessSerialize()
        {
            //为所有节点建立Id 并排序子节点
            for (int i = 0; i < AllNodes.Count; i++)
            {
                int id = i + 1;
                BaseNode node = AllNodes[i];
                node.Id = id;   
                node.SortChild();
            }
            
            //重建父子Id
            if (RootNode != null)
            {
                RootNodeId = RootNode.Id;
            }
            foreach (BaseNode node in AllNodes)
            {
                node.RebuildId();
            }
        }

        /// <summary>
        /// 反序列化后处理
        /// </summary>
        public void PostProcessDeserialize()
        {
            RootNode = (RootNode)GetNode(RootNodeId);
            
            foreach (BaseNode node in AllNodes)
            {
                //重建对父子节点和Owner的引用
                node.Owner = this;
                node.RebuildNodeReference(AllNodes);  
            }
        }
        
       
        /// <summary>
        /// 序列化为字符串
        /// </summary>
        public string SerializeToString()
        {
            if (StringSerializer == null)
            {
                throw new Exception("试图将行为树序列化为字符串，但未设置对应的序列化器");
            }
            
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
        /// 序列化为二进制
        /// </summary>
        public byte[] SerializeToBinary()
        {
            if (BinarySerializer == null)
            {
                throw new Exception("试图将行为树序列化为二进制，但未设置对应的序列化器");
            }   
            PreProcessSerialize();
            byte[] bytes = BinarySerializer.Serialize(this);
            return bytes;
        }

        /// <summary>
        /// 从二进制反序列化
        /// </summary>
        public static BehaviourTree Deserialize(byte[] bytes)
        {
            var bt = BinarySerializer.Deserialize(bytes);
            bt.PostProcessDeserialize();
            return bt;
        }
        
    }

}
