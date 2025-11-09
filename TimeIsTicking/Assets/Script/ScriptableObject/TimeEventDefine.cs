using UnityEngine;

public enum TimeEventType
{
    Show,
    Hide,
}

[CreateAssetMenu(fileName = "TimeEventDefine", menuName = "Scriptable Objects/TimeEventDefine")]
public class TimeEventDefine : ScriptableObject
{
    public TimeEventType timeEventType;
    public string targetName;
    
    private bool _isTrigger;
    public bool IsTrigger => _isTrigger;
    public void Trigger() =>_isTrigger = true;
    public void ResetTrigger() =>_isTrigger = false;
}
