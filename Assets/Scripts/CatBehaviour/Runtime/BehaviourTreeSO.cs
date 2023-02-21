using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CatBehaviour.Runtime
{
    /// <summary>
    /// 行为树SO
    /// </summary>
    public class BehaviourTreeSO : ScriptableObject,ISerializationCallbackReceiver
    {
        /// <summary>
        /// 行为树
        /// </summary>
        public BehaviourTree BT;

        /// <summary>
        /// 黑板参数列表
        /// </summary>
        [SerializeReference]
        public List<BBParam> BBParams = new List<BBParam>();

        
        
        /// <summary>
        /// 黑板位置与大小
        /// </summary>
        public Rect BlackBoardRect;

        public void SetParam(string key, BBParam param)
        {
            BT.BlackBoard.SetParam(key,param);
            BBParams.Add(param);
        }

        public void RemoveParam(string key)
        {
            var param = BT.BlackBoard.GetParam(key);
            BBParams.Remove(param);
            BT.BlackBoard.RemoveParam(key);
        }
        
        public void OnBeforeSerialize()
        {
            if (BT == null)
            {
                return;
            }
            
            BT.PreProcessSerialize();
            BlackBoardRect = BT.BlackBoard.Position;
        }

        
        public void OnAfterDeserialize()
        {
            if (BT == null)
            {
                return;
            }
            
            //恢复黑板数据
            BT.BlackBoard.ParamDict.Clear();
            for (int i = 0; i < BBParams.Count; i++)
            {
                BBParam bbParam = BBParams[i];
                BT.BlackBoard.SetParam(bbParam.Key,bbParam);
            }
            BT.BlackBoard.Position = BlackBoardRect;

            BT.PostProcessDeserialize();
        }

        /// <summary>
        /// 复制行为树
        /// </summary>
        public BehaviourTree CloneBehaviourTree()
        {
            return Instantiate(this).BT;
        }
    }
}