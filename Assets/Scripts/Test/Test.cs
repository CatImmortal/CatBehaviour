using System;
using System.Collections;
using System.Collections.Generic;
using CatBehaviour.Runtime;
using UnityEngine;

public class Test : MonoBehaviour
{
    private void Start()
    {
        RootNode rootNode = new RootNode();
        
        SequenceNode sequenceNode = new SequenceNode();
        
        sequenceNode.AddChild(new LogNode(){Log = "1111"});
        sequenceNode.AddChild(new DelayNode(){DelayTime = 3});
        sequenceNode.AddChild(new LogNode(){Log = "2222"});

        RepeaterNode repeaterNode = new RepeaterNode
        {
            repeatCount = 3
        };
        repeaterNode.AddChild(new LogNode(){Log = "3333"});
        sequenceNode.AddChild(repeaterNode);
        
        rootNode.AddChild(sequenceNode);

        SelectorNode selectorNode = new SelectorNode();
        
        FailureNode failureNode = new FailureNode();
        failureNode.AddChild(new LogNode(){Log = "4444"});
        selectorNode.AddChild(failureNode);
        
        selectorNode.AddChild(new LogNode(){Log = "5555"});
        
        sequenceNode.AddChild(selectorNode);
        
        BehaviourTree bt = new BehaviourTree
        {
            RootNode = rootNode
        };
        bt.Start();
    }
}
