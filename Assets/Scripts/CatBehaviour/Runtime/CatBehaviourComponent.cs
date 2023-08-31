using System;
using UnityEngine;

namespace CatBehaviour.Runtime
{
    public class CatBehaviourComponent : MonoBehaviour
    {
        private void Update()
        {
            UpdateManager.OnUpdate(Time.deltaTime);
        }
    }
}