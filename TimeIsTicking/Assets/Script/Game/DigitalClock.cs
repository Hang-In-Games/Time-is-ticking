using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public enum PauseReason
{
    MiniGame,
}

public class DigitalClock : MonoBehaviour
{
    [SerializeField] private TextMeshPro clockText;

    [Header("초기 시간 설정")]
    [SerializeField] private int startHour = 12;
    [SerializeField] private int startMinute = 0;
    [SerializeField] private int startSecond = 0;

    [Header("루프 주기(초 단위)")]
    [SerializeField] private float loopDuration = 300f; // 5분(300초)

    private List<PauseReason> pauseReasons = new();
    private float elapsedSeconds, StartElapsedSeconds;
    private float loopTriggerElapsedTime;
    private bool isPaused = false;
    
    public Action OnReset;
    public float ElapsedTime => elapsedSeconds;
    public float ElapsedTimeAfterStart => elapsedSeconds - StartElapsedSeconds;
    
    void Start()
    {
        // 초기 시간 계산
        StartElapsedSeconds = elapsedSeconds = startHour * 3600 + startMinute * 60 + startSecond;
        loopTriggerElapsedTime = elapsedSeconds + loopDuration;
    }

    void Update()
    {
        if (isPaused)
            return;
        
        // 시간 흘러감
        elapsedSeconds += Time.deltaTime;

        // 루프 처리
        if (elapsedSeconds >= loopTriggerElapsedTime)
        {
            elapsedSeconds = StartElapsedSeconds;
            OnReset?.Invoke();
        }
        // 디지털 시계 표시
        int hours = (int)(elapsedSeconds / 3600) % 24;
        int minutes = (int)(elapsedSeconds / 60) % 60;
        int seconds = (int)(elapsedSeconds % 60);

        clockText.text = $"{hours:D2}:{minutes:D2}:{seconds:D2}";
    }

    public void Pause(PauseReason reason)
    {
        pauseReasons.Add(reason);
        isPaused = true;
    }

    public void Resume(PauseReason reason)
    {
        pauseReasons.Remove(reason);
        
        if(pauseReasons.Any(e=>e == reason) == false)
            isPaused = false;
    }
}