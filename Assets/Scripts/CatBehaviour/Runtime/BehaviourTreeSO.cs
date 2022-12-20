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
        public void OnBeforeSerialize()
        {
            BT.PreProcessSerialize();
        }

        public void OnAfterDeserialize()
        {
            BT.PostProcessDeserialize();
        }
    }
}