using System;
using CatBehaviour.Runtime;

namespace CatBehaviour.Editor
{
    public partial class BehaviourTreeGraphView
    {
        /// <summary>
        /// 添加黑板参数
        /// </summary>
        public void AddBlackBoardParam(string key,Type type,object value = null)
        {
            window.RecordObject($"AddBlackBoard {key}");
            
            BBParam param = (BBParam)Activator.CreateInstance(type);
            param.ValueObj = value;
            BT.BlackBoard.SetParam(key, param);

            OnBlackBoardChanged?.Invoke();
        }

        /// <summary>
        /// 移除黑板参数
        /// </summary>
        public void RemoveBlackBoardParam(string key)
        {
            window.RecordObject($"RemoveBlackBoard {key}");
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
                return false;
            }
            
            window.RecordObject($"RenameBlackBoard {oldKey} -> {newKey}");
            
            BT.BlackBoard.RemoveParam(oldKey);
            BT.BlackBoard.SetParam(newKey,param);

            return true;
        }
    }
}