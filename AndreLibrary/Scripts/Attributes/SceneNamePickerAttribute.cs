using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class SceneNamePickerAttribute : PropertyAttribute
{
    public bool showPath = true;

    public SceneNamePickerAttribute() { }

    public SceneNamePickerAttribute(bool showPath)
    {
        this.showPath = showPath;
    }
}
