using System;
using UnityEngine;

namespace CatBehaviour
{
    public class CatBehaviourComponent : MonoBehaviour
    {
        private void Update()
        {
            BehaviourTree.OnUpdate(Time.deltaTime);
        }
    }
}