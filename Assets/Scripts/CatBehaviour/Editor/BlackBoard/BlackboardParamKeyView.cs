using CatBehaviour.Runtime;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace CatBehaviour.Editor
{
    /// <summary>
    /// 黑板参数key视图
    /// </summary>
    public class BlackboardParamKeyView : BlackboardField
    {
        private BehaviourTreeGraphView graphView;
        private BBParam param;

        private TextField keyField;
        
        public BlackboardParamKeyView(BehaviourTreeGraphView graphView,BBParam param,string key,string typeName) :base(null,key,typeName)
        {
            this.graphView = graphView;
            this.param = param;
            this.AddManipulator(new ContextualMenuManipulator(BuildContextualMenu));  //增加右键菜单
            this.Q("icon").AddToClassList("parameter-" + typeName);
            this.Q("icon").visible = true;

            keyField = this.Q<TextField>("textField");
            
            keyField.RegisterValueChangedCallback((e) =>
            {
                //重命名了黑板key
                string oldKey = e.previousValue;
                string newKey = e.newValue;
                if (graphView.RenameBlackBoardParam(oldKey,newKey,param))
                {
                    text = e.newValue;
                }
            });
            
        }

        /// <summary>
        /// 构建右键菜单
        /// </summary>
        /// <param name="evt"></param>
        private void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Rename", (a) => OpenTextEditor(), DropdownMenuAction.AlwaysEnabled);
            evt.menu.AppendAction("Delete", (a) => graphView.RemoveBlackBoardParam(text), DropdownMenuAction.AlwaysEnabled);

            evt.StopPropagation();
        }
    }
}