using System;
using System.Collections;
using System.Collections.Generic;
using CatBehaviour.Runtime;
using UnityEngine;

public class Test : MonoBehaviour
{
    public BehaviourTreeSO BTSO;
    
    private void Start()
    {
        var bt = BTSO.CloneBehaviourTree();
        bt.OnFinish += () =>
        {
            Debug.Log("行为树运行结束");
        };
        bt.Start("TestBT");
        
    }
}
