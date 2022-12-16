using System.Collections;
using System.Collections.Generic;
using CatBehaviour.Runtime;
using CatJson;
using UnityEngine;

public class JsonSerializer : IStringSerializer
{
    public string Serialize(BehaviourTree bt)
    {
        string json = bt.ToJson();
        return json;
    }

    public BehaviourTree Deserialize(string str)
    {
        var bt = str.ParseJson<BehaviourTree>();
        return bt;
    }
}
