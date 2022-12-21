using System.Collections.Generic;
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
        
        public void OnBeforeSerialize()
        {
            if (BT == null)
            {
                return;
            }
            BT.PreProcessSerialize();
            
            BBParams.Clear();
            foreach (KeyValuePair<string,BBParam> pair in BT.BlackBoard.ParamDict)
            {
                BBParams.Add(pair.Value);
            }
            BlackBoardRect = BT.BlackBoard.Position;
        }

        public void OnAfterDeserialize()
        {
            if (BT == null)
            {
                return;
            }
            
            for (int i = 0; i < BBParams.Count; i++)
            {
                BBParam bbParam = BBParams[i];
                BT.BlackBoard.SetParam(bbParam.Key,bbParam);
            }
            BBParams.Clear();
            BT.BlackBoard.Position = BlackBoardRect;

            BT.PostProcessDeserialize();
        }
    }
}