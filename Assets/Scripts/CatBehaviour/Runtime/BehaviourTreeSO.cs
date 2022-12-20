using System.Collections.Generic;
using UnityEngine;

namespace CatBehaviour.Runtime
{
    /// <summary>
    /// 行为树SO
    /// </summary>
    public class BehaviourTreeSO : ScriptableObject,ISerializationCallbackReceiver
    {
        public BehaviourTree BT;
        
        public List<string> BBParamKeys = new List<string>();

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

            BBParamKeys.Clear();
            BBParams.Clear();
            foreach (KeyValuePair<string,BBParam> pair in BT.BlackBoard.ParamDict)
            {
                BBParamKeys.Add(pair.Key);
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
            
            for (int i = 0; i < BBParamKeys.Count; i++)
            {
                string key = BBParamKeys[i];
                BBParam value = BBParams[i];
                BT.BlackBoard.SetParam(key,value);
            }
            BBParamKeys.Clear();
            BBParams.Clear();

            BT.BlackBoard.Position = BlackBoardRect;

            BT.PostProcessDeserialize();
        }
    }
}