using UnityEngine;

namespace CatBehaviour.Runtime
{
    /// <summary>
    /// 黑板参数赋值节点基类
    /// </summary>
    public abstract class BBParamAssignmentNode<T,V> : BaseActionNode where T: BBParam<V>,new()
    {
        public T FakeBBParam;

        protected override void OnStart()
        {
            if (FakeBBParam != null)
            {
                var bbParam = Owner.BlackBoard.GetParam<T>(FakeBBParam.Key);
                bbParam.Value = FakeBBParam.Value;
            }

            Finish(true);
        }

        protected override void OnCancel()
        {
       
        }
    }

    [NodeInfo(Name = "内置/黑板参数赋值/黑板参数赋值(Bool)")]
    public class BBParamBoolAssignmentNode : BBParamAssignmentNode<BBParamBool,bool>
    {
        
    }
    
    [NodeInfo(Name = "内置/黑板参数赋值/黑板参数赋值(Int)")]
    public class BBParamIntAssignmentNode : BBParamAssignmentNode<BBParamInt,int>
    {
        
    }
    
    [NodeInfo(Name = "内置/黑板参数赋值/黑板参数赋值(Float)")]
    public class BBParamFloatAssignmentNode : BBParamAssignmentNode<BBParamFloat,float>
    {
        
    }
    
    [NodeInfo(Name = "内置/黑板参数赋值/黑板参数赋值(String)")]
    public class BBParamStringAssignmentNode : BBParamAssignmentNode<BBParamString,string>
    {
        
    }
    
    [NodeInfo(Name = "内置/黑板参数赋值/黑板参数赋值(Vector2)")]
    public class BBParamVector2AssignmentNode : BBParamAssignmentNode<BBParamVector2,Vector2>
    {
        
    }
    
    [NodeInfo(Name = "内置/黑板参数赋值/黑板参数赋值(Vector3)")]
    public class BBParamVector3AssignmentNode : BBParamAssignmentNode<BBParamVector3,Vector3>
    {
        
    }
    
    [NodeInfo(Name = "内置/黑板参数赋值/黑板参数赋值(Vector4)")]
    public class BBParamVector4AssignmentNode : BBParamAssignmentNode<BBParamVector4,Vector4>
    {
        
    }
}