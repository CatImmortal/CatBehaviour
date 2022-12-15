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
        /// 根节点
        /// </summary>
        public RootNode RootNode;

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
        
        
        public static void OnUpdate(float deltaTime)
        {
            UpdateManager.OnUpdate(deltaTime);
        }
    }

}
