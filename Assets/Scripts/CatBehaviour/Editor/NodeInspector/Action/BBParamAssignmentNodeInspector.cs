using System;
using CatBehaviour.Runtime;
using UnityEditor;
using UnityEngine;

namespace CatBehaviour.Editor
{
    public class BBParamAssignmentNodeInspector<T,V> : BaseNodeInspector where T: BBParam<V>,new()
    {
        private int selectedKeyIndex = 0;

        public override void OnGUI()
        {
            base.OnGUI();
            var node = (BBParamAssignmentNode<T,V>)Target;
            
            //选择要赋值的黑板参数Key
            string key = null;
            if (node.FakeBBParam != null)
            {
                key = node.FakeBBParam.Key;
            }

            var (curIndex, curKey) =
                BBParamDrawerHelper.DrawBBParamKeyPopup(Target.BlackBoard.GetKeys(typeof(T)), "Key:", selectedKeyIndex, key);
            selectedKeyIndex = curIndex;

            if (string.IsNullOrEmpty(curKey))
            {
                //Key为空 返回
                return;
            }

            //根据选择的key获取到黑板参数类型，以获取对应的drawer
            BBParam bbParam = Target.BlackBoard.GetParam(curKey);
            Type type = bbParam.GetType();
            bool hasDrawer = BaseBBParamDrawer.BBParamDrawerDict.TryGetValue(bbParam.GetType(), out var drawer);
            if (!hasDrawer)
            {
                EditorGUILayout.HelpBox($"没有此类型的Drawer:{type.Name}", MessageType.Error);
                return;
            }

            if (node.FakeBBParam == null)
            {
                //如果fakeBBParam为空 就创建相关类型的对象
                node.FakeBBParam = (T)Activator.CreateInstance(type);
            }
            node.FakeBBParam.Key = curKey;
          

            drawer.Target = node.FakeBBParam;
            drawer.OnGUI(false,Target.Owner);
        }
    }

    [NodeInspector(NodeType = typeof(BBParamBoolAssignmentNode))]
    public class BBParamBoolAssignmentNodeInspector : BBParamAssignmentNodeInspector<BBParamBool, bool>
    {
        
    }
    
    [NodeInspector(NodeType = typeof(BBParamIntAssignmentNode))]
    public class BBParamIntAssignmentNodeInspector : BBParamAssignmentNodeInspector<BBParamInt, int>
    {
        
    }
    
    [NodeInspector(NodeType = typeof(BBParamFloatAssignmentNode))]
    public class BBParamFloatAssignmentNodeInspector : BBParamAssignmentNodeInspector<BBParamFloat, float>
    {
        
    }
    
    [NodeInspector(NodeType = typeof(BBParamStringAssignmentNode))]
    public class BBParamStringAssignmentNodeInspector : BBParamAssignmentNodeInspector<BBParamString, string>
    {
        
    }
    
    [NodeInspector(NodeType = typeof(BBParamVector2AssignmentNode))]
    public class BBParamVector2AssignmentNodeInspector : BBParamAssignmentNodeInspector<BBParamVector2, Vector2>
    {
        
    }
    
    [NodeInspector(NodeType = typeof(BBParamVector3AssignmentNode))]
    public class BBParamVector3AssignmentNodeInspector : BBParamAssignmentNodeInspector<BBParamVector3, Vector3>
    {
        
    }
    
    [NodeInspector(NodeType = typeof(BBParamVector4AssignmentNode))]
    public class BBParamVector4AssignmentNodeInspector : BBParamAssignmentNodeInspector<BBParamVector4, Vector4>
    {
        
    }
}