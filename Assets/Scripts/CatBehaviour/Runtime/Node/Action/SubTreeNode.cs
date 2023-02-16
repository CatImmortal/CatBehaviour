using System;
using System.Reflection;
using UnityEngine;

namespace CatBehaviour.Runtime
{
    /// <summary>
    /// 子树节点
    /// </summary>
    [NodeInfo(Name = "内置/子树",Icon = "Icon/Root")]
    public class SubTreeNode : BaseActionNode
    {
        private static FieldInfo[] fieldInfos = typeof(SubTreeNode).GetFields(BindingFlags.Public | BindingFlags.Instance);

        /// <inheritdoc />
        public override FieldInfo[] FieldInfos => fieldInfos;
        
        /// <summary>
        /// 子树名
        /// </summary>
        [BBParamInfo(Name = "子树名")]
        public BBParamString SubTreeName;
        
        /// <summary>
        /// 是否继承黑板数据
        /// </summary>
        public bool IsInheritBlackBoard = true;
        
        private Action<BehaviourTree> onCreateSubTreeCallback;
        private Action<bool> onFinishCallback;
        
        private BehaviourTree subTree;
        private string subTreeDebugName;
        

        
        public SubTreeNode()
        {
            onCreateSubTreeCallback = OnSubTreeCreated;
            onFinishCallback = OnFinished;
        }

        /// <summary>
        /// 子树创建时的回调
        /// </summary>
        private void OnSubTreeCreated(BehaviourTree bt)
        {
            subTree = bt;
            subTree.OnFinished += onFinishCallback;

            //继承黑板数据
            if (IsInheritBlackBoard)
            {
                foreach (var pair in Owner.BlackBoard.ParamDict)
                {
                    subTree.BlackBoard.SetParam(pair.Key,pair.Value);
                }
            }
            
            subTree.Start(subTreeDebugName);
        }

        /// <summary>
        /// 子树运行结束时的回调
        /// </summary>
        private void OnFinished(bool result)
        {
            Finish(result);
        }

        protected override void OnStart()
        {
            if (string.IsNullOrEmpty(subTreeDebugName))
            {
                //生成子树的调试名
                subTreeDebugName = $"{Owner.DebugName} - 子树 {SubTreeName.Value}".Replace('/','_');
            }
            
            if (subTree != null)
            {
                //子树已创建过 直接运行
                subTree.Start(subTreeDebugName);
                return;
            }
            
            if (BehaviourTree.OnCreateSubTreeCallback == null)
            {
                Debug.LogError("创建子树失败，子树创建回调为空");
                Finish(false);
            }
            else
            {
                //子树为空 创建子树
                BehaviourTree.OnCreateSubTreeCallback(SubTreeName.Value,Owner, onCreateSubTreeCallback);
            }
        }

        protected override void OnCancel()
        {
            subTree?.RootNode.Cancel();
        }
    }
}