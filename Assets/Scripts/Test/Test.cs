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
        BTSO.CloneBehaviourTree().Start();
    }
}
