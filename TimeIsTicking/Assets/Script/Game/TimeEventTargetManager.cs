using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeEventTargetManager : MonoBehaviour
{
    [SerializeField] private DigitalClock clock;
    [SerializeField] private TimeEventCollection timeEventCollection;
    
    private List<TimeEventTarget> _timeEventTargets = new();

    private void Awake()
    {
        clock.OnReset += OnTimeReset;
        var roots = SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (var root in roots)
        {
            var timeEventTarget = root.GetComponentsInChildren<TimeEventTarget>(true);

            _timeEventTargets.AddRange(timeEventTarget);
        }
    }
    
    void Update()
    {
        if (clock == null)
            return;

        int currentTime = Mathf.FloorToInt(clock.ElapsedTimeAfterStart);

        foreach (var e in timeEventCollection.timeEventDefineDictionary)
        {
            if (!e.Value.IsTrigger && currentTime >= e.Key)
            {
                e.Value.Trigger();
                ExecuteEvent(e.Value);
            }
        }
    }

    void ExecuteEvent(TimeEventDefine eventDefine)
    {
        foreach (var target in _timeEventTargets)
        {
            if(eventDefine.targetName.Equals(target.Key))
            {
                target.InvokeTimeEvent(eventDefine);
                return;
            }
        }
    }

    private void OnDestroy()
    {
        clock.OnReset -= OnTimeReset;
    }

    private void OnTimeReset()
    {
        timeEventCollection.ResetAllTrigger();
    }
    
}