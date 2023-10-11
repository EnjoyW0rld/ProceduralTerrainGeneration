using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.AttributeUsage(System.AttributeTargets.Field)]
public class HideIfAttribute : PropertyAttribute
{
    public enum Comparison { Bigger, Smaller, Equals, NotEquals }
    public Comparison comparison;
    public object value;
    public string targetName;

    public HideIfAttribute(string targetName, object value, Comparison comparison = Comparison.Equals)
    {
        this.value = value;
        this.targetName = targetName;
        this.comparison = comparison;
    }

}
