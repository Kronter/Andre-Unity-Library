using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Andre.AI.BehaviourTree.Scriptable
{
    public abstract class SO_Action : ScriptableObject
    {
        public virtual void Initialize(GameObject obj = null) { }
        public abstract NodeStates Action();
    }
}

