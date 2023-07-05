using System;
using CatBehaviour.Runtime;
using UnityEditor;
using UnityEngine;

namespace CatBehaviour.Editor
{
    /// <summary>
    /// 黑板参数条件节点Inspector基类
    /// </summary>
    public abstract class BBParamConditionNodeInspector<T,V> : BaseNodeInspector where T: BBParam<V>,new()
    {
        private int selectedKeyIndex = 0;

        public override void OnGUI()
        {
            base.OnGUI();
            var node = (BBParamConditionNode<T,V>)Target;

            //选择要比较的黑板参数Key
            var (curIndex, curKey) =
                BBParamDrawerHelper.DrawBBParamKeyPopup(BTSO.GetParamKeys(typeof(T)), string.Empty,
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
                
            //操作类型
            node.Type = (BBParamConditionNode<T,V>.OpType)EditorGUILayout.EnumPopup(string.Empty, node.Type);
            
            if (node.OtherBBParam == null)
            {
                node.OtherBBParam = new T();
            }
            drawer.Target = node.OtherBBParam;
            drawer.OnGUI(BTSO,true);
            
            
        }
    }
    
    [NodeInspector(NodeType = typeof(BBParamBoolConditionNode))]
    public class BBParamBoolConditionNodeInspector : BBParamConditionNodeInspector<BBParamBool, bool>
    {
        
    }
    
    [NodeInspector(NodeType = typeof(BBParamIntConditionNode))]
    public class BBParamIntConditionNodeInspector : BBParamConditionNodeInspector<BBParamInt, int>
    {
        
    }
    
    [NodeInspector(NodeType = typeof(BBParamFloatConditionNode))]
    public class BBParamFloatConditionNodeInspector : BBParamConditionNodeInspector<BBParamFloat, float>
    {
        
    }
    
    [NodeInspector(NodeType = typeof(BBParamStringConditionNode))]
    public class BBParamStringConditionNodeInspector : BBParamConditionNodeInspector<BBParamString, string>
    {
        
    }
    
    [NodeInspector(NodeType = typeof(BBParamVector2ConditionNode))]
    public class BBParamVector2ConditionNodeInspector : BBParamConditionNodeInspector<BBParamVector2, Vector2>
    {
        
    }
    
    [NodeInspector(NodeType = typeof(BBParamVector3ConditionNode))]
    public class BBParamVector3ConditionNodeInspector : BBParamConditionNodeInspector<BBParamVector3, Vector3>
    {
        
    }
    
    [NodeInspector(NodeType = typeof(BBParamVector4ConditionNode))]
    public class BBParamVector4ConditionNodeInspector : BBParamConditionNodeInspector<BBParamVector4, Vector4>
    {
        
    }
    
}