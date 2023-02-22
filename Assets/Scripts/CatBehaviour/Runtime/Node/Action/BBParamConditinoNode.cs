using System;
using UnityEngine;

namespace CatBehaviour.Runtime
{
    /// <summary>
    /// 黑板条件节点基类
    /// </summary>
    public abstract class BBParamConditionNode <T,V> : BaseActionNode where T: BBParam<V>,new()
    {
        /// <summary>
        /// 操作类型
        /// </summary>
        public enum OpType
        {
            Equal, //==
            NotEqual, // !=

            Greater, //>
            GreaterOrEqual, //>=
            
            Less, //<
            LessOrEqual //<=
        }

        public string Key;
        public OpType Type;
        public T OtherBBParam;
        
        
        
        protected override void OnStart()
        {
            var bbParam = Owner.BlackBoard.GetParam<T>(Key);

            if (!string.IsNullOrEmpty(OtherBBParam.Key))
            {
                //需要从其他黑板参数获取值
                var otherBBParam = Owner.BlackBoard.GetParam<T>(OtherBBParam.Key);
                OtherBBParam.Value = otherBBParam.Value;
            }

            bool result = false;
            switch (Type)
            {
                case OpType.Equal:
                    result = bbParam.Value.Equals(OtherBBParam.Value);
                    break;
                
                case OpType.NotEqual:
                    result = !bbParam.Value.Equals(OtherBBParam.Value);
                    break;
                
                case OpType.Greater:
                    result = IsGreater(bbParam.Value, OtherBBParam.Value);
                    break;
                
                case OpType.GreaterOrEqual:
                    result = IsGreater(bbParam.Value, OtherBBParam.Value) || bbParam.Value.Equals(OtherBBParam.Value);
                    break;
                
                case OpType.Less:
                    result = !(IsGreater(bbParam.Value, OtherBBParam.Value) ||
                               bbParam.Value.Equals(OtherBBParam.Value));
                    break;
                
                case OpType.LessOrEqual:
                    result = !IsGreater(bbParam.Value, OtherBBParam.Value);
                    break;
            }
            
            Finish(result);
        }

        protected override void OnCancel()
        {
  
        }

        /// <summary>
        /// a是否等于b
        /// </summary>
        protected virtual bool Equals(V a,V b)
        {
            return a.Equals(b);
        }
        
        /// <summary>
        /// a是否大于b
        /// </summary>
        protected virtual bool IsGreater(V a,V b)
        {
            throw new NotImplementedException($"{GetType()}未实现{nameof(OpType.Greater)}方法");
        }
    }

    [NodeInfo(Name = "内置/黑板参数条件/黑板参数条件(Bool)")]
    public class BBParamBoolConditionNode : BBParamConditionNode<BBParamBool, bool>
    {
        
    }
    
    [NodeInfo(Name = "内置/黑板参数条件/黑板参数条件(Int)")]
    public class BBParamIntConditionNode : BBParamConditionNode<BBParamInt, int>
    {
        protected override bool IsGreater(int a, int b)
        {
            return a > b;
        }
    }
    
    [NodeInfo(Name = "内置/黑板参数条件/黑板参数条件(Float)")]
    public class BBParamFloatConditionNode : BBParamConditionNode<BBParamFloat, float>
    {
        protected override bool Equals(float a, float b)
        {
            return Math.Abs(a - b) < float.Epsilon;
        }

        protected override bool IsGreater(float a, float b)
        {
            return a > b;
        }
    }
    
    [NodeInfo(Name = "内置/黑板参数条件/黑板参数条件(String)")]
    public class BBParamStringConditionNode : BBParamConditionNode<BBParamString, string>
    {
        
    }
    
    [NodeInfo(Name = "内置/黑板参数条件/黑板参数条件(Vector2)")]
    public class BBParamVector2ConditionNode : BBParamConditionNode<BBParamVector2, Vector2>
    {
    }
    
    [NodeInfo(Name = "内置/黑板参数条件/黑板参数条件(Vector3)")]
    public class BBParamVector3ConditionNode : BBParamConditionNode<BBParamVector3, Vector3>
    {
    }
    
    [NodeInfo(Name = "内置/黑板参数条件/黑板参数条件(Vector4)")]
    public class BBParamVector4ConditionNode : BBParamConditionNode<BBParamVector4, Vector4>
    {
    }
}