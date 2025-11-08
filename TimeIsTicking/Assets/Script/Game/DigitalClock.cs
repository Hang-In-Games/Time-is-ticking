using UnityEngine;
using TMPro;

public class DigitalClock : MonoBehaviour
{
    [SerializeField] private TextMeshPro clockText;

    [Header("초기 시간 설정")]
    [SerializeField] private int startHour = 12;
    [SerializeField] private int startMinute = 0;
    [SerializeField] private int startSecond = 0;

    [Header("루프 주기(초 단위)")]
    [SerializeField] private float loopDuration = 300f; // 5분(300초)

    private float elapsedSeconds;
    private float loopTriggerElapsedTime;
    private bool eventTriggered = false;

    void Start()
    {
        // 초기 시간 계산
        elapsedSeconds = startHour * 3600 + startMinute * 60 + startSecond;
        loopTriggerElapsedTime = elapsedSeconds + loopDuration;
    }

    void Update()
    {
        // 시간 흘러감
        elapsedSeconds += Time.deltaTime;

        // 루프 처리
        if (elapsedSeconds >= loopTriggerElapsedTime)
        {
            elapsedSeconds = startHour * 3600 + startMinute * 60 + startSecond;
            eventTriggered = false; // 루프 시 이벤트 리셋
        }
        

        // 디지털 시계 표시
        int hours = (int)(elapsedSeconds / 3600) % 24;
        int minutes = (int)(elapsedSeconds / 60) % 60;
        int seconds = (int)(elapsedSeconds % 60);

        clockText.text = $"{hours:D2}:{minutes:D2}:{seconds:D2}";
    }
    
}