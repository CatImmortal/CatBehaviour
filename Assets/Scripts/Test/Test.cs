using System;
using System.Collections;
using System.Collections.Generic;
using CatBehaviour.Runtime;
using UnityEditor;
using UnityEngine;

public class Test : MonoBehaviour
{
    public BehaviourTreeSO BTSO;
    private BehaviourTree bt;

    private void Awake()
    {
        //设置子树创建的处理方法
        BehaviourTree.OnCreateSubTreeCallback = (name,parent, action) =>
        {
            var so = AssetDatabase.LoadAssetAtPath<BehaviourTreeSO>(name);
            var bt = so.CloneBehaviourTree();
            action(bt);
        };
    }

    private void Start()
    {
        bt = BTSO.CloneBehaviourTree();
        bt.OnFinished += (result) =>
        {
            Debug.Log($"行为树运行结束:{result}");
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
