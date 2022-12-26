using System;
using System.Collections;
using System.Collections.Generic;
using CatBehaviour.Runtime;
using UnityEngine;

public class Test : MonoBehaviour
{
    public BehaviourTreeSO BTSO;
    private BehaviourTree bt;
    private void Start()
    {
        bt = BTSO.CloneBehaviourTree();
        bt.OnFinish += () =>
        {
            Debug.Log("行为树运行结束");
        };
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            bt.Start("TestBT");
        }
    }
}
