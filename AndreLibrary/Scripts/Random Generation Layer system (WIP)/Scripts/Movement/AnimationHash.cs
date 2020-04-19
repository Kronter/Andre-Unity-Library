using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimationHash
{
    public static readonly int Walk = Animator.StringToHash("Walk");
    public static readonly int Run = Animator.StringToHash("Run");
    public static readonly int Attack = Animator.StringToHash("Attack");
}

public enum AnimationState{Idle,Walk, Run,Attack};
