using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "TimeEventCollection", menuName = "Scriptable Objects/TimeEventCollection")]
public class TimeEventCollection : SerializedScriptableObject
{
    public Dictionary<int ,TimeEventDefine> timeEventDefineDictionary = new();

    public void ResetAllTrigger()
    {
        foreach (var e in timeEventDefineDictionary)
        {
            e.Value.ResetTrigger();
        }
    }
}
