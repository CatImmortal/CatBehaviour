using System.Collections.Generic;
using UnityEngine;

namespace CatBehaviour.Runtime
{
    /// <summary>
    /// 基于Unity ScriptableObject的序列化数据提供者
    /// </summary>
    public class BehaviourTreeSO : ScriptableObject, IBehaviourTreeSerializeDataProvider
    {
        /// <summary>
        /// 行为树视口的位置与缩放
        /// </summary>
        public Rect ViewportRect;
        
        /// <summary>
        /// 节点属性面板宽度
        /// </summary>
        public float InspectorWidth;
        
        /// <summary>
        /// 黑板位置与大小
        /// </summary>
        public Rect BlackBoardRect;
        
        /// <summary>
        /// 节点列表
        /// </summary>
        [SerializeReference]
        public List<BaseNode> AllNodes = new List<BaseNode>();

        /// <summary>
        /// 黑板参数列表
        /// </summary>
        [SerializeReference] 
        public List<BBParam> BBParams = new List<BBParam>();
        
        /// <summary>
        /// 注释块列表
        /// </summary>
        public List<CommentBlock> CommentBlocks = new List<CommentBlock>();
        
        public void Serialize(BehaviourTree behaviourTree)
        {
            
            behaviourTree.SerializePreProcess();
            AllNodes = behaviourTree.AllNodes;
            
            //黑板参数
            BBParams.Clear();
            foreach (var pair in behaviourTree.BlackBoard.ParamDict)
            {
                BBParams.Add(pair.Value);
            }
            
            //重建注释块包含的节点id
            CommentBlocks.Clear();
            foreach (var commentBlock in CommentBlocks)
            {
                commentBlock.RebuildId();
            }
        }

        public BehaviourTree Deserialize()
        {
            BehaviourTree behaviourTree = new BehaviourTree();
            behaviourTree.AllNodes = AllNodes;
            behaviourTree.DeserializePostProcess();
            
            //黑板参数
            behaviourTree.BlackBoard = new BlackBoard();
            foreach (BBParam param in BBParams)
            {
                behaviourTree.BlackBoard.SetParam(param.Key,param);
            }
            
            //重建对注释块包含的节点的引用
            foreach (var commentBlock in CommentBlocks)
            {
                commentBlock.RebuildNodeReference(AllNodes);
            }

            return behaviourTree;
        }


    }
}