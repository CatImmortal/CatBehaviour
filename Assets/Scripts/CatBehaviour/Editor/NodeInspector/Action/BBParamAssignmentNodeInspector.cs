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

            var (curIndex, curKey) =
                BBParamDrawerHelper.DrawBBParamKeyPopup(Target.BlackBoard.GetKeys(typeof(T)), "要赋值的黑板Key:",
                    selectedKeyIndex, node.Key);
            selectedKeyIndex = curIndex;
            node.Key = curKey;
            if (string.IsNullOrEmpty(node.Key))
            {
                //Key为空
                return;
            }

            //根据选择的key获取到黑板参数类型，以获取对应类型的drawer
            BBParam bbParam = Target.BlackBoard.GetParam(node.Key);
            Type type = bbParam.GetType();
            bool hasDrawer = BaseBBParamDrawer.BBParamDrawerDict.TryGetValue(type, out var drawer);
            if (!hasDrawer)
            {
                EditorGUILayout.HelpBox($"没有此类型的Drawer:{type.Name}", MessageType.Error);
                return;
            }
            
            if (node.OtherBBParam == null)
            {
                node.OtherBBParam = new T();
            }
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("数据源:");
            drawer.Target = node.OtherBBParam;
            drawer.OnGUI(Target.Owner,true);
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